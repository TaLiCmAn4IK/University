#include <iostream>
#include "stdafx.h"
#include "Error.h"

namespace Error {

    ERROR errors[ERROR_MAX_ENTRY] =
    {
        ERROR_ENTRY(0,   "Semantic error: Invalid error code"),
        ERROR_ENTRY(1,   "System failure"),
        ERROR_ENTRY_NODEF(2), ERROR_ENTRY_NODEF(3), ERROR_ENTRY_NODEF(4), ERROR_ENTRY_NODEF(5),
        ERROR_ENTRY_NODEF(6), ERROR_ENTRY_NODEF(7), ERROR_ENTRY_NODEF(8), ERROR_ENTRY_NODEF(9),

        ERROR_ENTRY_NODEF10(10), ERROR_ENTRY_NODEF10(20), ERROR_ENTRY_NODEF10(30),
        ERROR_ENTRY_NODEF10(40), ERROR_ENTRY_NODEF10(50), ERROR_ENTRY_NODEF10(60),
        ERROR_ENTRY_NODEF10(70), ERROR_ENTRY_NODEF10(80), ERROR_ENTRY_NODEF10(90),

        ERROR_ENTRY(100, "Semantic error: The -in parameter must be specified"),
        ERROR_ENTRY_NODEF(101), ERROR_ENTRY_NODEF(102), ERROR_ENTRY_NODEF(103),
        ERROR_ENTRY(104, "Lexical error: Input parameter length exceeded"),
        ERROR_ENTRY_NODEF(105), ERROR_ENTRY_NODEF(106), ERROR_ENTRY_NODEF(107),
        ERROR_ENTRY_NODEF(108), ERROR_ENTRY_NODEF(109),

        ERROR_ENTRY(110, "System error: Failed to open source code input file (-in)"),
        ERROR_ENTRY(111, "Lexical error: Invalid character in source code (-in)"),
        ERROR_ENTRY(112, "System error: Failed to create log file (-log)"),
        ERROR_ENTRY_NODEF(113), ERROR_ENTRY_NODEF(114), ERROR_ENTRY_NODEF(115),
        ERROR_ENTRY_NODEF(116), ERROR_ENTRY_NODEF(117), ERROR_ENTRY_NODEF(118), ERROR_ENTRY_NODEF(119),

        ERROR_ENTRY_NODEF10(120), ERROR_ENTRY_NODEF10(130), ERROR_ENTRY_NODEF10(140),
        ERROR_ENTRY_NODEF10(150), ERROR_ENTRY_NODEF10(160), ERROR_ENTRY_NODEF10(170),
        ERROR_ENTRY_NODEF10(180), ERROR_ENTRY_NODEF10(190),

        ERROR_ENTRY(200, "Lexical error: Invalid character in source code (-in)"),
        ERROR_ENTRY(201, "System error: Lexeme table size exceeded"),
        ERROR_ENTRY(202, "System error: Lexeme table overflow"),
        ERROR_ENTRY(203, "System error: Identifier table size exceeded"),
        ERROR_ENTRY(204, "System error: Identifier table overflow"),
        ERROR_ENTRY(205, "Lexical error: Unknown character sequence"),
        ERROR_ENTRY(206, "System error: Out of bounds access in lexeme or identifier table"),
        ERROR_ENTRY_NODEF(207),
        ERROR_ENTRY_NODEF(208), ERROR_ENTRY_NODEF(209),

        ERROR_ENTRY_NODEF10(210), ERROR_ENTRY_NODEF10(220), ERROR_ENTRY_NODEF10(230),
        ERROR_ENTRY_NODEF10(240), ERROR_ENTRY_NODEF10(250), ERROR_ENTRY_NODEF10(260),
        ERROR_ENTRY_NODEF10(270), ERROR_ENTRY_NODEF10(280), ERROR_ENTRY_NODEF10(290),

        ERROR_ENTRY(300, "Syntax error: Unterminated string literal"),
        ERROR_ENTRY(301, "Syntax error: More than one entry point in main"),
        ERROR_ENTRY(302, "Syntax error: Missing entry point main"),
        ERROR_ENTRY(303, "Lexical error: String literal size exceeded"),
        ERROR_ENTRY_NODEF(304),
        ERROR_ENTRY(305, "Syntax error: Undeclared identifier"),
        ERROR_ENTRY_NODEF(306),
        ERROR_ENTRY(307, "Semantic error: Attempt to redefine an existing function"),
        ERROR_ENTRY_NODEF(308),
        ERROR_ENTRY(309, "Semantic error: Function argument type mismatch"),
        ERROR_ENTRY_NODEF(310),
        ERROR_ENTRY_NODEF(311),
        ERROR_ENTRY(312, "Semantic error: Data type mismatch"),
        ERROR_ENTRY(313, "Syntax error: Mismatched opening and closing parentheses in expression"),
        ERROR_ENTRY(314, "Semantic error: Function returns an invalid data type"),
        ERROR_ENTRY_NODEF(315),
        ERROR_ENTRY(316, "Syntax error: Variable redeclaration"),
        ERROR_ENTRY(317, "Syntax error: Invalid operator usage in expression"),
        ERROR_ENTRY_NODEF(318),
        ERROR_ENTRY_NODEF(319),
        ERROR_ENTRY_NODEF(320),
        ERROR_ENTRY_NODEF(321),
        ERROR_ENTRY_NODEF(322), ERROR_ENTRY_NODEF(323), ERROR_ENTRY_NODEF(324),
        ERROR_ENTRY_NODEF(325), ERROR_ENTRY_NODEF(326), ERROR_ENTRY_NODEF(327),
        ERROR_ENTRY_NODEF(328), ERROR_ENTRY_NODEF(329),

        ERROR_ENTRY_NODEF10(330), ERROR_ENTRY_NODEF10(340), ERROR_ENTRY_NODEF10(350),
        ERROR_ENTRY_NODEF10(360), ERROR_ENTRY_NODEF10(370), ERROR_ENTRY_NODEF10(380),
        ERROR_ENTRY_NODEF10(390),

        ERROR_ENTRY_NODEF100(400),

        ERROR_ENTRY_NODEF10(500), ERROR_ENTRY_NODEF10(510), ERROR_ENTRY_NODEF10(520),
        ERROR_ENTRY_NODEF10(530), ERROR_ENTRY_NODEF10(540), ERROR_ENTRY_NODEF10(550),
        ERROR_ENTRY_NODEF10(560), ERROR_ENTRY_NODEF10(570), ERROR_ENTRY_NODEF10(580),
        ERROR_ENTRY_NODEF10(590),

        ERROR_ENTRY(600, "Semantic error: Invalid program structure"),
        ERROR_ENTRY(601, "Missing parameters"),
        ERROR_ENTRY(602, "Invalid function parameters"),
        ERROR_ENTRY(603, "Missing function body"),
        ERROR_ENTRY(604, "Invalid expression"),
        ERROR_ENTRY(605, "Lexical error: Invalid statement"),
        ERROR_ENTRY(606, "Invalid function body construct"),
        ERROR_ENTRY(607, "Error in loop body parameters"),
        ERROR_ENTRY(608, "Error in function call"),
        ERROR_ENTRY(609, "Syntax error: Error in arithmetic expression"),
        ERROR_ENTRY(610, "Error in argument list during function call"),
        ERROR_ENTRY(611, "Error in arithmetic expression"),
        ERROR_ENTRY(612, "Invalid construct in loop body"),

        ERROR_ENTRY(700, "Semantic error: switch is allowed only on type short"),
        ERROR_ENTRY(701, "Semantic error: case outside of switch"),
        ERROR_ENTRY(702, "Semantic error: case type does not match switch expression type"),
        ERROR_ENTRY(703, "Semantic error: duplicate case value"),
        ERROR_ENTRY(704, "Semantic error: case must be followed by a literal"),
        ERROR_ENTRY(705, "Semantic error: default outside of switch"),

        ERROR_ENTRY(720, "Semantic error: invalid number of arguments in function call"),
        ERROR_ENTRY(721, "Semantic error: missing/corrupted Pn token (parameter count) after function call"),
        ERROR_ENTRY(722, "Semantic error: attempt to call an identifier that is not a function"),
        ERROR_ENTRY(723, "Semantic error: not enough arguments in RPN for function call"),
        ERROR_ENTRY(724, "Semantic error: invalid idxTI (identifier table reference)"),
        ERROR_ENTRY(725, "Semantic error: strlen expects 1 argument of type string"),
        ERROR_ENTRY(726, "Semantic error: strcmp expects 2 arguments of type string"),
        ERROR_ENTRY(727, "Semantic error: invalid operands for comparison operation"),
        ERROR_ENTRY(728, "Semantic error: invalid operands for arithmetic operation"),
        ERROR_ENTRY(729, "Semantic error: arithmetic operations are allowed only for type short"),

        ERROR_ENTRY_NODEF100(800),
        ERROR_ENTRY_NODEF100(900)
    };

    ERROR geterror(int id) {
        if (id > 0 && id < ERROR_MAX_ENTRY)
            return errors[id];
        return errors[0];
    }

    ERROR geterrorin(int id, int line, int col)
    {
        std::cout << "\n[ERROR_DEBUG] id=" << id
            << " line=" << line
            << " col=" << col << std::endl;

        if (id > 0 && id < ERROR_MAX_ENTRY)
        {
            errors[id].inext.line = static_cast<short>(line);
            errors[id].inext.col = static_cast<short>(col);
            return errors[id];
        }
        return errors[0];
    }

}
