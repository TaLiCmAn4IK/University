#include <iostream>
#include <vector>
#include <string>
#include <limits>
#include <locale>
#include <iomanip>
#include <fstream>
#include <regex>
#include <codecvt>  

#ifdef _WIN32
#include <io.h>
#include <fcntl.h>
#define NOMINMAX
#include <windows.h>
#endif

using namespace std;

enum MenuOption {
    ADD_STATE = 1,
    PRINT_ALL,
    DELETE_STATE,
    SEARCH_BY_NAME,
    SAVE_TO_FILE,
    LOAD_FROM_FILE,
    EXIT_PROGRAM
};

struct State {
    wstring name;         // Наименование государства
    wstring capital;      // Столица (либо одно слово, либо несколько с дефисами)
    double population;    // Численность населения
    double area;          // Площадь
    wstring president;    // Фамилия президента (допускается двойная)
};

wstring inputCountryName() {
    static const wregex nameRegex(
        LR"(^[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)*(?:\s[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)*)*$)"
    );
    while (true) {
        wcout << L"Введите название государства (пример: Беларусь, Коста-Рика): ";
        wstring input;
        getline(wcin, input);

        if (regex_match(input, nameRegex)) {
            return input;
        }
        else {
            wcout << L"Неверный формат названия. Повторите ввод.\n";
        }
    }
}

// ВАЖНО: здесь меняем шаблон на «одно слово или несколько через дефис, без пробелов»
wstring inputCapitalName() {
    static const wregex cityRegex(
        LR"(^[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)*$)"
    );
    while (true) {
        wcout << L"Введите столицу (пример: Минск или Крунг-Тхеп-Маха-Накхон): ";
        wstring input;
        getline(wcin, input);

        if (regex_match(input, cityRegex)) {
            return input;
        }
        else {
            wcout << L"Неверный формат названия столицы. "
                L"Допустимо одно слово или несколько частей через дефис.\n";
        }
    }
}

wstring inputPresidentSurname() {
    static const wregex surnameRegex(
        LR"(^[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)?$)"
    );
    while (true) {
        wcout << L"Введите фамилию президента (пример: Семенович, Иванов-Петров): ";
        wstring input;
        getline(wcin, input);

        if (regex_match(input, surnameRegex)) {
            return input;
        }
        else {
            wcout << L"Неверный формат фамилии. Допускается одна двойная фамилия.\n";
        }
    }
}

double readDoubleNoLetters(const wstring& prompt, double minVal, double maxVal) {
    static const wregex numericRegex(LR"(^\d+(\.\d+)?$)");

    while (true) {
        wcout << prompt;
        wstring input;
        getline(wcin, input);

        if (!regex_match(input, numericRegex)) {
            wcout << L"Ошибка! Допускаются только цифры и одна точка (без букв).\n";
            continue;
        }

        double value;
        try {
            value = stod(input);
        }
        catch (...) {
            wcout << L"Ошибка преобразования в число.\n";
            continue;
        }

        if (value < minVal || value > maxVal) {
            wcout << L"Число должно быть в диапазоне ["
                << minVal << L", " << maxVal << L"].\n";
            continue;
        }
        return value;
    }
}

State inputState() {
    State st;
    st.name = inputCountryName();
    st.capital = inputCapitalName();

    st.population = readDoubleNoLetters(L"Введите численность населения (0..3000000000): ",
        0.0, 3000000000.0);

    st.area = readDoubleNoLetters(L"Введите площадь (0..1000000000): ",
        0.0, 1000000000.0);

    st.president = inputPresidentSurname();
    return st;
}

void printState(const State& st, int index) {
    wcout << index << L". Государство: " << st.name << L"\n"
        << L"   Столица: " << st.capital << L"\n"
        << L"   Численность населения: " << st.population << L"\n"
        << L"   Площадь: " << st.area << L"\n"
        << L"   Фамилия президента: " << st.president << L"\n";
}

