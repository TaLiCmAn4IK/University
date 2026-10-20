#include <iostream>
#include <tchar.h>
#include <locale.h>

using namespace std;

typedef unsigned char Day;
typedef unsigned char Month;
typedef unsigned short Year;

struct Date
{
    Day dd;
    Month mm;
    Year yy;
};

bool operator<(Date d1, Date d2)
{
    if (d1.yy < d2.yy) return true;
    else if (d1.yy > d2.yy) return false;
    else if (d1.mm < d2.mm) return true;
    else if (d1.mm > d2.mm) return false;
    else if (d1.dd < d2.dd) return true;
    else return false;
}

bool operator>(Date d1, Date d2)
{
    if (d1.yy > d2.yy) return true;
    else if (d1.yy < d2.yy) return false;
    else if (d1.mm > d2.mm) return true;
    else if (d1.mm < d2.mm) return false;
    else if (d1.dd > d2.dd) return true;
    else return false;
}

bool operator==(Date d1, Date d2)
{
    if ((d1.dd == d2.dd) && (d1.mm == d2.mm) && (d1.yy == d2.yy))
        return true;
    else
        return false;
}

int _tmain(int argc, _TCHAR* argv[])
{
    setlocale(LC_ALL, "Russian");

    Date date1 = { 7, 1, 1980 };
    Date date2 = { 7, 2, 1993 };
    Date date3 = { 7, 1, 1980 };

    if (date1 < date3)
        cout << "date1 < date3 - true" << endl;
    else
        cout << "date1 > date3 - false" << endl;

    if (date1 > date2)
        cout << "date1 > date2 - true" << endl;
    else
        cout << "date1 < date2 - false" << endl;

    if (date1 == date2)
        cout << "date1 == date2 - true" << endl;
    else
        cout << "date1 != date2 - false" << endl;

    if (date1 == date3)
        cout << "date1 == date3 - true" << endl;
    else
        cout << "date1 != date3 - false" << endl;
    system("pause");
    return 0;
}