#include "stdafx.h"

using namespace std;

namespace Out {
    OUT getout(wchar_t outfile[]) {
        OUT out;
        out.stream = new std::ofstream;
        out.stream->open(outfile);
        if (!out.stream->is_open())
        {
            throw ERROR_THROW(112);
        }
        wcscpy_s(out.outfile, PARM_MAX_SIZE, outfile);
        return out;
    }

    void WriteOut(In::IN in, wchar_t outfile[]) {
        ofstream fout(outfile);
        if (fout.is_open() && in.text != nullptr) {
            // Безопасная запись
            for (int i = 0; i < in.size; i++) {
                fout << in.text[i];
            }
            fout.close();
        }
    }

    void WriteError(OUT out, Error::ERROR error) {
        *out.stream << "--- Ошибки --- " << endl;
        *out.stream << "Ошибка " << error.id << ":" << error.message << endl;

        if (error.inext.line != -1)
        {
            *out.stream << "Строка " << error.inext.line << " позиция: " << error.inext.col << endl << endl;
        }
    }

    void Close(OUT out) {
        if (out.stream != nullptr) {
            out.stream->close();
            delete out.stream;
        }
    }
}