#include <iostream>
#include <fstream>
#include <cctype>
#include <string>

using namespace std;

bool isDelimiter(char ch) {
    return ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r' || ch == '.' || ch == ',' || ch == '!' || ch == '?';
}

bool isValidWord(const string& word) {
    for (char ch : word) {
        if (isdigit(ch)) {
            return false; // Если слово содержит цифру, оно не считается словом
        }
    }
    return !word.empty();
}

string getFirstWord(const string& line) {
    string word;
    size_t i = 0;
    while (i < line.length() && (isDelimiter(line[i]) || isdigit(line[i]))) {
        i++; // Пропускаем ведущие разделители и цифры
    }
    while (i < line.length()) {
        word.clear();
        while (i < line.length() && !isDelimiter(line[i])) {
            word += line[i];
            i++;
        }
        if (isValidWord(word)) {
            return word;
        }
        while (i < line.length() && isDelimiter(line[i])) {
            i++; // Пропускаем разделители после некорректного "слова"
        }
    }
    return "";
}

bool containsWord(const string& line, const string& word) {
    size_t len = line.length(), i = 0;
    while (i < len) {
        while (i < len && isDelimiter(line[i])) i++; // Пропускаем разделители
        string foundWord;
        while (i < len && !isDelimiter(line[i])) { // Считываем слово
            foundWord += line[i];
            i++;
        }
        if (isValidWord(foundWord) && foundWord == word) return true;
    }
    return false;
}

int countCharacters(const string& filename) {
    ifstream file(filename);
    if (!file) {
        cerr << "Ошибка открытия файла " << filename << endl;
        return -1;
    }
    char ch;
    int count = 0;
    while (file.get(ch)) {
        if (!isspace(static_cast<unsigned char>(ch))) { // Игнорируем пробелы
            count++;
        }
    }
    return count;
}

int main() {
    setlocale(LC_ALL, "Russian");
    ifstream inFile("FILE1.txt");
    if (!inFile) {
        cerr << "Ошибка открытия файла FILE1.txt. Убедитесь, что файл существует и содержит текст." << endl;
        return 1;
    }

    ofstream outFile("FILE2.txt");
    if (!outFile) {
        cerr << "Ошибка создания файла FILE2.txt" << endl;
        return 1;
    }

    string firstLine, firstWord;
    if (getline(inFile, firstLine)) {
        firstWord = getFirstWord(firstLine);
        if (!firstWord.empty() && containsWord(firstLine, firstWord)) {
            outFile << firstLine << "\n";
        }
    }

    string line;
    while (getline(inFile, line)) {
        if (!firstWord.empty() && containsWord(line, firstWord)) {
            outFile << line << "\n";
        }
    }

    inFile.close();
    outFile.close();

    int characterCount = countCharacters("FILE2.txt");
    cout << "Количество символов в FILE2.txt: " << characterCount << endl;

    return 0;
}
