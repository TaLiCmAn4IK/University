#include "Generation.h"
#include <stack>
#include <string>
#include <vector>
#include <windows.h>

#include "../StandartLib/StandartLib.h"

using namespace std;

namespace Gen {

    // поиск парной правой круглой скобки
    int Generator::findRightPar(int leftParPos)
    {
        int depth = 0;
        for (int i = leftParPos; i < lexT.size; ++i) {
            if (lexT.table[i].lexema == LEX_LEFTTHESIS) depth++;
            else if (lexT.table[i].lexema == LEX_RIGHTTHESIS) {
                depth--;
                if (depth == 0) return i;
            }
        }
        return -1;
    }

    // преобразование одного токена в фрагмент JS?кода
    static string tokToJs(const LT::Entry& e, IT::IdTable& idT)
    {
        if (e.lexema == LEX_ID || e.lexema == LEX_LITERAL) {
            if (e.idxTI == LT_TI_NULLIDX) return "";
            return string(idT.table[e.idxTI].id);
        }

        if (e.lexema == LEX_OPERATOR) {
            switch (e.op) {
            case LT::OPLUS:      return "+";
            case LT::OMINUS:     return "-";
            case LT::OMUL:       return "*";
            case LT::ODIV:       return "/";
            case LT::OMOD:       return "%";
            case LT::OEQU:       return "==";
            case LT::ONEQU:      return "!=";
            case LT::OLESS:      return "<";
            case LT::OMORE:      return ">";
            case LT::OLESS_EQU:  return "<=";
            case LT::OMORE_EQU:  return ">=";
            default:             return "";
            }
        }

        return "";
    }

    static bool isOperandLex(const LT::Entry& e)
    {
        return e.lexema == LEX_ID || e.lexema == LEX_LITERAL;
    }

    static bool isBinaryOp(const LT::Entry& e)
    {
        if (e.lexema != LEX_OPERATOR) return false;
        switch (e.op) {
        case LT::OPLUS:
        case LT::OMINUS:
        case LT::OMUL:
        case LT::ODIV:
        case LT::OMOD:
        case LT::OEQU:
        case LT::ONEQU:
        case LT::OLESS:
        case LT::OMORE:
        case LT::OLESS_EQU:
        case LT::OMORE_EQU:
            return true;
        default:
            return false;
        }
    }

    // построение JS?выражения по ОПЗ?последовательности
    static std::string buildJsExprRpn(LT::LexTable& lt, IT::IdTable& idT, int from, int to)
    {
        while (to >= from && lt.table[to].lexema == '#') --to;

        std::stack<std::string> st;

        for (int i = from; i <= to && i < lt.size; ++i) {
            const LT::Entry& e = lt.table[i];

            if (e.lexema == LEX_CALL) {
                int cntPos = i + 1;
                if (cntPos > to || cntPos >= lt.size) { st.push("0"); continue; }

                const LT::Entry& cntTok = lt.table[cntPos];
                if (cntTok.lexema != LEX_LITERAL || cntTok.idxTI == LT_TI_NULLIDX) {
                    st.push("0");
                    i = cntPos;
                    continue;
                }

                int paramCount = idT.table[cntTok.idxTI].value.vint;

                if (e.idxTI == LT_TI_NULLIDX) { st.push("0"); i = cntPos; continue; }
                std::string fname = idT.table[e.idxTI].id;

                if ((int)st.size() < paramCount) { st.push("0"); i = cntPos; continue; }

                std::vector<std::string> args(paramCount);
                for (int k = paramCount - 1; k >= 0; --k) { args[k] = st.top(); st.pop(); }

                if (StandartLib::IsStdFunction(fname))
                {
                    std::string js = StandartLib::EmitJsCall(fname, args);
                    st.push(js.empty() ? "0" : js);
                }
                else
                {
                    std::string call = fname + "(";
                    for (int k = 0; k < paramCount; ++k) {
                        if (k) call += ", ";
                        call += args[k];
                    }
                    call += ")";
                    st.push(call);
                }

                i = cntPos;
                continue;
            }

            if (isOperandLex(e)) {
                std::string t = tokToJs(e, idT);
                if (!t.empty()) st.push(t);
                continue;
            }

            if (isBinaryOp(e)) {
                std::string op = tokToJs(e, idT);
                if (st.size() < 2 || op.empty()) { st.push("0"); continue; }
                std::string rhs = st.top(); st.pop();
                std::string lhs = st.top(); st.pop();
                st.push("(" + lhs + " " + op + " " + rhs + ")");
                continue;
            }
        }

        return st.empty() ? "0" : st.top();
    }

