#include "stdafx.h"
#include <iostream>
#include "Error.h"		//обработка ошибок
#include "Parm.h"		//обработка параметров
#include "Log.h"		//работа с протоколом
#include "In.h"			//ввод исходного файла
#include <cwchar>
#include <time.h>
#include "Out.h"
#include "LT.h"  
#include "IT.h"
#include "Lexer.h"

using namespace std;

int _tmain(int argc, _TCHAR* argv[])
{
	setlocale(LC_ALL, "russian");

	cout << "---- тест getparm ----" << endl;
	try
	{
		Parm::PARM parm = Parm::getparm(argc, argv);
		wcout << "-in:" << parm.in << ", -out:" << parm.out << ", -log:" << parm.log << endl << endl;
	}
	catch (Error::ERROR e)
	{
		cout << "ID: " << e.id << " Сообщение: " << e.message << " Строка: " << e.inext.line << " Позиция:" << e.inext.col << endl;
	}

	cout << "---- тест getin----" << endl;
	try
	{
		Parm::PARM parm = Parm::getparm(argc, argv);
		In::IN in = In::getin(parm.in);
		cout << "in.text получен успешно" << endl;
		cout << "Всего символов: " << in.size << endl;
		cout << "Всего строк: " << in.lines << endl;
		cout << "Пропущено: " << in.ignor << endl;
	}
	catch (Error::ERROR e)
	{
		cout << "ID: " << e.id << " Сообщение: " << e.message << " Строка: " << e.inext.line << " Позиция:" << e.inext.col << endl;
	}

	cout << "---- основная программа ----" << endl;

	Log::LOG log = Log::INITLOG;
	try
	{
		cout << "1. Получаем параметры..." << endl;
		Parm::PARM parm = Parm::getparm(argc, argv);

		cout << "2. Открываем лог..." << endl;
		log = Log::getlog(parm.log);
		Log::WriteLog(log);
		Log::WriteParm(log, parm);

		cout << "3. Читаем входной файл..." << endl;
		In::IN in = In::getin(parm.in);
		Log::WriteIn(log, in);

		cout << "4. Запускаем лексический анализ..." << endl;
		Lexer::LEX lex = Lexer::LexAnalysis(in, log);

		cout << "5. Выводим таблицы..." << endl;
		Lexer::PrintLexTable(lex, log);
		Lexer::PrintIdTable(lex, log);

		cout << "6. Записываем выходной файл..." << endl;
		Out::WriteOut(in, parm.out);

		cout << "7. Закрываем лог..." << endl;
		Log::Close(log);

		cout << "УСПЕХ: Все этапы выполнены!" << endl;
	}
	catch (Error::ERROR e)
	{
		cout << "ОШИБКА на этапе: смотри выше" << endl;
		cout << "ID: " << e.id << " Сообщение: " << e.message << endl;
		if (e.inext.line != -1) {
			cout << "Строка: " << e.inext.line << " Позиция: " << e.inext.col << endl;
		}

		// Записываем ошибку в лог если он открыт
		if (log.stream != nullptr && log.stream->is_open()) {
			Log::WriteError(log, e);
			Log::Close(log);
		}
	}

	system("pause");
	return 0;
}