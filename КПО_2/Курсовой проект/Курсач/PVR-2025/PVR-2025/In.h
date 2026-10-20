#pragma once
#include "Error.h"
#include "Parm.h"
#include <memory>

#define IN_MAX_LEN_TEXT (1024 * 1024)
#define IN_CODE_ENDL    '\n'
#define IN_CODE_END     '|'

namespace In {

    struct IN {
        enum {
            T = 1 << 10,
            F = 1 << 11,
            I = 1 << 12,
            BR = 1 << 13,
            C = 1 << 14,
            L = 1 << 15
        };

        int code[256] = {
            F, F, F, F, F, F, F, F, F, I, BR, F, F, BR, F, F,
            F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F,
            I, T, L, T, T, T, T, T, T, T, T, T, T, T, T, T, 
            T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T,
            T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T,
            T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T,
            T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T,
            T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, F,

            F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F,
            F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F,
            F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F,
            F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F,
            F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F,
            F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F,
            F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F,
            F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F
        };

        int    size = 0;
        int    lines = 1;
        int    ignor = 0;

        std::unique_ptr<char[]> text;
    };

    IN getin(const Parm::PARM& parm);
}
