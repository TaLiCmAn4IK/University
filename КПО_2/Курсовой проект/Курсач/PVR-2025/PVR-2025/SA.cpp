#include "stdafx.h"
#include "SA.h"

#include <vector>
#include <string>
#include <algorithm>

namespace Semantic {

    static bool IsCompareOp(const LT::Entry& e)
    {
        return e.lexema == LEX_OPERATOR &&
            (e.op == LT::OEQU || e.op == LT::ONEQU ||
                e.op == LT::OLESS || e.op == LT::OMORE ||
                e.op == LT::OLESS_EQU || e.op == LT::OMORE_EQU);
    }

    // проверка, является ли лексема ключевым словом типа (short, string и т.п.)
    static bool IsTypeLex(const LT::Entry& e)
    {
        return e.lexema == LEX_SHORT;
        // если есть другие типы – добавить сюда
    }

    // вычисление результирующего типа выражения в ПОЛИЗ
    static bool EvalRpnType(
        const Lex::LEX& lex,
        int from, int to,
        IT::IDDATATYPE fallbackType,
        IT::IDDATATYPE& outType,
        bool& outHasCompare,
        Log::LOG log,
        int errLine,
        bool& ok)
    {
        outHasCompare = false;
        std::vector<IT::IDDATATYPE> st;
        st.reserve(32);

        auto pushTypeOfToken = [&](const LT::Entry& e) -> bool
            {
                if (e.idxTI == LT_TI_NULLIDX || e.idxTI < 0 || e.idxTI >= lex.idtable.size) {
                    Log::WriteError(log, Error::geterrorin(724, errLine, -1));
                    ok = false;
                    return false;
                }
                st.push_back(lex.idtable.table[e.idxTI].idDataType);
                return true;
            };

        for (int i = from; i < to && i < lex.lextable.size; ++i)
        {
            const LT::Entry& e = lex.lextable.table[i];

            if (e.lexema == LEX_ID || e.lexema == LEX_LITERAL)
            {
                if (!pushTypeOfToken(e)) return false;
                continue;
            }

            if (e.lexema == LEX_CALL)
            {
                if (i + 1 >= to) {
                    Log::WriteError(log, Error::geterrorin(721, errLine, -1));
                    ok = false;
                    return false;
                }

                const LT::Entry& pnTok = lex.lextable.table[i + 1];
                if (pnTok.lexema != LEX_LITERAL || pnTok.idxTI == LT_TI_NULLIDX ||
                    pnTok.idxTI < 0 || pnTok.idxTI >= lex.idtable.size)
                {
                    Log::WriteError(log, Error::geterrorin(721, errLine, -1));
                    ok = false;
                    return false;
                }

                int paramCount = lex.idtable.table[pnTok.idxTI].value.vint;

                if (e.idxTI == LT_TI_NULLIDX || e.idxTI < 0 || e.idxTI >= lex.idtable.size)
                {
                    Log::WriteError(log, Error::geterrorin(724, errLine, -1));
                    ok = false;
                    return false;
                }

                const IT::Entry& f = lex.idtable.table[e.idxTI];

                if (f.idtype != IT::F) {
                    Log::WriteError(log, Error::geterrorin(722, errLine, -1));
                    ok = false;
                    return false;
                }

                if (f.countOfPar != paramCount) {
                    Log::WriteError(log, Error::geterrorin(720, errLine, -1));
                    ok = false;
                    return false;
                }

                if ((int)st.size() < paramCount) {
                    Log::WriteError(log, Error::geterrorin(723, errLine, -1));
                    ok = false;
                    return false;
                }

                if (strcmp(f.id, "strlen") == 0)
                {
                    IT::IDDATATYPE a0 = st.back();
                    st.pop_back();
                    if (a0 != IT::STR) {
                        Log::WriteError(log, Error::geterrorin(725, errLine, -1));
                        ok = false;
                        return false;
                    }
                    st.push_back(IT::SHORT);
                }
                else if (strcmp(f.id, "strcmp") == 0)
                {
                    IT::IDDATATYPE a1 = st.back(); st.pop_back();
                    IT::IDDATATYPE a0 = st.back(); st.pop_back();
                    if (a0 != IT::STR || a1 != IT::STR) {
                        Log::WriteError(log, Error::geterrorin(726, errLine, -1));
                        ok = false;
                        return false;
                    }
                    st.push_back(IT::SHORT);
                }
                else
                {
                    for (int k = 0; k < paramCount; ++k) st.pop_back();
                    st.push_back(f.idDataType);
                }

                ++i;
                continue;
            }

            if (e.lexema == LEX_OPERATOR)
            {
                if (IsCompareOp(e))
                {
                    outHasCompare = true;
                    if (st.size() < 2) {
                        Log::WriteError(log, Error::geterrorin(727, errLine, -1));
                        ok = false;
                        return false;
                    }
                    st.pop_back();
                    st.pop_back();
                    st.push_back(IT::SHORT);
                    continue;
                }

                if (st.size() < 2) {
                    Log::WriteError(log, Error::geterrorin(728, errLine, -1));
                    ok = false;
                    return false;
                }

                IT::IDDATATYPE b = st.back(); st.pop_back();
                IT::IDDATATYPE a = st.back(); st.pop_back();

                if (a != IT::SHORT || b != IT::SHORT) {
                    Log::WriteError(log, Error::geterrorin(729, errLine, -1));
                    ok = false;
                    return false;
                }

                st.push_back(IT::SHORT);
                continue;
            }
        }

        if (st.empty()) {
            outType = fallbackType;
            return true;
        }

        outType = outHasCompare ? IT::SHORT : st.back();
        return true;
    }

