#include "dayBirthday.h"
#include "dayYear.h"
#include "isLeapYear.h"

int dayBirthday(int day, int month, int year) {

    int birthdayDay = 26;
    int birthdayMonth = 2;

    int currentDayNum = dayYear(day, month, year);
    int birthdayDayNum = dayYear(birthdayDay, birthdayMonth, year);

    if (currentDayNum <= birthdayDayNum) {
        // День рождения ещё не наступил в этом году
        return birthdayDayNum - currentDayNum;
    }
    else {
        // День рождения уже прошёл в этом году, считаем до следующего
        int nextYear = year + 1;
        int nextBirthdayDayNum = dayYear(birthdayDay, birthdayMonth, nextYear);

        int daysInCurrentYear = isLeapYear(year) ? 366 : 365;
        int daysLeftCurrentYear = daysInCurrentYear - currentDayNum;

        return daysLeftCurrentYear + nextBirthdayDayNum;
    }
}
