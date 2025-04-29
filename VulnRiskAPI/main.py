from flask import Flask, request, jsonify
from flask_cors import CORS
import re

app = Flask(__name__)
CORS(app)  # Activăm CORS pentru toate rutele

print("Inițializare API pentru detectarea vulnerabilităților...")

# Încercăm să încărcăm modelul specializat pentru detectarea vulnerabilităților
USE_ML_MODEL = False
try:
    import torch
    from transformers import AutoTokenizer, AutoModelForSequenceClassification
    import numpy as np

    # Înlocuim modelul cu CodeBERT fin-tuned pentru detectarea codului nesigur
    MODEL_NAME = "mrm8488/codebert-base-finetuned-detect-insecure-code"

    print(f"Încărcare model: {MODEL_NAME}")
    tokenizer = AutoTokenizer.from_pretrained(MODEL_NAME)
    model = AutoModelForSequenceClassification.from_pretrained(MODEL_NAME)
    model.eval()  # Setăm modelul în modul de evaluare
    USE_ML_MODEL = True
    print("Model ML încărcat cu succes!")

    # Acest model are output binar (0 = cod sigur, 1 = cod nesigur)
    # Definim o mapare simplă pentru rezultate
    SECURITY_CLASSES = {
        0: {"name": "Secure Code", "description": "Cod considerat sigur", "risk_level": 0},
        1: {"name": "Insecure Code", "description": "Cod potențial nesigur", "risk_level": 3}
    }

except Exception as e:
    print(f"Nu se poate încărca modelul ML: {e}")
    print("Se va folosi doar detecția bazată pe reguli.")

# Definim tipurile de vulnerabilități și nivelurile de risc
VULNERABILITY_TYPES = {
    "sql_injection": "SQL Injection",
    "xss": "Cross-Site Scripting (XSS)",
    "auth_issues": "Probleme de autentificare",
    "access_control": "Bypass-uri de autorizare",
    "insecure_objects": "Referințe directe nesigure la obiecte",
    "config_issues": "Configurări greșite de securitate",
    "data_exposure": "Expunere de date sensibile",
    "function_access": "Lipsă control acces la nivel de funcție",
    "csrf": "Cross-Site Request Forgery (CSRF)",
    "known_vulns": "Utilizarea componentelor cu vulnerabilități cunoscute"
}

RISK_LEVELS = {
    0: "Sigur",
    1: "Scăzut",
    2: "Mediu",
    3: "Ridicat",
    4: "Critic"
}

