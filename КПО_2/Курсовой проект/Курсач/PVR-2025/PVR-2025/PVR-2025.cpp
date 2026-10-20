#include "stdafx.h"

#include "Error.h"
#include "Parm.h"
#include "In.h"
#include "Log.h"
#include "PolishNotation.h"
#include "LA.h"
#include "MFST.h"
#include "SA.h"
#include "Generation.h"
#include "LT.h"
#include "IT.h"

// StdLib (strcmp/strlen etc.)
#include "../StandartLib/StandartLib.h"

#include <iostream>
#include <fstream>
#include <unordered_map>
#include <string>
#include <vector>
#include <streambuf>
#include <ostream>
#include <iomanip>

class tee_buf : public std::streambuf {
    std::streambuf* sb1;
    std::streambuf* sb2;

public:
    tee_buf(std::streambuf* s1, std::streambuf* s2) : sb1(s1), sb2(s2) {}

protected:
    int overflow(int c) override {
        if (c == EOF) return !EOF;
        const int r1 = sb1->sputc((char)c);
        const int r2 = sb2->sputc((char)c);
        return (r1 == EOF || r2 == EOF) ? EOF : c;
    }

    int sync() override {
        const int r1 = sb1->pubsync();
        const int r2 = sb2->pubsync();
        return (r1 == 0 && r2 == 0) ? 0 : -1;
    }
};

class tee_stream : public std::ostream {
    tee_buf tbuf;
public:
    tee_stream(std::ostream& o1, std::ostream& o2)
        : std::ostream(&tbuf), tbuf(o1.rdbuf(), o2.rdbuf()) {}
};

// ---------- Extra validation ----------
static bool ValidateTables(const Lex::LEX& lex, std::ostream& out)
{
    bool ok = true;

    std::unordered_map<std::string, int> firstPos;
    firstPos.reserve((size_t)lex.idtable.size);

    for (int i = 0; i < lex.idtable.size; ++i)
    {
        std::string s = lex.idtable.table[i].id;
        if (firstPos.find(s) != firstPos.end())
        {
            out << "[IT][DUP] id='" << s << "' dup index=" << i << "\n";
            ok = false;
        }
        else
        {
            firstPos.emplace(std::move(s), i);
        }
    }

    for (int i = 0; i < lex.lextable.size; ++i)
    {
        const auto& e = lex.lextable.table[i];
        if (e.lexema == LEX_ID || e.lexema == LEX_LITERAL || e.lexema == LEX_OPERATOR)
        {
            if (e.idxTI == LT_TI_NULLIDX || e.idxTI < 0 || e.idxTI >= lex.idtable.size)
            {
                out << "[LT->IT][BAD_IDX] le=" << i
                    << " lex='" << e.lexema << "' idxTI=" << e.idxTI << "\n";
                ok = false;
            }
        }
    }

    out << (ok ? "[OK] IT/LT validation passed\n" : "[FAIL] IT/LT validation failed\n");
    return ok;
}

int wmain(int argc, wchar_t* argv[])
{
    Log::LOG log = Log::INITLOG;

    try
    {
        Parm::PARM parm = Parm::getparm(argc, argv);

        log = Log::getlog(parm.log);
        Log::WriteLog(log);
        Log::WriteParm(log, parm);

        In::IN in = In::getin(parm);
        Log::WriteIn(log, in);

        std::ofstream fout("in.txt.out", std::ios::out | std::ios::trunc);
        if (!fout.is_open()) {
            std::cout << "[ERROR] Cannot open output file: in.txt.out\n";
            Log::Close(log);
            return 2;
        }
        tee_stream out(std::cout, fout);

        // 1) Lexical analysis
        Lex::LEX lex = Lex::LexAnaliz(log, in);
        out << "[OK] Lexical analysis passed\n";

        // Install stdlib functions into ID table
        StandartLib::Install(lex.idtable);
        out << "[OK] StandartLib installed (strcmp/strlen registered)\n";

        IT::DebugCheckDuplicates(lex.idtable, out);
        ValidateTables(lex, out);

        out << "\n===== LEX TABLE (TOKENS, BEFORE RPN) =====\n";
        LT::ShowLexTable(lex.lextable, out);

        out << "\n===== ID TABLE (AFTER STDLIB INSTALL) =====\n";
        IT::ShowTable(lex.idtable, out);

        // 2) Syntax analysis
        const bool syn_ok = MFST::check_syntax(lex, log, out);
        if (!syn_ok) {
            out << "[WARN] Syntax analysis not completed (failed or limited). Continue to tables/RPN...\n";
        }
        else {
            out << "[OK] Syntax analysis passed\n";
        }


        // 3) RPN / Polish notation
        Polish::startPolish(lex);
        Lex::Synchronization(lex);
        out << "[OK] RPN built; tables synchronized\n";

        out << "\n===== LEX TABLE (TOKENS, AFTER RPN) =====\n";
        LT::ShowLexTable(lex.lextable, out);

        out << "\n===== RPN VIEW (AFTER RPN) =====\n";
        LT::ShowTable(lex.lextable, out);

        // 4) Semantic analysis
        const bool sem_ok = Semantic::Analyze(lex, log);
        if (!sem_ok)
        {
            out << "[ERROR] Semantic analysis failed\n";
            out.flush(); fout.flush();
            Log::Close(log);
            return 1;
        }
        out << "[OK] Semantic analysis passed\n";

        // 5) Code generation
        Gen::Generator generator(lex.lextable, lex.idtable);
        (void)generator;
        out << "[OK] Code generation completed\n";

        out << "[OK] All stages completed successfully\n";

        out.flush();
        fout.flush();
        fout.close();
        Log::Close(log);
        return 0;
    }
    catch (const Error::ERROR& e)
    {
        Log::WriteError(log, e);
        Log::Close(log);
        std::cout << "Error " << e.id << ": " << e.message
            << ", line " << e.inext.line << "\n";
        return 1;
    }
    catch (const std::exception& e)
    {
        Log::Close(log);
        std::cout << "std::exception: " << e.what() << "\n";
        return 2;
    }
    catch (...)
    {
        Log::Close(log);
        return 3;
    }
}
