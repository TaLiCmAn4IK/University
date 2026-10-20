#include <iostream>
using namespace std;

int main() {
    setlocale(LC_ALL, "RUS");
    int N;

    cout << "Введите размер последовательности: "; cin >> N;
    int* arr = new int[N];
    int* buff = new int[N];
    int* path = new int[N];

    for (int i = 0; i < N; i++) {
        cout << "Введите " << i + 1 << " элемент последовательности: "; cin >> arr[i];
        buff[i] = 1;
        path[i] = -1;
    }

    cout << "\nВведенная последовательность: ";
    for (int i = 0; i < N; i++) {
        cout << arr[i] << " ";
    }
    cout << endl;

    // Возрастающая подпоследовательность
    for (int j = 1; j < N; j++) {
        for (int k = 0; k < j; k++) {
            if (arr[j] > arr[k]) {
                if (buff[j] <= buff[k]) {
                    buff[j] = buff[k] + 1;
                    path[j] = k;
                }
            }
        }
    }

    int max_len = 0, end_index = 0;
    for (int i = 0; i < N; i++) {
        if (buff[i] > max_len) {
            max_len = buff[i];
            end_index = i;
        }
    }

    int* subsequence = new int[max_len];
    int index = max_len - 1;
    for (int i = end_index; i >= 0; i = path[i]) {
        subsequence[index--] = arr[i];
        if (path[i] == -1) break;
    }

    cout << "\nМаксимальная возрастающая подпоследовательность: ";
    for (int i = 0; i < max_len; i++) {
        cout << subsequence[i] << " ";
    }
    cout << endl;

    // Убывающая подпоследовательность
    for (int i = 0; i < N; i++) {
        buff[i] = 1;
        path[i] = -1;
    }

    for (int j = 1; j < N; j++) {
        for (int k = 0; k < j; k++) {
            if (arr[j] < arr[k]) {  // Изменено условие на < для убывающей
                if (buff[j] <= buff[k]) {
                    buff[j] = buff[k] + 1;
                    path[j] = k;
                }
            }
        }
    }

    max_len = 0, end_index = 0;
    for (int i = 0; i < N; i++) {
        if (buff[i] > max_len) {
            max_len = buff[i];
            end_index = i;
        }
    }

    index = max_len - 1;
    for (int i = end_index; i >= 0; i = path[i]) {
        subsequence[index--] = arr[i];
        if (path[i] == -1) break;
    }

    cout << "Максимальная убывающая подпоследовательность: ";
    for (int i = 0; i < max_len; i++) {
        cout << subsequence[i] << " ";
    }
    cout << endl;

    // Подпоследовательность с разницей 1 (возрастающая)
    for (int i = 0; i < N; i++) {
        buff[i] = 1;
        path[i] = -1;
    }

    for (int j = 1; j < N; j++) {
        for (int k = 0; k < j; k++) {
            if (arr[j] > arr[k] && arr[j] - arr[k] == 1) {
                if (buff[j] <= buff[k]) {
                    buff[j] = buff[k] + 1;
                    path[j] = k;
                }
            }
        }
    }

    max_len = 0, end_index = 0;
    for (int i = 0; i < N; i++) {
        if (buff[i] > max_len) {
            max_len = buff[i];
            end_index = i;
        }
    }

    index = max_len - 1;
    for (int i = end_index; i >= 0; i = path[i]) {
        subsequence[index--] = arr[i];
        if (path[i] == -1) break;
    }

    cout << "Подпоследовательность с разницей 1: ";
    for (int i = 0; i < max_len; i++) {
        cout << subsequence[i] << " ";
    }
    cout << endl;

    delete[] arr;
    delete[] buff;
    delete[] path;
    delete[] subsequence;

    return 0;  // Добавлен возврат значения для main()
}