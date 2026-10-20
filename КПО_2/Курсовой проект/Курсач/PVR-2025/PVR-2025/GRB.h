#pragma once
#include "stdafx.h"
#include "LT.h"
#include <initializer_list>
#include <utility>

#define GRB_ERROR_SERIES 600

#define NS(n)    GRB::Rule::Chain::N(n)
#define TS(n)    GRB::Rule::Chain::T(n)
#define ISNS(n)  GRB::Rule::Chain::isN(n)

#define EMPTY_CHAIN 0
typedef short GRBALPHABET;

// ===== Nonterminals (ALL UNIQUE) =====
#define STARTING_SYMBOL   'S'
#define BODY              'B'
#define STMT_LIST         'Q'
#define DECL_STMT         'V'
#define DECL_TAIL         'J'
#define INIT_TAIL         'I'
#define END_STMT          'X'
#define ASSIGN_STMT       'A'
#define OUT_STMT          'O'
#define BREAK_STMT        'R'
#define SHORT_DECL        'U'
#define STR_DECL          'q'
#define SWITCH_STMT       'W'
#define SWITCH_HEAD       '1'
#define SWITCH_MID        '2'
#define SWITCH_TAIL       '3'
#define CASE_LIST         'C'
#define CASE_HEAD         '4'
#define CASE_BLOCK        'G'
#define DEFAULT_CASE      'D'
#define DEFAULT_HEAD      '5'
#define ID_OR_LITERAL     'N'
#define EXPR              'E'
#define TERM              'T'
#define LOGICAL_EXPR      'L'
#define FUNCTION_CALL     'K'
#define CALL_PARAMS       'F'
#define PARAM_TAIL        'M'
#define PAREN_NEST        'p'
#define PAREN_TAIL        'h'
#define ID_TAIL           'y'

// ===== Added nonterminals (needed by your current GRB.cpp) =====
#define ID_START          'Y'
#define CALL_PARAMS_OPT   'Z'
#define SWITCH_TAIL_STMT  'H'

namespace GRB
{
    struct Rule
    {
        GRBALPHABET nn;
        int         iderror;
        short       size;

        struct Chain
        {
            short        size;
            GRBALPHABET* nt;

            Chain() : size(0), nt(nullptr) {}
            Chain(std::initializer_list<GRBALPHABET> init);

            // Rule of five
            ~Chain();
            Chain(const Chain& other);
            Chain& operator=(const Chain& other);
            Chain(Chain&& other) noexcept;
            Chain& operator=(Chain&& other) noexcept;

            char* getCChain(char* b);

            static GRBALPHABET T(char t) { return GRBALPHABET(t); }
            static GRBALPHABET N(char n) { return -GRBALPHABET(n); }
            static bool isT(GRBALPHABET s) { return s > 0; }
            static bool isN(GRBALPHABET s) { return !isT(s); }
            static char alphabet_to_char(GRBALPHABET s) { return isT(s) ? char(s) : char(-s); }
        } *chains;

        Rule() : nn(0x00), iderror(0), size(0), chains(nullptr) {}
        Rule(GRBALPHABET pnn, int piderror, std::initializer_list<Chain> chainsInit);

        // rule of five for Rule (owns chains[])
        ~Rule();
        Rule(const Rule& other);
        Rule& operator=(const Rule& other);
        Rule(Rule&& other) noexcept;
        Rule& operator=(Rule&& other) noexcept;

        char* getCRule(char* b, short nchain);
        short getNextChain(GRBALPHABET t, Rule::Chain& pchain, short j);
    };

    struct Greibach
    {
        short       size;
        GRBALPHABET startN;
        GRBALPHABET stbottomT;
        Rule* rules;

        Greibach() : size(0), startN(0), stbottomT(0), rules(nullptr) {}
        Greibach(GRBALPHABET pstartN, GRBALPHABET pstbottom, std::initializer_list<Rule> rulesInit);

        // owns rules[]
        ~Greibach();
        Greibach(const Greibach& other);
        Greibach& operator=(const Greibach& other);
        Greibach(Greibach&& other) noexcept;
        Greibach& operator=(Greibach&& other) noexcept;

        short getRule(GRBALPHABET pnn, Rule& prule);
        Rule  getRule(short n);
    };

    Greibach getGreibach();
}
