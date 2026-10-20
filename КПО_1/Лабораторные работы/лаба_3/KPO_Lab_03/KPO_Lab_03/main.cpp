#include <iostream>
#include <string>
#include <iomanip>

using namespace std;

bool isLeapYear(int year) {
    return (year % 400 == 0) || (year % 4 == 0 && year % 100 != 0);
}

string monthName(int month) {
    string months[] = { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
                        "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
    return (month >= 1 && month <= 12) ? months[month - 1] : "Некорректный месяц";
}

int dayYear(int day, int month, int year) {
    int daysInMonth[] = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    if (isLeapYear(year)) daysInMonth[1] = 29;

    int dayNum = 0;
    for (int i = 0; i < month - 1; i++) {
        dayNum += daysInMonth[i];
    }
    dayNum += day;
    return dayNum;
}

void dateFromDayNumber(int dayNum, int year) {
    int daysInMonth[] = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    if (isLeapYear(year)) daysInMonth[1] = 29;

    int month = 1;
    while (dayNum > daysInMonth[month - 1]) {
        dayNum -= daysInMonth[month - 1];
        month++;
    }

    cout << "День " << dayNum << " " << monthName(month) << " " << year << " года." << endl;
}

int daysUntilBirthday(int day, int month, int year, int birthdayDay, int birthdayMonth) {
    int currentDayNum = dayYear(day, month, year);
    int birthdayDayNum = dayYear(birthdayDay, birthdayMonth, year);

    if (currentDayNum <= birthdayDayNum) {
        return birthdayDayNum - currentDayNum;
    }
    else {
        int nextYear = year + 1;
        int nextBirthdayDayNum = dayYear(birthdayDay, birthdayMonth, nextYear);
        int daysInCurrentYear = isLeapYear(year) ? 366 : 365;
        return daysInCurrentYear - currentDayNum + nextBirthdayDayNum;
    }
}

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

    if (month < 1 || month > 12 || day < 1 || day > 31) {
        cout << "Некорректная дата." << endl;
        return 1;
    }

    cout << "Введите день и месяц дня рождения в формате ДД ММ: ";
    int birthdayDay, birthdayMonth;
    cin >> birthdayDay >> birthdayMonth;

    if (birthdayMonth < 1 || birthdayMonth > 12 || birthdayDay < 1 || birthdayDay > 31) {
        cout << "Некорректная дата дня рождения." << endl;
        return 1;
    }

    bool leap = isLeapYear(year);
    int dayNum = dayYear(day, month, year);
    int toBirthday = daysUntilBirthday(day, month, year, birthdayDay, birthdayMonth);

    cout << "Год " << year << (leap ? " является високосным." : " не является високосным.") << endl;
    cout << "Дата: " << day << " " << monthName(month) << " " << year << endl;
    cout << "Порядковый номер дня в году: " << dayNum << endl;
    cout << "До ближайшего дня рождения осталось " << toBirthday << " дней." << endl;

    cout << "Введите порядковый номер дня в году: ";
    int inputDayNum;
    cin >> inputDayNum;

    if (inputDayNum < 1 || inputDayNum >(leap ? 366 : 365)) {
        cout << "Некорректный порядковый номер дня в году." << endl;
    }
    else {
        dateFromDayNumber(inputDayNum, year);
    }

    return 0;
}
