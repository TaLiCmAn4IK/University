#include "stdafx.h"
#include "GRB.h"
#include <utility>

namespace GRB
{
    Greibach greibach(
        NS(STARTING_SYMBOL),
        TS('$'),
        {
            Rule(NS(STARTING_SYMBOL), GRB_ERROR_SERIES + 0, {
                Rule::Chain({ TS(LEX_SHORT), TS(LEX_MAIN), TS(LEX_LEFTTHESIS), TS(LEX_RIGHTTHESIS), NS(BODY) })
            }),

            Rule(NS(BODY), GRB_ERROR_SERIES + 1, {
                Rule::Chain({
                    TS(LEX_LEFTBRACE),
                    NS(STMT_LIST),
                    TS(LEX_RET), NS(ID_OR_LITERAL), TS(LEX_SEMICOLON),
                    TS(LEX_RIGHTBRACE)
                })
            }),

            Rule(NS(ID_OR_LITERAL), GRB_ERROR_SERIES + 2, {
                Rule::Chain({ TS(LEX_ID) }),
                Rule::Chain({ TS(LEX_LITERAL) })
            }),

            Rule(NS(END_STMT), GRB_ERROR_SERIES + 3, {
                Rule::Chain({ TS(LEX_SEMICOLON) })
            }),

                // -------------------- STATEMENTS --------------------
                Rule(NS(STMT_LIST), GRB_ERROR_SERIES + 10, {
                    Rule::Chain({ TS(LEX_SHORT),  TS(LEX_ID), NS(DECL_TAIL), NS(STMT_LIST) }),
                    Rule::Chain({ TS(LEX_STRING), TS(LEX_ID), NS(DECL_TAIL), NS(STMT_LIST) }),

                    Rule::Chain({ TS(LEX_ID), NS(ID_START), NS(STMT_LIST) }),

                    Rule::Chain({ TS(LEX_OUTPUT), NS(EXPR), NS(END_STMT), NS(STMT_LIST) }),
                    Rule::Chain({ TS(LEX_SWITCH), NS(SWITCH_TAIL_STMT), NS(STMT_LIST) }),
                    Rule::Chain({ TS(LEX_BREAK),  NS(END_STMT), NS(STMT_LIST) }),

                    // stop markers
                    Rule::Chain({ TS(LEX_RET),        TS(EMPTY_CHAIN) }),
                    Rule::Chain({ TS(LEX_RIGHTBRACE), TS(EMPTY_CHAIN) }),

                    // IMPORTANT for switch-case: stop stmt list before next case/default
                    Rule::Chain({ TS(LEX_CASE),    TS(EMPTY_CHAIN) }),
                    Rule::Chain({ TS(LEX_DEFAULT), TS(EMPTY_CHAIN) }),

                    Rule::Chain({ TS(EMPTY_CHAIN) })
                }),

                Rule(NS(ID_START), GRB_ERROR_SERIES + 11, {
                    Rule::Chain({ TS(LEX_ASSIGN), NS(EXPR), NS(END_STMT) }),
                    Rule::Chain({ TS(LEX_LEFTTHESIS), NS(CALL_PARAMS_OPT), TS(LEX_RIGHTTHESIS), NS(END_STMT) })
                }),

                Rule(NS(DECL_TAIL), GRB_ERROR_SERIES + 21, {
                    Rule::Chain({ TS(LEX_SEMICOLON) }),
                    Rule::Chain({ TS(LEX_ASSIGN), NS(INIT_TAIL) })
                }),

                Rule(NS(INIT_TAIL), GRB_ERROR_SERIES + 23, {
                    Rule::Chain({ TS(LEX_ID), NS(ID_TAIL), NS(LOGICAL_EXPR), NS(END_STMT) }),
                    Rule::Chain({ TS(LEX_LITERAL), NS(LOGICAL_EXPR), NS(END_STMT) }),
                    Rule::Chain({ TS(LEX_LEFTTHESIS), NS(EXPR), TS(LEX_RIGHTTHESIS), NS(LOGICAL_EXPR), NS(END_STMT) })
                }),

                // -------------------- EXPRESSIONS --------------------
                Rule(NS(EXPR), GRB_ERROR_SERIES + 80, {
                    Rule::Chain({ TS(LEX_ID),      NS(ID_TAIL), NS(LOGICAL_EXPR) }),
                    Rule::Chain({ TS(LEX_LITERAL), NS(LOGICAL_EXPR) }),
                    Rule::Chain({ TS(LEX_LEFTTHESIS), NS(EXPR), TS(LEX_RIGHTTHESIS), NS(LOGICAL_EXPR) })
                }),

                Rule(NS(TERM), GRB_ERROR_SERIES + 81, {
                    Rule::Chain({ TS(LEX_ID),      NS(ID_TAIL) }),
                    Rule::Chain({ TS(LEX_LITERAL) }),
                    Rule::Chain({ TS(LEX_LEFTTHESIS), NS(EXPR), TS(LEX_RIGHTTHESIS) })
                }),

                Rule(NS(LOGICAL_EXPR), GRB_ERROR_SERIES + 82, {
                    Rule::Chain({ TS(LEX_OPERATOR), NS(TERM), NS(LOGICAL_EXPR) }),
                    Rule::Chain({ TS(EMPTY_CHAIN) })
                }),

                Rule(NS(ID_TAIL), GRB_ERROR_SERIES + 84, {
                    Rule::Chain({ TS(LEX_LEFTTHESIS), NS(CALL_PARAMS_OPT), TS(LEX_RIGHTTHESIS) }),
                    Rule::Chain({ TS(EMPTY_CHAIN) })
                }),

                // Z: id M | literal M | e   (deterministic by first terminal)
                Rule(NS(CALL_PARAMS_OPT), GRB_ERROR_SERIES + 91, {
                    Rule::Chain({ TS(LEX_ID),      NS(PARAM_TAIL) }),
                    Rule::Chain({ TS(LEX_LITERAL), NS(PARAM_TAIL) }),
                    Rule::Chain({ TS(EMPTY_CHAIN) })
                }),

                Rule(NS(PARAM_TAIL), GRB_ERROR_SERIES + 92, {
                    Rule::Chain({ TS(LEX_COMMA), TS(LEX_ID),      NS(PARAM_TAIL) }),
                    Rule::Chain({ TS(LEX_COMMA), TS(LEX_LITERAL), NS(PARAM_TAIL) }),
                    Rule::Chain({ TS(EMPTY_CHAIN) })
                }),

                // -------------------- SWITCH --------------------
                Rule(NS(SWITCH_TAIL_STMT), GRB_ERROR_SERIES + 100, {
                    Rule::Chain({
                        TS(LEX_LEFTTHESIS), NS(EXPR), TS(LEX_RIGHTTHESIS),
                        TS(LEX_LEFTBRACE),  NS(CASE_LIST), NS(DEFAULT_CASE),
                        TS(LEX_RIGHTBRACE)
                    })
                }),

                Rule(NS(CASE_LIST), GRB_ERROR_SERIES + 110, {
                    Rule::Chain({ TS(LEX_CASE), TS(LEX_LITERAL), TS(LEX_COLON), NS(CASE_BLOCK), NS(CASE_LIST) }),
                    Rule::Chain({ TS(EMPTY_CHAIN) })
                }),

                Rule(NS(CASE_BLOCK), GRB_ERROR_SERIES + 112, {
                    Rule::Chain({ NS(STMT_LIST), TS(LEX_BREAK), NS(END_STMT) }),
                    Rule::Chain({ TS(LEX_BREAK), NS(END_STMT) }),
                    Rule::Chain({ NS(STMT_LIST) }),
                    Rule::Chain({ TS(EMPTY_CHAIN) })
                }),

                Rule(NS(DEFAULT_CASE), GRB_ERROR_SERIES + 120, {
                    Rule::Chain({ TS(LEX_DEFAULT), TS(LEX_COLON), NS(CASE_BLOCK) }),
                    Rule::Chain({ TS(EMPTY_CHAIN) })
                }),
        }
    );

