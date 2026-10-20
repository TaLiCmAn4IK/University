#pragma once
#include "stdafx.h"

namespace FST {

    struct RELATION {
        char  symbol;
        short nnode;
        RELATION(char c = 0, short nn = 0) : symbol(c), nnode(nn) {}
    };

    struct NODE {
        short     n_relation;
        RELATION* relations;
        NODE() : n_relation(0), relations(nullptr) {}
        NODE(short n, RELATION rel, ...) {
            n_relation = n;
            relations = new RELATION[n];
            RELATION* p = &rel;
            for (short i = 0; i < n; ++i)
                relations[i] = p[i];
        }
    };

    struct FST {
        char* string;
        short position;
        short nstates;
        NODE* nodes;
        short* rstates;
        FST(const char* s, short ns, NODE n, ...) {
            string = (char*)s;
            position = -1;
            nstates = ns;
            nodes = new NODE[ns];
            NODE* p = &n;
            for (short i = 0; i < ns; ++i)
                nodes[i] = p[i];
            rstates = new short[ns];
        }
    };

    bool execute(FST fst);

    /* --------- ÊËÞ×ÅÂÛÅ ÑËÎÂÀ --------- */

/* --------- ÊËÞ×ÅÂÛÅ ÑËÎÂÀ --------- */

// let
    static FST FST_LET(
        "Let", 4,
        NODE(1, RELATION('L', 1)),
        NODE(1, RELATION('e', 2)),
        NODE(1, RELATION('t', 3)),
        NODE()
    );

    // short
    static FST FST_SHORT(
        "short", 6,
        NODE(1, RELATION('s', 1)),
        NODE(1, RELATION('h', 2)),
        NODE(1, RELATION('o', 3)),
        NODE(1, RELATION('r', 4)),
        NODE(1, RELATION('t', 5)),
        NODE()
    );

    // string
    static FST FST_STRING(
        "string", 7,
        NODE(1, RELATION('s', 1)),
        NODE(1, RELATION('t', 2)),
        NODE(1, RELATION('r', 3)),
        NODE(1, RELATION('i', 4)),
        NODE(1, RELATION('n', 5)),
        NODE(1, RELATION('g', 6)),
        NODE()
    );

    // function
    static FST FST_FUNCTION(
        "Func", 5,
        NODE(1, RELATION('F', 1)),
        NODE(1, RELATION('u', 2)),
        NODE(1, RELATION('n', 3)),
        NODE(1, RELATION('c', 4)),
        NODE()
    );

    // return
    static FST FST_RET(
        "Ret", 4,
        NODE(1, RELATION('R', 1)),
        NODE(1, RELATION('e', 2)),
        NODE(1, RELATION('t', 3)),
        NODE()
    );

    // main
    static FST FST_MAIN(
        "Main", 5,
        NODE(1, RELATION('M', 1)),
        NODE(1, RELATION('a', 2)),
        NODE(1, RELATION('i', 3)),
        NODE(1, RELATION('n', 4)),
        NODE()
    );

    // output
    static FST FST_OUTPUT(
        "output", 7,
        NODE(1, RELATION('o', 1)),
        NODE(1, RELATION('u', 2)),
        NODE(1, RELATION('t', 3)),
        NODE(1, RELATION('p', 4)),
        NODE(1, RELATION('u', 5)),
        NODE(1, RELATION('t', 6)),
        NODE()
    );

    // switch
    static FST FST_SWITCH(
        "switch", 7,
        NODE(1, RELATION('s', 1)),
        NODE(1, RELATION('w', 2)),
        NODE(1, RELATION('i', 3)),
        NODE(1, RELATION('t', 4)),
        NODE(1, RELATION('c', 5)),
        NODE(1, RELATION('h', 6)),
        NODE()
    );

    // case
    static FST FST_CASE(
        "case", 5,
        NODE(1, RELATION('c', 1)),
        NODE(1, RELATION('a', 2)),
        NODE(1, RELATION('s', 3)),
        NODE(1, RELATION('e', 4)),
        NODE()
    );

    // default
    static FST FST_DEFAULT(
        "default", 8,
        NODE(1, RELATION('d', 1)),
        NODE(1, RELATION('e', 2)),
        NODE(1, RELATION('f', 3)),
        NODE(1, RELATION('a', 4)),
        NODE(1, RELATION('u', 5)),
        NODE(1, RELATION('l', 6)),
        NODE(1, RELATION('t', 7)),
        NODE()
    );


