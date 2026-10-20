#include "PolishNotation.h"
#include <stack>
#include <queue>
#include <vector>
#include <cstring>

#include "../StandartLib/StandartLib.h"

namespace Polish {

    // создаЄт/находит literal Pn (число параметров) в idtable
    static int ensureParamCountLiteral(int value, IT::IdTable& idtable)
    {
        for (int k = 0; k < idtable.size; ++k) {
            if (idtable.table[k].idtype == IT::L &&
                idtable.table[k].idDataType == IT::SHORT &&
                idtable.table[k].value.vint == value)
                return k;
        }

        IT::Entry e{};
        e.idtype = IT::L;
        e.idDataType = IT::SHORT;
        e.value.vint = value;
        e.idxFirstLE = TI_NULLIDX;

        char name[ID_MAXSIZE]{};
        strcpy_s(name, "P");
        char num[16]{};
        _itoa_s(value, num, 10);
        strcat_s(name, num);
        strcpy_s(e.id, name);

        // на вс€кий случай делаем им€ уникальным
        if (IT::IsId(idtable, e.id) != TI_NULLIDX) {
            strcat_s(e.id, "_");
        }

        IT::Add(idtable, e);
        return idtable.size - 1;
    }

    // найти конец выражени€ (точку с зап€той)
    static int findExprEnd(int from, Lex::LEX& lex)
    {
        for (int j = from; j < lex.lextable.size; ++j) {
            if (lex.lextable.table[j].lexema == LEX_SEMICOLON) return j;
        }
        return -1;
    }

    static void writeQueueToRange(int from, int toExclusive, Lex::LEX& lex, std::queue<LT::Entry>& q)
    {
        int pos = from;

        // 1) пишем ќѕ«
        while (pos < toExclusive && !q.empty()) {
            lex.lextable.table[pos++] = q.front();
            q.pop();
        }

        // 2) остаток диапазона добиваем '#', чтобы ни семантика, ни генератор не видели "хвост инфикса"
        while (pos < toExclusive) {
            LT::Entry pad{};
            pad.lexema = '#';                 // важное: именно '#'
            pad.idxTI = LT_TI_NULLIDX;
            pad.line = lex.lextable.table[from].line;
            pad.op = LT::ONOT;
            pad.priority = 0;
            lex.lextable.table[pos++] = pad;
        }
    }



    bool polishNotation(int from, Lex::LEX& lex)
    {
        // ќпредел€ем границы выражени€
        const int semi = findExprEnd(from, lex);
        if (semi < 0) return false;

        std::stack<LT::Entry> opStack;
        std::queue<LT::Entry> outQ;

        // режим обработки std-функции
        bool isStdFuncCall = false;
        int  idxFunc = LT_TI_NULLIDX;
        int  commaCount = 0;
        bool hadAnyArg = false;

        // по диапазону [from..semi-1]
        for (int i = from; i < semi; ++i)
        {
            LT::Entry cur = lex.lextable.table[i]; // cur - перем. где лежит копи€ одной записи из лекс. табл.

            switch (cur.lexema)
            {
            case LEX_ID:
            {
                // если это std-функци€ Ч включаем режим вызова и Ќ≈ кладЄм им€ как операнд
                if (cur.idxTI != LT_TI_NULLIDX) {
                    const char* nm = lex.idtable.table[cur.idxTI].id;
                    if (StandartLib::IsStdFunction(nm)) {
                        isStdFuncCall = true;
                        idxFunc = cur.idxTI;
                        commaCount = 0;
                        hadAnyArg = false;
                        break; 
                    }
                }

                // обычный идентификатор = операнд
                outQ.push(cur);
                if (isStdFuncCall) hadAnyArg = true;
                break;
            }

            case LEX_LITERAL:
                outQ.push(cur);
                if (isStdFuncCall) hadAnyArg = true;
                break;

            case LEX_COMMA:
                // разделитель аргументов функции:
                // выгружаем операторы до ближайшей '('
                if (isStdFuncCall) {
                    ++commaCount;
                    while (!opStack.empty() && opStack.top().lexema != LEX_LEFTTHESIS) {
                        outQ.push(opStack.top());
                        opStack.pop();
                    }
                }
                break;

            case LEX_LEFTTHESIS:
                opStack.push(cur);
                break;

            case LEX_RIGHTTHESIS:
                // выгружаем всЄ до '('
                while (!opStack.empty() && opStack.top().lexema != LEX_LEFTTHESIS) {
                    outQ.push(opStack.top());
                    opStack.pop();
                }
                if (opStack.empty()) return false; // нет '('
                opStack.pop(); // снимаем '('

                // если это было закрытие вызова std-функции Ч добавл€ем маркер вызова
                if (isStdFuncCall) {
                    LT::Entry call{};
                    call.lexema = LEX_CALL;
                    call.idxTI = idxFunc;
                    call.line = cur.line;
                    call.op = LT::ONOT;
                    call.priority = 0;
                    outQ.push(call);

                    const int realCount = hadAnyArg ? (commaCount + 1) : 0;
                    const int litIdx = ensureParamCountLiteral(realCount, lex.idtable);

                    LT::Entry cnt{};
                    cnt.lexema = LEX_LITERAL;
                    cnt.idxTI = litIdx;
                    cnt.line = cur.line;
                    cnt.op = LT::ONOT;
                    cnt.priority = 0;
                    outQ.push(cnt);

                    // сброс режима
                    isStdFuncCall = false;
                    idxFunc = LT_TI_NULLIDX;
                    commaCount = 0;
                    hadAnyArg = false;
                }
                break;

            case LEX_OPERATOR:
                // классический shunting-yard: пока на стеке оператор с >= приоритетом Ч выгружаем
                // priority = 0 , ()
                // priority = 1 , ,
                // priority = 2 , ==, !=, <, >, <=, >=
                // priority = 3 , +, -
                // priority = 4 , *, /, %
                // priority = 5 , []
                while (!opStack.empty()
                    && opStack.top().lexema == LEX_OPERATOR
                    && cur.priority <= opStack.top().priority)
                {
                    outQ.push(opStack.top());
                    opStack.pop();
                }
                opStack.push(cur);
                break;

            default:
                // всЄ прочее внутри выражени€ игнорируем
                break;
            }
        }

        // выгружаем остаток стека
        while (!opStack.empty()) {
            if (opStack.top().lexema == LEX_LEFTTHESIS ||
                opStack.top().lexema == LEX_RIGHTTHESIS)
                return false;
            outQ.push(opStack.top());
            opStack.pop();
        }

        // перезаписываем выражение в LT в том же диапазоне [from..semi-1]
        writeQueueToRange(from, semi, lex, outQ);

        return true;
    }

    void startPolish(Lex::LEX& lex)
    {
        for (int i = 0; i < lex.lextable.size; ++i) {
            // выражение после '='
            if (lex.lextable.table[i].lexema == LEX_ASSIGN) {
                
                polishNotation(i + 1, lex);
            } 
            // выражение после output
            else if (lex.lextable.table[i].lexema == LEX_OUTPUT) {
               
                polishNotation(i + 1, lex);
            }
            // дл€ будущих процедур
            else if (lex.lextable.table[i].lexema == LEX_ID &&
                lex.lextable.table[i].idxTI != LT_TI_NULLIDX &&
                lex.idtable.table[lex.lextable.table[i].idxTI].idDataType == IT::PROC)
            {
                polishNotation(i, lex);
            }
        }
        // синхронизацию Ч один раз после всех перестановок
        Lex::Synchronization(lex);
    }

}
