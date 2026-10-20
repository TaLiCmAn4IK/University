#include <iostream>
#include <vector>
#include <string>
#include <regex>
#include <limits>
#include <locale>
#include <iomanip>
#include <fstream>
#include <codecvt>

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
    SAVE_TO_FILE,
    LOAD_FROM_FILE,
    EXIT
};

union UDate {
    unsigned int rawData;
    struct {
        unsigned int day : 5;
        unsigned int month : 4;
        unsigned int year : 12;
        unsigned int _unused : 11;
    } parts;
};

static inline void setDate(UDate& u, unsigned day, unsigned month, unsigned year) {
    u.parts.day = day;
    u.parts.month = month;
    u.parts.year = (year >= 1900) ? (year - 1900) : 0;
    u.parts._unused = 0;
}

static inline unsigned getDay(const UDate& u) {
    return u.parts.day;
}

static inline unsigned getMonth(const UDate& u) {
    return u.parts.month;
}

static inline unsigned getYear(const UDate& u) {
    return u.parts.year + 1900;
}

struct Record {
    wstring fullName;
    UDate   birthDate;
    wstring address;
    wstring phone;
    wstring place;
    wstring position;
};

bool fioExists(const vector<Record>& notebook, const wstring& fio) {
    for (const auto& rec : notebook) {
        if (rec.fullName == fio) {
            return true;
        }
    }
    return false;
}

bool phoneExists(const vector<Record>& notebook, const wstring& phone) {
    for (const auto& rec : notebook) {
        if (rec.phone == phone) {
            return true;
        }
    }
    return false;
}

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

// Удаление записи
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

// Извлекает фамилию из ФИО (первое слово)
wstring extractSurname(const wstring& fullName) {
    size_t spacePos = fullName.find(L' ');
    if (spacePos == wstring::npos) {
        return fullName;
    }
    return fullName.substr(0, spacePos);
}


bool isLeapYear(int year) {
    return ((year % 400 == 0) || (year % 4 == 0 && year % 100 != 0));
}


bool isValidDate(int day, int month, int year) {
    if (year < 1900 || year > 2100) {
        return false;
    }
    if (month < 1 || month > 12) {
        return false;
    }

    static const int daysInMonth[12] = { 31,28,31,30,31,30,31,31,30,31,30,31 };
    int maxDay = daysInMonth[month - 1];


    if (month == 2 && isLeapYear(year)) {
        maxDay = 29;
    }
    return (day >= 1 && day <= maxDay);
}



static int readTwoDigits(const wstring& prompt, const wregex& re, const wstring& errorMsg) {
    while (true) {
        wcout << prompt;
        wstring input;
        getline(wcin, input);

        if (!regex_match(input, re)) {
            wcout << errorMsg << L"\n";
            continue;
        }
        return stoi(input);
    }
}

int readDay2Digits() {
    static const wregex dayRegex(LR"(^(0[1-9]|[12]\d|3[01])$)");
    return readTwoDigits(L"Введите день (01..31): ", dayRegex, L"Ошибка! День должен быть 01..31.");
}

int readMonth2Digits() {
    static const wregex monthRegex(LR"(^(0[1-9]|1[0-2])$)");
    return readTwoDigits(L"Введите месяц (01..12): ", monthRegex, L"Ошибка! Месяц должен быть 01..12.");
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
                wcout << L"Ошибка! Год должен быть в диапазоне 1900..2100.\n";
            }
        }
        else {
            wcout << L"Ошибка! Введите только число.\n";
        }
    }
}

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
            wcout << L"Ошибка! Номер телефона не соответствует формату +375XX XXX XX XX.\n";
        }
    }
}

wstring inputFullName() {
    static const wregex fioRegex(
        LR"(^[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)?\s+[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)?(?:\s+[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)?)?$)"
    );
    while (true) {
        wcout << L"Введите ФИО (Фамилия Имя Отчество): ";
        wstring input;
        getline(wcin, input);
        if (regex_match(input, fioRegex)) {
            return input;
        }
        else {
            wcout << L"Ошибка! Пример: Иванов-Петров Пётр-Василий [Сидорович].\n";
        }
    }
}


wstring inputTextNoDigits(const wstring& prompt) {
    static const wregex noDigitRegex(LR"(^(?!.*\d)[А-Яа-яЁёA-Za-z\s.,\-\(\)]+$)");
    while (true) {
        wcout << prompt;
        wstring input;
        getline(wcin, input);
        if (!regex_match(input, noDigitRegex) || input.empty()) {
            wcout << L"Ошибка! Поле не должно содержать цифры и не должно быть пустым.\n";
        }
        else {
            return input;
        }
    }
}


struct Record inputRecord(const vector<Record>& notebook) {
    Record rec;


