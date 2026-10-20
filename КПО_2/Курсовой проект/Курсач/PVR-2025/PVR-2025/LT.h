#pragma once
#include "stdafx.h"
#include "Error.h"
#include "Parm.h"

#define LEXEMA_FIXSIZE   1
#define LT_MAXSIZE       4096
#define LT_TI_NULLIDX    0xffffffff

#define LEX_SHORT        't'
#define LEX_STRING       'g'

#define LEX_ID           'i'
#define LEX_LITERAL      'l'

#define LEX_LET          'd'
#define LEX_FUNCTION     'f'
#define LEX_RET          'r'
#define LEX_MAIN         'm'

#define LEX_SWITCH       'w'
#define LEX_CASE         'c'
#define LEX_DEFAULT      'u'
#define LEX_OUTPUT       'o'
#define LEX_BREAK        'k'

#define LEX_COLON        ':'
#define LEX_SEMICOLON    ';'
#define LEX_COMMA        ','
#define LEX_LEFTBRACE    '{'
#define LEX_RIGHTBRACE   '}'
#define LEX_LEFTTHESIS   '('
#define LEX_RIGHTTHESIS  ')'
#define LEX_ASSIGN       '='

#define LEX_OPERATOR     'p'
#define LEX_CALL '@'

namespace LT {

    enum operations {
        ONOT = -1,
        OPLUS,
        OMINUS,
        OMUL,
        ODIV,
        OMOD,
        OMORE,
        OLESS,
        OEQU,
        ONEQU,
        OMORE_EQU,
        OLESS_EQU
    };

    struct Entry {
        char       lexema;
        int        line;
        int        idxTI;
        int        priority;
        operations op;
    };

    struct LexTable {
        int    maxsize;
        int    size;
        Entry* table;
    };

    LexTable Create(int size);
    void     Add(LexTable& lextable, Entry entry);
    Entry    GetEntry(LexTable& lextable, int n);
    void     Delete(LexTable& lextable);
    Entry    WriteEntry(Entry& entry, char lexema, int indx, int line);

    void     ShowLexTable(LexTable lextable, std::ostream& stream_out);
    void     ShowTable(LexTable lextable, std::ostream& stream_out);
}
