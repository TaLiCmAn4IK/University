#include <iostream>
using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");

    const int MAX_LENGTH = 100;
    char str[MAX_LENGTH];

    cout << "Введите строку (максимум 99 символов): ";
    cin.getline(str, MAX_LENGTH); // Чтение строки с клавиатуры (считывает строку до тех пор, пока не встретит символ новой строки)

    // Защита от выхода за границы массива в случае нечетного количества символов
    for (int i = 0; str[i] != '\0' && str[i + 1] != '\0'; i += 2) {
        // Меняем местами символы (цикл проходит по строке парами символов)
        char temp = str[i];
        str[i] = str[i + 1];
        str[i + 1] = temp;
    }

    cout << "Зашифрованная строка: " << str << endl;

    return 0;
}