#include "stdafx.h"
#include "Lexer.h"
#include "Error.h"
#include <cctype>
#include <iomanip>
#include <cstring>

namespace Lexer {

    // Конечные автоматы для ключевых слов (остаются без изменений)
    bool IsInteger(char* word) {
        FST::FST fst(
            word, 8,
            FST::NODE(1, FST::RELATION('i', 1)),
            FST::NODE(1, FST::RELATION('n', 2)),
            FST::NODE(1, FST::RELATION('t', 3)),
            FST::NODE(1, FST::RELATION('e', 4)),
            FST::NODE(1, FST::RELATION('g', 5)),
            FST::NODE(1, FST::RELATION('e', 6)),
            FST::NODE(1, FST::RELATION('r', 7)),
            FST::NODE()
        );
        return FST::Execute(fst);
    }

    bool IsString(char* word) {
        FST::FST fst(
            word, 7,
            FST::NODE(1, FST::RELATION('s', 1)),
            FST::NODE(1, FST::RELATION('t', 2)),
            FST::NODE(1, FST::RELATION('r', 3)),
            FST::NODE(1, FST::RELATION('i', 4)),
            FST::NODE(1, FST::RELATION('n', 5)),
            FST::NODE(1, FST::RELATION('g', 6)),
            FST::NODE()
        );
        return FST::Execute(fst);
    }

    bool IsFunction(char* word) {
        FST::FST fst(
            word, 9,
            FST::NODE(1, FST::RELATION('f', 1)),
            FST::NODE(1, FST::RELATION('u', 2)),
            FST::NODE(1, FST::RELATION('n', 3)),
            FST::NODE(1, FST::RELATION('c', 4)),
            FST::NODE(1, FST::RELATION('t', 5)),
            FST::NODE(1, FST::RELATION('i', 6)),
            FST::NODE(1, FST::RELATION('o', 7)),
            FST::NODE(1, FST::RELATION('n', 8)),
            FST::NODE()
        );
        return FST::Execute(fst);
    }

    bool IsDeclare(char* word) {
        FST::FST fst(
            word, 8,
            FST::NODE(1, FST::RELATION('d', 1)),
            FST::NODE(1, FST::RELATION('e', 2)),
            FST::NODE(1, FST::RELATION('c', 3)),
            FST::NODE(1, FST::RELATION('l', 4)),
            FST::NODE(1, FST::RELATION('a', 5)),
            FST::NODE(1, FST::RELATION('r', 6)),
            FST::NODE(1, FST::RELATION('e', 7)),
            FST::NODE()
        );
        return FST::Execute(fst);
    }

    bool IsReturn(char* word) {
        FST::FST fst(
            word, 7,
            FST::NODE(1, FST::RELATION('r', 1)),
            FST::NODE(1, FST::RELATION('e', 2)),
            FST::NODE(1, FST::RELATION('t', 3)),
            FST::NODE(1, FST::RELATION('u', 4)),
            FST::NODE(1, FST::RELATION('r', 5)),
            FST::NODE(1, FST::RELATION('n', 6)),
            FST::NODE()
        );
        return FST::Execute(fst);
    }

    bool IsPrint(char* word) {
        FST::FST fst(
            word, 6,
            FST::NODE(1, FST::RELATION('p', 1)),
            FST::NODE(1, FST::RELATION('r', 2)),
            FST::NODE(1, FST::RELATION('i', 3)),
            FST::NODE(1, FST::RELATION('n', 4)),
            FST::NODE(1, FST::RELATION('t', 5)),
            FST::NODE()
        );
        return FST::Execute(fst);
    }

    bool IsMain(char* word) {
        FST::FST fst(
            word, 5,
            FST::NODE(1, FST::RELATION('m', 1)),
            FST::NODE(1, FST::RELATION('a', 2)),
            FST::NODE(1, FST::RELATION('i', 3)),
            FST::NODE(1, FST::RELATION('n', 4)),
            FST::NODE()
        );
        return FST::Execute(fst);
    }

    // Проверка валидности идентификатора
    bool IsValidIdentifier(char* word, int& errorCode) {
        int len = strlen(word);

        if (len == 0) {
            errorCode = 120;
            return false;
        }

        int checkLen = (len > 5) ? 5 : len;

        for (int i = 0; i < checkLen; i++) {
            char c = word[i];

            if (isdigit(c)) {
                errorCode = 121;
                return false;
            }

            if (isupper(c)) {
                errorCode = 122;
                return false;
            }

            if (!islower(c)) {
                errorCode = 120;
                return false;
            }
        }

        if (len > 5) {
            word[5] = '\0';
        }

        errorCode = 0;
        return true;
    }

