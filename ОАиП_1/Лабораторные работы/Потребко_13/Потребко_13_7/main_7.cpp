#include <iostream>
#include <cstring>

using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    const int MAX_LEN = 1000;         
    char inputString[MAX_LEN];      
    char numberSubstring[MAX_LEN];     

    cout << "Введите строку, состоящую из букв, цифр, запятых, точек, знаков + и -: ";
    cin.getline(inputString, MAX_LEN);

    int length = strlen(inputString);    
    bool numberFound = false;           
    int start = 0;                      
    int end = 0;                        
   
    char* ptr = inputString;

  
    for (int i = 0; i < length; i++) {

        if ((inputString[i] == '+' || inputString[i] == '-') && (i + 1 < length) && (inputString[i + 1] >= '0' && inputString[i + 1] <= '9')) {
            
 
            int j = i + 1;
            while (j < length && inputString[j] >= '0' && inputString[j] <= '9') {
                j++;
            }
            end = j;
            numberFound = true;
            break;
        }
    
        else if (inputString[i] >= '0' && inputString[i] <= '9' && (i == 0 || !(inputString[i - 1] >= '0' && inputString[i - 1] <= '9'))) {
            start = i; 
      
            int j = i + 1;
            while (j < length && inputString[j] >= '0' && inputString[j] <= '9') {
                j++;
            }
            end = j;
            numberFound = true;
            break;
        }
    }

    if (numberFound) {
        // Вычисляем длину подстроки
        int numLength = end - start;
        // Копируем подстроку числа в отдельный массив
        for (int i = 0; i < numLength; i++) {
            numberSubstring[i] = inputString[start + i];
        }
        numberSubstring[numLength] = '\0'; 


        cout << "Выделенная подстрока, соответствующая целому числу: " << numberSubstring << endl;
    }
    else {
        cout << "В строке не найдено целого числа." << endl;
    }

    return 0;
}