# Baza de cunoștințe pentru detectarea vulnerabilităților
VULNERABILITY_RULES = [
    # SQL Injection
    {
        "id": "sql_injection",
        "patterns": [
            r"string\s+sql\s*=.*\+",
            r"executeQuery\(.*\+",
            r"ExecuteNonQuery\(.*\+",
            r"execute\(.*\+",
            r"SqlCommand\(.*\+",
            r"new\s+SqlCommand\(.*\+",
            r"\.Raw\(.*\)",
            r"db\.Execute\(.*\+",
            r"\.Query\<.*\>\(.*\+",
            r"\.ExecuteSqlCommand\(.*\+"
        ],
        "risk_level": 3,
        "recommendation": "Folosiți interogări parametrizate sau prepared statements în loc de a construi interogări SQL prin concatenare de string-uri. Utilizați ORM-uri sau parametrizați toate datele introduse de utilizator."
    },
    # XSS
    {
        "id": "xss",
        "patterns": [
            r"innerHTML\s*=",
            r"document\.write\(",
            r"eval\(",
            r"Response\.Write\(",
            r"HttpContext\.Response\.Write\(",
            r"@Html\.Raw\(",
            r"\.raw\.html",
            r"dangerouslySetInnerHTML",
            r"\.parseHTML\("
        ],
        "risk_level": 3,
        "recommendation": "Evitați scrierea directă de conținut HTML din surse externe. Folosiți librării de sanitizare pentru conținut HTML sau metode encode/escape. Pentru React, folosiți metode sigure în loc de dangerouslySetInnerHTML."
    },
    # Probleme de autentificare
    {
        "id": "auth_issues",
        "patterns": [
            r"password\s*=\s*\"[^\"]+\"",
            r"hardcoded.*password",
            r"credentials.*in.*code",
            r"\.Password\s*=\s*\"",
            r"\.Password\s*=\s*'",
            r"api[kK]ey\s*=\s*\"",
            r"token\s*=\s*\"",
            r"\.authenticate\(.*false\)",
            r"AllowAnonymous",
            r"\.AllowAnonymous\("
        ],
        "risk_level": 2,
        "recommendation": "Nu stocați niciodată credențiale hardcodate în cod. Folosiți variabile de mediu, secrete sigure sau servicii de stocare a credențialelor pentru date sensibile. Verificați aplicarea corectă a atributelor de autentificare."
    },
    # Bypass-uri de autorizare
    {
        "id": "access_control",
        "patterns": [
            r"\.Authorize\(.*false\)",
            r"isAdmin\s*=\s*true",
            r"userIsAdmin\s*=\s*true",
            r"\.Role\s*=\s*\"Admin\"",
            r"Request\.QueryString\[\"role\"\]",
            r"\.GetSession\(\)\.IsAdmin",
            r"\.SetAdmin\(true\)",
            r"User\.IsInRole\(.*\)",
            r"\.HasRole\(.*\)"
        ],
        "risk_level": 2,
        "recommendation": "Nu preluați roluri sau permisiuni direct din parametri accesibili utilizatorului. Verificați permisiunile întotdeauna pe server. Asigurați-vă că verificările de autorizare sunt aplicate consistent la fiecare endpoint."
    },
    # Referințe directe nesigure la obiecte
    {
        "id": "insecure_objects",
        "patterns": [
            r"Request\.QueryString\[\"id\"\]",
            r"Request\.Params\[\"id\"\]",
            r"req\.params\.id",
            r"urlParams\.get\(\"id\"\)",
            r"\.getElementById\(",
            r"fromId\(",
            r"\.get\(\"id\"\)"
        ],
        "risk_level": 1,
        "recommendation": "Validați întotdeauna că utilizatorul are dreptul să acceseze resursa cu ID-ul specificat. Folosiți ID-uri care nu pot fi ghicite și verificați apartenența înainte de a oferi acces la obiecte."
    },
    # Configurări greșite de securitate
    {
        "id": "config_issues",
        "patterns": [
            r"debug\s*=\s*true",
            r"IsDebug\s*=\s*true",
            r"EnableDebug\(",
            r"\.EnableSsl\s*=\s*false",
            r"RequireHttps\s*=\s*false",
            r"\.RequireHttps\(false\)",
            r"ValidateAntiForgeryToken\(false\)",
            r"\.AllowAllHeaders\(",
            r"cors\.AllowAnyOrigin\(",
            r"\.SetIsOriginAllowed\(\(\) => true\)"
        ],
        "risk_level": 1,
        "recommendation": "Dezactivați modurile de debug în producție. Activați întotdeauna SSL/TLS. Configurați CORS pentru a permite doar originile necesare. Folosiți token-uri antiforgery pentru toate formularele POST."
    },
    # Expunere de date sensibile
    {
        "id": "data_exposure",
        "patterns": [
            r"\.CreateEncryptor\(",
            r"\.CreateDecryptor\(",
            r"MD5\.",
            r"\.MD5\(",
            r"new\s+MD5",
            r"\.SHA1\(",
            r"new\s+SHA1",
            r"\.DES\.",
            r"\.TripleDES",
            r"CreditCard",
            r"\.SSN",
            r"\.SocialSecurity"
        ],
        "risk_level": 2,
        "recommendation": "Nu folosiți algoritmi criptografici slabi (MD5, SHA1, DES). Utilizați algoritmi recomandați (AES, SHA-256, bcrypt). Protejați tot traficul cu date sensibile prin HTTPS. Minimizați stocarea datelor sensibile."
    },
    # Lipsă control acces la nivel de funcție
    {
        "id": "function_access",
        "patterns": [
            r"\.IsAdmin\(\)",
            r"UserManager\.",
            r"\.GetCurrentUser\(\)",
            r"\.User\.",
            r"\.GetUserId\(",
            r"\.FindByIdAsync\("
        ],
        "risk_level": 1,
        "recommendation": "Implementați verificări de acces pentru toate funcțiile sensibile. Utilizați decoratori sau middlewares pentru a aplica verificări de acces consistente. Nu vă bazați doar pe ascunderea interfețelor UI."
    },
    # Cross-Site Request Forgery (CSRF)
    {
        "id": "csrf",
        "patterns": [
            r"\[HttpPost\](?!.*(?:\[ValidateAntiForgeryToken\]|\[AutoValidateAntiforgeryToken\]))",
            r"\[HttpPut\](?!.*(?:\[ValidateAntiForgeryToken\]|\[AutoValidateAntiforgeryToken\]))",
            r"\[HttpDelete\](?!.*(?:\[ValidateAntiForgeryToken\]|\[AutoValidateAntiforgeryToken\]))",
            r"app\.post\(",
            r"router\.post\(",
            r"fetch\(",
            r"axios\.post\(",
            r"\.ajax\("
        ],
        "risk_level": 2,
        "recommendation": "Implementați protecție CSRF pentru toate formularele și solicitările POST. Folosiți AntiForgeryToken în ASP.NET. Pentru API-uri moderne, folosiți tokens CSRF sau politici CORS corespunzătoare."
    },
    # Utilizarea componentelor cu vulnerabilități cunoscute
    {
        "id": "known_vulns",
        "patterns": [
            r"jQuery.{0,10}[\"']1\.[0-9]\.[0-9][\"']",
            r"bootstrap.{0,10}[\"']2\.[0-9]\.[0-9][\"']",
            r"angular.{0,10}[\"']1\.[0-9]\.[0-9][\"']",
            r"\.net\s+framework\s+4\.[0-5]",
            r"log4j",
            r"struts",
            r"jackson.{0,20}2\.[0-9]\.[0-9]"
        ],
        "risk_level": 2,
        "recommendation": "Actualizați toate bibliotecile și framework-urile la cele mai recente versiuni. Configurați alertele pentru vulnerabilități cunoscute. Implementați un proces de actualizare regulată a dependențelor."
    }
]