    // ================== Chain ==================
    Rule::Chain::Chain(std::initializer_list<GRBALPHABET> init)
    {
        size = (short)init.size();
        nt = (size > 0) ? new GRBALPHABET[size] : nullptr;
        int i = 0;
        for (auto v : init) nt[i++] = v;
    }

    Rule::Chain::~Chain()
    {
        delete[] nt;
        nt = nullptr;
        size = 0;
    }

    Rule::Chain::Chain(const Chain& other)
    {
        size = other.size;
        nt = (size > 0) ? new GRBALPHABET[size] : nullptr;
        for (int i = 0; i < size; ++i) nt[i] = other.nt[i];
    }

    Rule::Chain& Rule::Chain::operator=(const Chain& other)
    {
        if (this == &other) return *this;
        delete[] nt;
        size = other.size;
        nt = (size > 0) ? new GRBALPHABET[size] : nullptr;
        for (int i = 0; i < size; ++i) nt[i] = other.nt[i];
        return *this;
    }

    Rule::Chain::Chain(Chain&& other) noexcept
        : size(other.size), nt(other.nt)
    {
        other.size = 0;
        other.nt = nullptr;
    }

    Rule::Chain& Rule::Chain::operator=(Chain&& other) noexcept
    {
        if (this == &other) return *this;
        delete[] nt;
        size = other.size;
        nt = other.nt;
        other.size = 0;
        other.nt = nullptr;
        return *this;
    }

    char* Rule::Chain::getCChain(char* b)
    {
        for (int i = 0; i < size; ++i)
            b[i] = (nt[i] == EMPTY_CHAIN) ? 'e' : Chain::alphabet_to_char(nt[i]);
        b[size] = 0x00;
        return b;
    }

