#include <iostream>
#include <cmath> 
using namespace std;

double f(double x) {
    return 5 - (x * x);
}

int main() {
    double a = 8;
    double b = 12;
    int n = 200;
    double h = (b - a) / n;
    double x = a;
    double S = 0;

    while (x < (b - h)) {
        S = S + h * (f(x) + f(x + h)) / 2;
        x = x + h;
    }
    cout << "S = " << S << endl;
    return 0;
}