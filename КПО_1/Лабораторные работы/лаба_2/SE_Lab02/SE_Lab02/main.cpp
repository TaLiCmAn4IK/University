#include <iostream>      
#include <iomanip>       // Для манипуляторов форматирования (setw, setfill, hex)
#include <limits>        // Для получения числовых ограничений
using namespace std;

int main() {
    setlocale(LC_ALL, "Russian");
    const int n = 12; // Порядковый номер студента
    // Вычисляем значения
    int X = 9 + n;     // X = 21
    int Y = 10 + n;    // Y = 22 
    int Z = 11 + n;    // Z = 23
    float S = 1.0f + n; // S = 13.0

    cout << "Лабораторная работа 2: Фундаментальные типы данных" << endl;
    cout << "Порядковый номер студента (n) = " << n << endl;
    cout << "X = " << X << ", Y = " << Y << ", Z = " << Z << ", S = " << S << endl << endl;

    // Задание 4: Объявляем две переменные типа bool и инициализируем их значениями false и true.
    bool boolTrue = true;
    bool boolFalse = false;

    // Задание 5: Объявляем переменную типа char с латинским символом (второй символ фамилии на английском).
    // Фамилия Potrebko - "o"
    char engChar = 'o'; // ASCII

    // Задание 6: Объявляем переменную типа char с русским символом (второй символ фамилии на русском).
    // Фамилия Потребко - "о"
    char rusChar = 'о'; // CP1251

    // Задание 7: Объявляем переменную типа wchar_t с латинским символом (первый символ фамилии на английском).
    // Фамилия Potrebko - "P"
    wchar_t wEng = L'P'; // UTF-16

    // Задание 8: Объявляем переменную типа wchar_t с русским символом (первый символ фамилии на русском).
    // Фамилия Потребко - "П"
    wchar_t wRus = L'П'; // CP1251

    // Задание 9: Объявляем две переменные типа short с значениями X и -X.
    short shortPos = X;  // 21
    short shortNeg = -X; // -21

    // Задание 10: Самое большое и самое маленькое число типа short.
    short shortMax = 0x7FFF;             // 0x7FFF = 32767
    short shortMin = static_cast<short>(0x8000); // 0x8000 = -32768 (в двухкомплементарном представлении)
    cout << "Задание 10" << endl;
    cout << "shortMax (0x7FFF) в десятичном виде: " << shortMax << endl;
    cout << "shortMin (0x8000) в десятичном виде: " << shortMin << endl;
    cout << endl;

    // Задание 11: Самое большое и самое маленькое число типа unsigned short.
    unsigned short ushortMax = 0xFFFF; // 65535
    unsigned short ushortMin = 0x0000; // 0
    cout << "Задание 11" << endl;
    cout << "ushortMax (0xFFFF) в десятичном виде: " << ushortMax << endl;
    cout << "ushortMin (0x0000) в десятичном виде: " << ushortMin << endl;
    cout << endl;

    // Задание 12: Объявляем две переменные типа int с значениями Y и -Y.
    int intPos = Y;   // 22
    int intNeg = -Y;  // -22

    // Задание 13: Самое большое и самое маленькое число типа int.
    int intMax = 0x7FFFFFFF;              // 2147483647
    int intMin = static_cast<int>(0x80000000); // -2147483648 
    cout << "Задание 13" << endl;
    cout << "intMax (0x7FFFFFFF) в десятичном виде: " << intMax << endl;
    cout << "intMin (0x80000000) в десятичном виде: " << intMin << endl;
    cout << endl;

    // Задание 14: Самое большое и самое маленькое число типа unsigned int.
    unsigned int uintMax = 0xFFFFFFFF; // 4294967295
    unsigned int uintMin = 0x00000000; // 0
    cout << "Задание 14" << endl;
    cout << "uintMax (0xFFFFFFFF) в десятичном виде: " << uintMax << endl;
    cout << "uintMin (0x00000000) в десятичном виде: " << uintMin << endl;
    cout << endl;

    // Задание 15: Объявляем две переменные типа long с значениями Z и -Z.
    long longPos = Z;   // 23
    long longNeg = -Z;  // -23

    // Задание 16: Самое большое и самое маленькое число типа long.
    long longMax = 0x7FFFFFFF;              // 2147483647
    long longMin = static_cast<long>(0x80000000); // -2147483648
    cout << "Задание 16: long max и min" << endl;
    cout << "longMax (0x7FFFFFFF) в десятичном виде: " << longMax << endl;
    cout << "longMin (0x80000000) в десятичном виде: " << longMin << endl;
    cout << endl;

    // Задание 17: Самое большое и самое маленькое число типа unsigned long.
    unsigned long ulongMax = 0xFFFFFFFF; // 4294967295
    unsigned long ulongMin = 0x00000000; // 0
    cout << "Задание 17: unsigned long max и min" << endl;
    cout << "ulongMax (0xFFFFFFFF) в десятичном виде: " << ulongMax << endl;
    cout << "ulongMin (0x00000000) в десятичном виде: " << ulongMin << endl;
    cout << endl;

    // Задание 18: Объявляем две переменные типа float с значениями S и -S.
    float floatPos = S;   // 13.0
    float floatNeg = -S;  // -13.0


    // Задание 19: Операции, дающие специальные значения (бесконечность и неопределённое значение)
    //float posInf = 1.0f / 0.0f;   При делении на ноль получается +бесконечность (обычно отображается как 1.#INF)
    //float negInf = -1.0f / 0.0f;  -бесконечность (обычно отображается как -1.#INF)
    //float indVal = 0.0f / 0.0f;   Неопределённое значение (NaN, может отображаться как -1.#IND)

    // Задание 20: Указатели на различные типы и арифметика указателей
// Объявление и инициализация переменных
    char char_val = 'A';
    wchar_t wchar_val = L'Б';
    short short_val = 10;
    int int_val = 100;
    float float_val = 3.14f;
    double double_val = 2.71828;

    // Объявление и инициализация указателей
    char* char_ptr = &char_val;
    wchar_t* wchar_ptr = &wchar_val;
    short* short_ptr = &short_val;
    int* int_ptr = &int_val;
    float* float_ptr = &float_val;
    double* double_ptr = &double_val;

    // Вывод значений указателей (до увеличения)
    cout << "Изначальное значение:" << endl;
    cout << "char_ptr: " << (void*)char_ptr << endl; // (void*) необходимо для правильного вывода
    cout << "wchar_ptr: " << (void*)wchar_ptr << endl;
    cout << "short_ptr: " << (void*)short_ptr << endl;
    cout << "int_ptr: " << (void*)int_ptr << endl;
    cout << "float_ptr: " << (void*)float_ptr << endl;
    cout << "double_ptr: " << (void*)double_ptr << endl;

    // Увеличение значений указателей на 3
    char_ptr += 3;
    wchar_ptr += 3;
    short_ptr += 3;
    int_ptr += 3;
    float_ptr += 3;
    double_ptr += 3;

    // Вывод значений указателей (после увеличения)
    cout << "\nПосле увеличения на 3:" << endl;
    cout << "char_ptr: " << (void*)char_ptr << endl;
    cout << "wchar_ptr: " << (void*)wchar_ptr << endl;
    cout << "short_ptr: " << (void*)short_ptr << endl;
    cout << "int_ptr: " << (void*)int_ptr << endl;
    cout << "float_ptr: " << (void*)float_ptr << endl;
    cout << "double_ptr: " << (void*)double_ptr << endl;

    //Объяснение
    cout << "\nОбъяснение:" << endl;
    cout << "char_ptr * sizeof(char) = 3 * 1 = 3 байта." << endl;
    cout << "wchar_ptr * sizeof(wchar_t) = 3 * 2 = 6 байта." << endl;
    cout << "short_ptr * sizeof(short) = 3 * 2 = 6 байта." << endl;
    cout << "int_ptr * sizeof(int) = 3 * 4 = 12 байта." << endl;
    cout << "float_ptr * sizeof(float) = 3 * 4 = 12 байта." << endl;
    cout << "double_ptr * sizeof(double) = 3 * 8 = 24 байта." << endl;
    cout << endl;

    return 0;
}
