#include <iostream>
#include <cmath> 
using namespace std;

double f(double x) {
    return pow(cos(x),3);
}

int main() {
    double a = 4;
    double b = 7;
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