#include <iostream> 
#include <vector>
#include <string>
#include <regex>
#include <limits>
#include <locale>
#include <iomanip>

#ifdef _WIN32
#include <io.h>
#include <fcntl.h>
#define NOMINMAX
#include <windows.h>
#endif

using namespace std;

enum MenuOption {
    ADD = 1,
    PRINT,
    DELETE_REC,
    SEARCH,
    EXIT
};

struct Date {
    unsigned int day : 5;     // 0..31 (макс 31)
    unsigned int month : 4;   // 0..12 (макс 12)
    unsigned int year : 11;   // 0..2047, но мы используем 1900..2100
};

struct Record {
    wstring fullName;
    Date birthDate;
    wstring address;
    wstring phone;
};

// Выделяем из ФИО только фамилию (первое слово)
wstring extractSurname(const wstring& fullName) {
    size_t spacePos = fullName.find(L' ');
    if (spacePos == wstring::npos) {
        return fullName;
    }
    return fullName.substr(0, spacePos);
}

// Проверка, есть ли уже такое ФИО в записной книжке
bool fioExists(const vector<Record>& notebook, const wstring& fio) {
    for (const auto& rec : notebook) {
        if (rec.fullName == fio) {
            return true;
        }
    }
    return false;
}

// Проверка, есть ли уже такой номер телефона в записной книжке
bool phoneExists(const vector<Record>& notebook, const wstring& phone) {
    for (const auto& rec : notebook) {
        if (rec.phone == phone) {
            return true;
        }
    }
    return false;
}

// Функция проверки високосного года
bool isLeapYear(int year) {
    // Год високосный, если он кратен 400, 
    // или он кратен 4, но при этом не кратен 100.
    return ((year % 400 == 0) || (year % 4 == 0 && year % 100 != 0));
}

// Функция проверки корректности дня в месяце (с учётом високосности)
bool isValidDate(int day, int month, int year) {
    if (year < 1900 || year > 2100) {
        return false;
    }
    if (month < 1 || month > 12) {
        return false;
    }

    // Количество дней в каждом месяце по умолчанию
    static const int daysInMonth[12] = { 31,28,31,30,31,30,31,31,30,31,30,31 };
    int maxDay = daysInMonth[month - 1];

    // Учитываем високосность для февраля
    if (month == 2 && isLeapYear(year)) {
        maxDay = 29;
    }

    // Проверяем, не вылезли ли за границы
    return (day >= 1 && day <= maxDay);
}

// Функция для считывания двухзначного числа (например, день/месяц)
int readTwoDigits(const wstring& prompt, const wregex& re, const wstring& errorMsg) {
    while (true) {
        wcout << prompt;
        wstring input;
        getline(wcin, input);

        if (!regex_match(input, re)) {
            wcout << errorMsg << L"\n";
            continue;
        }
        int value = stoi(input);
        return value;
    }
}

int readDay2Digits() {
    static const wregex dayRegex(LR"(^(0[1-9]|[12]\d|3[01])$)");
    return readTwoDigits(
        L"Введите день (01..31): ",
        dayRegex,
        L"Ошибка! День должен быть в формате 01..31."
    );
}

int readMonth2Digits() {
    static const wregex monthRegex(LR"(^(0[1-9]|1[0-2])$)");
    return readTwoDigits(
        L"Введите месяц (01..12): ",
        monthRegex,
        L"Ошибка! Месяц должен быть в формате 01..12."
    );
}

int readYear() {
    while (true) {
        wcout << L"Введите год (1900..2100): ";
        wstring input;
        getline(wcin, input);

        bool allDigits = true;
        for (wchar_t ch : input) {
            if (!iswdigit(ch)) {
                allDigits = false;
                break;
            }
        }

        if (!input.empty() && allDigits) {
            int year = stoi(input);
            if (year >= 1900 && year <= 2100) {
                return year;
            }
            else {
                wcout << L"Ошибка! Год должен быть в диапазоне [1900..2100].\n";
            }
        }
        else {
            wcout << L"Ошибка! Введите только число.\n";
        }
    }
}

// Функция для считывания телефона
wstring inputPhone() {
    static const wregex phoneRegex(LR"(^\+375\s*\d{2}\s*\d{3}\s*\d{2}\s*\d{2}$)");

    while (true) {
        wcout << L"Введите телефон (например, +375111111111 или +375 11 111 11 11): ";
        wstring input;
        getline(wcin, input);

        if (regex_match(input, phoneRegex)) {
            return input;
        }
        else {
            wcout << L"Ошибка! Номер телефона не соответствует формату.\n"
                << L"Примеры: +375111111111, +375 11 111 11 11.\n";
        }
    }
}

// Функция для считывания ФИО (Фамилия Имя, при необходимости отчество)
wstring inputFullName() {
    static const wregex fioRegex(
        LR"(^[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)?\s+[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)?(?:\s+[А-ЯЁ][а-яё]+)?$)"
    );

    while (true) {
        wcout << L"Введите ФИО (Фамилия Имя, отчество при необходимости): ";
        wstring input;
        getline(wcin, input);

        if (regex_match(input, fioRegex)) {
            return input;
        }
        else {
            wcout << L"Ошибка! Неверный формат ФИО.\n"
                << L"Примеры:\n"
                << L"  - Иванов-Петров Пётр-Василий\n"
                << L"  - Иванов-Петров Пётр-Василий Сидорович\n"
                << L"(Двойное отчество не допускается.)\n";
        }
    }
}

