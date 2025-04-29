# Raport: Concepte și metode din articolul  
**„TestLab: An Intelligent Automated Software Testing Framework” [1]**

**Autori:**  
Stoinea Maria Miruna  
Nazare Elena-Denisa  
Ciurescu Irina Alexandra  
Antonescu Ionut-Andrei

---

## 1. Introducere / Context

Deoarece sistemele software sunt peste tot, ele nu mai sunt doar opționale sau de nișă, ci parte fundamentală a vieții de zi cu zi. Astfel, aplicațiile au devenit din ce în ce mai mari și mai complicate. Nu mai e vorba de aplicații simple, ci de sisteme complexe, cu sute de mii de linii de cod și componente interconectate. Această complexitate are un cost: durează mai mult să proiectezi, implementezi, testezi și întreții software. [2]

Articolul abordează problema calității scăzute a software-ului, cauzată adesea de testarea insuficientă sau inexistentă, din dorința de a accelera ciclul de dezvoltare. În acest context, este propus **TestLab**, un cadru inteligent pentru testarea automată a software-ului, care îmbină diverse metode de testare cu tehnici de Inteligență Artificială (IA) pentru a asigura testarea continuă și eficientă a aplicațiilor software.

TestLab este gândit să funcționeze pe toată durata dezvoltării software:
- **Dezvoltatori**: verifică codul sursă.
- **Testeri**: generează cazuri de test.
- **Utilizatori finali**: validează comportamentul aplicației.

Și face asta la toate nivelurile:
- unitate
- integrare
- sistem
- acceptanță

Este împărțit în 3 componente specializate, fiecare cu un rol: descoperă vulnerabilități în API-uri (**FuzzTheREST**), analizează codul pentru riscuri (**VulnRISKatcher**), generează automat teste (**CodeAssert**).

---

## 2. Concepte-cheie

Autorii subliniază cele două obiective fundamentale ale testării:
- **Validare**: sistemul construit răspunde cerințelor utilizatorului? (realizată de utilizatori și client)
- **Verificare**: implementarea respectă specificațiile și regulile? (realizată de o echipă tehnică)

### Testarea automată a software-ului:
Executarea testelor fără intervenție umană, cu scopul de a identifica erori și vulnerabilități în mod rapid și eficient.

### Testarea pe mai multe niveluri:
- **Unit testing** – pe componente izolate  
- **Integration testing** – interacțiunea dintre module  
- **System testing** – testarea întregului sistem  
- **Acceptance testing** – validare de către utilizatori

### Tipuri de testare:
- **White-box**: Analizează logica internă a codului.
- **Black-box**: Testează funcționalitatea fără a cunoaște implementarea internă.
- **Grey-box**: Combină abordările de mai sus.

### Inteligență Artificială în testare:
Utilizarea tehnicilor de machine learning și reinforcement learning pentru generarea automată de cazuri de test și detectarea vulnerabilităților.

După compararea unor framework-uri populare, autorii au concluzionat că nu există o soluție completă care să acopere toate tipurile de testare (black, white, grey), toate nivelurile (unit, integration, system, acceptance) și să integreze totul într-un singur cadru automatizat — de aici necesitatea și originalitatea TestLab. [3]

---

## 3. Structura și modulele TestLab

TestLab este compus din trei module principale:

### a. FuzzTheREST
- Fuzzer inteligent pentru API-uri REST, bazat pe Reinforcement Learning
- Funcționează în mod **black-box**, generând input-uri deformate și folosind feedback-ul API-ului
- Poate fi extins la **grey-box** prin analiza execuției codului

### b. VulnRISKatcher
- Detectează vulnerabilități în codul sursă, folosind modele de **machine learning**
- Suportă mai multe limbaje și funcționează pe fragmente de cod

### c. CodeAssert
- Automatizează generarea de scripturi de test folosind **analiza codului** și **NLP**
- Asigură acoperire de 100% pentru **testare unitară și de integrare** (white-box)

---

## 4. Exemplu practic

### a. Aplicația: Developer Toolbox

**Descriere:**  
Platformă interactivă pentru învățarea și exersarea programării. Utilizatorii pot adresa întrebări, rezolva exerciții, urmări progresul și participa la provocări săptămânale.

**Funcționalități principale:**
- Întrebări și răspunsuri tehnice
- Exerciții de programare cu editor integrat
- Puncte de reputație, medalii, clasamente
- Provocări săptămânale
- Administrare și moderare

**Beneficii:**
- Începătorii pot învăța și primi ajutor
- Utilizatorii avansați pot contribui și rezolva exerciții complexe
- Platforma încurajează competiția și învățarea continuă

**Tehnologii utilizate:**
- **ASP.NET Core, C#** – aplicație web
- **Entity Framework Core, LINQ** – gestionarea datelor
- **SQL Server** – stocarea datelor
- **JavaScript, HTML, CSS** – interfață
- **Python** – server backend și compilator

**Scopuri pentru testare:**  
Validarea funcționalităților aplicației: logare, gestionare întrebări/răspunsuri, exerciții, recompense.

---

### b. Implementarea primului modul: FuzzTheREST

**RESTler** [4] – un instrument de fuzzing pentru API-uri REST.

**Caracteristici:**
- Instrument „stateful”, analizează comportamentul anterior al aplicației
- Folosește **Reinforcement Learning** pentru generarea input-urilor
- Evită inputurile invalide, optimizează procesul de testare
- Poate integra analiza execuției pentru **grey-box testing**

