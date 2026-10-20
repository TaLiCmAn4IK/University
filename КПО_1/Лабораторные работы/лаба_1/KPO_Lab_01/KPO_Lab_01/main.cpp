#include <iostream>
#include <string>
#include <iomanip>
#include "isLeapYear.h"
#include "dayYear.h"
#include "dayBirthday.h"

using namespace std;

int main() {
    setlocale(LC_ALL, "Russian");

    cout << "Введите дату в формате ДДММГГГГ: ";
    string input;
    cin >> input;  

    if (input.size() != 8) {
        cout << "Некорректный ввод! Формат должен быть ДДММГГГГ (8 цифр)." << endl;
        return 1;
    }

    int day = stoi(input.substr(0, 2));
    int month = stoi(input.substr(2, 2));
    int year = stoi(input.substr(4, 4));

    if (month < 1 || month > 12) {
        cout << "Некорректный месяц: " << month
            << ". Месяц должен быть в диапазоне от 1 до 12." << endl;
        return 1;
    }

    if (day < 1 || day > 31) {
        cout << "Некорректный день: " << day
            << ". День должен быть в диапазоне от 1 до 31." << endl;
        return 1;
    }

    bool leap = isLeapYear(year);

    int dayNum = dayYear(day, month, year);

    int toBirthday = dayBirthday(day, month, year);

    cout << "Год " << year << (leap ? " является високосным." : " не является високосным.") << endl;
    cout << "Порядковый номер дня " << day << "." << month << "." << year
        << " в году: " << dayNum << endl;
    cout << "До ближайшего дня рождения осталось " << toBirthday << " дней." << endl;

    return 0;
}