// Функция для создания новой записи (добавлена проверка корректности даты)
Record inputRecord(const vector<Record>& notebook) {
    Record rec;

    // Считывание ФИО + проверка на дубликаты
    while (true) {
        wstring tmpFio = inputFullName();
        if (fioExists(notebook, tmpFio)) {
            wcout << L"Ошибка: Запись с таким ФИО уже существует! Повторите ввод ФИО.\n";
        }
        else {
            rec.fullName = tmpFio;
            break;
        }
    }

    // Считываем дату рождения в цикле, пока не будет введена корректная
    while (true) {
        int d = readDay2Digits();
        int m = readMonth2Digits();
        int y = readYear();

        if (isValidDate(d, m, y)) {
            rec.birthDate.day = d;
            rec.birthDate.month = m;
            rec.birthDate.year = y;
            break;
        }
        else {
            wcout << L"Ошибка! Некорректная дата (например, 30 февраля). Повторите ввод.\n";
        }
    }

    // Адрес
    wcout << L"Введите адрес: ";
    getline(wcin, rec.address);

    // Телефон + проверка на дубликаты
    while (true) {
        wstring tmpPhone = inputPhone();
        if (phoneExists(notebook, tmpPhone)) {
            wcout << L"Ошибка: Запись с таким номером телефона уже существует! Повторите ввод.\n";
        }
        else {
            rec.phone = tmpPhone;
            break;
        }
    }

    return rec;
}

// Печать одной записи
void printRecord(const Record& r, int index) {
    wcout << index << L". ФИО: " << r.fullName << L"\n"
        << L"   Дата рождения: "
        << setw(2) << setfill(L'0') << r.birthDate.day << L"."
        << setw(2) << setfill(L'0') << r.birthDate.month << L"."
        << r.birthDate.year << L"\n"
        << L"   Адрес: " << r.address << L"\n"
        << L"   Телефон: " << r.phone << L"\n";
}

// Печать всех записей
void printAllRecords(const vector<Record>& notebook) {
    if (notebook.empty()) {
        wcout << L"Записная книжка пуста.\n";
        return;
    }
    for (size_t i = 0; i < notebook.size(); ++i) {
        printRecord(notebook[i], static_cast<int>(i + 1));
    }
}

// Удаление записи по индексу
void deleteRecord(vector<Record>& notebook) {
    if (notebook.empty()) {
        wcout << L"Записная книжка пуста, удалять нечего.\n";
        return;
    }
    while (true) {
        wcout << L"Введите номер записи для удаления: ";
        wstring input;
        getline(wcin, input);

        bool allDigits = true;
        for (wchar_t ch : input) {
            if (!iswdigit(ch)) {
                allDigits = false;
                break;
            }
        }

        if (!input.empty() && allDigits) {
            int index = stoi(input);
            if (index < 1 || index >(int)notebook.size()) {
                wcout << L"Некорректный индекс.\n";
            }
            else {
                notebook.erase(notebook.begin() + (index - 1));
                wcout << L"Запись номер " << index << L" удалена.\n";
                break;
            }
        }
        else {
            wcout << L"Ошибка ввода. Введите корректное число.\n";
        }
    }
}

// Поиск по фамилии (фамилия — первое слово)
void searchBySurname(const vector<Record>& notebook) {
    if (notebook.empty()) {
        wcout << L"Записная книжка пуста.\n";
        return;
    }

    wcout << L"Введите фамилию (или часть фамилии) для поиска: ";
    wstring inputSurname;
    getline(wcin, inputSurname);

    bool found = false;
    for (size_t i = 0; i < notebook.size(); ++i) {
        wstring fam = extractSurname(notebook[i].fullName);
        if (fam.find(inputSurname) != wstring::npos) {
            printRecord(notebook[i], static_cast<int>(i + 1));
            found = true;
        }
    }

    if (!found) {
        wcout << L"Нет записей, где фамилия содержит \"" << inputSurname << L"\".\n";
    }
}

// Считывание пункта меню
int readMenuChoice() {
    while (true) {
        wcout << L"Выберите пункт меню: ";
        wstring input;
        getline(wcin, input);

        bool allDigits = true;
        for (wchar_t ch : input) {
            if (!iswdigit(ch)) {
                allDigits = false;
                break;
            }
        }

        if (!input.empty() && allDigits) {
            return stoi(input);
        }
        else {
            wcout << L"Ошибка ввода. Введите число.\n";
        }
    }
}

int main() {
#ifdef _WIN32
    _setmode(_fileno(stdin), _O_WTEXT);
    _setmode(_fileno(stdout), _O_WTEXT);
#endif
    setlocale(LC_ALL, "Russian");

    vector<Record> notebook;

    while (true) {
        wcout << L"\n=== МЕНЮ ===\n"
            << L"1. Добавить запись\n"
            << L"2. Вывести все записи\n"
            << L"3. Удалить запись\n"
            << L"4. Поиск по фамилии\n"
            << L"5. Выход\n";

        int choice = readMenuChoice();
        MenuOption option = static_cast<MenuOption>(choice);

        switch (option) {
        case ADD: {
            wcout << L"Добавление записи.\n";
            Record newRec = inputRecord(notebook);
            notebook.push_back(newRec);
            break;
        }
        case PRINT:
            wcout << L"Вывод всех записей.\n";
            printAllRecords(notebook);
            break;
        case DELETE_REC:
            wcout << L"Удаление записи.\n";
            deleteRecord(notebook);
            break;
        case SEARCH:
            wcout << L"Поиск по фамилии.\n";
            searchBySurname(notebook);
            break;
        case EXIT:
            wcout << L"Выход из программы.\n";
            return 0;
        default:
            wcout << L"Некорректный пункт меню. Повторите ввод.\n";
            break;
        }
    }

    return 0;
}
