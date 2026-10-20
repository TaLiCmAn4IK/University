#include <iostream>
using namespace std;

double f(double x, int n) {
    if (n == 0)
        return 1.0;
    else if (n == 1)
        return x;
    else
        return (x * x / n / (n - 1) * f(x, n - 2));
}

int main() {
    setlocale(LC_ALL, "rus");
    double x;
    int n;
    cout << "¬ведите x (вещественное число): ";
    cin >> x;
    cout << "¬ведите n (неотрицательное целое число): ";
    cin >> n;

    if (n < 0) {
        cout << "ќшибка: n должно быть неотрицательным." << endl;
        return 1;
    }

    double result = f(x, n);
    cout << "f(x, n) = " << result << endl;

    return 0;
}