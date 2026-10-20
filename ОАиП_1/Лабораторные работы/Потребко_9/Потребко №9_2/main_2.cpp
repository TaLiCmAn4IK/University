#include <iostream>
#include <cmath> 
using namespace std;

double f(double x) {
    return x * x * x + 2 * x - 4;
}

int main() {
    double a = 5;
    double b = 11;
    double e = 0.0001;
    double x = (a + b) / 2;

    do{
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