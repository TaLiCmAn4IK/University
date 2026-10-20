#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <stdlib.h>
#include <iostream>

int main(int argc, char* argv[]) {
    setlocale(LC_CTYPE, "Russian");
    // Проверка количества аргументов командной строки
    if (argc != 3) {
        printf("Использование: %s output_file k\n", argv[0]);
        return 1;
    }

    const char* output_filename = argv[1];  // Имя выходного файла
    int k = atoi(argv[2]);                   // Номер строки для извлечения

    // Проверка корректности значения k
    if (k <= 0) {
        printf("Некорректный номер строки k: %d. Должен быть положительным.\n", k);
        return 1;
    }

    FILE* fin;
    const char* input_filename = "C:\\Документы\\ОАиП_2\\Потребко_2\\matrix.txt"; // Жестко заданный путь к входному файлу

    // Открытие входного файла с использованием fopen_s
    errno_t err = fopen_s(&fin, input_filename, "r");
    if (err != 0 || fin == NULL) {
        printf("Ошибка открытия входного файла: %s\n", input_filename);
        return 1;
    }

    double n_float;
    // Чтение первого числа из файла, представляющего количество столбцов (n)
    if (fscanf(fin, "%lf", &n_float) != 1) {
        printf("Ошибка чтения количества столбцов.\n");
        fclose(fin);
        return 1;
    }

    int n = (int)n_float;
    // Проверка корректности значения n
    if (n <= 0) {
        printf("Некорректное количество столбцов: %d\n", n);
        fclose(fin);
        return 1;
    }

    // Проверка, что k не превышает количество строк (так как матрица квадратная)
    if (k > n) {
        printf("Некорректный номер строки k: %d. В матрице %d строк.\n", k, n);
        fclose(fin);
        return 1;
    }

    FILE* fout;
    // Открытие выходного файла для записи с использованием fopen_s
    err = fopen_s(&fout, output_filename, "w");
    if (err != 0 || fout == NULL) {
        printf("Ошибка открытия выходного файла: %s\n", output_filename);
        fclose(fin);
        return 1;
    }

    double num;
    // Итерация по строкам матрицы
    for (int row = 1; row <= n; row++) {
        // Итерация по столбцам текущей строки
        for (int col = 1; col <= n; col++) {
            // Чтение текущего элемента матрицы
            if (fscanf(fin, "%lf", &num) != 1) {
                printf("Ошибка чтения элемента матрицы в строке %d, столбце %d.\n", row, col);
                fclose(fin);
                fclose(fout);
                return 1;
            }
            // Если текущая строка - это k-я строка, записываем элемент в выходной файл
            if (row == k) {
                fprintf(fout, "%.6lf ", num); // Форматирование числа с 6 знаками после запятой
            }
        }
        // После завершения k-й строки добавляем перевод строки в выходной файл
        if (row == k) {
            fprintf(fout, "\n");
        }
    }

    // Закрытие файлов
    fclose(fin);
    fclose(fout);

    printf("К %d-й строке матрицы успешно записано в файл %s\n", k, output_filename);

    return 0;
}
