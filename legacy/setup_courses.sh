#!/bin/bash

# Enhanced grading script with multi-course support and AI analysis using Ollama
# Includes abstracted file handling for any assignment requiring data files

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Configuration
RUBRIC_DIR="$HOME/bin/rubrics"
OLLAMA_MODEL="llama3.1:8b"
SELECTED_COURSE=""
SELECTED_WEEK=""
SELECTED_LANGUAGE=""
SOURCE_FILE=""

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_ai() {
    echo -e "${CYAN}[AI ANALYSIS]${NC} $1"
}

print_section() {
    echo -e "${BLUE}==== $1 ====${NC}"
}

# Function to convert to uppercase (zsh compatible)
to_upper() {
    echo "$1" | tr '[:lower:]' '[:upper:]'
}

# Function to select course
select_course() {
    print_section "COURSE SELECTION"
    
    # Check if rubrics directory exists
    if [ ! -d "$RUBRIC_DIR" ]; then
        print_warning "Rubrics directory not found. Creating $RUBRIC_DIR"
        mkdir -p "$RUBRIC_DIR"
        print_status "Please create course directories in $RUBRIC_DIR"
        return 1
    fi
    
    # List available courses (directories in rubrics folder)
    course_dirs=($(find "$RUBRIC_DIR" -maxdepth 1 -type d -not -path "$RUBRIC_DIR" | sort))
    
    if [ ${#course_dirs[@]} -eq 0 ]; then
        print_error "No course directories found in $RUBRIC_DIR"
        print_status "Create course directories like: $RUBRIC_DIR/cse111, $RUBRIC_DIR/cse210"
        return 1
    fi
    
    # Extract course names from paths
    courses=()
    for dir in "${course_dirs[@]}"; do
        course_name=$(basename "$dir")
        courses+=("$course_name")
    done
    
    echo "Available courses:"
    for i in "${!courses[@]}"; do
        course_upper=$(to_upper "${courses[$i]}")
        echo "$((i+1)). $course_upper"
    done
    
    echo
    while true; do
        read -p "Select course (1-${#courses[@]}) or 'q' to quit: " choice
        
        if [ "$choice" = "q" ]; then
            print_status "Exiting..."
            exit 0
        fi
        
        if [[ "$choice" =~ ^[0-9]+$ ]] && [ "$choice" -ge 1 ] && [ "$choice" -le ${#courses[@]} ]; then
            SELECTED_COURSE="${courses[$((choice-1))]}"
            course_upper=$(to_upper "$SELECTED_COURSE")
            print_status "Selected course: $course_upper"
            break
        else
            echo "Please enter a valid number (1-${#courses[@]}) or 'q' to quit."
        fi
    done
    
    return 0
}

# Function to detect and select programming language
select_language() {
    print_section "PROGRAMMING LANGUAGE DETECTION"
    
    # Look for common source files
    python_files=(*.py)
    csharp_files=(*.cs)
    javascript_files=(*.js)
    html_files=(*.html *.htm)
    css_files=(*.css)
    java_files=(*.java)
    cpp_files=(*.cpp *.cc *.cxx)
    c_files=(*.c)
    php_files=(*.php)
    ruby_files=(*.rb)
    go_files=(*.go)
    rust_files=(*.rs)
    
    # Check if files actually exist (not just globs)
    if [ ! -e "${python_files[0]}" ]; then python_files=(); fi
    if [ ! -e "${csharp_files[0]}" ]; then csharp_files=(); fi
    if [ ! -e "${javascript_files[0]}" ]; then javascript_files=(); fi
    if [ ! -e "${html_files[0]}" ]; then html_files=(); fi
    if [ ! -e "${css_files[0]}" ]; then css_files=(); fi
    if [ ! -e "${java_files[0]}" ]; then java_files=(); fi
    if [ ! -e "${cpp_files[0]}" ]; then cpp_files=(); fi
    if [ ! -e "${c_files[0]}" ]; then c_files=(); fi
    if [ ! -e "${php_files[0]}" ]; then php_files=(); fi
    if [ ! -e "${ruby_files[0]}" ]; then ruby_files=(); fi
    if [ ! -e "${go_files[0]}" ]; then go_files=(); fi
    if [ ! -e "${rust_files[0]}" ]; then rust_files=(); fi
    
    # Determine available languages
    languages=()
    if [ ${#python_files[@]} -gt 0 ]; then languages+=("python"); fi
    if [ ${#csharp_files[@]} -gt 0 ]; then languages+=("csharp"); fi
    if [ ${#javascript_files[@]} -gt 0 ]; then languages+=("javascript"); fi
    if [ ${#html_files[@]} -gt 0 ]; then languages+=("html"); fi
    if [ ${#css_files[@]} -gt 0 ]; then languages+=("css"); fi
    if [ ${#java_files[@]} -gt 0 ]; then languages+=("java"); fi
    if [ ${#cpp_files[@]} -gt 0 ]; then languages+=("cpp"); fi
    if [ ${#c_files[@]} -gt 0 ]; then languages+=("c"); fi
    if [ ${#php_files[@]} -gt 0 ]; then languages+=("php"); fi
    if [ ${#ruby_files[@]} -gt 0 ]; then languages+=("ruby"); fi
    if [ ${#go_files[@]} -gt 0 ]; then languages+=("go"); fi
    if [ ${#rust_files[@]} -gt 0 ]; then languages+=("rust"); fi
    
    if [ ${#languages[@]} -eq 0 ]; then
        print_error "No supported source files found"
        print_status "Supported extensions: .py .cs .js .html .htm .css .java .cpp .c .php .rb .go .rs"
        return 1
    elif [ ${#languages[@]} -eq 1 ]; then
        # Auto-select if only one language found
        SELECTED_LANGUAGE="${languages[0]}"
        
        case "$SELECTED_LANGUAGE" in
            "python") lang_display="Python"; SOURCE_FILE="${python_files[0]}" ;;
            "csharp") lang_display="C#"; SOURCE_FILE="${csharp_files[0]}" ;;
            "javascript") lang_display="JavaScript"; SOURCE_FILE="${javascript_files[0]}" ;;
            "html") lang_display="HTML"; SOURCE_FILE="${html_files[0]}" ;;
            "css") lang_display="CSS"; SOURCE_FILE="${css_files[0]}" ;;
            "java") lang_display="Java"; SOURCE_FILE="${java_files[0]}" ;;
            "cpp") lang_display="C++"; SOURCE_FILE="${cpp_files[0]}" ;;
            "c") lang_display="C"; SOURCE_FILE="${c_files[0]}" ;;
            "php") lang_display="PHP"; SOURCE_FILE="${php_files[0]}" ;;
            "ruby") lang_display="Ruby"; SOURCE_FILE="${ruby_files[0]}" ;;
            "go") lang_display="Go"; SOURCE_FILE="${go_files[0]}" ;;
            "rust") lang_display="Rust"; SOURCE_FILE="${rust_files[0]}" ;;
        esac
        
        print_status "Auto-detected: $lang_display"
        print_status "Using source file: $SOURCE_FILE"
    else
        # Multiple languages found, let user choose
        echo "Multiple languages detected:"
        for i in "${!languages[@]}"; do
            case "${languages[$i]}" in
                "python") lang_display="Python" ;;
                "csharp") lang_display="C#" ;;
                "javascript") lang_display="JavaScript" ;;
                "html") lang_display="HTML" ;;
                "css") lang_display="CSS" ;;
                "java") lang_display="Java" ;;
                "cpp") lang_display="C++" ;;
                "c") lang_display="C" ;;
                "php") lang_display="PHP" ;;
                "ruby") lang_display="Ruby" ;;
                "go") lang_display="Go" ;;
                "rust") lang_display="Rust" ;;
            esac
            echo "$((i+1)). $lang_display"
        done
        
        echo
        while true; do
            read -p "Select language (1-${#languages[@]}): " choice
            
            if [[ "$choice" =~ ^[0-9]+$ ]] && [ "$choice" -ge 1 ] && [ "$choice" -le ${#languages[@]} ]; then
                SELECTED_LANGUAGE="${languages[$((choice-1))]}"
                
                case "$SELECTED_LANGUAGE" in
                    "python") lang_display="Python"; files_array=("${python_files[@]}") ;;
                    "csharp") lang_display="C#"; files_array=("${csharp_files[@]}") ;;
                    "javascript") lang_display="JavaScript"; files_array=("${javascript_files[@]}") ;;
                    "html") lang_display="HTML"; files_array=("${html_files[@]}") ;;
                    "css") lang_display="CSS"; files_array=("${css_files[@]}") ;;
                    "java") lang_display="Java"; files_array=("${java_files[@]}") ;;
                    "cpp") lang_display="C++"; files_array=("${cpp_files[@]}") ;;
                    "c") lang_display="C"; files_array=("${c_files[@]}") ;;
                    "php") lang_display="PHP"; files_array=("${php_files[@]}") ;;
                    "ruby") lang_display="Ruby"; files_array=("${ruby_files[@]}") ;;
                    "go") lang_display="Go"; files_array=("${go_files[@]}") ;;
                    "rust") lang_display="Rust"; files_array=("${rust_files[@]}") ;;
                esac
                
                print_status "Selected: $lang_display"
                break
            else
                echo "Please enter a valid number (1-${#languages[@]})."
            fi
        done
        
        # Select source file based on language
        if [ ${#files_array[@]} -eq 1 ]; then
            SOURCE_FILE="${files_array[0]}"
        else
            echo "Multiple $lang_display files found:"
            for i in "${!files_array[@]}"; do
                echo "$((i+1)). ${files_array[$i]}"
            done
            
            while true; do
                read -p "Select $lang_display file (1-${#files_array[@]}): " choice
                if [[ "$choice" =~ ^[0-9]+$ ]] && [ "$choice" -ge 1 ] && [ "$choice" -le ${#files_array[@]} ]; then
                    SOURCE_FILE="${files_array[$((choice-1))]}"
                    break
                fi
            done
        fi
        
        print_status "Using source file: $SOURCE_FILE"
    fi
    
    return 0
}

# Function to run the appropriate compiler/interpreter
run_program() {
    print_section "EXECUTING PROGRAM"
    
    case "$SELECTED_LANGUAGE" in
        "python")
            print_status "Running $SOURCE_FILE with Python 3..."
            python3 "$SOURCE_FILE"
            return $?
            ;;
        "csharp")
            print_status "Compiling and running $SOURCE_FILE with C#..."
            
            # Check if dotnet is available
            if command -v dotnet &> /dev/null; then
                if [ -f "*.csproj" ] || [ -f "Program.cs" ]; then
                    dotnet run
                else
                    dotnet build
                    if [ $? -eq 0 ]; then
                        dotnet run
                    else
                        print_error "C# compilation failed"
                        return 1
                    fi
                fi
            elif command -v mcs &> /dev/null; then
                output_file="${SOURCE_FILE%.cs}.exe"
                mcs "$SOURCE_FILE" -out:"$output_file"
                if [ $? -eq 0 ]; then
                    mono "$output_file"
                    rm -f "$output_file"
                else
                    print_error "C# compilation failed"
                    return 1
                fi
            else
                print_error "No C# compiler found (dotnet or mcs required)"
                return 1
            fi
            return $?
            ;;
        "javascript")
            print_status "Running $SOURCE_FILE with Node.js..."
            if command -v node &> /dev/null; then
                node "$SOURCE_FILE"
                return $?
            else
                print_error "Node.js not found. Install Node.js to run JavaScript files."
                return 1
            fi
            ;;
        "html")
            print_status "Opening $SOURCE_FILE in default browser..."
            if command -v xdg-open &> /dev/null; then
                xdg-open "$SOURCE_FILE"
            elif command -v open &> /dev/null; then
                open "$SOURCE_FILE"
            elif command -v start &> /dev/null; then
                start "$SOURCE_FILE"
            else
                print_status "Cannot automatically open browser. Please manually open: $SOURCE_FILE"
            fi
            print_status "HTML file opened in browser. Press Enter when ready to continue..."
            read
            return 0
            ;;
        "css")
            print_status "CSS files are stylesheets and cannot be executed directly."
            print_status "Displaying CSS content for review..."
            echo "----------------------------------------"
            cat "$SOURCE_FILE"
            echo "----------------------------------------"
            return 0
            ;;
        "java")
            print_status "Compiling and running $SOURCE_FILE with Java..."
            if command -v javac &> /dev/null && command -v java &> /dev/null; then
                javac "$SOURCE_FILE"
                if [ $? -eq 0 ]; then
                    class_name=$(basename "$SOURCE_FILE" .java)
                    java "$class_name"
                    # Clean up class file
                    rm -f "${class_name}.class"
                else
                    print_error "Java compilation failed"
                    return 1
                fi
            else
                print_error "Java compiler (javac) or runtime (java) not found"
                return 1
            fi
            return $?
            ;;
        "cpp")
            print_status "Compiling and running $SOURCE_FILE with C++..."
            if command -v g++ &> /dev/null; then
                output_file="${SOURCE_FILE%.*}"
                g++ "$SOURCE_FILE" -o "$output_file"
                if [ $? -eq 0 ]; then
                    "./$output_file"
                    rm -f "$output_file"
                else
                    print_error "C++ compilation failed"
                    return 1
                fi
            else
                print_error "g++ compiler not found"
                return 1
            fi
            return $?
            ;;
        "c")
            print_status "Compiling and running $SOURCE_FILE with C..."
            if command -v gcc &> /dev/null; then
                output_file="${SOURCE_FILE%.*}"
                gcc "$SOURCE_FILE" -o "$output_file"
                if [ $? -eq 0 ]; then
                    "./$output_file"
                    rm -f "$output_file"
                else
                    print_error "C compilation failed"
                    return 1
                fi
            else
                print_error "gcc compiler not found"
                return 1
            fi
            return $?
            ;;
        "php")
            print_status "Running $SOURCE_FILE with PHP..."
            if command -v php &> /dev/null; then
                php "$SOURCE_FILE"
                return $?
            else
                print_error "PHP interpreter not found"
                return 1
            fi
            ;;
        "ruby")
            print_status "Running $SOURCE_FILE with Ruby..."
            if command -v ruby &> /dev/null; then
                ruby "$SOURCE_FILE"
                return $?
            else
                print_error "Ruby interpreter not found"
                return 1
            fi
            ;;
        "go")
            print_status "Running $SOURCE_FILE with Go..."
            if command -v go &> /dev/null; then
                go run "$SOURCE_FILE"
                return $?
            else
                print_error "Go compiler not found"
                return 1
            fi
            ;;
        "rust")
            print_status "Compiling and running $SOURCE_FILE with Rust..."
            if command -v rustc &> /dev/null; then
                output_file="${SOURCE_FILE%.*}"
                rustc "$SOURCE_FILE" -o "$output_file"
                if [ $? -eq 0 ]; then
                    "./$output_file"
                    rm -f "$output_file"
                else
                    print_error "Rust compilation failed"
                    return 1
                fi
            else
                print_error "Rust compiler (rustc) not found"
                return 1
            fi
            return $?
            ;;
        *)
            print_error "Unsupported language: $SELECTED_LANGUAGE"
            return 1
            ;;
    esac
}

