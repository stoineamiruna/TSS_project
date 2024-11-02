import subprocess, sys, os, re

# comenzile de executie in functie de limbaj
commands = {
    "python": {
        "filename": "python_code.py",
        "command": ["python", "python_code.py"]
    },
    "cpp": {
        "filename": "cpp_code.cpp",
        "command": "./cpp_code"
    }
}

# preia stringul cu test_cases, il parseaza si returneaza o lista de dictionare cu input si output pentru fiecare test
def prepare_test_cases(test_cases_string):
    test_cases = []
    tests = re.split('[\r\n]*Test case #[0-9]+:\r\n', test_cases_string, flags=re.IGNORECASE)
    print(tests)
    for test in tests:
        if len(test) == 0:
            continue
        m = re.match('Input\r\n(?P<inp>[^\r\n]+)\r\nOutput\r\n(?P<outp>[^\r\n]+)', test, flags=re.IGNORECASE)
        if m is None:
            raise ValueError('Invalid format of the test cases')
        inp = re.split(r'[(\r\n) ]+', m.group('inp'))
        outp = m.group('outp')
        test_cases.append(
            {
                "input": inp,
                "output": outp
            }
        )
    return test_cases

def write_code_to_file(code, filename): # mai tarziu rulez fisierul in linie de comanda
    with open(filename, 'w') as file:
        file.write(code)

def compile_cpp_code(filename):
    try:
        compilation_output = subprocess.check_output(
            ["g++", "-std=c++14", filename, "-o", "cpp_code"],
            stderr=subprocess.STDOUT,
            text=True
        )
        print("C++ code compiled successfully!")
    except subprocess.CalledProcessError as e:
        print(f"Error compiling C++ code: {e.output}")
        exit(1)

def execute_program(command, inputs):
    try:
        process = subprocess.Popen(
            command,
            stdin=subprocess.PIPE,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True
        )
        print("input:\n" + "\n".join(inputs) + "\n")
        actual_output, _ = process.communicate(input="\n".join(inputs) + "\n")
        return actual_output
    except subprocess.CalledProcessError as e:
        return e.output

# verific dc este python instalat
def find_python_env():
    try:
        subprocess.run(["python", "--version"], check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE) # comunicarea interprocese, redirectare proces copil (python program) catre procesul parinte
    except (subprocess.CalledProcessError, FileNotFoundError):
        try:
            subprocess.run(["python3", "--version"], check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
            # update comanda
            commands["python"]["command"] = ["python3", "python_code.py"]
        except (subprocess.CalledProcessError, FileNotFoundError):
            print("Python interpreter not found...")
            exit(1)

# rulez testele
def run_test_cases(test_cases, lang, commands):
    test_results = {}
    passed_tests = 0  # counter pentru nr de teste trecute

    for i, test_case in enumerate(test_cases):
        output = execute_program(commands[lang]["command"], test_case["input"])
        print(f"Test case #{i + 1}: {output}")
        # Compare output with expected output
        if output.strip(' \r\n') == test_case["output"]:
            test_results[str(i + 1)] = {"success": True, "output": output}
            passed_tests += 1  # Increment the count of passed tests
        else:
            test_results[str(i + 1)] = {"success": False, "actual_output": output, "expected_output": test_case["output"]}

    # Calculez scorul in functie de testele trecute
    total_tests = len(test_cases)
    if total_tests > 0:
        passed_percentage = (passed_tests / total_tests) * 100
    else:
        passed_percentage = 0
    return {
        "test_results": test_results,
        "score": passed_percentage
    }

# Clean up - stergere fisiere py, cpp
def cleanup(lang):
    if lang == "python":
        if os.path.exists(commands["python"]["filename"]):
            os.remove(commands["python"]["filename"])
    elif lang == "cpp":
        if os.path.exists(commands["cpp"]["filename"]):
            os.remove(commands["cpp"]["filename"])
        if os.path.exists("cpp_code"):
            os.remove("cpp_code")

# logica rulare fisiere
def execute_test_cases(code, test_cases, lang, commands):
    # lang = "python" # p/u testare schimbam numele in cpp / python
    if lang == "python":
        find_python_env()
        write_code_to_file(code, commands["python"]["filename"])
    elif lang == "cpp":
        write_code_to_file(code, commands["cpp"]["filename"])
        compile_cpp_code(commands["cpp"]["filename"])  # codul cpp trebuie compilat

    test_results = run_test_cases(test_cases, lang, commands)
    cleanup(lang)

    return test_results

