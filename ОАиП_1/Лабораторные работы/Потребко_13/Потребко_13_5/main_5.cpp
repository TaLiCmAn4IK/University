#include <iostream>
#include <cstring> 

using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    const int MAX_LEN = 1000;          
    char binaryString[MAX_LEN];        


    cout << "Введите строку, состоящую из групп нулей и единиц, разделённых пробелами: ";
    cin.getline(binaryString, MAX_LEN);

    int length = strlen(binaryString);  
    int countFiveGroups = 0;          


    char* ptr = binaryString;

    while (*ptr != '\0') {

        while (*ptr == ' ') {
            ptr++;
        }

        if (*ptr == '\0') {
            break; 
        }

        char* groupStart = ptr;
        int groupLength = 0;

        while (*ptr == '0' || *ptr == '1') {
            groupLength++;
            ptr++;
        }


        if (groupLength == 5) {
            countFiveGroups++;
        }

    }


    cout << "Количество групп из пяти символов: " << countFiveGroups << endl;

    return 0;
}