# Function to select assignment week
select_week() {
    print_section "ASSIGNMENT SELECTION"
    
    # Build path to course/language rubrics
    course_lang_dir="$RUBRIC_DIR/$SELECTED_COURSE/$SELECTED_LANGUAGE"
    
    if [ ! -d "$course_lang_dir" ]; then
        print_warning "Rubrics directory not found: $course_lang_dir"
        mkdir -p "$course_lang_dir"
        print_status "Please create rubric files in $course_lang_dir"
        return 1
    fi
    
    # List available rubric files
    rubric_files=($(ls "$course_lang_dir"/*.txt 2>/dev/null))
    
    if [ ${#rubric_files[@]} -eq 0 ]; then
        print_error "No rubric files found in $course_lang_dir"
        print_status "Create rubric files like: week01.txt, week02.txt, etc."
        return 1
    fi
    
    course_upper=$(to_upper "$SELECTED_COURSE")
    echo "Available assignments for $course_upper ($SELECTED_LANGUAGE):"
    for i in "${!rubric_files[@]}"; do
        filename=$(basename "${rubric_files[$i]}" .txt)
        echo "$((i+1)). $filename"
    done
    
    echo
    while true; do
        read -p "Select assignment (1-${#rubric_files[@]}) or 'q' to quit: " choice
        
        if [ "$choice" = "q" ]; then
            print_status "Exiting..."
            exit 0
        fi
        
        if [[ "$choice" =~ ^[0-9]+$ ]] && [ "$choice" -ge 1 ] && [ "$choice" -le ${#rubric_files[@]} ]; then
            SELECTED_WEEK="${rubric_files[$((choice-1))]}"
            selected_name=$(basename "$SELECTED_WEEK" .txt)
            print_status "Selected: $selected_name"
            break
        else
            echo "Please enter a valid number (1-${#rubric_files[@]}) or 'q' to quit."
        fi
    done
    
    return 0
}

# Generic function to setup assignment required files
setup_assignment_files() {
    local assignment_name=$(basename "$SELECTED_WEEK" .txt)
    
    # Keep track of files we copy for exclusion from output display
    COPIED_FILES=()
    
    # Special handling for Week 02 CSE 111 Python (hardcoded for reliability)
    if [[ "$assignment_name" == "week02" && "$SELECTED_COURSE" == "cse111" && "$SELECTED_LANGUAGE" == "python" ]]; then
        local wordlist_source="$HOME/bin/rubrics/cse111/python/week02_data/wordlist.txt"
        local passwords_source="$HOME/bin/rubrics/cse111/python/week02_data/toppasswords.txt"
        
        local files_copied=0
        
        if [ -f "$wordlist_source" ] && [ ! -f "wordlist.txt" ]; then
            cp "$wordlist_source" "wordlist.txt"
            print_status "Copied wordlist.txt for Week 02"
            COPIED_FILES+=("wordlist.txt")
            files_copied=$((files_copied + 1))
        fi
        
        if [ -f "$passwords_source" ] && [ ! -f "toppasswords.txt" ]; then
            cp "$passwords_source" "toppasswords.txt"
            print_status "Copied toppasswords.txt for Week 02"
            COPIED_FILES+=("toppasswords.txt")
            files_copied=$((files_copied + 1))
        fi
        
        if [ $files_copied -gt 0 ]; then
            print_status "Week 02 password files setup complete"
        fi
        
        return 0
    fi
    
    # Generic handling for other assignments
    local data_dir="$RUBRIC_DIR/$SELECTED_COURSE/$SELECTED_LANGUAGE/${assignment_name}_data"
    
    # Check if data directory exists for this assignment
    if [ ! -d "$data_dir" ]; then
        return 0  # No data files needed for this assignment
    fi
    
    print_status "Setting up required files for $assignment_name..."
    
    # Find all files in the data directory
    local files_copied=0
    for source_file in "$data_dir"/*; do
        if [ -f "$source_file" ]; then
            local filename=$(basename "$source_file")
            
            # Only copy if file doesn't already exist in current directory
            if [ ! -f "$filename" ]; then
                cp "$source_file" "$filename"
                print_status "Copied $filename from data directory"
                COPIED_FILES+=("$filename")
                files_copied=$((files_copied + 1))
            fi
        fi
    done
    
    if [ $files_copied -gt 0 ]; then
        print_status "Successfully copied $files_copied file(s)"
    fi
}

# Generic function to cleanup assignment files
cleanup_assignment_files() {
    local assignment_name=$(basename "$SELECTED_WEEK" .txt)
    local data_dir="$RUBRIC_DIR/$SELECTED_COURSE/$SELECTED_LANGUAGE/${assignment_name}_data"
    
    # Check if data directory exists for this assignment
    if [ -d "$data_dir" ]; then
        # Add all files that exist in both data directory and current directory to cleanup
        for source_file in "$data_dir"/*; do
            if [ -f "$source_file" ]; then
                local filename=$(basename "$source_file")
                
                # Only remove if the file exists in current directory and in our data directory
                # (meaning we likely copied it)
                if [ -f "$filename" ]; then
                    files_to_remove+=("$filename")
                fi
            fi
        done
    fi
}

# Function to check if Ollama is available
check_ollama() {
    if ! command -v ollama &> /dev/null; then
        print_warning "Ollama not found. AI analysis will be skipped."
        return 1
    fi
    
    # Check if the model is available
    if ! ollama list | grep -q "$OLLAMA_MODEL"; then
        print_warning "Model $OLLAMA_MODEL not found. Run: ollama pull $OLLAMA_MODEL"
        return 1
    fi
    
    return 0
}

# Function to analyze code with AI
analyze_code() {
    local source_file="$1"
    local output_files=("${@:2}")
    
    print_section "AI CODE ANALYSIS"
    
    # Read rubric if it exists
    local rubric_content=""
    if [ -f "$SELECTED_WEEK" ]; then
        rubric_content=$(cat "$SELECTED_WEEK")
        course_upper=$(to_upper "$SELECTED_COURSE")
        print_ai "Using rubric: $course_upper $(basename "$SELECTED_WEEK")"
    else
        print_warning "Selected rubric file not found: $SELECTED_WEEK"
        rubric_content="General code quality and correctness assessment"
    fi
    
    # Prepare the analysis prompt
    local lang_display=""
    case "$SELECTED_LANGUAGE" in
        "python") lang_display="Python" ;;
        "csharp") lang_display="C#" ;;
        "javascript") lang_display="JavaScript" ;;
        "html") lang_display="HTML" ;;
        "css") lang_display="CSS" ;;
        "java") lang_display="Java" ;;
        "cpp") lang_display="C++" ;;
        "c") lang_display="C" ;;
        "php") lang_display="PHP" ;;
        "ruby") lang_display="Ruby" ;;
        "go") lang_display="Go" ;;
        "rust") lang_display="Rust" ;;
        *) lang_display="$SELECTED_LANGUAGE" ;;
    esac
    
    local course_upper=$(to_upper "$SELECTED_COURSE")
    local assignment_name=$(basename "$SELECTED_WEEK" .txt)
    
    local prompt="You are grading $course_upper $assignment_name. Grade ONLY based on the rubric criteria below. Do NOT add extra requirements or subjective opinions.

RUBRIC CRITERIA TO CHECK:
$rubric_content

SOURCE CODE:
$(cat "$source_file")

OUTPUT FILES:"
    
    # Add output file contents
    for file in "${output_files[@]}"; do
        if [ -f "$file" ]; then
            prompt="$prompt
$file: $(cat "$file")"
        fi
    done
    
    prompt="$prompt

STRICT GRADING RULES:
- Grade ONLY what the rubric explicitly states
- Do NOT penalize for things not mentioned in the rubric  
- Do NOT add your own requirements (error handling, comments, etc.)
- Use ONLY Complete/Developing/Missing categories as defined
- If rubric says 'Complete: does X' and code does X, give Complete points
- Do NOT give partial credit for subjective 'best practices'

GRADING FORMAT:
1. [Criterion Name]: [Complete/Developing/Missing] ([Points]) - [Brief factual reason if not Complete]
2. [Criterion Name]: [Complete/Developing/Missing] ([Points]) - [Brief factual reason if not Complete]
[Continue for all criteria...]

TOTAL GRADE: ___/100 points

ACTUAL ISSUES: [Only list items that fail rubric requirements]

Be objective and literal. Grade what IS there, not what COULD be better."
    
    # Send to Ollama
    print_ai "Analyzing $lang_display code with AI..."
    echo "$prompt" | ollama run "$OLLAMA_MODEL"
}

# Function to get confirmation with AI insights
get_confirmation_with_ai() {
    echo
    print_section "GRADING CONFIRMATION"
    echo "Based on the AI analysis above and your own review:"
    echo
    
    while true; do
        read -p "Have you finished grading and reviewing the AI feedback? (y/n): " yn
        case $yn in
            [Yy]* ) 
                print_status "Proceeding with cleanup..."
                break
                ;;
            [Nn]* ) 
                echo
                read -p "Would you like to see the AI analysis again? (y/n): " show_again
                if [[ $show_again == [Yy]* ]]; then
                    analyze_code "$1" "${@:2}"
                fi
                print_status "Take your time to finish grading..."
                echo
                ;;
            * ) 
                echo "Please answer yes (y) or no (n)."
                ;;
        esac
    done
}

# Main script execution starts here
print_section "AUTOMATED GRADING SCRIPT"

# Select course first
if ! select_course; then
    exit 1
fi

# Detect and select programming language
if ! select_language; then
    exit 1
fi

# Select assignment/week
if ! select_week; then
    exit 1
fi

# Setup required files for any assignment
setup_assignment_files

# Check if source file exists
if [ ! -f "$SOURCE_FILE" ]; then
    print_error "$SOURCE_FILE not found in current directory!"
    exit 1
fi

print_status "Found $SOURCE_FILE"

# Display source code content
print_section "SOURCE CODE CONTENT"
print_status "Contents of $SOURCE_FILE:"
echo "----------------------------------------"
cat "$SOURCE_FILE"
echo "----------------------------------------"
echo

# Run the program
if ! run_program; then
    print_error "Program failed to run successfully!"
    read -p "Continue with grading anyway? (y/n): " continue_anyway
    if [[ ! $continue_anyway == [Yy]* ]]; then
        exit 1
    fi
fi

# Look for possible output files (language-agnostic)
possible_output_files=("volumes.txt" "volume.txt" "output.txt" "results.txt" "data.txt" "report.txt")
output_files=()

# Files to exclude from output display (these are input/data files we copied)
exclude_files=("wordlist.txt" "toppasswords.txt")

# Find which output files were actually created
for file in "${possible_output_files[@]}"; do
    if [ -f "$file" ]; then
        output_files+=("$file")
    fi
done

# Check for any text files that might have been created recently
for file in *.txt; do
    if [ -f "$file" ]; then
        # Skip if it's a file we copied (tracked in COPIED_FILES array)
        if [[ " ${COPIED_FILES[@]} " =~ " ${file} " ]]; then
            print_status "Skipping display of copied data file: $file"
            continue
        fi
        
        # Skip if it's in our static exclude list
        if [[ " ${exclude_files[@]} " =~ " ${file} " ]]; then
            print_status "Skipping display of excluded file: $file"
            continue
        fi
        
        # Skip if already in output_files array
        if [[ ! " ${output_files[@]} " =~ " ${file} " ]]; then
            # Only include if it was created recently (within 5 minutes)
            # AND it's not a data file we copied
            if [ -n "$(find "$file" -mmin -5 2>/dev/null)" ]; then
                output_files+=("$file")
            fi
        fi
    fi
done

# Check for common data files (CSV, JSON, XML)
for file in *.csv *.json *.xml; do
    if [ -f "$file" ]; then
        if [ $(find "$file" -mmin -5 2>/dev/null) ]; then
            output_files+=("$file")
        fi
    fi
done

# Language-specific output patterns
case "$SELECTED_LANGUAGE" in
    "csharp")
        for file in *.out *.log; do
            if [ -f "$file" ]; then
                if [ $(find "$file" -mmin -5 2>/dev/null) ]; then
                    output_files+=("$file")
                fi
            fi
        done
        ;;
    "java")
        for file in *.class; do
            if [ -f "$file" ]; then
                if [ $(find "$file" -mmin -5 2>/dev/null) ]; then
                    output_files+=("$file")
                fi
            fi
        done
        ;;
    "html")
        # For HTML, also check for generated CSS/JS files
        for file in *.css *.js; do
            if [ -f "$file" ]; then
                if [ $(find "$file" -mmin -5 2>/dev/null) ]; then
                    output_files+=("$file")
                fi
            fi
        done
        ;;
    "javascript")
        # Check for generated files from JS execution
        for file in *.log *.out; do
            if [ -f "$file" ]; then
                if [ $(find "$file" -mmin -5 2>/dev/null) ]; then
                    output_files+=("$file")
                fi
            fi
        done
        ;;
esac

# Report on output files found
if [ ${#output_files[@]} -eq 0 ]; then
    print_warning "No output files were found. Expected files like volumes.txt, volume.txt, etc."
else
    print_status "Found ${#output_files[@]} output file(s): ${output_files[*]}"
fi

# Show the contents of output files for review
print_section "OUTPUT FILE CONTENTS"
for file in "${output_files[@]}"; do
    if [ -f "$file" ]; then
        echo
        print_status "Contents of $file:"
        echo "----------------------------------------"
        
        # Get file size and line count for intelligent display
        local file_size=$(wc -c < "$file" 2>/dev/null || echo "0")
        local line_count=$(wc -l < "$file" 2>/dev/null || echo "0")
        
        # Skip displaying very large data files (like wordlist.txt, toppasswords.txt)
        if [[ "$file" == "wordlist.txt" || "$file" == "toppasswords.txt" ]]; then
            echo "📁 Large data file detected ($(wc -l < "$file") lines)"
            echo "First 10 lines:"
            head -10 "$file"
            echo "..."
            echo "Last 5 lines:"
            tail -5 "$file"
            echo "ℹ️  File too large to display completely ($line_count lines, $(numfmt --to=iec $file_size) bytes)"
        elif [[ "$file" == *.csv ]]; then
            # For CSV files, show first 10 lines to avoid overwhelming output
            if [ $line_count -gt 20 ]; then
                head -10 "$file"
                echo "... (file has $line_count total lines, showing first 10)"
            else
                cat "$file"
            fi
        elif [[ "$file" == *.json ]]; then
            # For JSON files, show formatted if jq is available, otherwise raw
            if command -v jq &> /dev/null; then
                if [ $file_size -gt 10240 ]; then  # > 10KB
                    echo "📄 Large JSON file ($(numfmt --to=iec $file_size) bytes)"
                    head -50 "$file" | jq '.' 2>/dev/null || head -50 "$file"
                    echo "... (truncated for readability)"
                else
                    jq '.' "$file" 2>/dev/null || cat "$file"
                fi
            else
                if [ $line_count -gt 50 ]; then
                    head -50 "$file"
                    echo "... (showing first 50 lines of $line_count total)"
                else
                    cat "$file"
                fi
            fi
        elif [[ "$file" == *.xml ]]; then
            # For XML files, show formatted if xmllint is available
            if [ $line_count -gt 50 ]; then
                if command -v xmllint &> /dev/null; then
                    head -50 "$file" | xmllint --format - 2>/dev/null || head -50 "$file"
                else
                    head -50 "$file"
                fi
                echo "... (showing first 50 lines of $line_count total)"
            else
                if command -v xmllint &> /dev/null; then
                    xmllint --format "$file" 2>/dev/null || cat "$file"
                else
                    cat "$file"
                fi
            fi
        elif [[ "$file" == *.html ]]; then
            # For HTML files, show content but truncate if too long
            if [ $line_count -gt 100 ]; then
                head -50 "$file"
                echo "... (showing first 50 lines of $line_count total)"
                tail -20 "$file"
            else
                cat "$file"
            fi
        elif [[ "$file" == *.css ]]; then
            # For CSS files, truncate if very long
            if [ $line_count -gt 100 ]; then
                head -50 "$file"
                echo "... (showing first 50 lines of $line_count total)"
            else
                cat "$file"
            fi
        elif [[ "$file" == *.js ]]; then
            # For JavaScript files, show content but truncate if very long
            if [ $line_count -gt 100 ]; then
                head -50 "$file"
                echo "... (showing first 50 lines of $line_count total)"
            else
                cat "$file"
            fi
        elif [[ "$file" == *.txt ]]; then
            # For text files, be smart about large files
            if [ $line_count -gt 100 ]; then
                echo "📄 Text file with $line_count lines"
                echo "First 20 lines:"
                head -20 "$file"
                echo "..."
                echo "Last 10 lines:"
                tail -10 "$file"
                echo "ℹ️  File truncated for readability (use 'cat $file' to see full content)"
            else
                cat "$file"
            fi
        else
            # For other files, show content with size limit
            if [ $line_count -gt 50 ]; then
                head -50 "$file"
                echo "... (showing first 50 lines of $line_count total)"
            else
                cat "$file"
            fi
        fi
        echo "----------------------------------------"
    fi
done

# Run AI analysis if Ollama is available
if check_ollama; then
    analyze_code "$SOURCE_FILE" "${output_files[@]}"
else
    print_warning "Skipping AI analysis - Ollama not available"
fi

# Get confirmation (with AI context if available)
if check_ollama; then
    get_confirmation_with_ai "$SOURCE_FILE" "${output_files[@]}"
else
    echo
    while true; do
        read -p "Have you finished grading the files? (y/n): " yn
        case $yn in
            [Yy]* ) 
                print_status "Proceeding with cleanup..."
                break
                ;;
            [Nn]* ) 
                print_status "Waiting for you to finish grading..."
                ;;
            * ) 
                echo "Please answer yes (y) or no (n)."
                ;;
        esac
    done
fi

# Final confirmation before deletion
echo
print_warning "This will permanently delete $SOURCE_FILE and all output files"
echo "Files to be deleted: $SOURCE_FILE ${output_files[*]}"
while true; do
    read -p "Are you sure you want to delete these files? (y/n): " yn
    case $yn in
        [Yy]* ) 
            break
            ;;
        [Nn]* ) 
            print_status "Cleanup cancelled. Files preserved."
            exit 0
            ;;
        * ) 
            echo "Please answer yes (y) or no (n)."
            ;;
    esac
done

# Remove the files
files_to_remove=("$SOURCE_FILE")
for file in "${output_files[@]}"; do
    if [ -f "$file" ]; then
        files_to_remove+=("$file")
    fi
done

# Add assignment-specific files to cleanup
cleanup_assignment_files

# Language-specific cleanup
case "$SELECTED_LANGUAGE" in
    "csharp")
        for file in *.exe *.dll bin/ obj/; do
            if [ -e "$file" ]; then
                files_to_remove+=("$file")
            fi
        done
        ;;
    "java")
        for file in *.class; do
            if [ -e "$file" ]; then
                files_to_remove+=("$file")
            fi
        done
        ;;
    "cpp"|"c")
        # Remove compiled executables (files without extensions that are executable)
        for file in *; do
            if [ -f "$file" ] && [ -x "$file" ] && [[ ! "$file" == *.* ]]; then
                files_to_remove+=("$file")
            fi
        done
        ;;
    "rust")
        # Remove compiled Rust executables
        for file in *; do
            if [ -f "$file" ] && [ -x "$file" ] && [[ ! "$file" == *.* ]]; then
                files_to_remove+=("$file")
            fi
        done
        ;;
esac

for file in "${files_to_remove[@]}"; do
    if [ -f "$file" ]; then
        rm "$file"
        print_status "Removed $file"
    elif [ -d "$file" ]; then
        rm -rf "$file"
        print_status "Removed directory $file"
    fi
done

print_status "Cleanup complete!"
