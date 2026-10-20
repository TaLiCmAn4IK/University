#include <iostream>
#include <cmath>

using namespace std;

double sumCos(int n, double x) {
    if (n == 0) {
        return 0; 
    }
    return cos(n * x) + sumCos(n - 1, x);
}

int main() {
    setlocale(LC_ALL, "rus");
    double x;
    int n;
    cout << "Введите значение x (в радианах): ";
    cin >> x;
    cout << "Введите значение n: ";
    cin >> n;

    double result = sumCos(n, x);
    cout << "Сумма cos(x) + cos(2x) + ... + cos(nx) = " << result << endl;

    return 0;
}