    while (true) {
        wstring tmpFio = inputFullName();
        if (fioExists(notebook, tmpFio)) {
            wcout << L"Ошибка: запись с таким ФИО уже существует! Повторите ввод.\n";
        }
        else {
            rec.fullName = tmpFio;
            break;
        }
    }

  
    while (true) {
        int day = readDay2Digits();
        int month = readMonth2Digits();
        int year = readYear();

        if (isValidDate(day, month, year)) {
            setDate(rec.birthDate, day, month, year);
            break;
        }
        else {
            wcout << L"Ошибка! Некорректная дата (например, 30 февраля). Повторите ввод.\n";
        }
    }


    wcout << L"Введите адрес: ";
    getline(wcin, rec.address);

 
    while (true) {
        wstring tmpPhone = inputPhone();
        if (phoneExists(notebook, tmpPhone)) {
            wcout << L"Ошибка: запись с таким номером телефона уже есть! Повторите ввод.\n";
        }
        else {
            rec.phone = tmpPhone;
            break;
        }
    }


    rec.place = inputTextNoDigits(L"Введите место работы или учебы: ");

 
    rec.position = inputTextNoDigits(L"Введите должность или статус: ");

    return rec;
}


void printRecord(const Record& r, int index) {
    wcout << index << L". ФИО: " << r.fullName << L"\n"
        << L"   Дата рождения: "
        << setw(2) << setfill(L'0') << getDay(r.birthDate) << L"."
        << setw(2) << setfill(L'0') << getMonth(r.birthDate) << L"."
        << getYear(r.birthDate) << L"\n"
        << L"   Адрес: " << r.address << L"\n"
        << L"   Телефон: " << r.phone << L"\n"
        << L"   Место работы/учёбы: " << r.place << L"\n"
        << L"   Должность/статус: " << r.position << L"\n";
}


void printAllRecords(const vector<Record>& notebook) {
    if (notebook.empty()) {
        wcout << L"Записная книжка пуста.\n";
        return;
    }
    for (size_t i = 0; i < notebook.size(); ++i) {
        printRecord(notebook[i], static_cast<int>(i + 1));
    }
}


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


bool saveToFile(const vector<Record>& notebook, const wstring& filename) {
    wofstream outFile(filename);
    if (!outFile.is_open()) {
        wcerr << L"Не удалось открыть файл для записи: " << filename << L"\n";
        return false;
    }

#ifdef _WIN32
    outFile.imbue(std::locale(outFile.getloc(), new std::codecvt_utf8<wchar_t>));
#else
    outFile.imbue(std::locale(""));
#endif

    for (const auto& rec : notebook) {
        outFile << rec.fullName << L"\n"
            << setw(2) << setfill(L'0') << getDay(rec.birthDate) << L"\n"  // Добавлены ведущие нули
            << setw(2) << setfill(L'0') << getMonth(rec.birthDate) << L"\n"  // Добавлены ведущие нули
            << getYear(rec.birthDate) << L"\n"
            << rec.address << L"\n"
            << rec.phone << L"\n"
            << rec.place << L"\n"
            << rec.position << L"\n";
    }
    outFile.close();
    return true;
}

