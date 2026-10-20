#include <iostream>
#include <cstring> 

using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    const int MAX_LEN = 1000;         
    char binaryString[MAX_LEN];       


    cout << "Введите строку, состоящую из 0 и 1: ";
    cin.getline(binaryString, MAX_LEN);

    int length = strlen(binaryString); 
    int totalOnes = 0;                


    char* ptr = binaryString;

    while (*ptr != '\0') {
        char currentChar = *ptr;      
        int count = 0;                 


        char* groupStart = ptr;

  
        while (*ptr == currentChar && *ptr != '\0') {
            count++;
            ptr++;
        }


        if (currentChar == '1' && count % 2 != 0) {
            totalOnes += count;
        }
    }


    cout << "Количество единиц в группах с нечётным количеством символов: " << totalOnes << endl;

    return 0;
}
