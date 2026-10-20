#pragma once

#include <string>
#include <vector>

#include "../PVR-2025/IT.h"   // или "IT.h", если настроишь include directories

namespace StandartLib
{
    // ---- Signature info (for semantic checks) ----
    struct Signature
    {
        IT::IDDATATYPE returnType;
        std::vector<IT::IDDATATYPE> params;
    };

    // true for: strcmp / strlen
    bool IsStdFunction(const std::string& name);

    // -1 if not std function
    int ParamCount(const std::string& name);

    // nullptr if not std function
    const Signature* GetSignature(const std::string& name);

    // Register std functions in identifier table as IT::F:
    //  strlen(string) -> short
    //  strcmp(string,string) -> short
    void Install(IT::IdTable& idt);

    // ---- JS mapping ----
    // Returns JS expression for a call of a standard function.
    // Examples:
    //  EmitJsCall("strlen", {"s"}) -> "(s).length"
    //  EmitJsCall("strcmp", {"a","b"}) -> "(a).localeCompare(b)"
    std::string EmitJsCall(const std::string& name,
        const std::vector<std::string>& args);
}
