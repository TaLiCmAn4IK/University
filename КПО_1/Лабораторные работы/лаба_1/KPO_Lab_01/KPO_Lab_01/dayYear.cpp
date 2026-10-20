#include "dayYear.h"
#include "isLeapYear.h"

int dayYear(int day, int month, int year) {
   
    int daysInMonth[12] = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    if (isLeapYear(year)) {
        daysInMonth[1] = 29;
    }

    int dayNum = 0;
    for (int i = 0; i < month - 1; i++) {
        dayNum += daysInMonth[i];
    }
    dayNum += day;
    return dayNum;
}
