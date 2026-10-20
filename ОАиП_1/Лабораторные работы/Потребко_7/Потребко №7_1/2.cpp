#include <iostream>

using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    int x, y, z;

    cout << "Введите три натуральных числа (x, y, z): ";
    cin >> x >> y >> z;

    int a = x;
    int b = y;
    while (b != 0) {
        int h = b;
        b = a % b;
        a = h;
    }
    int nodXY = a; // НОД(x, y)

    int a = nodXY;
    int b = z;
    while (b != 0) {
        int h = b;
        b = a % b;
        a = h;
    }
    int nodXYZ = a; // НОД(x, y, z)

    cout << "Наибольший общий делитель (" << x << ", " << y << ", " << z << ") = " << nodXYZ << endl;

    return 0;
}