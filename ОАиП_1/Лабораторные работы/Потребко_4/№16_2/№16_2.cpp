#include <stdio.h>
#include <conio.h>
#include <iomanip>
#include <iostream>

int main()
{
    setlocale(LC_CTYPE, "Russian");
    double t, r, k, z, x, a;
    printf("—редн€€ скорость на первом участке: ");
    scanf_s("%d", &t);
    printf("—редн€€ скорость на втором участке: ");
    scanf_s("%d", &r);
    printf("—редн€€ скорость на третьем участке: ");
    scanf_s("%d", &k);
    printf("—редн€€ скорость на четвЄртом участке: ");
    scanf_s("%d", &z);
    printf("—редн€€ скорость на п€том участке: ");
    scanf_s("%d", &x);

    a = (t + r + k + z + x) / 5 ;

    printf("—редн€€ скорость на всЄм пути: %d", a);
    getchar();
}