def analyze_code_with_patterns(code, language="csharp"):
    """
    Analizăm codul pentru potențiale vulnerabilități folosind reguli predefinite
    """
    vulnerabilities = []
    lines = code.split('\n')

    for rule in VULNERABILITY_RULES:
        matches = []
        for i, line in enumerate(lines):
            for pattern in rule["patterns"]:
                if re.search(pattern, line, re.IGNORECASE):
                    matches.append(i + 1)  # +1 pentru că numerele liniilor încep de la 1

        if matches:
            vuln_type = VULNERABILITY_TYPES.get(rule["id"], "Altă vulnerabilitate")
            risk_level = rule["risk_level"]

            # Eliminăm duplicatele din match-uri
            matches = list(set(matches))

            vulnerabilities.append({
                "vulnerability_type": vuln_type,
                "confidence": 0.85,  # Valoare fixă pentru reguli
                "risk_level": RISK_LEVELS.get(risk_level),
                "line_numbers": matches,
                "recommendation": rule["recommendation"]
            })

    return vulnerabilities


def analyze_code_with_ml(code):
    """
    Analizăm codul folosind modelul CodeBERT pentru detectarea codului nesigur
    """
    if not USE_ML_MODEL:
        return []

    try:
        # Împărțim codul în linii pentru referință
        lines = code.split('\n')
        vulnerabilities = []

        # Analizăm tot codul odată
        inputs = tokenizer(code, return_tensors="pt", truncation=True, padding='max_length')

        with torch.no_grad():
            outputs = model(**inputs)

        # Obținem predicțiile și probabilitățile
        predictions = torch.softmax(outputs.logits, dim=1).detach().numpy()[0]
        predicted_class = np.argmax(predictions)
        confidence = float(predictions[predicted_class])

        # Clasa 1 reprezintă "cod nesigur" pentru acest model
        if predicted_class == 1 and confidence > 0.6:
            security_info = SECURITY_CLASSES.get(1)

            vulnerabilities.append({
                "vulnerability_type": "Vulnerabilitate generală detectată",
                "confidence": confidence,
                "risk_level": RISK_LEVELS.get(security_info['risk_level']),
                "line_numbers": list(range(1, len(lines) + 1)),  # Toate liniile sunt potențial afectate
                "recommendation": "Codul a fost detectat ca potențial vulnerabil. Revizuiți-l pentru probleme de securitate și aplicați practici de codare securizată.",
                "ml_detection": True,
                "security_class": security_info['name']
            })

        # Analizăm și pe bucăți mai mici pentru a identifica secțiuni problematice
        window_size = 15  # Dimensiunea bucății de cod analizate
        window_overlap = 5  # Overlap între bucăți pentru a nu rata vulnerabilități la granițe

        for i in range(0, len(lines), window_size - window_overlap):
            window = '\n'.join(lines[i:i + window_size])

            # Dacă fereastra este prea mică, ignorăm
            if len(window.strip()) < 10:
                continue

            inputs = tokenizer(window, return_tensors="pt", truncation=True, padding='max_length')

            with torch.no_grad():
                outputs = model(**inputs)

            predictions = torch.softmax(outputs.logits, dim=1).detach().numpy()[0]
            predicted_class = np.argmax(predictions)
            confidence = float(predictions[predicted_class])

            if predicted_class == 1 and confidence > 0.7:  # Prag mai mare pentru bucăți individuale
                security_info = SECURITY_CLASSES.get(1)

                # Liniile exacte din această bucată
                line_numbers = list(range(i + 1, min(i + window_size + 1, len(lines) + 1)))

                # Verificăm dacă nu avem deja o vulnerabilitate similară în aceeași regiune
                if not any(v.get("ml_detection") and any(line in v["line_numbers"] for line in line_numbers) for v in
                           vulnerabilities):
                    # Identificăm tipurile potențiale de vulnerabilități bazate pe conținutul codului
                    potential_type = "Cod nesigur"
                    for rule in VULNERABILITY_RULES:
                        for pattern in rule["patterns"]:
                            if any(re.search(pattern, line, re.IGNORECASE) for line in lines[i:i + window_size]):
                                potential_type = VULNERABILITY_TYPES.get(rule["id"], "Vulnerabilitate nespecificată")
                                break

                    vulnerabilities.append({
                        "vulnerability_type": f"{potential_type} (detectată de ML)",
                        "confidence": confidence,
                        "risk_level": RISK_LEVELS.get(security_info['risk_level']),
                        "line_numbers": line_numbers,
                        "recommendation": "Această secțiune de cod conține potențiale probleme de securitate. Revizuiți și aplicați practici de codare securizată.",
                        "ml_detection": True,
                        "security_class": security_info['name']
                    })

        return vulnerabilities

    except Exception as e:
        print(f"Eroare la analiza ML: {e}")
        return []


