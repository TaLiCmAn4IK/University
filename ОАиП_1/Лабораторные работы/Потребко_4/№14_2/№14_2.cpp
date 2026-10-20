#include <stdio.h>
#include <conio.h>
#include <iomanip>
#include <iostream>

int main()
{
    setlocale(LC_CTYPE, "Russian");
    int t, r, b, a;
    printf("Введите количество сбитых самолётов: ");
    scanf_s("%d", &t);
    printf("Введите количество сбитых ракет: ");
    scanf_s("%d", &r);
    printf("Введите количество сбитых спутников: ");
    scanf_s("%d", &b);  

    a = t * 50 + r * 100 + b * 200;

    printf("Количество очков: %d", a);
    getchar();
}