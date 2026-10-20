#include <iostream>
#include <fstream>
#include <string>
#include <locale>
#include <codecvt>
#include <sstream>
#include <vector>
#include <io.h>
#include <fcntl.h>

using namespace std;

// Функция для записи строки в файл (UTF-8)
void writeToFile(const wstring& filename) {
    wofstream file(filename, ios::out);
    file.imbue(locale(locale(), new codecvt_utf8<wchar_t>));
    if (!file) {
        wcerr << L"Ошибка при открытии файла для записи!" << endl;
        exit(1);
    }

    wcout << L"Введите строку с числами и текстом: ";
    wstring input;
    getline(wcin, input);

    file << input;
    file.close();
}

// Функция для чтения строки из файла (UTF-8)
wstring readFromFile(const wstring& filename) {
    wifstream file(filename, ios::in);
    file.imbue(locale(locale(), new codecvt_utf8<wchar_t>));
    if (!file) {
        wcerr << L"Ошибка при открытии файла для чтения!" << endl;
        exit(1);
    }

    wstring data;
    getline(file, data, L'\0');
    file.close();
    return data;
}

// Функция для проверки, является ли строка числом
bool isNumber(const wstring& str, int& num) {
    wstringstream ss(str);
    ss >> num;
    return !ss.fail() && ss.eof();
}

int main() {
    // Настройка UTF-16 для консоли (для корректного отображения русских символов)
    _setmode(_fileno(stdin), _O_WTEXT);
    _setmode(_fileno(stdout), _O_WTEXT);

    const wstring filename = L"FILE.txt";

    writeToFile(filename);
    wstring fileData = readFromFile(filename);

    wcout << L"Прочитанные данные: " << fileData << endl;

    // Разбор строки и вывод нечетных чисел
    wstringstream ss(fileData);
    wstring word;
    vector<int> oddNumbers;
    while (ss >> word) {
        int num;
        if (isNumber(word, num) && num % 2 != 0) {
            oddNumbers.push_back(num);
        }
    }

    wcout << L"Нечетные числа: ";
    for (int num : oddNumbers) {
        wcout << num << L" ";
    }
    wcout << endl;

    return 0;
}