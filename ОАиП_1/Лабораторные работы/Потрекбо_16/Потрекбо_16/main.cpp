#include <iostream>
using namespace std;

// Прототипы функций
void replaceDiagonalWithMin(int** A, int row, int col);
void findEqualRowsAndColumns(int** A, int size);
void matrix();

int main(void)
{
    setlocale(LC_CTYPE, "Russian");

    int choice;
    do
    {
        cout << "Выберите вариант работы" << endl;
        cout << "1 - с матрицей" << endl;
        cout << "2 - выход" << endl;
        cin >> choice;

        switch (choice)
        {
        case 1:
            matrix();
            break;

        case 2:
            break;

        default:
            cout << "Неверный ввод! Пожалуйста, попробуйте снова." << endl;
            break;
        }
    } while (choice != 2);  // Исправлено с 'c' на 'choice'

    return 0;
}

void matrix()
{
    int** A, row, col, i, j;

    cout << "Введите число строк матрицы (d <= 16): ";
    cin >> row;
    cout << "Введите число столбцов (p <= 18): ";
    cin >> col;

    // Проверка на правильность введенных размеров
    if (row > 16 || col > 18) {
        cout << "Ошибка: превышены максимальные размеры матрицы." << endl;
        return;
    }

    // Выделение памяти для матрицы
    A = new int* [row];
    for (i = 0; i < row; i++)
        A[i] = new int[col];

    // Ввод элементов матрицы
    for (i = 0; i < row; i++)
        for (j = 0; j < col; j++)
        {
            cout << "Введите A[" << i << "][" << j << "]: ";
            cin >> *(*(A + i) + j);
        }

    // Выполнение замены диагонали
    replaceDiagonalWithMin(A, row, col);

    // Поиск совпадающих строк и столбцов
    findEqualRowsAndColumns(A, row);

    // Освобождение памяти
    for (i = 0; i < row; i++)
        delete[] A[i];
    delete[] A;
}

void replaceDiagonalWithMin(int** A, int row, int col)
{
    for (int i = 0; i < row && i < col; i++)
    {
        int minElement = A[i][0];
        // Нахождение минимального значения в строке
        for (int j = 1; j < col; j++)
        {
            if (A[i][j] < minElement)
                minElement = A[i][j];
        }
        // Замена элемента на главной диагонали
        A[i][i] = minElement;
    }

    // Вывод результата после замены диагонали
    cout << "Матрица после замены элементов главной диагонали:" << endl;
    for (int i = 0; i < row; i++)
    {
        for (int j = 0; j < col; j++)
        {
            cout << A[i][j] << "\t";
        }
        cout << endl;
    }
}

void findEqualRowsAndColumns(int** A, int size)
{
    for (int k = 0; k < size; k++)
    {
        bool equal = true;
        for (int j = 0; j < size; j++)
        {
            if (A[k][j] != A[j][k])
            {
                equal = false;
                break;
            }
        }
        if (equal)
        {
            cout << k << " ";
        }
    }
    cout << endl;
}