#include "stdafx.h"
#include "Lexer.h"
#include "Error.h"
#include <cctype>
#include <iomanip>
#include <cstring>

namespace Lexer {


    bool IsInteger(char* word) {
        FST::FST fst(
            word, 8,  // word - проверяемая строка, 8 - количество состояний автомата
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

    // Идентификатор должен: быть не пустым, содержать только строчные буквы, не начинаться с цифры, быть не длиннее 5 символов
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

    // Число может быть отрицательным (начинаться с '-') и содержать только цифры
    bool IsNumber(char* word) {
        if (strlen(word) == 0) return false;

        int start = 0;
        if (word[0] == '-') start = 1;

        for (int i = start; i < strlen(word); i++) {
            if (!isdigit(word[i])) return false;  // Если нашли не-цифру, возвращаем false
        }

        return true;
    }

    int GetColumnInLine(In::IN in, int position) {
        int col = 1;
        // Идем назад от текущей позиции до начала строки или разделителя строк
        for (int i = position - 1; i >= 0; i--) {
            // Если нашли начало строки, прекращаем подсчет
            if (in.text[i] == '\n' || in.text[i] == '|') {
                break;
            }
            col++;
        }
        return col;
    }

    // определяет тип лексемы и добавляет ее в таблицы
    void ProcessIdentifier(LexContext& ctx, char* word, int wordStartPos) {
        LT::Entry le{};  // Создаем запись для таблицы лексем
        le.sn = ctx.line;  // Устанавливаем номер строки
        le.idxTI = LT_TI_NULLIDX;  // Изначально нет ссылки на таблицу идентификаторов

        IT::IDTYPE idtype = IT::V;  // Тип идентификатора по умолчанию - переменная
        bool isKeyword = false;

        // Проверка всех возможных ключевых слов
        if (IsInteger(word)) {
            strcpy_s(le.lexema, LEX_INTEGER);  // Устанавливаем лексему для integer
            ctx.currentDataType = IT::INT;  // Устанавливаем текущий тип данных как целый
            isKeyword = true;
        }
        else if (IsString(word)) {
            strcpy_s(le.lexema, LEX_STRING);
            ctx.currentDataType = IT::STR;
            isKeyword = true;
        }
        else if (IsFunction(word)) {
            strcpy_s(le.lexema, LEX_FUNCTION);
            ctx.expectingFunctionName = true;
            isKeyword = true;
        }
        else if (IsDeclare(word)) {
            strcpy_s(le.lexema, LEX_DECLARE);
            isKeyword = true;
        }
        else if (IsReturn(word)) {
            strcpy_s(le.lexema, LEX_RETURN);
            isKeyword = true;
        }
        else if (IsPrint(word)) {
            strcpy_s(le.lexema, LEX_PRINT);
            isKeyword = true;
        }
        else if (IsMain(word)) {
            strcpy_s(le.lexema, LEX_MAIN);
            idtype = IT::F;
            ctx.currentFunctionIdx = ctx.lex.idtable.size;

            // Создаем запись для main в таблице идентификаторов
            IT::Entry ie{};
            strncpy_s(ie.id, word, ID_MAXSIZE - 1);  // Копируем имя функции
            ie.idxfirstLE = ctx.lex.lextable.size;  // индекс первой лексемы
            ie.iddatatype = IT::INT;
            ie.idtype = IT::F;
            ie.parentFunctionIdx = -1;
            IT::Add(ctx.lex.idtable, ie);
            le.idxTI = ctx.lex.idtable.size - 1;  // Связываем лексему с идентификатором
            isKeyword = true;
        }

        // Если слово не ключевое, обрабатываем как пользовательский идентификатор
        if (!isKeyword) {
            int errorCode = 0;
            if (!IsValidIdentifier(word, errorCode)) {
                int col = GetColumnInLine(ctx.in, wordStartPos);
                throw ERROR_THROW_IN(errorCode, ctx.line, col);
            }

            strcpy_s(le.lexema, LEX_ID);  // Устанавливаем лексему как идентификатор


            if (ctx.expectingFunctionName) {
                idtype = IT::F;  // Если ожидалось имя функции, устанавливаем тип функции
                ctx.expectingFunctionName = false;
            }
            else if (ctx.insideParameters) {
                idtype = IT::P;
            }
            else {
                idtype = IT::V;
            }

            // Проверяем является ли это объявлением переменной/параметра
            bool isDeclaration = false;
            if (ctx.lex.lextable.size > 0) {
                // Получаем предыдущую лексему для анализа контекста
                LT::Entry prevLex = LT::GetEntry(ctx.lex.lextable, ctx.lex.lextable.size - 1);

                if (strcmp(prevLex.lexema, LEX_INTEGER) == 0 || strcmp(prevLex.lexema, "i") == 0) {
                    isDeclaration = true;
                }
                // Если внутри параметров и перед идентификатором скобка или запятая - это объявление параметра
                else if ((strcmp(prevLex.lexema, LEX_LPAREN) == 0 || strcmp(prevLex.lexema, LEX_COMMA) == 0)
                    && ctx.insideParameters) {
                    isDeclaration = true;
                }
            }


            int existingIdIndex = -1;

            if (!isDeclaration) {
                // Если не объявление, ищем идентификатор в текущей области видимости
                if (ctx.insideFunctionBody || ctx.insideParameters) {
                    for (int k = 0; k < ctx.lex.idtable.size; k++) {
                        IT::Entry existing = IT::GetEntry(ctx.lex.idtable, k);
                        // Проверяем совпадение имени и область видимости
                        if (strcmp(existing.id, word) == 0) {
                            if ((existing.idtype == IT::P || existing.idtype == IT::V) &&
                                existing.parentFunctionIdx == ctx.currentFunctionIdx) {
                                existingIdIndex = k;  // Нашли существующий идентификатор
                                break;
                            }
                        }
                    }
                }
                // Если не нашли в текущей области, ищем глобально
                if (existingIdIndex == -1) {
                    existingIdIndex = IT::IsId(ctx.lex.idtable, word);
                }
            }

            // Обработка найденного или нового идентификатора
            if (existingIdIndex != TI_NULLIDX && existingIdIndex != -1 && !isDeclaration) {
                // Используем существующий идентификатор
                le.idxTI = existingIdIndex;
            }
            else {
                // Создаем новую запись в таблице идентификаторов
                IT::Entry ie{};
                strncpy_s(ie.id, word, ID_MAXSIZE - 1);  // Копируем имя идентификатора
                ie.idxfirstLE = ctx.lex.lextable.size;
                ie.iddatatype = ctx.currentDataType;
                ie.idtype = idtype;
                ie.parentFunctionIdx = -1;  // Изначально нет родительской функции

                // Устанавливаем связь с родительской функцией 
                if (idtype == IT::F) {
                    ctx.currentFunctionIdx = ctx.lex.idtable.size;  // Запоминаем индекс новой функции
                }
                else if ((idtype == IT::P || idtype == IT::V) && ctx.currentFunctionIdx != -1) {
                    ie.parentFunctionIdx = ctx.currentFunctionIdx;  // Связываем с текущей функцией
                }

                IT::Add(ctx.lex.idtable, ie);
                le.idxTI = ctx.lex.idtable.size - 1;  // Связываем лексему с идентификатором
            }
        }

        LT::Add(ctx.lex.lextable, le);
    }

    // Обработка числового литерала
    void ProcessNumber(LexContext& ctx, char* num) {
        LT::Entry le{};
        le.sn = ctx.line;
        strcpy_s(le.lexema, LEX_LITERAL);

        // Поиск существующего литерала с таким же значением
        int existingLiteral = -1;
        int value = atoi(num);

        // Ищем в таблице идентификаторов литерал с таким же значением
        for (int i = 0; i < ctx.lex.idtable.size; i++) {
            IT::Entry existing = IT::GetEntry(ctx.lex.idtable, i);
            if (existing.idtype == IT::L && existing.iddatatype == IT::INT) {
                if (existing.value.vint == value) {
                    existingLiteral = i;  // Нашли существующий литерал
                    break;
                }
            }
        }

        // Если литерал уже существует, используем его
        if (existingLiteral != -1) {
            le.idxTI = existingLiteral;
        }
        else {
            // Создаем новую запись для литерала
            IT::Entry ie{};
            char literalId[ID_MAXSIZE];
            // Генерируем уникальное имя для литерала (L0, L1, L2, ...)
            snprintf(literalId, ID_MAXSIZE, "L%d", ctx.literalCounter++);
            strncpy_s(ie.id, literalId, ID_MAXSIZE - 1);
            ie.id[ID_MAXSIZE - 1] = '\0';

            ie.idxfirstLE = ctx.lex.lextable.size;
            ie.iddatatype = IT::INT;
            ie.idtype = IT::L;
            ie.value.vint = value;
            ie.parentFunctionIdx = -1;  // Литералы не привязаны к функциям

            IT::Add(ctx.lex.idtable, ie);  // Добавляем в таблицу идентификаторов
            le.idxTI = ctx.lex.idtable.size - 1;  // Связываем лексему с идентификатором
        }

        LT::Add(ctx.lex.lextable, le);
    }

    // Обработка строкового литерала
    void ProcessString(LexContext& ctx, char* str) {
        LT::Entry le{};
        le.sn = ctx.line;
        strcpy_s(le.lexema, LEX_LITERAL);

        // Поиск существующего строкового литерала
        int existingLiteral = -1;

        for (int i = 0; i < ctx.lex.idtable.size; i++) {
            IT::Entry existing = IT::GetEntry(ctx.lex.idtable, i);
            if (existing.idtype == IT::L && existing.iddatatype == IT::STR) {
                // Сравниваем содержимое строк
                if (strcmp(existing.value.vstr.str, str) == 0) {
                    existingLiteral = i;
                    break;
                }
            }
        }

        // Используем существующий или создаем новый литерал
        if (existingLiteral != -1) {
            le.idxTI = existingLiteral;
        }
        else {
            IT::Entry ie{};
            char literalId[ID_MAXSIZE];
            snprintf(literalId, ID_MAXSIZE, "L%d", ctx.literalCounter++);
            strncpy_s(ie.id, literalId, ID_MAXSIZE - 1);
            ie.id[ID_MAXSIZE - 1] = '\0';

            ie.idxfirstLE = ctx.lex.lextable.size;
            ie.iddatatype = IT::STR;  // Тип данных - строковый
            ie.idtype = IT::L;
            ie.parentFunctionIdx = -1;

            // Инициализируем строковое значение
            memset(&ie.value, 0, sizeof(ie.value));
            ie.value.vstr.len = (unsigned char)strlen(str);  // Длина строки
            // Копируем строку в структуру
            strncpy_s(ie.value.vstr.str, TI_STR_MAXSIZE, str, TI_STR_MAXSIZE - 1);
            ie.value.vstr.str[TI_STR_MAXSIZE - 1] = '\0';

            IT::Add(ctx.lex.idtable, ie);
            le.idxTI = ctx.lex.idtable.size - 1;
        }

        LT::Add(ctx.lex.lextable, le);
    }

    // Обработка разделителей и операторов
    void ProcessDelimiter(LexContext& ctx, char c) {
        LT::Entry le{};
        le.sn = ctx.line;
        le.idxTI = LT_TI_NULLIDX;  // Разделители не ссылаются на таблицу идентификаторов

        // Обработка различных типов разделителей
        switch (c) {
        case ';':
            strcpy_s(le.lexema, LEX_SEMICOLON);
            break;
        case ',':
            strcpy_s(le.lexema, LEX_COMMA);
            break;
        case '{':
            strcpy_s(le.lexema, LEX_LBRACE);
            ctx.insideParameters = false;
            ctx.insideFunctionBody = true;
            break;
        case '}':
            strcpy_s(le.lexema, LEX_RBRACE);
            ctx.insideFunctionBody = false;
            ctx.currentFunctionIdx = -1;  // Сбрасываем текущую функцию
            break;
        case '(':
            strcpy_s(le.lexema, LEX_LPAREN);  // Левая круглая скобка
            // Проверяем контекст чтобы определить параметры функции
            if (ctx.lex.lextable.size > 0) {
                LT::Entry prevLex = LT::GetEntry(ctx.lex.lextable, ctx.lex.lextable.size - 1);
                // Если перед скобкой идентификатор или main, проверяем контекст
                if (strcmp(prevLex.lexema, LEX_ID) == 0 || strcmp(prevLex.lexema, LEX_MAIN) == 0) {
                    // Просматриваем предыдущие лексемы чтобы найти ключевое слово function
                    if (ctx.lex.lextable.size > 1) {
                        for (int back = ctx.lex.lextable.size - 2;
                            back >= 0 && back >= ctx.lex.lextable.size - 5; back--) {
                            LT::Entry checkLex = LT::GetEntry(ctx.lex.lextable, back);
                            if (strcmp(checkLex.lexema, LEX_FUNCTION) == 0) {
                                ctx.insideParameters = true;  // Входим в режим параметров функции
                                break;
                            }
                            // Если встретили конец выражения, прекращаем поиск
                            if (strcmp(checkLex.lexema, LEX_SEMICOLON) == 0 ||
                                strcmp(checkLex.lexema, LEX_RBRACE) == 0) {
                                break;
                            }
                        }
                    }
                }
            }
            break;
        case ')':
            strcpy_s(le.lexema, LEX_RPAREN);
            ctx.insideParameters = false;
            break;
        case '+': strcpy_s(le.lexema, LEX_PLUS); break;
        case '-': strcpy_s(le.lexema, LEX_MINUS); break;
        case '*': strcpy_s(le.lexema, LEX_MULT); break;
        case '/': strcpy_s(le.lexema, LEX_DIV); break;
        case '=': strcpy_s(le.lexema, "v"); break;
        default: return;
        }

        LT::Add(ctx.lex.lextable, le);
    }

    // Читает исходный код и преобразует его в последовательность лексем
    LEX LexAnalysis(In::IN in, Log::LOG log) {
        LexContext ctx;  // Контекст анализатора
        ctx.in = in;
        ctx.log = log;
        ctx.lex.lextable = LT::Create(1000);
        ctx.lex.idtable = IT::Create(100);
        ctx.position = 0;
        ctx.line = 1;
        ctx.currentFunctionIdx = -1;       // Индекс текущей функции (-1 - нет функции)
        ctx.expectingFunctionName = false; // Флаг ожидания имени функции
        ctx.insideParameters = false;
        ctx.insideFunctionBody = false;
        ctx.currentDataType = IT::INT;
        ctx.literalCounter = 0;

        *log.stream << "Начало лексического анализа" << std::endl;

        char buffer[256] = "";  // Буфер для накопления символов текущей лексемы
        int bufferIdx = 0;
        int wordStartPos = 0;
        bool inWord = false;    // Флаг обработки идентификатора/ключа
        bool inNumber = false;
        bool inString = false;

        // Основной цикл чтения исходного кода
        while (ctx.position < in.size && in.text[ctx.position] != '\0') {
            unsigned char c = in.text[ctx.position];  // Текущий символ

            // Обработка пробельных символов (пробел, табуляция, возврат каретки)
            if (c == ' ' || c == '\t' || c == '\r') {
                if (inWord) {
                    // Завершаем обработку идентификатора
                    buffer[bufferIdx] = '\0';
                    ProcessIdentifier(ctx, buffer, wordStartPos);
                    bufferIdx = 0;
                    inWord = false;
                }
                else if (inNumber) {
                    // Завершаем обработку числа
                    buffer[bufferIdx] = '\0';
                    ProcessNumber(ctx, buffer);
                    bufferIdx = 0;
                    inNumber = false;
                }
                ctx.position++;
                continue;
            }

            // Обработка перевода строки
            if (c == '\n' || c == '|') {
                if (inWord) {
                    buffer[bufferIdx] = '\0';
                    ProcessIdentifier(ctx, buffer, wordStartPos);
                    bufferIdx = 0;
                    inWord = false;
                }
                else if (inNumber) {
                    buffer[bufferIdx] = '\0';
                    ProcessNumber(ctx, buffer);
                    bufferIdx = 0;
                    inNumber = false;
                }
                ctx.line++;  // Увеличиваем счетчик строк
                ctx.position++;
                continue;
            }

            // Обработка строковых литералов (обрамляются одинарными кавычками)
            if (c == '\'') {
                if (!inString) {
                    // Начало строкового литерала
                    inString = true;
                    bufferIdx = 0;
                    ctx.position++;
                }
                else {
                    // Конец строкового литерала
                    buffer[bufferIdx] = '\0';
                    ProcessString(ctx, buffer);
                    bufferIdx = 0;
                    inString = false;
                    ctx.position++;
                }
                continue;
            }

            // Накопление символов строкового литерала
            if (inString) {
                if (bufferIdx < TI_STR_MAXSIZE - 1) {
                    buffer[bufferIdx++] = c;  // Добавляем символ в буфер
                }
                ctx.position++;
                continue;
            }

            // Обработка букв (идентификаторы и ключевые слова)
            if (isalpha(c)) {
                if (inNumber) {
                    // Если были в числе, завершаем его обработку
                    buffer[bufferIdx] = '\0';
                    ProcessNumber(ctx, buffer);
                    bufferIdx = 0;
                    inNumber = false;
                }
                if (!inWord) {
                    wordStartPos = ctx.position;  // Запоминаем начало слова
                }
                inWord = true;
                if (bufferIdx < 99) {
                    buffer[bufferIdx++] = c;  // Добавляем букву в буфер
                }
                ctx.position++;
                continue;
            }

            // Обработка цифр (числовые литералы)
            if (isdigit(c)) {
                if (inWord) {
                    // Если в идентификаторе, добавляем цифру к нему
                    if (bufferIdx < 99) {
                        buffer[bufferIdx++] = c;
                    }
                }
                else {
                    // Начинаем новое число
                    inNumber = true;
                    if (bufferIdx < 31) {
                        buffer[bufferIdx++] = c;
                    }
                }
                ctx.position++;
                continue;
            }

            // Обработка разделителей и операторов
            if (strchr("();{},+-*/=", c)) {
                if (inWord) {
                    // Завершаем идентификатор перед разделителем
                    buffer[bufferIdx] = '\0';
                    ProcessIdentifier(ctx, buffer, wordStartPos);
                    bufferIdx = 0;
                    inWord = false;
                }
                else if (inNumber) {
                    // Завершаем число перед разделителем
                    buffer[bufferIdx] = '\0';
                    ProcessNumber(ctx, buffer);
                    bufferIdx = 0;
                    inNumber = false;
                }
                ProcessDelimiter(ctx, c);  // Обрабатываем сам разделитель
                ctx.position++;
                continue;
            }

            // Переход к следующему символу для необработанных случаев
            ctx.position++;
        }

        // Обработка оставшихся лексем в буфере после завершения чтения
        if (inWord) {
            buffer[bufferIdx] = '\0';
            ProcessIdentifier(ctx, buffer, wordStartPos);
        }
        else if (inNumber) {
            buffer[bufferIdx] = '\0';
            ProcessNumber(ctx, buffer);
        }
        else if (inString) {
            // Если остались в строке - ошибка незакрытой строки
            int col = GetColumnInLine(ctx.in, ctx.position);
            throw ERROR_THROW_IN(125, ctx.line, col);
        }

        *log.stream << "Лексический анализ завершен. Найдено " << ctx.lex.lextable.size << " лексем." << std::endl;
        return ctx.lex;
    }

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
            if (le.sn != currentLine) {
                *log.stream << std::endl;
                currentLine = le.sn;
                *log.stream << std::setw(2) << std::setfill('0') << currentLine << " ";
            }
            *log.stream << le.lexema;
        }
        *log.stream << std::endl;
    }
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
                    if (ie.value.vstr.len > 0 && ie.value.vstr.str[0] != '\0') {
                        *log.stream << "'" << ie.value.vstr.str << "'";
                    }
                    else {
                        *log.stream << "''";
                    }
                }
            }

            *log.stream << std::endl;
        }
    }
}