    // получение результирующего типа выражения
    static IT::IDDATATYPE GetExprResultType(
        const Lex::LEX& lex,
        int from, int to,
        IT::IDDATATYPE fallbackType,
        bool& hasCompare,
        Log::LOG log,
        int errLine,
        bool& ok)
    {
        IT::IDDATATYPE result = fallbackType;
        bool hc = false;
        EvalRpnType(lex, from, to, fallbackType, result, hc, log, errLine, ok);
        hasCompare = hc;
        return result;
    }

    // основной проход семантического анализатора по лексической таблице
    bool Analyze(Lex::LEX lex, Log::LOG log)
    {
        bool ok = true;

        std::vector<std::string> functions_id;
        std::vector<std::string> variables_id;

        bool in_switch = false;
        IT::IDDATATYPE switch_expr_type = IT::SHORT;
        int switch_brace_level = 0;
        std::vector<long> case_values;
        bool has_default = false;

        for (int i = 0; i < lex.lextable.size; ++i)
        {
            LT::Entry& le = lex.lextable.table[i];

            // проверка объявления переменной через LEX_LET и запрет повторного имени
            if (le.lexema == LEX_LET)
            {
                if (i + 2 < lex.lextable.size && lex.lextable.table[i + 2].lexema == LEX_ID)
                {
                    int ti = lex.lextable.table[i + 2].idxTI;
                    if (ti >= 0 && ti < lex.idtable.size)
                    {
                        std::string var_name = lex.idtable.table[ti].id;

                        if (std::count(variables_id.begin(), variables_id.end(), var_name))
                        {
                            Log::WriteError(log, Error::geterrorin(316, le.line, -1));
                            ok = false;
                        }
                        else
                        {
                            variables_id.push_back(var_name);
                        }
                    }
                }
                continue;
            }

            // проверка объявления переменной вида "<тип> ID" и запрет повторного имени
            if (IsTypeLex(le))
            {
                if (i + 1 < lex.lextable.size && lex.lextable.table[i + 1].lexema == LEX_ID)
                {
                    int ti = lex.lextable.table[i + 1].idxTI;
                    if (ti >= 0 && ti < lex.idtable.size)
                    {
                        std::string var_name = lex.idtable.table[ti].id;

                        if (std::count(variables_id.begin(), variables_id.end(), var_name))
                        {
                            Log::WriteError(log, Error::geterrorin(316, le.line, -1));
                            ok = false;
                        }
                        else
                        {
                            variables_id.push_back(var_name);
                        }
                    }
                }
                continue;
            }

            switch (le.lexema)
            {
            case LEX_ASSIGN:                 // проверка совместимости типов в операторе присваивания
            {
                if (i == 0) break;

                int left_ti = lex.lextable.table[i - 1].idxTI;
                if (left_ti < 0 || left_ti >= lex.idtable.size) break;

                IT::IDDATATYPE left_type = lex.idtable.table[left_ti].idDataType;

                int j = i + 1;
                while (j < lex.lextable.size && lex.lextable.table[j].lexema != LEX_SEMICOLON)
                    ++j;

                bool hasCompare = false;
                IT::IDDATATYPE right_type = GetExprResultType(
                    lex, i + 1, j, left_type, hasCompare, log, le.line, ok);

                if (left_type != right_type)
                {
                    Log::WriteError(log, Error::geterrorin(312, le.line, -1));
                    ok = false;
                }
                break;
            }

            case LEX_FUNCTION:               // проверка повторного объявления функции и типа return
            {
                if (i + 1 >= lex.lextable.size) break;

                int ti = lex.lextable.table[i + 1].idxTI;
                if (ti < 0 || ti >= lex.idtable.size) break;

                std::string func_name = lex.idtable.table[ti].id;
                if (std::count(functions_id.begin(), functions_id.end(), func_name))
                {
                    Log::WriteError(log, Error::geterrorin(307, lex.lextable.table[i + 1].line, -1));
                    ok = false;
                }
                functions_id.push_back(func_name);

                IT::IDDATATYPE return_type = lex.idtable.table[ti].idDataType;

                int j = i + 1;
                while (j < lex.lextable.size && lex.lextable.table[j].lexema != LEX_RET)
                    ++j;

                if (j + 1 < lex.lextable.size)
                {
                    int rti = lex.lextable.table[j + 1].idxTI;
                    if (rti >= 0 && rti < lex.idtable.size)
                    {
                        if (lex.idtable.table[rti].idDataType != return_type)
                        {
                            Log::WriteError(log, Error::geterrorin(314, lex.lextable.table[j].line, -1));
                            ok = false;
                        }
                    }
                }
                break;
            }

            case LEX_SWITCH:                 // начало switch: проверка типа выражения и инициализация состояния
            {
                in_switch = true;
                has_default = false;
                case_values.clear();
                switch_expr_type = IT::SHORT;
                switch_brace_level = 0;

                int j = i + 1;
                while (j < lex.lextable.size && lex.lextable.table[j].lexema != LEX_LEFTBRACE)
                {
                    if (lex.lextable.table[j].lexema == LEX_ID || lex.lextable.table[j].lexema == LEX_LITERAL)
                    {
                        int ti = lex.lextable.table[j].idxTI;
                        if (ti >= 0 && ti < lex.idtable.size)
                        {
                            switch_expr_type = lex.idtable.table[ti].idDataType;
                            if (switch_expr_type != IT::SHORT)
                            {
                                Log::WriteError(log, Error::geterrorin(700, lex.lextable.table[j].line, -1));
                                ok = false;
                            }
                        }
                        break;
                    }
                    ++j;
                }
                break;
            }

            case LEX_CASE:                   // обработка метки case: тип, литерал и уникальность значения
            {
                if (!in_switch)
                {
                    Log::WriteError(log, Error::geterrorin(701, le.line, -1));
                    ok = false;
                    break;
                }

                if (i + 1 >= lex.lextable.size || lex.lextable.table[i + 1].lexema != LEX_LITERAL)
                {
                    int errLine = (i + 1 < lex.lextable.size) ? lex.lextable.table[i + 1].line : le.line;
                    Log::WriteError(log, Error::geterrorin(704, errLine, -1));
                    ok = false;
                    break;
                }

                int ti = lex.lextable.table[i + 1].idxTI;
                if (ti >= 0 && ti < lex.idtable.size)
                {
                    IT::IDDATATYPE case_type = lex.idtable.table[ti].idDataType;
                    if (case_type != switch_expr_type)
                    {
                        Log::WriteError(log, Error::geterrorin(702, lex.lextable.table[i + 1].line, -1));
                        ok = false;
                    }

                    long case_value = lex.idtable.table[ti].value.vint;
                    if (std::count(case_values.begin(), case_values.end(), case_value))
                    {
                        Log::WriteError(log, Error::geterrorin(703, lex.lextable.table[i + 1].line, -1));
                        ok = false;
                    }
                    else
                    {
                        case_values.push_back(case_value);
                    }
                }
                break;
            }

            case LEX_DEFAULT:                // обработка default: наличие внутри switch и запрет второго default
            {
                if (!in_switch)
                {
                    Log::WriteError(log, Error::geterrorin(705, le.line, -1));
                    ok = false;
                    break;
                }
                if (has_default)
                {
                    Log::WriteError(log, Error::geterrorin(706, le.line, -1));
                    ok = false;
                }
                has_default = true;
                break;
            }

            case LEX_LEFTBRACE:              // учёт вложенности фигурных скобок внутри switch
            {
                if (in_switch) ++switch_brace_level;
                break;
            }

            case LEX_RIGHTBRACE:             // выход из switch при закрытии соответствующей скобки
            {
                if (in_switch)
                {
                    --switch_brace_level;
                    if (switch_brace_level <= 0)
                    {
                        in_switch = false;
                        switch_expr_type = IT::SHORT;
                        switch_brace_level = 0;
                        case_values.clear();
                        has_default = false;
                    }
                }
                break;
            }

            default:                         // остальные лексемы семантический анализатор здесь не обрабатывает
                break;
            }
        }

        return ok;
    }
}
