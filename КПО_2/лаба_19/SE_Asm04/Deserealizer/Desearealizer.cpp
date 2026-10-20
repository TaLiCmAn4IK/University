#include "Desearealizer.h"
#include "stdafx.h"
#include <iostream>
using namespace std;

void Desearealizer::Deserialize(bool* valBool, short* valShort)
{
    ifstream file(FILENAME, ios::binary);
    if (file.is_open())
    {
        char type;

        // Читаем bool с метаданными
        file.read(&type, 1);  // Читаем тип
        if (type == 0x01) {
            cout << "Найден тип 0x01 (bool)" << endl;
        }
        file.read(reinterpret_cast<char*>(valBool), sizeof(bool));

        // Читаем short с метаданными
        file.read(&type, 1);  // Читаем тип 
        if (type == 0x02) {
            cout << "Найден тип 0x02 (short)" << endl;
        }
        file.read(reinterpret_cast<char*>(valShort), sizeof(short));

        file.close();
    }
    else
    {
        cout << "File not open" << endl;
    }
}