// Загрузка из файла
// Обновленная функция loadFromFile с добавлением ошибок и сообщений об исправлениях
bool loadFromFile(vector<Record>& notebook, const wstring& filename) {
    wifstream inFile(filename);
    if (!inFile.is_open()) {
        wcerr << L"Не удалось открыть файл для чтения: " << filename << L"\n";
        return false;
    }

#ifdef _WIN32
    // Для Windows: читаем в UTF-8
    inFile.imbue(std::locale(inFile.getloc(), new std::codecvt_utf8<wchar_t>));
#else
    inFile.imbue(std::locale(""));
#endif

    // Регулярные выражения для проверки корректности прочитанных строк
    static const wregex fioRegex(LR"(^[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)?\s+[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)?(?:\s+[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)?)?$)");
    static const wregex dayRegex(LR"(^(0[1-9]|[12]\d|3[01])$)");
    static const wregex monthRegex(LR"(^(0[1-9]|1[0-2])$)");
    static const wregex yearRegex(LR"(^(19[0-9]{2}|20[0-9]{2}|2100)$)");
    static const wregex phoneRegex(LR"(^\+375\s*\d{2}\s*\d{3}\s*\d{2}\s*\d{2}$)");
    static const wregex noDigitRegex(LR"(^(?!.*\d)[А-Яа-яЁёA-Za-z\s.,\-\(\)]+$)");

    auto validFio = [&](const wstring& s) {
        return regex_match(s, fioRegex);
        };
    auto validDay = [&](const wstring& s) {
        return regex_match(s, dayRegex);
        };
    auto validMonth = [&](const wstring& s) {
        return regex_match(s, monthRegex);
        };
    auto validYear = [&](const wstring& s) {
        if (!regex_match(s, yearRegex)) return false;
        int y = stoi(s);
        return (y >= 1900 && y <= 2100);
        };
    auto validPhone = [&](const wstring& s) {
        return regex_match(s, phoneRegex);
        };
    auto validNoDigits = [&](const wstring& s) {
        return !s.empty() && regex_match(s, noDigitRegex);
        };

    notebook.clear();
    while (true) {
        wstring lineFullName;
        if (!getline(inFile, lineFullName)) break;

        wstring lineDay, lineMonth, lineYear;
        wstring lineAddress;
        wstring linePhone;
        wstring linePlace;
        wstring linePosition;

        if (!getline(inFile, lineDay)) break;
        if (!getline(inFile, lineMonth)) break;
        if (!getline(inFile, lineYear)) break;
        if (!getline(inFile, lineAddress)) break;
        if (!getline(inFile, linePhone)) break;
        if (!getline(inFile, linePlace)) break;
        if (!getline(inFile, linePosition)) break;

        bool validRecord = true;
        bool correctedRecord = false;  // Флаг для отслеживания исправлений

        // Проверки входных данных
        if (!validFio(lineFullName)) {
            wcerr << L"Ошибка в ФИО: " << lineFullName << L"\n";
            validRecord = false;
        }
        if (!validDay(lineDay)) {
            wcerr << L"Ошибка в дне: " << lineDay << L"\n";
            validRecord = false;
        }
        if (!validMonth(lineMonth)) {
            wcerr << L"Ошибка в месяце: " << lineMonth << L"\n";
            validRecord = false;
        }
        if (!validYear(lineYear)) {
            wcerr << L"Ошибка в годе: " << lineYear << L"\n";
            validRecord = false;
        }
        if (!validPhone(linePhone)) {
            wcerr << L"Ошибка в телефоне: " << linePhone << L"\n";
            validRecord = false;
        }
        if (!validNoDigits(linePlace)) {
            wcerr << L"Ошибка в месте работы/учёбы: " << linePlace << L"\n";
            validRecord = false;
        }
        if (!validNoDigits(linePosition)) {
            wcerr << L"Ошибка в должности/статусе: " << linePosition << L"\n";
            validRecord = false;
        }

        // Если есть ошибки по формату — исправляем или пропускаем
        if (!validRecord) {
            wcerr << L"Запись пропущена из-за ошибок.\n";
            continue;
        }

        // Проверка дубликатов (ФИО, телефон)
        if (fioExists(notebook, lineFullName)) {
            wcerr << L"В файле обнаружено дублирующее ФИО: " << lineFullName << L"\n";
            wcerr << L"Запись пропущена.\n";
            continue;
        }
        if (phoneExists(notebook, linePhone)) {
            wcerr << L"В файле обнаружен дублирующий телефон: " << linePhone << L"\n";
            wcerr << L"Запись пропущена.\n";
            continue;
        }

        // Дополнительно проверяем реальную допустимость дня в месяце
        int d = stoi(lineDay);
        int m = stoi(lineMonth);
        int y = stoi(lineYear);
        if (!isValidDate(d, m, y)) {
            wcerr << L"Ошибка: недопустимая дата " << d << L"." << m << L"." << y << L"\n";
            wcerr << L"Запись пропущена.\n";
            continue;
        }

        // Если всё в порядке, формируем запись
        Record rec;
        rec.fullName = lineFullName;
        setDate(rec.birthDate, d, m, y);
        rec.address = lineAddress;
        rec.phone = linePhone;
        rec.place = linePlace;
        rec.position = linePosition;

        // Если данные были исправлены, выводим сообщение
        if (correctedRecord) {
            wcout << L"Запись была исправлена и добавлена в записную книжку.\n";
        }

        notebook.push_back(rec);
    }
    inFile.close();
    return true;
}

int main() {
#ifdef _WIN32
    _setmode(_fileno(stdin), _O_WTEXT);
    _setmode(_fileno(stdout), _O_WTEXT);
#endif
    setlocale(LC_ALL, "");

    vector<Record> notebook;
    wstring filename = L"FILE.txt";

    while (true) {
        wcout << L"\n=== МЕНЮ ===\n"
            << L"1. Добавить запись\n"
            << L"2. Вывести все записи\n"
            << L"3. Удалить запись\n"
            << L"4. Поиск по фамилии\n"
            << L"5. Сохранить в файл\n"
            << L"6. Загрузить из файла\n"
            << L"7. Выход\n";

        int choice = readMenuChoice();
        MenuOption option = static_cast<MenuOption>(choice);

        switch (option) {
        case ADD: {
            wcout << L"Добавление записи.\n";
            Record rec = inputRecord(notebook);
            notebook.push_back(rec);
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
        case SAVE_TO_FILE: {
            wcout << L"Сохранение в файл: " << filename << L"\n";
            if (saveToFile(notebook, filename)) {
                wcout << L"Данные успешно сохранены.\n";
            }
            else {
                wcout << L"Ошибка при записи в файл.\n";
            }
            break;
        }
        case LOAD_FROM_FILE: {
            wcout << L"Загрузка из файла: " << filename << L"\n";
            if (loadFromFile(notebook, filename)) {
                wcout << L"Данные успешно загружены.\n";
            }
            else {
                wcout << L"Ошибка при чтении файла.\n";
            }
            break;
        }
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