    /* --------- ËÈÒÅÐÀËÛ --------- */

    // äåñÿòè÷íûé öåëûé: ïîñëåäîâàòåëüíîñòü öèôð
    static FST FST_DECLIT(
        "0", 1,
        NODE(10,
            RELATION('0', 1), RELATION('1', 1), RELATION('2', 1), RELATION('3', 1),
            RELATION('4', 1), RELATION('5', 1), RELATION('6', 1), RELATION('7', 1),
            RELATION('8', 1), RELATION('9', 1))
    );

    // hex: 0h...
    static FST FST_HEXLIT(
        "0h", 2,
        NODE(1, RELATION('0', 1)),
        NODE(1, RELATION('h', 2))
    );

    // bin: 0b...
    static FST FST_BINLIT(
        "0b", 2,
        NODE(1, RELATION('0', 1)),
        NODE(1, RELATION('b', 2))
    );

    // ñòðîêîâûé ëèòåðàë (íà÷èíàåòñÿ è çàêàí÷èâàåòñÿ ')
    static FST FST_STRLIT(
        "''", 2,
        NODE(1, RELATION('\'', 1)),
        NODE(1, RELATION('\'', 2))
    );

    /* --------- ÈÄÅÍÒÈÔÈÊÀÒÎÐ ---------- */

    static FST FST_ID(
        "a", 1,
        NODE(53,
            RELATION('_', 1),
            RELATION('A', 1), RELATION('B', 1), RELATION('C', 1), RELATION('D', 1),
            RELATION('E', 1), RELATION('F', 1), RELATION('G', 1), RELATION('H', 1),
            RELATION('I', 1), RELATION('J', 1), RELATION('K', 1), RELATION('L', 1),
            RELATION('M', 1), RELATION('N', 1), RELATION('O', 1), RELATION('P', 1),
            RELATION('Q', 1), RELATION('R', 1), RELATION('S', 1), RELATION('T', 1),
            RELATION('U', 1), RELATION('V', 1), RELATION('W', 1), RELATION('X', 1),
            RELATION('Y', 1), RELATION('Z', 1),
            RELATION('a', 1), RELATION('b', 1), RELATION('c', 1), RELATION('d', 1),
            RELATION('e', 1), RELATION('f', 1), RELATION('g', 1), RELATION('h', 1),
            RELATION('i', 1), RELATION('j', 1), RELATION('k', 1), RELATION('l', 1),
            RELATION('m', 1), RELATION('n', 1), RELATION('o', 1), RELATION('p', 1),
            RELATION('q', 1), RELATION('r', 1), RELATION('s', 1), RELATION('t', 1),
            RELATION('u', 1), RELATION('v', 1), RELATION('w', 1), RELATION('x', 1),
            RELATION('y', 1), RELATION('z', 1),
            RELATION('0', 1), RELATION('1', 1), RELATION('2', 1), RELATION('3', 1),
            RELATION('4', 1), RELATION('5', 1), RELATION('6', 1), RELATION('7', 1),
            RELATION('8', 1), RELATION('9', 1))
    );

    /* --------- ÎÏÅÐÀÒÎÐÛ È ÐÀÇÄÅËÈÒÅËÈ --------- */

    static FST FST_OPERATOR(
        "+", 1,
        NODE(9,
            RELATION('+', 1),
            RELATION('-', 1),
            RELATION('*', 1),
            RELATION('/', 1),
            RELATION('%', 1),
            RELATION('<', 1),
            RELATION('>', 1),
            RELATION('=', 1),
            RELATION('!', 1))
    );

    static FST FST_SEMICOLON(";", 1, NODE(1, RELATION(';', 1)));
    static FST FST_COMMA(",", 1, NODE(1, RELATION(',', 1)));
    static FST FST_LEFTBRACE("{", 1, NODE(1, RELATION('{', 1)));
    static FST FST_RIGHTBRACE("}", 1, NODE(1, RELATION('}', 1)));
    static FST FST_LEFTTHESIS("(", 1, NODE(1, RELATION('(', 1)));
    static FST FST_RIGHTTHESIS(")", 1, NODE(1, RELATION(')', 1)));
    static FST FST_ASSIGN("=", 1, NODE(1, RELATION('=', 1)));

} // namespace FST
