#include <iostream>
using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    const int MAX_LENGTH = 100; 
    char str[MAX_LENGTH];       
    int commaCount = 0;         // запятые
    int periodCount = 0;        // точки

    cout << "Введите строку (максимум 99 символов): ";
    cin.getline(str, MAX_LENGTH);

    // проходим по каждому символу в строке, пока не встретим символ конца строки
    for (int i = 0; str[i] != '\0'; ++i) { // цикл работает до нуль-символа
        if (str[i] == ',') {       // если текущий символ - запятая
            commaCount++;          // увеличиваем счетчик запятых
        }
        else if (str[i] == '.') {  
            periodCount++;         
        }
    }

    cout << "Количество запятых: " << commaCount << endl;
    cout << "Количество точек: " << periodCount << endl;

    return 0;
}