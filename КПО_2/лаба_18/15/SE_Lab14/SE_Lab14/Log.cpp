#include "stdafx.h"
#include <cstdarg>
#include "Error.h"

using namespace std;

namespace Log {
    LOG getlog(wchar_t logfile[]) {
        LOG log;
        log.stream = new std::ofstream;
        log.stream->open(logfile);
        if (!log.stream->is_open())
        {
            throw ERROR_THROW(112);
        }
        wcscpy_s(log.logfile, PARM_MAX_SIZE, logfile);
        return log;
    }

    void WriteLine(LOG log, char* c, ...) {
        // Простая и безопасная версия
        if (c != nullptr) {
            *log.stream << c;
        }
        *log.stream << std::endl;
    }

    void WriteLine(LOG log, wchar_t* c, ...) {
        // Безопасная конвертация wchar_t в char
        if (c != nullptr) {
            size_t convertedChars = 0;
            char buffer[PARM_MAX_SIZE] = "";

            wcstombs_s(&convertedChars, buffer, PARM_MAX_SIZE, c, _TRUNCATE);
            *log.stream << buffer;
        }
        *log.stream << std::endl;
    }

    void WriteLog(LOG log) {
        char date[100];
        tm local;
        time_t currentTime;
        currentTime = time(NULL);
        localtime_s(&local, &currentTime);
        strftime(date, 100, "%d.%m.%Y %H:%M:%S", &local);
        *log.stream << "--- Протокол --- Дата: " << date << endl;
    }

    void WriteParm(LOG log, Parm::PARM parm) {
        char buff[PARM_MAX_SIZE];
        size_t tsize = 0;

        *log.stream << "---- Параметры ------" << endl;

        wcstombs_s(&tsize, buff, PARM_MAX_SIZE, parm.log, _TRUNCATE);
        *log.stream << "-log: " << buff << endl;

        wcstombs_s(&tsize, buff, PARM_MAX_SIZE, parm.out, _TRUNCATE);
        *log.stream << "-out: " << buff << endl;

        wcstombs_s(&tsize, buff, PARM_MAX_SIZE, parm.in, _TRUNCATE);
        *log.stream << "-in: " << buff << endl;
    }

    void WriteIn(LOG log, In::IN in) {
        *log.stream << "---- Исходные данные ------" << endl;
        *log.stream << "Количество символов: " << in.size << endl;
        *log.stream << "Проигнорированно   : " << in.ignor << endl;
        *log.stream << "Количество строк   : " << in.lines << endl;

        // Безопасный вывод текста
        if (in.text != nullptr && in.size > 0) {
            *log.stream << "Текст: ";
            for (int i = 0; i < in.size && i < 500; i++) { // ограничиваем вывод
                *log.stream << in.text[i];
            }
            *log.stream << endl;
        }
    }

    void WriteError(LOG log, Error::ERROR error) {
        *log.stream << "--- Ошибки --- " << endl;
        *log.stream << "Ошибка " << error.id << ":" << error.message << endl;

        if (error.inext.line != -1)
        {
            *log.stream << "Строка " << error.inext.line << " Символ: " << error.inext.col << endl << endl;
        }
    }

    void Close(LOG log) {
        if (log.stream != nullptr) {
            log.stream->close();
            delete log.stream;
        }
    }
};