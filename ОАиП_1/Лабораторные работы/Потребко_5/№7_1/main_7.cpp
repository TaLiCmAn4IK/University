#include <iostream>
using namespace std;

int main() 
{
    setlocale(LC_CTYPE, "Russian");
    int a, b, c;
    cout << "Введите три числа: ";
    cin >> a >> b >> c;
    int z = (a % 5 == 0 ? a : 0) + (b % 5 == 0 ? b : 0) + (c % 5 == 0 ? c : 0);
    (z > 0) ? cout << "Сумма чисел, делящихся на 5: " << z << endl
        : cout << "Error" << endl;
    return 0;
}