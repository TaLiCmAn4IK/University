#include <iostream>
#include <cmath> 
using namespace std;

double f(double x) {
    return 2 - (x * x) + x;
}

int main() {
    double a = 8;
    double b = 12;
    double e = 0.0001;
    double x = (a + b) / 2;

    do {
        x = (a + b) / 2;
        if (f(x) * f(a) <= 0) {
            b = x;
        }
        else {
            a = x;
        }
    } while (abs(a - b) > (2 * e));

    cout << "x = " << x << endl;
    return 0;
}