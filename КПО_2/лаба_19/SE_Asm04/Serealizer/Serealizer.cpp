#include "Serealizer.h"

void Serealizer::Serialize(bool valBool, short valShort)
{
    ofstream file(FILENAME, ios::binary);
    if (file.is_open())
    {
   

        // 1. Записываем тип bool (0x01) перед значением
        char boolType = 0x01;
        file.write(&boolType, 1);                     // Метаданные: тип
        file.write(reinterpret_cast<char*>(&valBool), sizeof(bool)); // Данные

        // 2. Записываем тип short (0x02) перед значением  
        char shortType = 0x02;
        file.write(&shortType, 1);                    // Метаданные: тип
        file.write(reinterpret_cast<char*>(&valShort), sizeof(short)); // Данные

        file.close();

        cout << "Сериализация с метаданными выполнена!" << endl;
        cout << "Формат файла:" << endl;
        cout << "  [0x01][bool] - тип bool + значение" << endl;
        cout << "  [0x02][short] - тип short + значение" << endl;
    }
    else
    {
        cout << "File not open" << endl;
    }
}