    // ================== Rule ==================
    Rule::Rule(GRBALPHABET pnn, int piderror, std::initializer_list<Chain> chainsInit)
        : nn(pnn), iderror(piderror), size(0), chains(nullptr)
    {
        size = (short)chainsInit.size();
        chains = (size > 0) ? new Chain[size] : nullptr;

        int i = 0;
        for (auto ch : chainsInit) chains[i++] = ch;
    }

    Rule::~Rule()
    {
        delete[] chains;
        chains = nullptr;
        size = 0;
    }

    Rule::Rule(const Rule& other)
        : nn(other.nn), iderror(other.iderror), size(other.size), chains(nullptr)
    {
        chains = (size > 0) ? new Chain[size] : nullptr;
        for (int i = 0; i < size; ++i) chains[i] = other.chains[i];
    }

    Rule& Rule::operator=(const Rule& other)
    {
        if (this == &other) return *this;
        delete[] chains;

        nn = other.nn;
        iderror = other.iderror;
        size = other.size;

        chains = (size > 0) ? new Chain[size] : nullptr;
        for (int i = 0; i < size; ++i) chains[i] = other.chains[i];
        return *this;
    }

    Rule::Rule(Rule&& other) noexcept
        : nn(other.nn), iderror(other.iderror), size(other.size), chains(other.chains)
    {
        other.size = 0;
        other.chains = nullptr;
        other.nn = 0;
        other.iderror = 0;
    }

    Rule& Rule::operator=(Rule&& other) noexcept
    {
        if (this == &other) return *this;
        delete[] chains;

        nn = other.nn;
        iderror = other.iderror;
        size = other.size;
        chains = other.chains;

        other.size = 0;
        other.chains = nullptr;
        other.nn = 0;
        other.iderror = 0;
        return *this;
    }

    char* Rule::getCRule(char* b, short nchain)
    {
        char bchain[200];
        b[0] = Chain::alphabet_to_char(nn);
        b[1] = '-';
        b[2] = '>';
        b[3] = 0x00;
        chains[nchain].getCChain(bchain);
        strcat_s(b, 200, bchain);
        return b;
    }

    short Rule::getNextChain(GRBALPHABET t, Rule::Chain& pchain, short j)
    {
        auto firstTerminal = [](const Rule::Chain& ch)->GRBALPHABET {
            for (int i = 0; i < ch.size; ++i)
                if (Rule::Chain::isT(ch.nt[i])) return ch.nt[i];
            return TS(EMPTY_CHAIN);
            };

        short k = j;
        while (k < size && firstTerminal(chains[k]) != t) ++k;
        if (k < size) { pchain = chains[k]; return k; }

        k = j;
        while (k < size && !(chains[k].size == 1 && chains[k].nt[0] == TS(EMPTY_CHAIN))) ++k;
        if (k < size) { pchain = chains[k]; return k; }

        return -1;
    }

    // ================== Greibach ==================
    Greibach::Greibach(GRBALPHABET pstartN, GRBALPHABET pstbottom, std::initializer_list<Rule> rulesInit)
        : size(0), startN(pstartN), stbottomT(pstbottom), rules(nullptr)
    {
        size = (short)rulesInit.size();
        rules = (size > 0) ? new Rule[size] : nullptr;

        int i = 0;
        for (auto r : rulesInit) rules[i++] = r;
    }

    Greibach::~Greibach()
    {
        delete[] rules;
        rules = nullptr;
        size = 0;
    }

    Greibach::Greibach(const Greibach& other)
        : size(other.size), startN(other.startN), stbottomT(other.stbottomT), rules(nullptr)
    {
        rules = (size > 0) ? new Rule[size] : nullptr;
        for (int i = 0; i < size; ++i) rules[i] = other.rules[i];
    }

    Greibach& Greibach::operator=(const Greibach& other)
    {
        if (this == &other) return *this;
        delete[] rules;

        size = other.size;
        startN = other.startN;
        stbottomT = other.stbottomT;

        rules = (size > 0) ? new Rule[size] : nullptr;
        for (int i = 0; i < size; ++i) rules[i] = other.rules[i];
        return *this;
    }

    Greibach::Greibach(Greibach&& other) noexcept
        : size(other.size), startN(other.startN), stbottomT(other.stbottomT), rules(other.rules)
    {
        other.size = 0;
        other.startN = 0;
        other.stbottomT = 0;
        other.rules = nullptr;
    }

    Greibach& Greibach::operator=(Greibach&& other) noexcept
    {
        if (this == &other) return *this;
        delete[] rules;

        size = other.size;
        startN = other.startN;
        stbottomT = other.stbottomT;
        rules = other.rules;

        other.size = 0;
        other.startN = 0;
        other.stbottomT = 0;
        other.rules = nullptr;
        return *this;
    }

    short Greibach::getRule(GRBALPHABET pnn, Rule& prule)
    {
        for (short k = 0; k < size; ++k)
            if (rules[k].nn == pnn) { prule = rules[k]; return k; }
        return -1;
    }

    Rule Greibach::getRule(short n)
    {
        Rule rc;
        if (n < size) rc = rules[n];
        return rc;
    }

    Greibach getGreibach()
    {
        return greibach;
    }
}