**Cum funcționează RESTler:**
1. **Input de la tester**: fișier OpenAPI, scenarii, parametri RL
2. **Generare input-uri**: bazată pe feedback-ul de la API
3. **Evaluarea vulnerabilităților**: detectează erori/vulnerabilități și ajustează căutarea
4. **Raport final**: vulnerabilități descoperite, input-uri testate, metrice de acoperire

RESTler eficientizează testarea automată a API-urilor REST și poate identifica vulnerabilități greu de descoperit manual.

---

## c. Implementarea celui de-al doilea modul -- VulnRISKatcher

Așa cum reiese din articol, **VulnRISKatcher** este un instrument inovator pentru identificarea și clasificarea vulnerabilităților în codul sursă, utilizând tehnici avansate de Machine Learning. Spre deosebire de instrumentele tradiționale de revizuire a codului care folosesc abordări statice, VulnRISKatcher oferă o alternativă mai eficientă care nu necesită înțelegerea completă a contextului codului.

### Caracteristici principale:

1. **Utilizarea tehnicilor ML pentru verificarea codului** - Permite analiza codului fără a necesita contextul complet, făcând instrumentul aplicabil la diferite niveluri de testare.

2. **Suport pentru diverse limbaje de programare** - Instrumentul analizează cod lexical din multiple limbaje de programare, crescând versatilitatea soluției.

3. **Pipeline de procesare structurat** - Procesul include:
   - Furnizarea codului și specificarea limbajului de programare
   - Preprocesarea datelor (curățarea și divizarea în unități discrete)
   - Analiza pentru identificarea tiparelor asociate vulnerabilităților
   - Clasificarea vulnerabilităților identificate
   - Generarea unui raport detaliat pentru utilizator

4. **Identificarea și clasificarea riscurilor** - Pe lângă detectarea vulnerabilităților, instrumentul evaluează și riscurile asociate acestora.

VulnRISKatcher reprezintă un pas important în evoluția instrumentelor de testare software, oferind o soluție mai rapidă și mai precisă pentru identificarea vulnerabilităților, contribuind astfel la îmbunătățirea calității software-ului și la reducerea costurilor de dezvoltare și mentenanță.

### Proof of concept

Pentru a ne apropia în practică de conceptul teoretic de VulnRISKatcher am decis să încercăm să folosim un model de Machine Learning, întrucât învățarea automată este una dintre caracteristicile care stau la baza conceptului.

Am ales să folosim modele de pe HuggingFace și astfel am găsit modelul *microsoft/codebert-base*, care însă ar fi avut nevoie de fine-tuning, așa că am încercat să găsim un model deja fine-tunat. Acest lucru s-a dovedit a fi un task destul de complicat, dar în urma căutărilor am ajuns la două variante:

1. *mahdin70/CodeBERT-VulnCWE* - antrenat pe un set de date curatoriat și îmbogățit pentru detectarea vulnerabilităților și clasificarea CWE. Modelul poate prezice dacă un fragment de cod este vulnerabil și, în caz afirmativ, poate identifica ID-ul CWE specific asociat. (părea o variantă promițătoare, dar am avut probleme cu importarea acestuia, deci am fost nevoiți să renunțăm)

2. *mrm8488/codebert-base-finetuned-detect-insecure-code* - Modelul analizează secvențe de cod sursă pentru a determina dacă acestea conțin vulnerabilități de securitate (precum scurgeri de resurse sau atacuri DoS), clasificându-le binar ca sigure (0) sau nesigure (1). (deși ne oferă doar o clasificare binară a erorilor, acest model s-a dovedit a fi o variantă mai sigură și cu care am reușit să lucrăm)

Deși modelul oferă doar o clasificare binară (cod sigur vs. cod nesigur), l-am integrat într-o analiză mai nuanțată. Pentru a face analiza mai detaliată, am implementat:

- Analiza globală a codului pentru o evaluare generală a întregului fișier
- Analiza pe secțiuni consecutive de cod (denumită "ferestre glisante" sau "sliding windows") - aceasta împarte codul în bucăți mai mici de câte 15 linii, care se suprapun parțial (cu 5 linii), pentru a identifica mai precis zonele problematice din cod

Tot pentru a compensa limitarea clasificării binare a modelului ML, am implementat și un sistem extins de reguli bazat pe expresii regulate, care poate identifica zece categorii specifice de vulnerabilități:

#### SQL Injection
Detectează construirea nesigură a interogărilor SQL prin concatenare de șiruri

```python
"patterns": [
    r"string\s+sql\s*=.*\+",
    r"executeQuery\(.*\+",
    # ... alte modele
]
```




## 5. Concluzii

**TestLab** propune o abordare integrată, inteligentă și automatizată pentru testarea software:

- Testare continuă
- Acoperire completă
- Detectarea timpurie a erorilor
- Automatizare extinsă → reducerea efortului uman

---

## 6. Referințe

[1] Dias, T., Batista, A., Maia, E., & Praça, I. (2023). *TestLab: An Intelligent Automated Software Testing Framework*. Research Group on Intelligent Engineering and Computing for Advanced Innovation and Development (GECAD), ISEP, Porto.

[2] „The prevalence of software systems has become an integral part of modern-day living.” / „Software usage has increased significantly, leading to its growth in both size and complexity.” / „Consequently, software development is becoming a more time-consuming process.” – din articolul TestLab

[3] „The integration of Artificial Intelligence (AI) in the software testing process is promising...” / „...no prior work has addressed the development of a comprehensive framework comprising multiple testing methods...” – din articolul TestLab

[4] Microsoft RESTler: Stateful REST API Fuzzing Tool