void printAllStates(const vector<State>& states) {
    if (states.empty()) {
        wcout << L"Список государств пуст.\n";
        return;
    }
    for (size_t i = 0; i < states.size(); ++i) {
        printState(states[i], static_cast<int>(i + 1));
    }
}

void deleteState(vector<State>& states) {
    if (states.empty()) {
        wcout << L"Список пуст, удалять нечего.\n";
        return;
    }
    while (true) {
        wcout << L"Введите номер записи для удаления: ";
        int index;
        wcin >> index;

        if (wcin.fail() || index < 1 || index >(int)states.size()) {
            wcin.clear();
            wcin.ignore(numeric_limits<streamsize>::max(), L'\n');
            wcout << L"Некорректный индекс.\n";
        }
        else {
            wcin.ignore(numeric_limits<streamsize>::max(), L'\n');
            states.erase(states.begin() + (index - 1));
            wcout << L"Запись номер " << index << L" удалена.\n";
            break;
        }
    }
}

void searchByName(const vector<State>& states) {
    if (states.empty()) {
        wcout << L"Список государств пуст.\n";
        return;
    }
    wcout << L"Введите название (или часть названия) для поиска: ";
    wstring inputName;
    getline(wcin, inputName);

    bool found = false;
    for (size_t i = 0; i < states.size(); ++i) {
        if (states[i].name.find(inputName) != wstring::npos) {
            printState(states[i], static_cast<int>(i + 1));
            found = true;
        }
    }
    if (!found) {
        wcout << L"Нет государств, где название содержит \"" << inputName << L"\".\n";
    }
}

bool saveToFile(const vector<State>& states, const wstring& filename) {
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

    for (const auto& st : states) {
        outFile << st.name << L"\n"
            << st.capital << L"\n"
            << st.population << L"\n"
            << st.area << L"\n"
            << st.president << L"\n";
    }

    outFile.close();
    return true;
}

bool loadFromFile(vector<State>& states, const wstring& filename) {
    wifstream inFile(filename);
    if (!inFile.is_open()) {
        wcerr << L"Не удалось открыть файл для чтения: " << filename << L"\n";
        return false;
    }
#ifdef _WIN32
    inFile.imbue(std::locale(inFile.getloc(), new std::codecvt_utf8<wchar_t>));
#else
    inFile.imbue(std::locale(""));
#endif

    // Регэкспы для валидации при загрузке
    static const wregex nameRegex(
        LR"(^[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)*(?:\s[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)*)*$)"
    );
    // Новый шаблон для столицы (одно слово или несколько через дефис)
    static const wregex cityRegex(
        LR"(^[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)*$)"
    );
    static const wregex surnameRegex(
        LR"(^[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)?$)"
    );
    static const wregex doubleRegex(
        LR"(^\d+(\.\d+)?$)"
    );

    auto validName = [&](const wstring& s) {
        return regex_match(s, nameRegex);
        };
    auto validCapital = [&](const wstring& s) {
        return regex_match(s, cityRegex);
        };
    auto validPresident = [&](const wstring& s) {
        return regex_match(s, surnameRegex);
        };
    auto validDoubleValue = [&](const wstring& s, double minV, double maxV) -> bool {
        if (!regex_match(s, doubleRegex)) return false;
        double val{};
        try {
            val = stod(s);
        }
        catch (...) {
            return false;
        }
        return (val >= minV && val <= maxV);
        };

    states.clear();
    while (true) {
        State st;
        wstring tempPop, tempArea;

        if (!getline(inFile, st.name)) break;
        if (!getline(inFile, st.capital)) break;
        if (!getline(inFile, tempPop)) break;
        if (!getline(inFile, tempArea)) break;
        if (!getline(inFile, st.president)) break;

        bool validRecord = true;
        // Проверяем название государства
        if (!validName(st.name)) {
            wcerr << L"Ошибка в названии: " << st.name << L"\n";
            validRecord = false;
        }
        // Проверяем столицу
        if (!validCapital(st.capital)) {
            wcerr << L"Ошибка в названии столицы: " << st.capital << L"\n";
            validRecord = false;
        }
        // Проверяем фамилию президента
        if (!validPresident(st.president)) {
            wcerr << L"Ошибка в фамилии президента: " << st.president << L"\n";
            validRecord = false;
        }
        // Проверяем численность населения
        if (!validDoubleValue(tempPop, 0.0, 3000000000.0)) {
            wcerr << L"Ошибка в численности населения: " << tempPop << L"\n";
            validRecord = false;
        }
        else {
            st.population = stod(tempPop);
        }
        // Проверяем площадь
        if (!validDoubleValue(tempArea, 0.0, 1000000000.0)) {
            wcerr << L"Ошибка в площади: " << tempArea << L"\n";
            validRecord = false;
        }
        else {
            st.area = stod(tempArea);
        }

        if (validRecord) {
            states.push_back(st);
        }
        else {
            wcerr << L"Запись пропущена из-за ошибок валидации.\n";
        }
    }

    inFile.close();
    return true;
}

