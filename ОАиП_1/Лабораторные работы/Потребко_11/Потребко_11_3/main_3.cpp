#include <iostream>
using namespace std;

void set_bits_to_zero(int& A, int p, int m) {

    int mask = (~0);

    mask = mask << p;

    mask = ~(mask);

    A &= mask;
}

void replace_bits(int& A, int& B, int p, int q, int m) {

    int extracted_bits = (A >> p) & ((1 << m) - 1);

    int mask = ((1 << m) - 1) << q;
    B &= ~mask;


    extracted_bits <<= q;


    B |= extracted_bits;
}

int main() {
    setlocale(LC_CTYPE, "Russian");
    int A, B, p, q, m;


    cout << "Введите число A: ";
    cin >> A;
    cout << "Введите число B: ";
    cin >> B;
    cout << "Введите позицию p: ";
    cin >> p;
    cout << "Введите позицию q: ";
    cin >> q;
    cout << "Введите количество битов m: ";
    cin >> m;

    cout << "До изменений: A = " << A << ", B = " << B << endl;
    set_bits_to_zero(A, p, m);


    replace_bits(A, B, p, q, m);
    cout << "После замены бит в B: B = " << B << endl;
    cout << "Результат в двоичной форме: ";
    for (int i = sizeof(B) * 8 - 1; i >= 0; i--) {
        cout << ((B >> i) & 1);
    }
    return 0;
}