#include <iostream>

int main()
{
    setlocale(LC_CTYPE, "Russian");
    using namespace std;
    int b, z, x, c, a;
    cout << "Введите трехзначное число: ";
    cin >> b;

     z = b % 10;  // сотни
     x = (b / 10) % 10;  // десятки
     c = b / 100;  // еденицы

     a = z * 100 + x * 10 + c;

    cout << "Реверсное число: " << a << endl;

    return 0;
}