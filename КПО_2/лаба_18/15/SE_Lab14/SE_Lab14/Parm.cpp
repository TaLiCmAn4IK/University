#include "stdafx.h"
#include "Parm.h"
#include "Error.h"

namespace Parm
{
    PARM getparm(int argc, wchar_t* argv[])
    {
        PARM parm;

        // Инициализация буферов
        parm.in[0] = L'\0';
        parm.out[0] = L'\0';
        parm.log[0] = L'\0';

        bool in_find = false, out_find = false, log_find = false;

        for (int i = 1; i < argc; i++)
        {
            if (wcslen(argv[i]) > PARM_MAX_SIZE) {
                throw ERROR_THROW(104);
            }

            if (wcsstr(argv[i], PARM_IN))
            {
                const wchar_t* in_value = argv[i] + wcslen(PARM_IN);
                wcsncpy_s(parm.in, PARM_MAX_SIZE, in_value, _TRUNCATE);
                in_find = true;
            }
            if (wcsstr(argv[i], PARM_OUT))
            {
                const wchar_t* out_value = argv[i] + wcslen(PARM_OUT);
                wcsncpy_s(parm.out, PARM_MAX_SIZE, out_value, _TRUNCATE);
                out_find = true;
            }
            if (wcsstr(argv[i], PARM_LOG))
            {
                const wchar_t* log_value = argv[i] + wcslen(PARM_LOG);
                wcsncpy_s(parm.log, PARM_MAX_SIZE, log_value, _TRUNCATE);
                log_find = true;
            }
        }

        if (!in_find) throw ERROR_THROW(100);

        if (!out_find)
        {
            wcsncpy_s(parm.out, PARM_MAX_SIZE, parm.in, _TRUNCATE);
            wcsncat_s(parm.out, PARM_MAX_SIZE, PARM_OUT_DEFAULT_EXT, _TRUNCATE);
        }

        if (!log_find)
        {
            wcsncpy_s(parm.log, PARM_MAX_SIZE, parm.in, _TRUNCATE);
            wcsncat_s(parm.log, PARM_MAX_SIZE, PARM_LOG_DEFAULT_EXT, _TRUNCATE);
        }
        return parm;
    }
}