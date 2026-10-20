#include <iostream>
#include <cstring> 

using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    const int MAX_LEN = 1000;         
    char text[MAX_LEN];                // Массив для хранения исходного текста
    char result[MAX_LEN];              


    cout << "Введите текст: ";
    cin.getline(text, MAX_LEN);

    int length = strlen(text);      
    int j = 0;                     

    bool insideQuotes = false;         

    for (int i = 0; i < length; i++) {

        if (text[i] == '"' || text[i] == '\'') {

            insideQuotes = !insideQuotes;

            continue;
        }

        if (!insideQuotes) {

            result[j] = text[i];
            j++;
        }

    }

    result[j] = '\0'; 


    cout << "Текст после удаления частей в кавычках: " << endl;
    cout << result << endl;

    return 0;
}
