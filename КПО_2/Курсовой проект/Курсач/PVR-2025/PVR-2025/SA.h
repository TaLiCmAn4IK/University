#pragma once
#include "stdafx.h"
#include "Log.h"
#include "LA.h"

namespace Semantic
{
    // Главная функция семантического анализатора
    bool Analyze(Lex::LEX lex, Log::LOG log);
}
