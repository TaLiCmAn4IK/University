#include <iostream>
#include <cstring> 

using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    const int MAX_LEN = 1000;      
    char inputString[MAX_LEN];       
    char substring[MAX_LEN];         


    cout << "Введите строку, содержащую два символа '*': ";
    cin.getline(inputString, MAX_LEN);

    int length = strlen(inputString);   
    int firstStar = -1;              
    int secondStar = -1;               

    char* ptr = inputString;

    for (int i = 0; i < length; i++) {
        if (inputString[i] == '*') {
            if (firstStar == -1) {
                firstStar = i;
            }
            else {
                secondStar = i;
                break;
            }
        }
    }

    if (firstStar == -1 || secondStar == -1) {
        cout << "В строке должно быть как минимум два символа '*'." << endl;
        return 1;
    }


    int subLength = secondStar - firstStar - 1;

    if (subLength <= 0) {
        cout << "Нет символов между двумя символами '*'." << endl;
        return 1;
    }


    for (int i = 0; i < subLength; i++) {
        substring[i] = inputString[firstStar + 1 + i];
    }
    substring[subLength] = '\0'; 


    cout << "Символы между первым и вторым '*' : " << substring << endl;

    return 0;
}
