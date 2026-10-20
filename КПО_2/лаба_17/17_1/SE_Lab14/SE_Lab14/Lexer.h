#pragma once
#include "In.h"
#include "Log.h"
#include "LT.h"
#include "IT.h"
#include "FST.h"

#define LEX_INTEGER "t"
#define LEX_STRING "t"
#define LEX_FUNCTION "f"
#define LEX_DECLARE "d"
#define LEX_RETURN "r"
#define LEX_PRINT "p"
#define LEX_MAIN "m"
#define LEX_SEMICOLON ";"
#define LEX_COMMA ","
#define LEX_LBRACE "{"
#define LEX_RBRACE "}"
#define LEX_LPAREN "("
#define LEX_RPAREN ")"
#define LEX_PLUS "v"
#define LEX_MINUS "v"
#define LEX_MULT "v"
#define LEX_DIV "v"
#define LEX_ASSIGN "v"
#define LEX_ID "i"
#define LEX_LITERAL "l"

namespace Lexer {
    struct LEX {
        LT::LexTable lextable;
        IT::IdTable idtable;
    };

    LEX LexAnalysis(In::IN in, Log::LOG log);
    void PrintLexTable(LEX lex, Log::LOG log);
    void PrintIdTable(LEX lex, Log::LOG log);
}