int main() {
#ifdef _WIN32
    // Настраиваем Unicode-потоки ввода/вывода в Windows
    _setmode(_fileno(stdin), _O_WTEXT);
    _setmode(_fileno(stdout), _O_WTEXT);
#endif
    setlocale(LC_ALL, "RUS");

    vector<State> states;
    const wstring filename = L"states.txt";

    while (true) {
        wcout << L"\n=== МЕНЮ ===\n"
            << L"1. Добавить государство\n"
            << L"2. Вывести все государства\n"
            << L"3. Удалить государство\n"
            << L"4. Поиск по названию\n"
            << L"5. Сохранить в файл (" << filename << L")\n"
            << L"6. Загрузить из файла (" << filename << L")\n"
            << L"7. Выход\n"
            << L"Выберите пункт меню: ";

        int choice;
        wcin >> choice;
        if (wcin.fail()) {
            wcin.clear();
            wcin.ignore(numeric_limits<streamsize>::max(), L'\n');
            wcout << L"Ошибка ввода. Введите число.\n";
            continue;
        }
        wcin.ignore(numeric_limits<streamsize>::max(), L'\n');

        MenuOption option = static_cast<MenuOption>(choice);
        switch (option) {
        case ADD_STATE: {
            wcout << L"Добавление нового государства.\n";
            State newState = inputState();

            // Доп. проверка дубликатов по названию
            bool duplicate = false;
            for (const auto& s : states) {
                if (s.name == newState.name) {
                    wcout << L"Государство с таким названием уже существует!\n";
                    duplicate = true;
                    break;
                }
            }
            if (!duplicate) {
                states.push_back(newState);
                wcout << L"Государство успешно добавлено.\n";
            }
            break;
        }
        case PRINT_ALL:
            wcout << L"Список всех государств.\n";
            printAllStates(states);
            break;

        case DELETE_STATE:
            wcout << L"Удаление государства.\n";
            deleteState(states);
            break;

        case SEARCH_BY_NAME:
            wcout << L"Поиск по названию.\n";
            searchByName(states);
            break;

        case SAVE_TO_FILE:
            if (saveToFile(states, filename)) {
                wcout << L"Данные успешно сохранены в файл.\n";
            }
            else {
                wcout << L"Ошибка при сохранении в файл.\n";
            }
            break;

        case LOAD_FROM_FILE:
            if (loadFromFile(states, filename)) {
                wcout << L"Данные успешно загружены из файла.\n";
            }
            else {
                wcout << L"Ошибка при чтении файла.\n";
            }
            break;

        case EXIT_PROGRAM:
            wcout << L"Выход из программы.\n";
            return 0;

        default:
            wcout << L"Некорректный пункт меню. Повторите ввод.\n";
            break;
        }
    }

    return 0;
}
