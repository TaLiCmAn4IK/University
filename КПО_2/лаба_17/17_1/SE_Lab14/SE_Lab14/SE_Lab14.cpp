#include "stdafx.h"
#include "Error.h"
#include "Parm.h"
#include "Log.h"
#include "In.h"
#include <fstream>

using namespace std;

void ProcessAndWriteOutput(wchar_t* outFile, In::IN in) {
    ofstream outStream;
    outStream.open(outFile);

    if (!outStream.is_open()) {
        throw ERROR_THROW(115);
    }

    outStream.write((char*)in.text, in.size);
    outStream.close();
}

int _tmain(int argc, _TCHAR* argv[]) {
    setlocale(LC_ALL, "rus");
    Log::LOG log;

    try {
        cout << "---- тест Parm::getparm ----" << endl << endl;
        Parm::PARM parm = Parm::getparm(argc, argv);
        wcout << L"-in: " << parm.in << L", -out: " << parm.out << L", -log: " << parm.log << endl << endl;

        log = Log::getlog(parm.log);
        Log::WriteLog(log);
        Log::WriteParm(log, parm);

        cout << "---- тест In::getin ----" << endl << endl;
        In::IN in = In::getin(parm.in);

        cout << "Текст файла:" << endl;
        cout << in.text << endl;
        cout << "Всего символов: " << in.size << endl;
        cout << "Всего строк: " << in.lines << endl;
        cout << "Пропущено: " << in.ignore << endl;

        cout << "---- запись в выходной файл ----" << endl << endl;
        ProcessAndWriteOutput(parm.out, in);
        cout << "Результат записан в файл: ";
        wcout << parm.out << endl;

        Log::WriteIn(log, in);
        Log::WriteLine(log, "Тест: без ошибок", "");
        Log::WriteLine(log, "Преобразование выполнено успешно. Результат записан в:", "");

        char outFile[PARM_MAX_SIZE];
        wcstombs(outFile, parm.out, sizeof(outFile));
        Log::WriteLine(log, outFile, "");

        Log::Close(log);
        cout << "Обработка завершена успешно!" << endl;
    }
    catch (Error::ERROR e) {
        cout << "Ошибка " << e.id << ": " << e.message << endl;
        if (e.inext.line != -1 && e.inext.col != -1) {
            cout << "Строка: " << e.inext.line << ", позиция: " << e.inext.col << endl;
        }

        // Обработка ошибок...
    }

    system("pause");
    return 0;
}