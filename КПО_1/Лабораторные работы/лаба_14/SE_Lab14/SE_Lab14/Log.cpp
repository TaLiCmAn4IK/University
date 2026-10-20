#include "Parm.h"
#include "Log.h"
#include <cstdarg>

namespace Log
{
	LOG getlog(wchar_t* logfile)
	{
		LOG temp;
		temp.stream.open(logfile);
		if (!temp.stream.is_open())
			throw ERROR_THROW(112);

		wcscpy_s(temp.logfile, PARM_MAX_SIZE, logfile);
		return temp; // Теперь работает благодаря перемещению
	}

	void WriteLine(LOG& log, const char* c, ...)
	{
		va_list args;
		va_start(args, c);

		const char* ptr = c;
		while (ptr != nullptr && strcmp(ptr, "") != 0)
		{
			log.stream << ptr;
			ptr = va_arg(args, const char*);
		}
		va_end(args);
		log.stream << endl;
	}

	void WriteLine(LOG& log, const wchar_t* wc, ...)
	{
		va_list args;
		va_start(args, wc);

		char temp[100];
		const wchar_t* ptr = wc;

		while (ptr != nullptr && wcscmp(ptr, L"") != 0)
		{
			wcstombs(temp, ptr, sizeof(temp));
			log.stream << temp;
			ptr = va_arg(args, const wchar_t*);
		}
		va_end(args);
		log.stream << endl;
	}

	void WriteLog(LOG& log)
	{
		time_t rawtime;
		struct tm timeinfo;
		char buffer[100];

		time(&rawtime);
		localtime_s(&timeinfo, &rawtime);

		log.stream << "-----Протокол----- " << endl;
		strftime(buffer, 100, "Выполнено: %d.%m.%Y %A %H:%M:%S", &timeinfo);
		log.stream << buffer << endl;
	}

	void WriteParm(LOG& log, Parm::PARM parm)
	{
		char inInfo[PARM_MAX_SIZE];
		char outInfo[PARM_MAX_SIZE];
		char logInfo[PARM_MAX_SIZE];

		wcstombs(inInfo, parm.in, sizeof(inInfo));
		wcstombs(outInfo, parm.out, sizeof(outInfo));
		wcstombs(logInfo, parm.log, sizeof(logInfo));

		log.stream << "-----Параметры----- " << endl;
		log.stream << "-in:\t" << inInfo << endl;
		log.stream << "-log:\t" << logInfo << endl;
		log.stream << "-out:\t" << outInfo << endl;
	}

	void WriteIn(LOG& log, In::IN in)
	{
		log.stream << "-----Исходные данные----- " << endl;
		log.stream << "Количество символов: " << in.size << endl;
		log.stream << "Проигнорировано: " << in.ignore << endl;
		log.stream << "Количество строк: " << in.lines << endl;
	}

	void WriteError(LOG& log, Error::ERROR error)
	{
		log.stream << "Ошибка " << error.id << ": " << error.message;
		if (error.inext.line != -1) {
			log.stream << " (строка " << error.inext.line << ", позиция " << error.inext.col << ")";
		}
		log.stream << endl;
	}

	void Close(LOG& log)
	{
		if (log.stream.is_open()) {
			log.stream.close();
		}
	}
}