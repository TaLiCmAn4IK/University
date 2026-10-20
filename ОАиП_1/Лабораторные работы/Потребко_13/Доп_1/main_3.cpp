#include <iostream>
#include <cstring> // Для функции strlen

using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    const int MAX_LEN = 1000;          // Максимальная длина текста
    char text[MAX_LEN];                // Массив для хранения исходного текста
    char result[MAX_LEN];              // Массив для хранения результирующего текста

    // Ввод текста от пользователя
    cout << "Введите текст: ";
    cin.getline(text, MAX_LEN);

    int length = strlen(text);          // Длина исходного текста
    int j = 0;                          // Индекс для результирующего текста

    bool insideQuotes = false;          // Флаг, указывающий, находится ли текущий символ внутри кавычек

    for (int i = 0; i < length; i++) {
        // Проверяем, является ли текущий символ кавычкой (одинарной или двойной)
        if (text[i] == '"' || text[i] == '\'') {
            // Переключаем состояние флага внутри кавычек
            insideQuotes = !insideQuotes;
            // Не копируем саму кавычку в результирующий текст
            continue;
        }

        if (!insideQuotes) {
            // Если не внутри кавычек, копируем символ в результат
            result[j] = text[i];
            j++;
        }
        // Если внутри кавычек, пропускаем символ
    }

    result[j] = '\0'; // Завершаем результирующую строку символом конца строки

    // Выводим результирующий текст
    cout << "Текст после удаления частей в кавычках: " << endl;
    cout << result << endl;

    return 0;
}
