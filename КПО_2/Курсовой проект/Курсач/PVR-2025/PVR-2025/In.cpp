#include "stdafx.h"
#include "In.h"
#include <fstream>
#include <filesystem>
#include <cwchar>

namespace In {

    IN getin(const Parm::PARM& parm) {
        IN in{};
        unsigned char letter = 0;
        int i = 0;
        int position = 0;
        bool str_literal = false;

        in.size = 0;
        in.lines = 1;
        in.ignor = 0;

        in.text = std::make_unique<char[]>(IN_MAX_LEN_TEXT + 1);

        std::ifstream file;
        char path[PARM_MAX_SIZE]{};
        size_t converted = 0;
        wcstombs_s(&converted, path, PARM_MAX_SIZE, parm.in, PARM_MAX_SIZE - 1);
        file.open(path, std::ios::in | std::ios::binary);
        if (!file.is_open())
            throw ERROR_THROW(102);

        while (in.size < IN_MAX_LEN_TEXT) {
            int ch = file.get();
            if (ch == EOF) break;
            unsigned char letter = (unsigned char)ch;
            int code = in.code[(int)letter];


            if (code & IN::L) {
                str_literal = !str_literal;
                in.text[i++] = (char)letter;
                ++in.size; ++position;
            }
            else if ((code & IN::C) && !str_literal) {
                while (true) {
                    int c2 = file.get();
                    if (c2 == EOF) break;
                    unsigned char l2 = (unsigned char)c2;
                    int code2 = in.code[(int)l2];
                    if (code2 & IN::BR) {
                        ++in.lines;
                        position = 0;
                        in.text[i++] = IN_CODE_END;
                        ++in.size;
                        break;
                    }
                }
            }
            else if ((code & IN::C) && str_literal) {
                in.text[i++] = (char)letter;
                ++in.size; ++position;
            }
            else if (code & IN::BR) {
                ++in.lines;
                position = 0;
                in.text[i++] = IN_CODE_END;
                ++in.size;
            }
            else if (code & IN::T) {
                in.text[i++] = (char)letter;
                ++in.size; ++position;
            }
            else if ((code & IN::I) && !str_literal) {
                ++in.ignor;
                ++position;
            }
            else if ((code & IN::I) && str_literal) {
                in.text[i++] = (char)letter;
                ++in.size; ++position;
            }
            else if (code & IN::F) {
                std::cout << "[IN_DEBUG] bad char code="
                    << (int)letter
                    << " line=" << in.lines
                    << " col=" << position
                    << "\n";
                throw ERROR_THROW_IN(111, in.lines, position);
            }
            else {
                in.text[i++] = (char)letter;
                ++in.size; ++position;
            }
        }

        in.text[i] = '\0';
        return in;
    }
}