    // Проверка числа
    bool IsNumber(char* word) {
        if (strlen(word) == 0) return false;

        int start = 0;
        if (word[0] == '-') start = 1;

        for (int i = start; i < strlen(word); i++) {
            if (!isdigit(word[i])) return false;
        }

        return true;
    }

    // Получение позиции в строке
    int GetColumnInLine(In::IN in, int position) {
        int col = 1;
        for (int i = position - 1; i >= 0; i--) {
            if (in.text[i] == '\n' || in.text[i] == '|') {
                break;
            }
            col++;
        }
        return col;
    }

    // Структура для хранения контекста анализа
    struct LexContext {
        In::IN in;
        LEX lex;
        int position;
        int line;
        char currentWord[256];
        int wordStartPos;
    };

    // Обработка идентификатора или ключевого слова
    void ProcessIdentifier(LexContext& ctx, char* word) {
        LT::Entry le{};
        le.sn = ctx.line;

        // ВАЖНО: Сохраняем само слово, а не код лексемы
        strncpy_s(le.lexema, LEXEMA_FIXSIZE, word, LEXEMA_FIXSIZE - 1);
        le.lexema[LEXEMA_FIXSIZE - 1] = '\0';

        le.idxTI = LT_TI_NULLIDX;

        IT::IDTYPE idtype = IT::V;

        // Проверка ключевых слов (только для определения типа, но лексема остается исходной)
        if (IsInteger(word)) {
            // Для ключевых слов тоже сохраняем исходное слово
            idtype = IT::V;
        }
        else if (IsString(word)) {
            idtype = IT::V;
        }
        else if (IsFunction(word)) {
            idtype = IT::F;
        }
        else if (IsDeclare(word)) {
            idtype = IT::V;
        }
        else if (IsReturn(word)) {
            idtype = IT::V;
        }
        else if (IsPrint(word)) {
            idtype = IT::V;
        }
        else if (IsMain(word)) {
            idtype = IT::F;
        }
        else {
            // Это пользовательский идентификатор
            int errorCode = 0;
            if (!IsValidIdentifier(word, errorCode)) {
                int col = GetColumnInLine(ctx.in, ctx.wordStartPos);
                throw ERROR_THROW_IN(errorCode, ctx.line, col);
            }

            idtype = IT::V;
        }

        // Добавляем в таблицу идентификаторов если это не ключевое слово
        if (!IsInteger(word) && !IsString(word) && !IsFunction(word) &&
            !IsDeclare(word) && !IsReturn(word) && !IsPrint(word) && !IsMain(word)) {

            int existingIdIndex = IT::IsId(ctx.lex.idtable, word);

            if (existingIdIndex != TI_NULLIDX) {
                le.idxTI = existingIdIndex;
            }
            else {
                IT::Entry ie{};
                strncpy_s(ie.id, word, ID_MAXSIZE - 1);
                ie.idxfirstLE = ctx.lex.lextable.size;
                ie.iddatatype = IT::INT;
                ie.idtype = idtype;
                IT::Add(ctx.lex.idtable, ie);
                le.idxTI = ctx.lex.idtable.size - 1;
            }
        }

        LT::Add(ctx.lex.lextable, le);
    }

    // Обработка числового литерала
    void ProcessNumber(LexContext& ctx, char* num) {
        LT::Entry le{};
        le.sn = ctx.line;

        // Сохраняем само число как лексему
        strncpy_s(le.lexema, LEXEMA_FIXSIZE, num, LEXEMA_FIXSIZE - 1);
        le.lexema[LEXEMA_FIXSIZE - 1] = '\0';

        le.idxTI = LT_TI_NULLIDX;

        // Для чисел тоже можно добавить в таблицу идентификаторов как литералы
        int value = atoi(num);
        bool found = false;

        for (int i = 0; i < ctx.lex.idtable.size; i++) {
            IT::Entry existing = IT::GetEntry(ctx.lex.idtable, i);
            if (existing.idtype == IT::L && existing.iddatatype == IT::INT) {
                if (existing.value.vint == value) {
                    le.idxTI = i;
                    found = true;
                    break;
                }
            }
        }

        if (!found) {
            IT::Entry ie{};
            char literalId[ID_MAXSIZE];
            snprintf(literalId, ID_MAXSIZE, "L%d", ctx.lex.idtable.size);
            strncpy_s(ie.id, literalId, ID_MAXSIZE - 1);
            ie.idxfirstLE = ctx.lex.lextable.size;
            ie.iddatatype = IT::INT;
            ie.idtype = IT::L;
            ie.value.vint = value;
            IT::Add(ctx.lex.idtable, ie);
            le.idxTI = ctx.lex.idtable.size - 1;
        }

        LT::Add(ctx.lex.lextable, le);
    }

