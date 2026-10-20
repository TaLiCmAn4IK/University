#include "stdafx.h"
#include "IT.h"
#include "Error.h"

#include <iomanip>
#include <cstring>
#include <cctype>
#include <iostream>

using namespace std;

namespace IT {

    // ===== DEBUG: поиск дублей и подозрительных символов в id =====
    void DebugCheckDuplicates(IdTable& idtable, std::ostream& out)
    {
        // 1) Явные дубли (строки равны strcmp)
        for (int i = 0; i < idtable.size; ++i) {
            for (int j = i + 1; j < idtable.size; ++j) {
                if (strcmp(idtable.table[i].id, idtable.table[j].id) == 0) {
                    out << "[DUP] i=" << i << " j=" << j
                        << " id='" << idtable.table[i].id << "'\n";
                }
            }
        }

        // 2) Подозрительные символы (в id должны быть только [A-Za-z0-9_]
        //    Если тут что-то печатается — значит в id попали ';' '\r' и т.п.
        for (int i = 0; i < idtable.size; ++i) {

            // операторы вида "==", "<", ">" специально содержат не-буквы
            if (idtable.table[i].idtype == IT::OP) continue;

            const char* s = idtable.table[i].id;
            size_t n = strlen(s);

            for (size_t k = 0; k < n; ++k) {
                unsigned char c = (unsigned char)s[k];
                if (!(std::isalnum(c) || c == '_')) {
                    out << "[SUSPECT] i=" << i << " id='" << s
                        << "' bad_char_code=" << (int)c << "\n";
                    break;
                }
            }
        }
    }

    // ===== Таблица идентификаторов =====
    IdTable Create(int size) {
        if (size > TI_MAXSIZE)
            throw ERROR_THROW(203);

        IdTable table{};
        table.maxsize = size;
        table.size = 0;
        table.table = new Entry[size];
        return table;
    }

    void Add(IdTable& idtable, Entry entry) {
        if (idtable.size + 1 > idtable.maxsize)
            throw ERROR_THROW(204);

        // ЖЁСТКАЯ защита от дублей (можно временно закомментировать, если мешает)
        // ВНИМАНИЕ: сейчас у тебя дубли реально есть, поэтому этот throw будет срабатывать.
        // Если хочешь просто "проверять", а не падать — закомментируй следующие 2 строки.
        if (IsId(idtable, entry.id) != TI_NULLIDX)
            return; // уже есть, просто не добавляем второй раз
        idtable.table[idtable.size++] = entry;

    }

    Entry GetEntry(IdTable& idtable, int n) {
        return idtable.table[n];
    }

    int IsId(IdTable& idtable, char id[ID_MAXSIZE]) {
        for (int i = 0; i < idtable.size; ++i) {
            if (strcmp(idtable.table[i].id, id) == 0)
                return i;
        }
        return TI_NULLIDX;
    }

    void Delete(IdTable& idtable) {
        delete[] idtable.table;
        idtable.table = nullptr;
        idtable.size = idtable.maxsize = 0;
    }

    void ShowTable(IdTable& idtable, std::ostream& out)
    {
        auto dtName = [](IDDATATYPE t) -> const char* {
            switch (t) {
            case SHORT: return "short";
            case STR:   return "string";
            case PROC:  return "proc";
            default:    return "unknown";
            }
            };

        auto idtName = [](IDTYPE t) -> const char* {
            switch (t) {
            case V:  return "var";
            case F:  return "func";
            case P:  return "param";
            case L:  return "lit";
            case OP: return "op";
            default: return "unknown";
            }
            };

        out << "\n===== ID TABLE =====\n";
        out << std::left
            << std::setw(6) << "#"
            << std::setw(16) << "id"
            << std::setw(10) << "data"
            << std::setw(8) << "kind"
            << std::setw(10) << "idxFirst"
            << "value\n";

        for (int i = 0; i < idtable.size; ++i)
        {
            const auto& e = idtable.table[i];

            out << std::left
                << std::setw(6) << i
                << std::setw(16) << e.id;

            if (e.idtype == IT::OP) out << std::setw(10) << "-";
            else                   out << std::setw(10) << dtName(e.idDataType);

            out << std::setw(8) << idtName(e.idtype);

            if (e.idxFirstLE == TI_NULLIDX) out << std::setw(10) << "-";
            else                           out << std::setw(10) << e.idxFirstLE;

            if (e.idDataType == IT::SHORT && (e.idtype == IT::V || e.idtype == IT::L))
            {
                out << e.value.vint;
            }
            else if (e.idDataType == IT::STR && (e.idtype == IT::V || e.idtype == IT::L))
            {
                out << "len=" << e.value.vstr.len << " \"" << e.value.vstr.str << "\"";
            }
            else
            {
                out << "-";
            }

            out << "\n";
        }

        out << "Count: " << idtable.size << "\n";
    }

    void PrintTable(IdTable& idtable) {
        ShowTable(idtable, std::cout);
    }

}