    // конструктор генератора: открытие файла и запуск этапов генерации
    Generator::Generator(LT::LexTable& plexT, IT::IdTable& pidT)
        : lexT(plexT), idT(pidT)
    {
        char exePath[MAX_PATH]{};
        GetModuleFileNameA(NULL, exePath, MAX_PATH);

        std::string exeDir(exePath);
        size_t lastSlash = exeDir.find_last_of("\\/");
        if (lastSlash != std::string::npos) exeDir = exeDir.substr(0, lastSlash + 1);
        else exeDir = ".\\";

        std::string exeName = (lastSlash != std::string::npos)
            ? std::string(exePath + lastSlash + 1)
            : std::string(exePath);

        size_t dotPos = exeName.find_last_of('.');
        if (dotPos != std::string::npos) exeName = exeName.substr(0, dotPos);

        std::string outPath = exeDir + exeName + ".js";

        out.open(outPath, ios::out | ios::trunc);
        if (!out.is_open())
            throw ERROR_THROW(112);

        Header();
        Vars();
        Code();

        out.close();
    }

    // генерация заголовка JS?файла (функция вывода)
    void Generator::Header()
    {
        out << "function output(x) { console.log(x); }\n\n";
    }

    // экранирование строковых литералов для JS
    static std::string escapeJsString(const char* s)
    {
        std::string r;
        for (const char* p = s; p && *p; ++p) {
            switch (*p) {
            case '\\': r += "\\\\"; break;
            case '\"': r += "\\\""; break;
            case '\n': r += "\\n"; break;
            case '\r': r += "\\r"; break;
            case '\t': r += "\\t"; break;
            default:   r += *p; break;
            }
        }
        return r;
    }

    static bool isLiteralToken(const LT::Entry& e) {
        return e.lexema == LEX_LITERAL && e.idxTI != LT_TI_NULLIDX;
    }

    // генерация объявлений констант?литералов и переменных
    void Generator::Vars()
    {
        std::vector<bool> usedLit(idT.size, false);

        for (int i = 0; i < lexT.size; ++i) {
            if (lexT.table[i].lexema == LEX_LITERAL && lexT.table[i].idxTI != LT_TI_NULLIDX) {
                int idx = lexT.table[i].idxTI;
                if (idx >= 0 && idx < idT.size && idT.table[idx].idtype == IT::L)
                    usedLit[idx] = true;
            }
        }

        for (int i = 0; i < idT.size; ++i) {
            auto& e = idT.table[i];
            if (e.idtype != IT::L) continue;
            if (!usedLit[i]) continue;

            out << "const " << e.id << " = ";
            if (e.idDataType == IT::SHORT) out << e.value.vint;
            else if (e.idDataType == IT::STR) out << "\"" << escapeJsString(e.value.vstr.str) << "\"";
            else out << "0";
            out << ";\n";
        }

        for (int i = 0; i < idT.size; ++i) {
            auto& e = idT.table[i];
            if (e.idtype != IT::V) continue;

            out << "let " << e.id << " = ";
            if (e.idDataType == IT::SHORT) out << "0";
            else if (e.idDataType == IT::STR) out << "\"\"";
            else out << "0";
            out << ";\n";
        }

        out << "\n";
    }

    // поиск следующей точки с запятой
    int Generator::findSemicolon(int from)
    {
        for (int i = from; i < lexT.size; ++i)
            if (lexT.table[i].lexema == LEX_SEMICOLON) return i;
        return -1;
    }

    // генерация оператора присваивания
    void Generator::GenerateAssignment(int posId, int eqPos)
    {
        int varIdx = lexT.table[posId].idxTI;
        if (varIdx == LT_TI_NULLIDX) return;

        string lhs = idT.table[varIdx].id;

        int semi = findSemicolon(eqPos);
        if (semi < 0) return;

        int from = eqPos + 1;
        while (from < semi && lexT.table[from].lexema == LEX_LEFTTHESIS) ++from;

        int to = semi - 1;
        while (to >= from && lexT.table[to].lexema == '#') --to;

        string rhs = buildJsExprRpn(lexT, idT, from, to);
        out << lhs << " = " << rhs << ";\n";
    }