    // Обработка строкового литерала
    void ProcessString(LexContext& ctx, char* str) {
        LT::Entry le{};
        le.sn = ctx.line;

        // Формируем строку с кавычками для отображения
        char stringWithQuotes[LEXEMA_FIXSIZE];
        snprintf(stringWithQuotes, LEXEMA_FIXSIZE, "'%s'", str);
        strcpy_s(le.lexema, stringWithQuotes);

        le.idxTI = LT_TI_NULLIDX;

        // Поиск существующего строкового литерала
        bool found = false;
        for (int i = 0; i < ctx.lex.idtable.size; i++) {
            IT::Entry existing = IT::GetEntry(ctx.lex.idtable, i);
            if (existing.idtype == IT::L && existing.iddatatype == IT::STR) {
                if (strcmp(existing.value.vstr.str, str) == 0) {
                    le.idxTI = i;
                    found = true;
                    break;
                }
            }
        }

        if (!found) {
            IT::Entry ie{};
            char literalId[ID_MAXSIZE];
            snprintf(literalId, ID_MAXSIZE, "L%d", ctx.lex.idtable.size);
            strncpy_s(ie.id, literalId, ID_MAXSIZE - 1);

            ie.idxfirstLE = ctx.lex.lextable.size;
            ie.iddatatype = IT::STR;
            ie.idtype = IT::L;

            memset(&ie.value, 0, sizeof(ie.value));
            ie.value.vstr.len = (unsigned char)strlen(str);
            strncpy_s(ie.value.vstr.str, TI_STR_MAXSIZE, str, TI_STR_MAXSIZE - 1);

            IT::Add(ctx.lex.idtable, ie);
            le.idxTI = ctx.lex.idtable.size - 1;
        }

        LT::Add(ctx.lex.lextable, le);
    }

    // Обработка разделителей
    void ProcessDelimiter(LexContext& ctx, char c) {
        LT::Entry le{};
        le.sn = ctx.line;

        // Сохраняем сам символ разделителя
        le.lexema[0] = c;
        le.lexema[1] = '\0';

        le.idxTI = LT_TI_NULLIDX;

        LT::Add(ctx.lex.lextable, le);
    }

