#include <iostream>
#include <cmath>

using namespace std;

int main()
{
    setlocale(LC_CTYPE, "Russian");
    double d = 51.9e-2;
    int a = 4;
    int k = 1;
    double y;
    double x = (a * a - 1) / (d + 1);

    for (double c = 3.7; c < 5.1; c += 0.1) {
        if (3 * x < a * c) {
            y = a * k + d;
        }
        else if (3 * x >= a * c) {
            y = cos(a * k) * exp(a + 1);
        }
        else {
            cout << "Îøèáêà" << endl;
        }
        cout << "y = " << y << ", x = " << x << endl;
    }
    return 0;
}