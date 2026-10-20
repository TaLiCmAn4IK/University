#pragma once
#include "stdafx.h"
#include "LA.h"
#include "Error.h"

namespace Polish
{
    // Преобразование выражения в обратную польскую запись (ОПЗ)
    bool polishNotation(int i, Lex::LEX& lex);

    // Запуск преобразования всех выражений программы
    void startPolish(Lex::LEX& lex);
}
