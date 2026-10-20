#include "Serealizer.h"
#include "stdafx.h"

void main()
{
    setlocale(LC_ALL, "ru");

    bool boolVal = false;
    short shortVal = 12;

    Serealizer ser;                                 // Создаем объект класса Serealizer с именем ser
    ser.Serialize(boolVal, shortVal);

    cout << "Сериализация выполнена!" << endl;
    cout << "bool: " << boolVal << endl;
    cout << "short: " << shortVal << endl;
}
