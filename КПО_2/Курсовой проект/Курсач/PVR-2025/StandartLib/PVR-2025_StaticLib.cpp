#include "pch.h"
#include "StandartLib.h"

#include <cstring>   // strcpy_s

namespace StandartLib
{
    // фиксируем сигнатуры в одном месте
    static const Signature SIG_STRLEN{ IT::SHORT, { IT::STR } };
    static const Signature SIG_STRCMP{ IT::SHORT, { IT::STR, IT::STR } };

    bool IsStdFunction(const std::string& name)
    {
        return name == "strcmp" || name == "strlen";
    }

    int ParamCount(const std::string& name)
    {
        if (name == "strlen") return 1;
        if (name == "strcmp") return 2;
        return -1;
    }

    const Signature* GetSignature(const std::string& name)
    {
        if (name == "strlen") return &SIG_STRLEN;
        if (name == "strcmp") return &SIG_STRCMP;
        return nullptr;
    }

    void Install(IT::IdTable& idt)
    {
        auto upsertFunc = [&](const char* name, IT::IDDATATYPE ret, int parCount)
            {
                char nm[ID_MAXSIZE]{};
                strcpy_s(nm, name);

                int idx = IT::IsId(idt, nm);
                if (idx != TI_NULLIDX)
                {
                    // ќЅЌќ¬Ћя≈ћ существующую запись
                    idt.table[idx].idtype = IT::F;
                    idt.table[idx].idDataType = ret;
                    idt.table[idx].countOfPar = parCount;
                    return;
                }

                // иначе добавл€ем новую
                IT::Entry e{};
                e.idxFirstLE = TI_NULLIDX;
                strcpy_s(e.visibility, "");
                strcpy_s(e.id, name);
                e.idDataType = ret;
                e.idtype = IT::F;
                e.countOfPar = parCount;
                e.value.vint = 0;
                IT::Add(idt, e);
            };

        upsertFunc("strlen", IT::SHORT, 1);
        upsertFunc("strcmp", IT::SHORT, 2);
    }


    std::string EmitJsCall(const std::string& name,
        const std::vector<std::string>& args)
    {
        // тут лучше не "молчать", а подстраховатьс€
        const Signature* sig = GetSignature(name);
        if (!sig) return "";

        // ћожно дополнительно проверить args.size()
        if ((int)args.size() != (int)sig->params.size())
            return "";

        if (name == "strlen")
        {
            // JS: (s).length  (length Ч свойство строки)  [web:302]
            return "(" + args[0] + ").length";
        }

        if (name == "strcmp")
        {
            // JS: (a).localeCompare(b) => negative/0/positive number  [web:246]
            return "(" + args[0] + ").localeCompare(" + args[1] + ")";
        }

        return "";
    }
}
