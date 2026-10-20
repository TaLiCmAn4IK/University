#include <iostream>
#include <cstring> 

using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    const int ORIGINAL_MAX = 1000; 
    const int INSERT_MAX = 1000;   
    const int RESULT_MAX = 2000;   

    char original[ORIGINAL_MAX];  
    char toInsert[INSERT_MAX];    
    char result[RESULT_MAX];   

    // Ввод исходной строки
    cout << "Введите исходную строку: ";
    cin.getline(original, ORIGINAL_MAX);

    // Ввод строки для вставки
    cout << "Введите строку для вставки: ";
    cin.getline(toInsert, INSERT_MAX);


    int k;
    cout << "Введите позицию вставки (0 - начало строки): ";
    cin >> k;


    int originalLen = strlen(original);
    int insertLen = strlen(toInsert);


    if (k < 0 || k > originalLen) {
        cout << "Некорректная позиция вставки." << endl;
        return 1;
    }

    if (originalLen + insertLen >= RESULT_MAX) {
        cout << "Итоговая строка превышает максимальную допустимую длину." << endl;
        return 1;
    }


    for (int i = 0; i < k; i++) {
        result[i] = original[i];
    }


    for (int i = 0; i < insertLen; i++) {
        result[k + i] = toInsert[i];
    }


    for (int i = k; i < originalLen; i++) {
        result[insertLen + i] = original[i];
    }


    result[originalLen + insertLen] = '\0';

   
    cout << "Итоговая строка после вставки: " << result << endl;

    return 0;
}
