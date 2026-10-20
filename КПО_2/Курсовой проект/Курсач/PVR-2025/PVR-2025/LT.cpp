#include "stdafx.h"
#include "LT.h"
#include "Error.h"
#include <iomanip>

namespace LT {

    LexTable Create(int size) {
        if (size > LT_MAXSIZE)
            throw ERROR_THROW(201);

        LexTable table{};
        table.maxsize = size;
        table.size = 0;
        table.table = new Entry[size];
        return table;
    }

    void Add(LexTable& lextable, Entry entry) {
        if (lextable.size + 1 > lextable.maxsize)
            throw ERROR_THROW(202);

        lextable.table[lextable.size++] = entry;
    }

    Entry GetEntry(LexTable& lextable, int n) {
        return lextable.table[n];
    }

    void Delete(LexTable& lextable) {
        delete[] lextable.table;
        lextable.table = nullptr;
        lextable.size = lextable.maxsize = 0;
    }

    Entry WriteEntry(Entry& entry, char lexema, int indx, int line) {
        entry.lexema = lexema;
        entry.idxTI = indx;
        entry.line = line;
        entry.op = LT::ONOT;
        entry.priority = 0;
        return entry;
    }

    static const char* LexName(char lx)
    {
        switch (lx)
        {
        case LEX_SHORT:       return "LEX_SHORT";
        case LEX_STRING:      return "LEX_STRING";
        case LEX_ID:          return "LEX_ID";
        case LEX_LITERAL:     return "LEX_LITERAL";
        case LEX_LET:         return "LEX_LET";
        case LEX_FUNCTION:    return "LEX_FUNCTION";
        case LEX_RET:         return "LEX_RET";
        case LEX_MAIN:        return "LEX_MAIN";
        case LEX_SWITCH:      return "LEX_SWITCH";
        case LEX_CASE:        return "LEX_CASE";
        case LEX_DEFAULT:     return "LEX_DEFAULT";
        case LEX_OUTPUT:      return "LEX_OUTPUT";
        case LEX_BREAK:       return "LEX_BREAK";
        case LEX_COLON:       return "LEX_COLON";
        case LEX_SEMICOLON:   return "LEX_SEMICOLON";
        case LEX_COMMA:       return "LEX_COMMA";
        case LEX_LEFTBRACE:   return "LEX_LEFTBRACE";
        case LEX_RIGHTBRACE:  return "LEX_RIGHTBRACE";
        case LEX_LEFTTHESIS:  return "LEX_LEFTTHESIS";
        case LEX_RIGHTTHESIS: return "LEX_RIGHTTHESIS";
        case LEX_ASSIGN:      return "LEX_ASSIGN";
        case LEX_OPERATOR:    return "LEX_OPERATOR";
        case LEX_CALL:        return "LEX_CALL";
        default:              return "LEX_UNKNOWN";
        }
    }

    static const char* OpName(operations op)
    {
        switch (op)
        {
        case OPLUS:      return "+";
        case OMINUS:     return "-";
        case OMUL:       return "*";
        case ODIV:       return "/";
        case OMOD:       return "%";
        case OMORE:      return ">";
        case OLESS:      return "<";
        case OEQU:       return "==";
        case ONEQU:      return "!=";
        case OMORE_EQU:  return ">=";
        case OLESS_EQU:  return "<=";
        default:         return "-";
        }
    }

    void ShowLexTable(LexTable lextable, std::ostream& out)
    {
        out << std::left
            << std::setw(6) << "#"
            << std::setw(6) << "line"
            << std::setw(6) << "lex"
            << std::setw(16) << "name"
            << std::setw(8) << "idxTI"
            << std::setw(6) << "prio"
            << "op\n";

        for (int i = 0; i < lextable.size; ++i)
        {
            const Entry& e = lextable.table[i];
            out << std::left
                << std::setw(6) << (i + 1)
                << std::setw(6) << e.line
                << std::setw(6) << e.lexema
                << std::setw(16) << LexName(e.lexema)
                << std::setw(8) << e.idxTI
                << std::setw(6) << e.priority
                << ((e.lexema == LEX_OPERATOR) ? OpName(e.op) : "-")
                << "\n";
        }
    }

    void ShowTable(LexTable lextable, std::ostream& stream_out) {
        int curLine = 2;

        stream_out << "\n\t\tReverse Polish Notation (RPN)\n";
        stream_out << "0001| ";

        for (int i = 0; i < lextable.size; ++i) {
            char lx = lextable.table[i].lexema;

            if (lx == LEX_SEMICOLON) {
                stream_out << lx << std::endl;

                if (i + 1 == lextable.size)
                    break;

                if (curLine > 999)
                    stream_out << curLine << "| ";
                else if (curLine > 99)
                    stream_out << "0" << curLine << "| ";
                else if (curLine > 9)
                    stream_out << "00" << curLine << "| ";
                else
                    stream_out << "000" << curLine << "| ";

                ++curLine;
            }
            else {
                if (lx == LEX_OPERATOR) {
                    switch (lextable.table[i].op) {
                    case OEQU:       stream_out << "=="; break;
                    case ONEQU:      stream_out << "!="; break;
                    case OLESS_EQU:  stream_out << "<="; break;
                    case OMORE_EQU:  stream_out << ">="; break;
                    case OLESS:      stream_out << "<";  break;
                    case OMORE:      stream_out << ">";  break;
                    case OPLUS:      stream_out << "+";  break;
                    case OMINUS:     stream_out << "-";  break;
                    case OMUL:       stream_out << "*";  break;
                    case ODIV:       stream_out << "/";  break;
                    case OMOD:       stream_out << "%";  break;
                    default:         stream_out << lx;   break;
                    }
                }
                else {
                    stream_out << lx;
                }
            }
        }

        stream_out << std::endl;
    }
}
