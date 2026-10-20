#include <iostream>
using namespace std;

int main()
{
    int n, a, b;
    setlocale(LC_CTYPE, "Russian");
    cout << "Введите натуральное двузначное число: ";
    cin >> n;
    if (n < 10 || n > 99)
    cout << "Ошибка: число должно быть двузначным." << endl;
    return 1;
    a = n / 10;
    b = n % 10;
    if (a == 1 || a == 9 || b == 1 || b == 9) 
    cout << "Да, есть." << endl;  
    else 
    cout << "Нет, нету." << endl;
    return 0;
}