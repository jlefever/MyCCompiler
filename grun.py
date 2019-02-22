from os import chdir, makedirs
from os.path import abspath, isfile, join
from subprocess import run
from sys import argv
from urllib.request import urlretrieve

# Settings
ANTLR_URL = "https://www.antlr.org/download/antlr-4.7.2-complete.jar"
ANTLR_JAR = "antlr.jar"
GRAMMAR = "C"
GRAMMAR_DIR = "./MyCCompiler/"
ROOT_PRODUCTION = "compilationUnit"
WORK_DIR = "./.grun/"

# Derived values
cp = ".;" + abspath(join(WORK_DIR, ANTLR_JAR))
grammar_file = abspath(join(GRAMMAR_DIR, GRAMMAR + ".g4"))
input_file = abspath(argv[2])
flag = argv[1]
java = ["java", "-cp", cp]

# Ensure WORK_DIR exists
makedirs(WORK_DIR, exist_ok=True)

# Change into WORK_DIR
chdir(WORK_DIR)

# Download Antrl4
if not isfile(ANTLR_JAR):
    print("Downloading {}...".format(ANTLR_URL))
    urlretrieve(ANTLR_URL, ANTLR_JAR)

# Generate parser and lexer
if not isfile(GRAMMAR + "Parser.java"):
    print("Generating {} parser and lexer...".format(GRAMMAR))
    run(java + ["org.antlr.v4.Tool", "-o", ".", grammar_file], check=True)

# Compile parser and lexer
if not isfile(GRAMMAR + "Parser.class"):
    print("Compiling {} parser and lexer...".format(GRAMMAR))
    run(["javac", "-cp", cp, "*.java"], check=True)

# Run grun
print("Running grun...")
grun = "org.antlr.v4.gui.TestRig"
args = [grun, GRAMMAR, ROOT_PRODUCTION, flag, input_file]
run(java + args, check=True)