#include "stdafx.h"
#include "Log.h"
#include <locale>

namespace Log {

    LOG getlog(const wchar_t logfile[])
    {
        LOG log;
        wcscpy_s(log.logfile, logfile);

        log.stream = new std::wofstream;
        log.stream->open(logfile, std::ios::out | std::ios::trunc);
        if (log.stream->fail())
            throw ERROR_THROW(112);

        try {
            log.stream->imbue(std::locale("ru_RU.UTF-8"));
        }
        catch (...) {
            try { log.stream->imbue(std::locale("Russian_Russia.65001")); }
            catch (...) { /* оставляем как есть */ }
        }

        return log;
    }

    void WriteLine(LOG log, const wchar_t* s)
    {
        if (!log.stream) return;
        (*log.stream) << (s ? s : L"") << L"\n";
        log.stream->flush();
    }

    void WriteLine(LOG log, const std::wstring& s)
    {
        WriteLine(log, s.c_str());
    }

    void WriteLog(LOG log)
    {
        time_t rawtime;
        tm current_time{};
        wchar_t wtemp[128]{};

        time(&rawtime);
        localtime_s(&current_time, &rawtime);

        WriteLine(log, L"|---------- Протокол ---------|");

        wcsftime(wtemp, sizeof(wtemp) / sizeof(wtemp[0]),
            L"| Время: %d.%m.%Y, %H:%M:%S |",
            &current_time);
        WriteLine(log, wtemp);

        WriteLine(log, L"|-----------------------------|");
    }

    void WriteParm(LOG log, const Parm::PARM& parm)
    {
        WriteLine(log, L"");
        WriteLine(log, L"---- Параметры ----");
        WriteLine(log, L"");

        WriteLine(log, std::wstring(L"-in:  ") + parm.in);
        WriteLine(log, std::wstring(L"-out: ") + parm.out);
        WriteLine(log, std::wstring(L"-log: ") + parm.log);
    }

    void WriteIn(LOG log, const In::IN& in)
    {
        WriteLine(log, L"");
        WriteLine(log, L"---- Данные ----");
        WriteLine(log, L"");

        WriteLine(log, std::wstring(L"Всего символов: ") + std::to_wstring(in.size));
        WriteLine(log, std::wstring(L"Всего строк: ") + std::to_wstring(in.lines));
        WriteLine(log, std::wstring(L"Пропущено: ") + std::to_wstring(in.ignor));
    }

    void WriteError(LOG log, const Error::ERROR& error)
    {
        if (!log.stream) return;

        (*log.stream) << L"\nОшибка " << error.id
            << L"\nСтрока " << error.inext.line
            << L" позиция " << error.inext.col << L"\n";
        log.stream->flush();
    }

    void Close(LOG log)
    {
        if (log.stream) {
            log.stream->flush();
            log.stream->close();
            delete log.stream;
            log.stream = nullptr;
        }
    }
}

void cleanup(char source[], int size, Log::LOG logfile)
{
    const char Separators[] = " ,;(){}=<>!+-*/|%";
    bool inLiteral = false;
    int  quoteCount = 0;

    for (int i = 0; i < size; ++i) {
        if (source[i] == '"') { inLiteral = !inLiteral; ++quoteCount; }
    }

    if (quoteCount % 2 != 0)
        Log::WriteError(logfile, Error::geterror(300));
}


char** divideText(char source[], int size)
{
    char** word = new char* [MAX_WORD];
    for (int i = 0; i < MAX_WORD; ++i)
        word[i] = new char[SIZE_WORD] { 0 };

    bool findSeparator = false;
    bool findLiteral = false;
    int  j = 0;
    const char Separators[] = " ,;(){}=<>!+-*/%|";

    for (int i = 0, k = 0; i < size; ++i, ++k) {
        findSeparator = false;
        if (source[i] == '"') findLiteral = !findLiteral;

        for (int t = 0; t < (int)strlen(Separators); ++t) {
            if (source[i] == Separators[t] && !findLiteral) {
                findSeparator = true;

                if (word[j][0] != 0) { word[j++][k] = '\0'; k = 0; }

                if (Separators[t] == ' ') { k = -1; break; }

                word[j][k++] = Separators[t];

                if ((Separators[t] == '=' || Separators[t] == '!' || Separators[t] == '<' || Separators[t] == '>')
                    && i + 1 < size && source[i + 1] == '=' && !findLiteral)
                {
                    word[j][k++] = '=';
                    ++i;
                }

                word[j++][k] = '\0';
                k = -1;
                break;
            }
        }

        if (!findSeparator)
            word[j][k] = source[i];
    }

    word[j] = nullptr;
    for (int i = 0; i < j; ++i)
        if (!strcmp(word[i], "")) return nullptr;

    return word;
}
