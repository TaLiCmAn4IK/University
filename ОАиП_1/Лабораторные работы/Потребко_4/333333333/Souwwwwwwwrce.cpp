# include <stdio.h>
# include <math.h>
# include <locale.h>
#include <conio.h>

int main(void) {
    int c;  /* Температура в градусах Цельсия */
    int f;  /* Температура в градусах Фаренгейта */

    /* Включение кириллицы в консоли */
    setlocale(LC_ALL, "russian");

    /* Ввод F */
    printf("Введите температуру в градусах Фаренгейта = ");
    scanf_s("%d", &f);

    /* Вычисление C */
    c = (f - 32) * 5 / 9;

    /* Вывод результата */
    printf("Температура  %s %p", "в градусах Цельсия", &c);
    getchar();

}