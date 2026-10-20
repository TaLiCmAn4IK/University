#include "AsmGen.h"
#include "stdafx.h"
#include <string>

string Join(string str);

void AsmGen::CreateFile(bool boolVal, short shortVal)
{
    ofstream file(ASMFILE);
    string strShort = to_string(shortVal);
    strShort = Join(strShort);

    if (file.is_open())
    {
        file ASMHEADER;

        // bool переменная С МЕТАДАННЫМИ
        file << "; === ДАННЫЕ С МЕТАИНФОРМАЦИЕЙ ===\n";
        file << "; Тип 0x01 = bool, Тип 0x02 = short\n\n";

        file << "FILEBOOL DB " << (int)boolVal << "      ; значение bool\n";
        file << "BOOLTYPE DB 1               ; метаданные: тип bool (0x01)\n";
        file << "BOOLSTR DB \"bool (тип 01):   \", 0" << endl << endl;

        // short литерал С МЕТАДАННЫМИ
        file << "SHORTTYPE DB 2              ; метаданные: тип short (0x02)\n";
        file << "FILESHORT DD " << strShort << "   ; цифры числа " << shortVal << endl;
        file << "SHORTSTR DB \"short (тип 02):  \", 0" << endl << endl
            << ".CODE\n" << endl
            << "main PROC\n" << endl;

        string footer = "";

        // Вывод bool с типом
        footer += "; Вывод bool с метаданными\n";
        footer += "mov al, BOOLTYPE        ; тип bool (0x01)\n";
        footer += "add al, 30h             ; в ASCII\n";
        footer += "mov BOOLSTR + 11, al    ; помещаем тип в строку\n\n";

        footer += "mov al, FILEBOOL\n";
        footer += "add al, 30h\n";
        footer += "mov BOOLSTR + 14, al    ; помещаем значение\n";
        footer += "invoke MessageBoxA, 0, offset BOOLSTR, ADDR TEXT_MESSAGE, OK\n\n";

        // Вывод short с типом
        footer += "; Вывод short с метаданными\n";
        footer += "mov al, SHORTTYPE       ; тип short (0x02)\n";
        footer += "add al, 30h             ; в ASCII\n";
        footer += "mov SHORTSTR + 12, al   ; помещаем тип в строку\n\n";

        footer += "; Вывод цифр числа\n";
        string cTypes = "";
        int startI = 14;  // Начинаем с позиции 14 (после "short (тип 02):  ")
        int countShort = to_string(shortVal).size();

        for (int i = 0; i < countShort; i++)
        {
            footer += "mov eax, FILESHORT " + cTypes;
            footer += "\n";
            footer += "add eax, 30h\n";
            footer += "mov SHORTSTR +" + to_string(startI++);
            footer += ", al\n";
            cTypes += " + type FILESHORT";
        }

        footer += "\ninvoke MessageBoxA, 0, offset SHORTSTR, ADDR TEXT_MESSAGE, OK\n";

        footer += "\n; Завершение программы\n";
        footer += "push - 1\n";
        footer += "call ExitProcess\n";
        footer += "main ENDP\n";
        footer += "end main\n";

        file << footer;
    }
    else
    {
        cout << "File not open" << endl;
    }
}

string Join(string str)
{
    string newStr = "";
    for (size_t i = 0; i < str.size(); i++)
    {
        newStr += str[i];
        if (i + 1 != str.size())
        {
            newStr += ", ";
        }
    }
    return newStr;
}