#pragma once
#include "stdafx.h"

#include <fstream>
#include <string>
#include <iostream>
#include <iomanip>

#include "LT.h"
#include "In.h"
#include "Parm.h"
#include "Error.h"

#define MAX_WORD   1000
#define SIZE_WORD  256

namespace Log {

    struct LOG {
        wchar_t        logfile[PARM_MAX_SIZE]{};
        std::wofstream* stream = nullptr;
    };

    static const LOG INITLOG = { L"", nullptr };

    LOG  getlog(const wchar_t logfile[]);

    void WriteLine(LOG log, const wchar_t* s);
    void WriteLine(LOG log, const std::wstring& s);

    void WriteLog(LOG log);
    void WriteParm(LOG log, const Parm::PARM& parm);
    void WriteIn(LOG log, const In::IN& in);
    void WriteError(LOG log, const Error::ERROR& error);
    void Close(LOG log);
}

// Вспомогательные функции для лексического анализатора
char** divideText(char source[], int size);
void   cleanup(char source[], int size, Log::LOG logfile);
