#include "stdafx.h"
#include "AsmGen.h"
#include "Desearealizer.h"
#include <iostream>
using namespace std;

int main()
{
    setlocale(LC_ALL, "ru");
    bool boolVal;
    short shortVal;

    Desearealizer des;
    des.Deserialize(&boolVal, &shortVal);

    cout << "bool: " << boolVal << endl;
    cout << "short: " << shortVal << endl;

    AsmGen AG;
    AG.CreateFile(boolVal, shortVal);

    cout << "ASM פאיכ סמחהאם!" << endl;

    system("pause");
    return 0;
}