@app.route('/analyze', methods=['POST'])
def analyze_code():
    if not request.json or 'code' not in request.json:
        return jsonify({'error': 'Nu a fost furnizat cod pentru analiză'}), 400

    code = request.json['code']
    language = request.json.get('language', 'csharp')  # Default la C# pentru .NET MVC

    # Analizăm codul folosind regex patterns
    pattern_vulnerabilities = analyze_code_with_patterns(code, language)

    # Dacă avem model ML, analizăm și cu acesta
    ml_vulnerabilities = []
    if USE_ML_MODEL:
        ml_vulnerabilities = analyze_code_with_ml(code)

    # Combinăm rezultatele, evitând duplicatele
    all_vulnerabilities = pattern_vulnerabilities.copy()

    # Adăugăm vulnerabilitățile detectate de ML care nu se suprapun cu cele detectate prin pattern-uri
    for ml_vuln in ml_vulnerabilities:
        # Verificăm dacă vulnerabilitatea ML se suprapune cu cele din pattern-uri
        overlapping = False
        for pattern_vuln in pattern_vulnerabilities:
            if any(line in ml_vuln["line_numbers"] for line in pattern_vuln["line_numbers"]):
                # Se suprapun, actualizăm încrederea vulnerabilității detectate prin pattern
                pattern_vuln["confidence"] = max(pattern_vuln["confidence"], ml_vuln["confidence"])
                overlapping = True
                break

        if not overlapping:
            all_vulnerabilities.append(ml_vuln)

    # Sortăm vulnerabilitățile după nivelul de risc (descrescător)
    risk_order = {"Critic": 4, "Ridicat": 3, "Mediu": 2, "Scăzut": 1, "Sigur": 0}
    all_vulnerabilities.sort(key=lambda x: (risk_order.get(x["risk_level"], 0), x["confidence"]), reverse=True)

    # Adăugăm o notă despre abordarea folosită
    approach = "hibridă (reguli + CodeBERT pentru detecție cod nesigur)" if USE_ML_MODEL else "bazată pe reguli"

    # Generăm raportul
    report = {
        "total_vulnerabilities": len(all_vulnerabilities),
        "vulnerabilities": all_vulnerabilities,
        "summary": {
            "critic": sum(1 for v in all_vulnerabilities if v["risk_level"] == "Critic"),
            "ridicat": sum(1 for v in all_vulnerabilities if v["risk_level"] == "Ridicat"),
            "mediu": sum(1 for v in all_vulnerabilities if v["risk_level"] == "Mediu"),
            "scazut": sum(1 for v in all_vulnerabilities if v["risk_level"] == "Scăzut")
        },
        "analysis_approach": approach
    }

    return jsonify(report)


@app.route('/health', methods=['GET'])
def health_check():
    return jsonify({'status': 'healthy'})


if __name__ == "__main__":
    app.run(debug=True, host='0.0.0.0', port=5000)