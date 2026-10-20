#include <iostream>

using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    int n, m;

    // Ввод размеров матрицы
    cout << "Введите количество строк (n): ";
    cin >> n;
    cout << "Введите количество столбцов (m): ";
    cin >> m;

    // Создание динамического двумерного массива
    int** A = new int* [n];
    for (int i = 0; i < n; ++i) {
        A[i] = new int[m];
    }

    // Ввод элементов матрицы
    cout << "Введите элементы матрицы A:" << endl;
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            cin >> A[i][j];
        }
    }

    int rowWithPositive = -1; // Переменная для хранения номера строки с положительным элементом

    // Проверка строк на наличие положительных элементов
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            if (A[i][j] > 0) {
                rowWithPositive = i; // Запоминаем номер строки
                break; // Выходим из внутреннего цикла, если нашли положительный элемент
            }
        }
        if (rowWithPositive != -1) {
            break; // Выходим из внешнего цикла, если нашли строку с положительным элементом
        }
    }

    // Если найдена строка с положительным элементом
    if (rowWithPositive != -1) {
        cout << "Номер строки с положительным элементом: " << rowWithPositive << endl;

        // Изменяем знаки элементов предыдущей строки, если она существует
        if (rowWithPositive > 0) {
            for (int j = 0; j < m; ++j) {
                A[rowWithPositive - 1][j] = -A[rowWithPositive - 1][j];
            }
            cout << "Элементы предыдущей строки изменены на противоположные.\n";
        }
    }
    else {
        cout << "В матрице нет строк с положительными элементами.\n";
    }

    // Вывод изменённой матрицы
    cout << "Изменённая матрица:\n";
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            cout << A[i][j] << "\t";
        }
        cout << endl;
    }

    // Освобождение памяти
    for (int i = 0; i < n; ++i) {
        delete[] A[i];
    }
    delete[] A;

    return 0;
}