    // Основная функция лексического анализа
    LEX LexAnalysis(In::IN in, Log::LOG log) {
        LEX lex;
        lex.lextable = LT::Create(1000);
        lex.idtable = IT::Create(100);

        LexContext ctx;
        ctx.in = in;
        ctx.lex = lex;
        ctx.position = 0;
        ctx.line = 1;
        ctx.currentWord[0] = '\0';
        ctx.wordStartPos = 0;

        char buffer[256] = "";
        int bufferIdx = 0;
        bool inWord = false;
        bool inNumber = false;
        bool inString = false;

        *log.stream << "Начало лексического анализа" << std::endl;

        while (ctx.position < in.size && in.text[ctx.position] != '\0') {
            unsigned char c = in.text[ctx.position];

            // Пробельные символы
            if (c == ' ' || c == '\t' || c == '\r') {
                if (inWord) {
                    buffer[bufferIdx] = '\0';
                    ctx.wordStartPos = ctx.position - bufferIdx;
                    ProcessIdentifier(ctx, buffer);
                    bufferIdx = 0;
                    inWord = false;
                }
                else if (inNumber) {
                    buffer[bufferIdx] = '\0';
                    ProcessNumber(ctx, buffer);
                    bufferIdx = 0;
                    inNumber = false;
                }
                ctx.position++;
                continue;
            }

            // Перевод строки
            if (c == '\n' || c == '|') {
                if (inWord) {
                    buffer[bufferIdx] = '\0';
                    ctx.wordStartPos = ctx.position - bufferIdx;
                    ProcessIdentifier(ctx, buffer);
                    bufferIdx = 0;
                    inWord = false;
                }
                else if (inNumber) {
                    buffer[bufferIdx] = '\0';
                    ProcessNumber(ctx, buffer);
                    bufferIdx = 0;
                    inNumber = false;
                }
                ctx.line++;
                ctx.position++;
                continue;
            }

            // Строковые литералы
            if (c == '\'') {
                if (!inString) {
                    inString = true;
                    bufferIdx = 0;
                    ctx.position++;
                }
                else {
                    buffer[bufferIdx] = '\0';
                    ProcessString(ctx, buffer);
                    bufferIdx = 0;
                    inString = false;
                    ctx.position++;
                }
                continue;
            }

            if (inString) {
                if (bufferIdx < TI_STR_MAXSIZE - 1) {
                    buffer[bufferIdx++] = c;
                }
                ctx.position++;
                continue;
            }

            // Буквы (идентификаторы)
            if (isalpha(c)) {
                if (inNumber) {
                    buffer[bufferIdx] = '\0';
                    ProcessNumber(ctx, buffer);
                    bufferIdx = 0;
                    inNumber = false;
                }
                if (!inWord) {
                    ctx.wordStartPos = ctx.position;
                }
                inWord = true;
                if (bufferIdx < 255) {
                    buffer[bufferIdx++] = c;
                }
                ctx.position++;
                continue;
            }

            // Цифры (числа)
            if (isdigit(c) || (bufferIdx == 0 && c == '-')) {
                if (inWord) {
                    if (bufferIdx < 255) {
                        buffer[bufferIdx++] = c;
                    }
                }
                else {
                    inNumber = true;
                    if (bufferIdx < 255) {
                        buffer[bufferIdx++] = c;
                    }
                }
                ctx.position++;
                continue;
            }

            // Разделители
            if (strchr("();{},+-*/=", c)) {
                if (inWord) {
                    buffer[bufferIdx] = '\0';
                    ctx.wordStartPos = ctx.position - bufferIdx;
                    ProcessIdentifier(ctx, buffer);
                    bufferIdx = 0;
                    inWord = false;
                }
                else if (inNumber) {
                    buffer[bufferIdx] = '\0';
                    ProcessNumber(ctx, buffer);
                    bufferIdx = 0;
                    inNumber = false;
                }
                ProcessDelimiter(ctx, c);
                ctx.position++;
                continue;
            }

            ctx.position++;
        }

        // Обработка оставшихся данных в буфере
        if (inWord) {
            buffer[bufferIdx] = '\0';
            ctx.wordStartPos = ctx.position - bufferIdx;
            ProcessIdentifier(ctx, buffer);
        }
        else if (inNumber) {
            buffer[bufferIdx] = '\0';
            ProcessNumber(ctx, buffer);
        }
        else if (inString) {
            int col = GetColumnInLine(ctx.in, ctx.position);
            throw ERROR_THROW_IN(125, ctx.line, col);
        }

        *log.stream << "Лексический анализ завершен. Найдено " << ctx.lex.lextable.size << " лексем." << std::endl;
        return ctx.lex;
    }

    // Вывод таблицы лексем - ИСПРАВЛЕННАЯ ВЕРСИЯ
    void PrintLexTable(LEX lex, Log::LOG log) {
        *log.stream << "---- Таблица лексем ----" << std::endl;
        if (lex.lextable.size == 0) {
            *log.stream << "Таблица лексем пуста." << std::endl;
            return;
        }

        int currentLine = lex.lextable.table[0].sn;
        *log.stream << std::setw(2) << std::setfill('0') << currentLine << " ";

        for (int i = 0; i < lex.lextable.size; i++) {
            LT::Entry le = LT::GetEntry(lex.lextable, i);

            // Если сменилась строка, переходим на новую строку
            if (le.sn != currentLine) {
                *log.stream << std::endl;
                currentLine = le.sn;
                *log.stream << std::setw(2) << std::setfill('0') << currentLine << " ";
            }

            // Выводим саму лексему, а не ее код
            *log.stream << le.lexema;
        }
        *log.stream << std::endl;
    }

    // Вывод таблицы идентификаторов (без изменений)
    void PrintIdTable(LEX lex, Log::LOG log) {
        *log.stream << "---- Таблица идентификаторов ----" << std::endl;
        *log.stream << "Всего идентификаторов: " << lex.idtable.size << std::endl;

        for (int i = 0; i < lex.idtable.size; i++) {
            IT::Entry ie = IT::GetEntry(lex.idtable, i);

            *log.stream << ie.idxfirstLE << ": " << ie.id
                << " type=" << (ie.iddatatype == IT::INT ? "INT" : "STR")
                << " idtype=";

            switch (ie.idtype) {
            case IT::V: *log.stream << "VAR"; break;
            case IT::F: *log.stream << "FUNC"; break;
            case IT::P: *log.stream << "PARAM"; break;
            case IT::L: *log.stream << "LIT"; break;
            default: *log.stream << "UNKNOWN"; break;
            }

            if (ie.idtype == IT::L) {
                *log.stream << " value=";
                if (ie.iddatatype == IT::INT) {
                    *log.stream << ie.value.vint;
                }
                else if (ie.iddatatype == IT::STR) {
                    *log.stream << "'" << ie.value.vstr.str << "'";
                }
            }

            *log.stream << std::endl;
        }
    }
}