    // генерация оператора вывода
    void Generator::GenerateOutput(int posOutput)
    {
        int semi = findSemicolon(posOutput);
        if (semi < 0) return;

        int j = posOutput + 1;
        while (j < semi &&
            (lexT.table[j].lexema == LEX_LEFTTHESIS ||
                lexT.table[j].lexema == LEX_RIGHTTHESIS ||
                lexT.table[j].lexema == '#'))
        {
            ++j;
        }

        if (j < semi && lexT.table[j].lexema == LEX_LITERAL &&
            lexT.table[j].idxTI != LT_TI_NULLIDX)
        {
            out << "output(" << idT.table[lexT.table[j].idxTI].id << ");\n";
            return;
        }

        if (j < semi && lexT.table[j].lexema == LEX_ID &&
            lexT.table[j].idxTI != LT_TI_NULLIDX)
        {
            out << "output(" << idT.table[lexT.table[j].idxTI].id << ");\n";
            return;
        }

        int from = posOutput + 1;
        if (from < semi && lexT.table[from].lexema == LEX_LEFTTHESIS) ++from;

        int to = semi - 1;
        if (to >= from && lexT.table[to].lexema == LEX_RIGHTTHESIS) --to;
        while (to >= from && lexT.table[to].lexema == '#') --to;

        std::string expr = buildJsExprRpn(lexT, idT, from, to);
        out << "output(" << expr << ");\n";
    }

    // генерация заголовка оператора switch
    void Generator::GenerateSwitch(int posSwitch)
    {
        int lpar = posSwitch + 1;
        while (lpar < lexT.size && lexT.table[lpar].lexema != LEX_LEFTTHESIS) ++lpar;

        int rpar = (lpar < lexT.size) ? findRightPar(lpar) : -1;
        if (lpar >= lexT.size || rpar < 0) {
            out << "switch (0) {\n";
            return;
        }

        string expr = buildJsExprRpn(lexT, idT, lpar + 1, rpar - 1);
        out << "switch (" << expr << ") {\n";
    }

    // генерация основного тела программы (функция main)
    void Generator::Code()
    {
        out << "function main() {\n";

        int  switchDepth = 0;
        bool seenSwitchLBrace = false;
        int  pendingSwitch = -1;

        for (int i = 0; i < lexT.size; ++i) {
            auto lx = lexT.table[i].lexema;

            switch (lx) {

            case LEX_ASSIGN:
                if (i > 0 && lexT.table[i - 1].lexema == LEX_ID) {
                    GenerateAssignment(i - 1, i);
                }
                break;

            case LEX_OUTPUT:
                GenerateOutput(i);
                break;

            case LEX_SWITCH:
                GenerateSwitch(i);
                pendingSwitch = i;
                switchDepth = 0;
                seenSwitchLBrace = false;
                break;

            case LEX_LEFTBRACE:
                if (!seenSwitchLBrace && pendingSwitch != -1) {
                    seenSwitchLBrace = true;
                    switchDepth = 1;
                    pendingSwitch = -1;
                }
                else if (seenSwitchLBrace && switchDepth > 0) {
                    ++switchDepth;
                }
                break;

            case LEX_RIGHTBRACE:
                if (switchDepth > 0) {
                    --switchDepth;
                    if (switchDepth == 0) {
                        out << "}\n";
                        seenSwitchLBrace = false;
                    }
                }
                break;

            case LEX_CASE:
                if (seenSwitchLBrace && i + 1 < lexT.size) {
                    int litIdx = lexT.table[i + 1].idxTI;
                    if (litIdx != LT_TI_NULLIDX) {
                        out << "  case " << idT.table[litIdx].value.vint << ":\n";
                    }
                }
                break;

            case LEX_DEFAULT:
                if (seenSwitchLBrace) out << "  default:\n";
                break;

            case LEX_BREAK:
                out << "    break;\n";
                break;

            default:
                break;
            }
        }

        out << "}\n\n";
        out << "main();\n";
    }

}
