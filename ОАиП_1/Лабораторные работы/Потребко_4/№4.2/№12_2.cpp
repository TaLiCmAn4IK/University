#include <stdio.h>
#include <conio.h>
#include <iomanip>
#include <iostream>

int main()
{
    setlocale(LC_CTYPE, "Russian");
    int b, z, x, c, a;
    printf("Введите трехзначное число: ");
    scanf_s("%d", &b);

    z = b % 10;  // сотни
    x = (b / 10) % 10;  // десятки
    c = b / 100;  // еденицы

    a = z * 100 + x * 10 + c;

    printf("Реверсное число: %d", a);
    getchar();
} 