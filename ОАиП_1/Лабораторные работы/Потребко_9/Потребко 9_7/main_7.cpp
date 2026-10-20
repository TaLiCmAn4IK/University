#include <iostream>
#include <cmath> 
using namespace std;

double f(double x) {
    return pow(cos(x), 3);
}

int main() {
    double a = 4;
    double b = 7;
    int n = 200;
    double h = (b - a) / (2 * n);
    double x = a + 2 * h;
    double s1 = 0;
    double s2 = 0;
    double i = 1;

    while (i < n) {
        s2 = s2 + f(x);
        x = x + h;
        s1 = s1 + f(x);
        x = x + h;
        i = i + 1;
    }
    double S = (h / 3) * (f(a) + 4 * f(a + h) + 4 * s1 + 2 * s2 + f(b));
    cout << "S = " << S << endl;
    return 0;
}