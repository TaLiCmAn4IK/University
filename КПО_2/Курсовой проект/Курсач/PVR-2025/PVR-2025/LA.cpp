#include "stdafx.h"
#include "LA.h"
#include "FST.h"
#include "Graph.h"
#include <string>

using namespace std;

namespace Lex {

	static void addIntLiteral(const char* /*src*/, int value,
		int /*indexLex*/, int line,
		LT::LexTable& lextable, IT::IdTable& idtable,
		IT::Entry& entryIT, LT::Entry& entryLT,
		int& literalCounter,
		IT::IDDATATYPE type)
	{
		if (value > 32767 || value < -32768)
			throw ERROR_THROW_IN(206, line, 0);

		// 1) reuse existing literal by value
		for (int k = 0; k < idtable.size; ++k) {
			if (idtable.table[k].idtype == IT::L &&
				idtable.table[k].idDataType == type &&
				idtable.table[k].value.vint == value)
			{
				LT::Entry e = LT::WriteEntry(entryLT, LEX_LITERAL, k, line);
				LT::Add(lextable, e);

				// idxFirstLE уже должен быть выставлен при первом создании
				return;
			}
		}

		// 2) create new literal in IT (idxFirstLE пока неизвестен)
		entryIT = {};
		entryIT.idtype = IT::L;
		entryIT.idDataType = type;
		entryIT.value.vint = value;
		entryIT.idxFirstLE = TI_NULLIDX;

		char num[16]{};
		_itoa_s(literalCounter++, num, 10);
		char name[16] = "L";
		strcat_s(name, num);
		strcpy_s(entryIT.id, name);

		IT::Add(idtable, entryIT);

		// 3) emit LEX_LITERAL to LT
		int idx = IT::IsId(idtable, entryIT.id);
		LT::Entry e = LT::WriteEntry(entryLT, LEX_LITERAL, idx, line);
		LT::Add(lextable, e);

		// 4) fix idxFirstLE exactly to the real LT index
		if (idtable.table[idx].idxFirstLE == TI_NULLIDX)
			idtable.table[idx].idxFirstLE = lextable.size - 1;

		entryIT = {};
	}


	static void addStringLiteral(const char* src,
		int /*indexLex*/, int line,
		LT::LexTable& lextable, IT::IdTable& idtable,
		IT::Entry& entryIT, LT::Entry& entryLT,
		int& literalCounter)
	{
		size_t len = strlen(src);
		if (len < 2 || src[0] != '\"' || src[len - 1] != '\"')
			throw ERROR_THROW_IN(300, line, 0);

		char content[TI_STR_MAXSIZE]{};
		size_t realLen = min(len - 2, (size_t)(TI_STR_MAXSIZE - 1));
		memcpy(content, src + 1, realLen);
		content[realLen] = '\0';

		if (realLen >= TI_STR_MAXSIZE - 1)
			throw ERROR_THROW_IN(303, line, 0);

		// 1) reuse by content
		for (int k = 0; k < idtable.size; ++k) {
			if (idtable.table[k].idtype == IT::L &&
				idtable.table[k].idDataType == IT::STR &&
				idtable.table[k].value.vstr.len == (int)realLen &&
				strcmp(idtable.table[k].value.vstr.str, content) == 0)
			{
				LT::Entry e = LT::WriteEntry(entryLT, LEX_LITERAL, k, line);
				LT::Add(lextable, e);
				return;
			}
		}

		// 2) create new string literal in IT
		entryIT = {};
		entryIT.idtype = IT::L;
		entryIT.idDataType = IT::STR;
		entryIT.value.vstr.len = (int)realLen;
		strcpy_s(entryIT.value.vstr.str, content);
		entryIT.idxFirstLE = TI_NULLIDX;

		char num[16]{};
		_itoa_s(literalCounter++, num, 10);
		char name[16] = "L";
		strcat_s(name, num);
		strcpy_s(entryIT.id, name);

		IT::Add(idtable, entryIT);

		// 3) emit to LT
		int idx = IT::IsId(idtable, entryIT.id);
		LT::Entry e = LT::WriteEntry(entryLT, LEX_LITERAL, idx, line);
		LT::Add(lextable, e);

		// 4) fix idxFirstLE exactly
		if (idtable.table[idx].idxFirstLE == TI_NULLIDX)
			idtable.table[idx].idxFirstLE = lextable.size - 1;

		entryIT = {};
	}


	LEX LexAnaliz(Log::LOG log, const In::IN& in) {
		LEX lex;
		LT::LexTable lextable = LT::Create(LT_MAXSIZE);
		IT::IdTable idtable = IT::Create(TI_MAXSIZE);

		cleanup(in.text.get(), in.size, log);

		char* txt = in.text.get();

		while (txt[0] && ((unsigned char)txt[0] > 127)) {
			char* p = strchr(txt, IN_CODE_END);
			if (!p) break;
			memmove(txt, p + 1, strlen(p));
		}

		char** word = nullptr;

		do {
			word = divideText(txt, in.size);
		} while (word == nullptr);


		int start = 0;
		while (word[start] != nullptr &&
			(unsigned char)word[start][0] > 127) {
			++start;
		}

		int indexLex = 0;
		int literalCounter = 1;
		int line = 1;
		int position = 0;
		bool findMain = false;
		int mainCounter = 0;
		bool findDeclaration = false;
		bool findType = false;
		IT::IDDATATYPE currentDeclType = IT::SHORT;
		bool expectDeclId = false;

		std::stack<std::string> functions;
		char bufprefix[ID_MAXSIZE]{};

		IT::Entry entryIT{};
		LT::Entry entryLT{};

		for (int i = start; word[i] != nullptr; ++i, ++indexLex) {

			position += (int)strlen(word[i]);

			std::string token = word[i];

			if (token == "==" || token.find("==") != std::string::npos)
			{
				IT::Entry opIt{};
				strcpy_s(opIt.id, "==");
				opIt.idtype = IT::OP;
				opIt.idxFirstLE = lextable.size;
				if (IT::IsId(idtable, opIt.id) == TI_NULLIDX) IT::Add(idtable, opIt);

				LT::Entry eOp = LT::WriteEntry(entryLT, LEX_OPERATOR, IT::IsId(idtable, opIt.id), line);
				eOp.op = LT::OEQU;
				eOp.priority = 1;
				LT::Add(lextable, eOp);
				continue;
			}

			if (token == "!=" || token.find("!=") != std::string::npos)
			{
				IT::Entry opIt{};
				strcpy_s(opIt.id, "!=");
				opIt.idtype = IT::OP;
				opIt.idxFirstLE = lextable.size;
				if (IT::IsId(idtable, opIt.id) == TI_NULLIDX) IT::Add(idtable, opIt);

				LT::Entry eOp = LT::WriteEntry(entryLT, LEX_OPERATOR, IT::IsId(idtable, opIt.id), line);
				eOp.op = LT::ONEQU;
				eOp.priority = 1;
				LT::Add(lextable, eOp);
				continue;
			}

#ifdef _DEBUG
#endif
			// shortmain() слитно
			if (token == "shortmain") {
				// 1) тип short
				LT::Entry eType = LT::WriteEntry(entryLT, LEX_SHORT, LT_TI_NULLIDX, line);
				LT::Add(lextable, eType);

				// 2) идентификатор main как функция
				strcpy_s(entryIT.id, "main");
				entryIT.idxFirstLE = TI_NULLIDX;
				entryIT.idtype = IT::F;
				entryIT.idDataType = IT::SHORT;
				IT::Add(idtable, entryIT);

				int mainIdx = IT::IsId(idtable, entryIT.id);
				entryIT = {};

				LT::Entry eMain = LT::WriteEntry(entryLT, LEX_MAIN, mainIdx, line);
				LT::Add(lextable, eMain);

				functions.push("main");
				findMain = true;
				++mainCounter;

				indexLex += 1; // учли дополнительную лексему
				continue; // этот токен полностью обработан
			}


			// простой спец-кейс для main
			if (token == "main") {

				strcpy_s(entryIT.id, word[i]);
				entryIT.idxFirstLE = TI_NULLIDX;
				entryIT.idtype = IT::F;
				entryIT.idDataType = IT::SHORT;
				IT::Add(idtable, entryIT);
				entryIT = {};

				LT::Entry e = LT::WriteEntry(entryLT, LEX_MAIN,
					IT::IsId(idtable, word[i]), line);
				LT::Add(lextable, e);
				functions.push(word[i]);
				findMain = true;
				++mainCounter;
				continue; // чтобы дальше по FST не ходить
			}
			// return0, return1 и т.п. слитно
			if (token.rfind("return", 0) == 0 && token.size() > 6) {
				std::string numPart = token.substr(6); // "0" из "return0"

				// 1) лексема return
				LT::Entry eRet = LT::WriteEntry(entryLT, LEX_RET, LT_TI_NULLIDX, line);
				LT::Add(lextable, eRet);

				// 2) числовой литерал
				int value = atoi(numPart.c_str());
				addIntLiteral(numPart.c_str(), value,
					indexLex + 1, line,
					lextable, idtable, entryIT, entryLT,
					literalCounter, IT::SHORT);

				indexLex += 1; // учли дополнительную лексему
				continue; // этот word[i] полностью разобран
			}
			if (token.size() >= 2 && token.back() == ':' && isdigit((unsigned char)token[0])) {
				std::string numStr = token.substr(0, token.size() - 1);

				int value = atoi(numStr.c_str());
				addIntLiteral(numStr.c_str(), value, indexLex, line,
					lextable, idtable, entryIT, entryLT, literalCounter, IT::SHORT);

				// ':' обязателен для грамматики case/default
				LT::Entry eColon = LT::WriteEntry(entryLT, LEX_COLON, LT_TI_NULLIDX, line);
				LT::Add(lextable, eColon);
				indexLex += 1;

				continue;
			}
			if (token == "break") {
				LT::Entry e;
				LT::WriteEntry(e, LEX_BREAK, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				continue;
			}

			// default: (ключевое слово default + двоеточие слитно)
			if (token == "default:") {
				LT::Entry eDef = LT::WriteEntry(entryLT, LEX_DEFAULT, LT_TI_NULLIDX, line);
				LT::Add(lextable, eDef);

				LT::Entry eColon = LT::WriteEntry(entryLT, LEX_COLON, LT_TI_NULLIDX, line);
				LT::Add(lextable, eColon);

				// 1 токен ("default:") -> 2 лексемы (default и ':')
				indexLex += 1;

				continue;
			}
			size_t q1 = token.find('\"');
			if (q1 != std::string::npos) {
				size_t q2 = token.find('\"', q1 + 1);
				if (q2 == std::string::npos) {

					std::string left = token.substr(0, q1);
					std::string firstPart = token.substr(q1);

					std::string full = firstPart;
					int j = i + 1;
					while (word[j] != nullptr) {
						std::string t = word[j];
						full += " " + t;

						if (!t.empty() && t.back() == '\"')
							break;
						++j;
					}

					if (full.size() >= 2 && full.front() == '\"' && full.back() == '\"') {
						if (!left.empty()) {
							if (left == "output") {
								LT::Entry eOut =
									LT::WriteEntry(entryLT, LEX_OUTPUT, LT_TI_NULLIDX, line);
								LT::Add(lextable, eOut);
							}
							else {
								char tmpId[ID_MAXSIZE]{};
								strncpy_s(tmpId, left.c_str(), ID_MAXSIZE - 1);

								int idx = IT::IsId(idtable, tmpId);
								if (idx != TI_NULLIDX) {
									LT::Entry eId =
										LT::WriteEntry(entryLT, LEX_ID, idx, line);
									LT::Add(lextable, eId);
								}
								else
								{

									entryIT = {};
									entryIT.idxFirstLE = TI_NULLIDX;
									entryIT.idtype = IT::V;
									strcpy_s(entryIT.id, tmpId);

									entryIT.idDataType = IT::SHORT;
									entryIT.value.vint = TI_INT_DEFAULT;

									IT::Add(idtable, entryIT);

									int newIdx = IT::IsId(idtable, entryIT.id);
									LT::Entry eId = LT::WriteEntry(entryLT, LEX_ID, newIdx, line);
									LT::Add(lextable, eId);

									entryIT = {};
								}

							}
						}

						addStringLiteral(full.c_str(),
							indexLex + (!left.empty() ? 1 : 0),
							line, lextable, idtable, entryIT, entryLT, literalCounter);

						indexLex += (!left.empty() ? 2 : 1);
						i = j;
						continue;
					}
				}
			}
			// ===== конец блока склейки =====

			if (token.size() >= 2 && token.front() == '\"' && token.back() == '\"') {
				addStringLiteral(token.c_str(), indexLex, line,
					lextable, idtable, entryIT, entryLT, literalCounter);
				continue;
			}

			// output"a" -> "output" + "\"a\""
			if (token.rfind("output", 0) == 0 && token.size() > 6) {
				std::string rest = token.substr(6);

				LT::Entry e1 = LT::WriteEntry(entryLT, LEX_OUTPUT, LT_TI_NULLIDX, line);
				LT::Add(lextable, e1);

				if (rest.size() >= 2 && rest.front() == '\"' && rest.back() == '\"') {
					addStringLiteral(rest.c_str(), indexLex + 1, line,
						lextable, idtable, entryIT, entryLT, literalCounter);
				}
				else {
					char tmpId[ID_MAXSIZE]{};
					strncpy_s(tmpId, rest.c_str(), ID_MAXSIZE - 1);

					entryIT.idxFirstLE = TI_NULLIDX;
					entryIT.idtype = IT::V;
					strcpy_s(entryIT.id, tmpId);
					entryIT.idDataType = IT::STR;
					entryIT.value.vstr.len = 0;
					entryIT.value.vstr.str[0] = '\0';

					IT::Add(idtable, entryIT);
					LT::Entry e2 = LT::WriteEntry(entryLT, LEX_ID,
						IT::IsId(idtable, entryIT.id), line);
					LT::Add(lextable, e2);
					entryIT = {};
				}

				indexLex += 1;
				continue;
			}

			// shorta -> "short" + "a"
			if (token.rfind("short", 0) == 0 && token.size() > 5 &&
				(isalpha((unsigned char)token[5]) || token[5] == '_')) {
				std::string rest = token.substr(5);

				// 1) добавляем ключевое слово short
				LT::Entry e1 = LT::WriteEntry(entryLT, LEX_SHORT, LT_TI_NULLIDX, line);
				LT::Add(lextable, e1);

				// 2) подготавливаем идентификатор из хвоста
				strcpy_s(entryIT.id, rest.c_str());
				entryIT.idxFirstLE = TI_NULLIDX;
				entryIT.idtype = IT::V;
				entryIT.idDataType = IT::SHORT;
				entryIT.value.vint = TI_INT_DEFAULT;

				IT::Add(idtable, entryIT);
				LT::Entry e2 = LT::WriteEntry(entryLT, LEX_ID,
					IT::IsId(idtable, entryIT.id), line);
				LT::Add(lextable, e2);
				entryIT = {};

				indexLex += 1;
				continue;
			}
			// stringa -> "string" + "a"
			if (token.rfind("string", 0) == 0 && token.size() > 6 &&
				(isalpha((unsigned char)token[6]) || token[6] == '_')) {

				std::string rest = token.substr(6); // хвост после "string"

				// 1) лексема типа string
				LT::Entry e1 = LT::WriteEntry(entryLT, LEX_STRING, LT_TI_NULLIDX, line);
				LT::Add(lextable, e1);

				// 2) идентификатор переменной
				entryIT = {};
				strcpy_s(entryIT.id, rest.c_str());
				entryIT.idxFirstLE = TI_NULLIDX;
				entryIT.idtype = IT::V;
				entryIT.idDataType = IT::STR;
				entryIT.value.vstr.len = 0;
				entryIT.value.vstr.str[0] = '\0';

				IT::Add(idtable, entryIT);

				LT::Entry e2 = LT::WriteEntry(entryLT, LEX_ID,
					IT::IsId(idtable, entryIT.id), line);
				LT::Add(lextable, e2);

				entryIT = {};

				indexLex += 1; // 2 лексемы вместо 1 токена
				continue;
			}


			// 0h0A, 0hFF и т.п. -> шестнадцатеричный литерал
			if (token.size() > 2 && token[0] == '0' && (token[1] == 'h' || token[1] == 'H')) {
				std::string hexpart = token.substr(2); // часть после 0h
				int value = (int)strtol(hexpart.c_str(), nullptr, 16);

				addIntLiteral(token.c_str(), value, indexLex, line,
					lextable, idtable, entryIT, entryLT,
					literalCounter, IT::SHORT);
				continue;
			}
			// 0b1010, 0B0110 и т.п. -> двоичный литерал
			if (token.size() > 2 && token[0] == '0' && (token[1] == 'b' || token[1] == 'B')) {
				std::string binpart = token.substr(2); // часть после 0b

				// проверим, что там только 0 и 1
				bool ok = true;
				for (char ch : binpart) {
					if (ch != '0' && ch != '1') { ok = false; break; }
				}
				if (!ok) {
					throw ERROR_THROW_IN(205, line, position);
				}

				int value = (int)strtol(binpart.c_str(), nullptr, 2);
				addIntLiteral(token.c_str(), value, indexLex, line,
					lextable, idtable, entryIT, entryLT,
					literalCounter, IT::SHORT);
				continue;
			}
			// --- ключевые слова должны обрабатываться ДО isId, иначе они станут LEX_ID ---
			if (token == "switch") {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_SWITCH, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				continue;
			}
			if (token == "case") {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_CASE, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				continue;
			}
			if (token == "default") {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_DEFAULT, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				continue;
			}
			// output как отдельное слово (не только output"строка")
			if (token == "output") {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_OUTPUT, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				continue;
			}
			if (token == "short") {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_SHORT, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				continue;
			}
			if (token == "string") {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_STRING, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);

				findType = true;
				currentDeclType = IT::STR;
				expectDeclId = true;
				continue;
			}

			// ====================== ВТОРАЯ ЧАСТЬ (приведена к валидному C++) ======================

			// Всегда синхронизируем indexLex с реальным размером LT (иначе idxFirstLE снова поедет)
			indexLex = lextable.size;


			// ===== helpers =====
			auto isIdent = [](const std::string& s) -> bool
			{
				if (s.empty()) return false;
				if (!(std::isalpha((unsigned char)s[0]) || s[0] == '_')) return false;
				for (size_t k = 1; k < s.size(); ++k)
					if (!(std::isalnum((unsigned char)s[k]) || s[k] == '_')) return false;
				return true;
			};
			auto currentLE = [&]() -> int {
				return (lextable.size > 0) ? (lextable.size - 1) : 0;
			};
			auto isDecInt = [](const std::string& s) -> bool
			{
				if (s.empty()) return false;
				for (char c : s)
					if (!std::isdigit((unsigned char)c)) return false;
				return true;
			};

			auto ensureOpInIT = [&](const std::string& opStr) -> int
			{
				char idbuf[ID_MAXSIZE]{};
				strcpy_s(idbuf, opStr.c_str());

				int idx = IT::IsId(idtable, idbuf);
				if (idx != TI_NULLIDX) return idx;

				IT::Entry opIt{};
				strcpy_s(opIt.id, idbuf);
				opIt.idtype = IT::OP;
				opIt.idxFirstLE = TI_NULLIDX; // выставит Synchronization()

				IT::Add(idtable, opIt);
				return IT::IsId(idtable, idbuf);
			};

			auto emitOp = [&](const std::string& opStr, LT::operations opEnum, int prio) -> void
			{
				int opIdx = ensureOpInIT(opStr);

				LT::Entry eOp = LT::WriteEntry(entryLT, LEX_OPERATOR, opIdx, line);
				eOp.op = opEnum;
				eOp.priority = prio;
				LT::Add(lextable, eOp);

				indexLex = lextable.size;
			};

			auto emitCompareOp = [&](const std::string& opStr) -> bool
			{
				LT::operations opEnum = LT::ONOT;

				if (opStr == "==") opEnum = LT::OEQU;
				else if (opStr == "!=") opEnum = LT::ONEQU;
				else if (opStr == "<=") opEnum = LT::OLESS_EQU;
				else if (opStr == ">=") opEnum = LT::OMORE_EQU;
				else if (opStr == "<") opEnum = LT::OLESS;
				else if (opStr == ">") opEnum = LT::OMORE;
				else return false;

				emitOp(opStr, opEnum, 1);
				return true;
			};

			auto emitArithmeticOp = [&](const std::string& opStr) -> bool
			{
				LT::operations opEnum = LT::ONOT;
				int prio = 0;

				if (opStr == "+") { opEnum = LT::OPLUS; prio = 2; }
				else if (opStr == "-") { opEnum = LT::OMINUS; prio = 2; }
				else if (opStr == "*") { opEnum = LT::OMUL; prio = 3; }
				else if (opStr == "/") { opEnum = LT::ODIV; prio = 3; }
				else if (opStr == "%") { opEnum = LT::OMOD; prio = 3; }
				else return false;

				emitOp(opStr, opEnum, prio);
				return true;
			};

			auto emitOperand = [&](const std::string& s) -> bool
			{
				if (s.empty()) return false;

				if (isIdent(s))
				{
					char tmpId[ID_MAXSIZE]{};
					strncpy_s(tmpId, s.c_str(), ID_MAXSIZE - 1);

					int idx = IT::IsId(idtable, tmpId);
					if (idx == TI_NULLIDX)
					{
						IT::Entry it{};
						it.idxFirstLE = TI_NULLIDX; // выставит Synchronization()
						it.idtype = IT::V;
						strcpy_s(it.id, tmpId);

						it.idDataType = expectDeclId ? currentDeclType : IT::SHORT;
						if (expectDeclId) expectDeclId = false;

						if (it.idDataType == IT::SHORT) it.value.vint = TI_INT_DEFAULT;
						else { it.value.vstr.len = 0; it.value.vstr.str[0] = '\0'; }

						IT::Add(idtable, it);
						idx = IT::IsId(idtable, tmpId);
					}

					LT::Entry e = LT::WriteEntry(entryLT, LEX_ID, idx, line);
					LT::Add(lextable, e);
					indexLex = lextable.size;
					return true;
				}

				if (isDecInt(s))
				{
					int v = std::stoi(s);

					int idx = TI_NULLIDX;
					for (int k = 0; k < idtable.size; ++k) {
						if (idtable.table[k].idtype == IT::L &&
							idtable.table[k].idDataType == IT::SHORT &&
							idtable.table[k].value.vint == v)
						{
							idx = k;
							break;
						}
					}

					if (idx == TI_NULLIDX)
					{
						IT::Entry lit{};
						lit.idxFirstLE = TI_NULLIDX; // выставит Synchronization()
						lit.idtype = IT::L;
						lit.idDataType = IT::SHORT;
						lit.value.vint = v;

						char name[ID_MAXSIZE]{};
						strcpy_s(name, "L");
						char num[32]{};
						_itoa_s(v, num, 10);
						strcat_s(name, num);
						strcpy_s(lit.id, name);

						if (IT::IsId(idtable, lit.id) != TI_NULLIDX) strcat_s(lit.id, "_");

						IT::Add(idtable, lit);
						idx = idtable.size - 1;
					}

					LT::Entry e = LT::WriteEntry(entryLT, LEX_LITERAL, idx, line);
					LT::Add(lextable, e);
					indexLex = lextable.size;
					return true;
				}

				return false;
			};

			// ======= MAIN token dispatch =======
			const std::string tok = token; // token (std::string) уже есть у тебя выше

			// --- glue "!" + "=" into "!=" (supports: "!" "=" and "!" "=c") ---
			if (tok == "!" && word[i + 1] != nullptr) {
				std::string t2 = word[i + 1];
				if (!t2.empty() && t2[0] == '=') {
					// emit operator !=
					if (!emitCompareOp("!=")) throw ERROR_THROW_IN(205, line, position);

					// optional right part if token was "=c"
					std::string right = t2.substr(1);
					if (!right.empty()) {
						if (!emitOperand(right)) throw ERROR_THROW_IN(205, line, position);
					}

					++i; // consumed word[i+1]
					continue;
				}
			}

			// 0) Комментарий: если текущий токен "/" и следующий тоже "/", пропускаем до конца строки
			if (tok == "/" && word[i + 1] != nullptr && strcmp(word[i + 1], "/") == 0) {
				int j = i + 2;
				while (word[j] != nullptr && strcmp(word[j], "|") != 0) ++j;
				if (word[j] != nullptr && strcmp(word[j], "|") == 0) { ++line; position = 0; }
				i = j;
				// indexLex не трогаем (ничего не добавляли)
				continue;
			}

			// 1) Перевод строки маркером '|'
			if (word[i][0] == '|' && word[i][1] == '\0') {
				++line;
				position = 0;
				continue;
			}

			// --- comparisons inside a single token: a<b, a<=b, a!=b, a==b, a>=b ---
			{
				size_t pos = std::string::npos;
				std::string op;

				// maximal munch: 2-char first
				if ((pos = tok.find("==")) != std::string::npos) op = "==";
				else if ((pos = tok.find("!=")) != std::string::npos) op = "!=";
				else if ((pos = tok.find("<=")) != std::string::npos) op = "<=";
				else if ((pos = tok.find(">=")) != std::string::npos) op = ">=";
				else if ((pos = tok.find('<')) != std::string::npos) op = "<";
				else if ((pos = tok.find('>')) != std::string::npos) op = ">";

				if (!op.empty()) {
					std::string left = tok.substr(0, pos);
					std::string right = tok.substr(pos + op.size());

					// строго: должны быть оба операнда
					if (!left.empty() && !right.empty() && emitOperand(left)) {
						if (!emitCompareOp(op)) throw ERROR_THROW_IN(205, line, position);
						if (!emitOperand(right)) throw ERROR_THROW_IN(205, line, position);
						continue; // токен полностью обработан
					}
				}
			}

			// --- lookahead for glued 2-char compare: "a<" + "=" => "<=" (do this BEFORE emitting "<") ---
			if (word[i + 1] != nullptr) {
				std::string t1 = tok;
				std::string t2 = word[i + 1];

				auto endsWith = [](const std::string& s, char c) -> bool {
					return !s.empty() && s.back() == c;
				};

				if ((endsWith(t1, '<') || endsWith(t1, '>') || endsWith(t1, '!') || endsWith(t1, '=')) &&
					!t2.empty() && t2[0] == '=')
				{
					std::string left = t1.substr(0, t1.size() - 1);
					std::string right = t2.substr(1); // после '='
					char last = t1.back();

					if (!left.empty() && !emitOperand(left)) throw ERROR_THROW_IN(205, line, position);

					if (last == '<') emitCompareOp("<=");
					else if (last == '>') emitCompareOp(">=");
					else if (last == '!') emitCompareOp("!=");
					else if (last == '=') emitCompareOp("==");

					// если right не пустой (это был токен вида "=c" или "=b)"), съедаем его как операнд
					if (!right.empty()) {
						if (!emitOperand(right)) {
							throw ERROR_THROW_IN(205, line, position);
						}
					}

					++i; // съели word[i+1]
					continue;
				}

			}

			// --- comparisons possibly glued to operands: a<b, a<, <b, a<=b, a<=, <=b, etc. ---
			{
				size_t pos = std::string::npos;
				std::string op;

				// maximal munch: 2-char first
				if ((pos = tok.find("==")) != std::string::npos) op = "==";
				else if ((pos = tok.find("!=")) != std::string::npos) op = "!=";
				else if ((pos = tok.find("<=")) != std::string::npos) op = "<=";
				else if ((pos = tok.find(">=")) != std::string::npos) op = ">=";
				else if ((pos = tok.find('<')) != std::string::npos) op = "<";
				else if ((pos = tok.find('>')) != std::string::npos) op = ">";

				if (!op.empty()) {
					std::string left = tok.substr(0, pos);
					std::string right = tok.substr(pos + op.size());

					bool ok = true;

					// левый кусок (если есть)
					if (!left.empty()) ok = emitOperand(left);

					// сам оператор
					if (ok) ok = emitCompareOp(op);

					// правый кусок (если есть)
					if (ok && !right.empty()) ok = emitOperand(right);

					// ВАЖНО: даже если right пустой (случай "a<"), оператор всё равно эмитим;
					// правый операнд придёт следующим токеном.
					// Аналогично "<b": левый операнд уже был раньше.
					if (ok) continue;

					// если не смогли распознать как (left?) op (right?) — падаем дальше по обычной логике
				}
			}

			// 2) Операнды (идентификаторы/числа)
			if (emitOperand(tok)) {
				continue;
			}

			// 3) Скобки/фигурные/точка с запятой
			if (strcmp(word[i], "(") == 0) {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_LEFTTHESIS, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				indexLex = lextable.size;
				continue;
			}
			if (strcmp(word[i], ")") == 0) {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_RIGHTTHESIS, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				indexLex = lextable.size;
				continue;
			}
			if (strcmp(word[i], ",") == 0) {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_COMMA, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				indexLex = lextable.size;
				continue;
			}
			if (strcmp(word[i], "{") == 0) {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_LEFTBRACE, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				indexLex = lextable.size;
				continue;
			}
			if (strcmp(word[i], "}") == 0) {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_RIGHTBRACE, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				indexLex = lextable.size;
				continue;
			}
			if (tok == ";") {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_SEMICOLON, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				indexLex = lextable.size;

				findDeclaration = findType = false;
				expectDeclId = false;
				entryIT = {};
				continue;
			}

			// 4) Двухсимвольные сравнения (maximal munch)
			if (word[i + 1] != nullptr) {
				if (strcmp(word[i], "=") == 0 && strcmp(word[i + 1], "=") == 0) { emitCompareOp("=="); ++i; continue; }
				if (strcmp(word[i], "!") == 0 && strcmp(word[i + 1], "=") == 0) { emitCompareOp("!="); ++i; continue; }
				if (strcmp(word[i], "<") == 0 && strcmp(word[i + 1], "=") == 0) { emitCompareOp("<="); ++i; continue; }
				if (strcmp(word[i], ">") == 0 && strcmp(word[i + 1], "=") == 0) { emitCompareOp(">="); ++i; continue; }
			}

			// 5) Одиночное '=' — присваивание
			if (strcmp(word[i], "=") == 0) {
				LT::Entry e = LT::WriteEntry(entryLT, LEX_ASSIGN, LT_TI_NULLIDX, line);
				LT::Add(lextable, e);
				indexLex = lextable.size;
				continue;
			}

			// 6) Одиночные сравнения < >
			if (strcmp(word[i], "<") == 0) { emitCompareOp("<"); continue; }
			if (strcmp(word[i], ">") == 0) { emitCompareOp(">"); continue; }

			// 7) Одиночный '!' запрещён
			if (strcmp(word[i], "!") == 0) {
				throw ERROR_THROW_IN(205, line, -1);
			}

			// 8) Арифметика
			if (emitArithmeticOp(tok)) {
				continue;
			}

			// --- case0: / case0h03: / case0b1010: ---
			if (tok.rfind("case", 0) == 0 && !tok.empty() && tok.back() == ':') {
				std::string middle = tok.substr(4, tok.size() - 5);

				LT::Entry eCase = LT::WriteEntry(entryLT, LEX_CASE, LT_TI_NULLIDX, line);
				LT::Add(lextable, eCase);
				indexLex = lextable.size;

				if (!middle.empty()) {
					int value = 0;

					if (middle.size() > 2 && middle[0] == '0' && (middle[1] == 'h' || middle[1] == 'H')) {
						value = (int)strtol(middle.c_str() + 2, nullptr, 16);
					}
					else if (middle.size() > 2 && middle[0] == '0' && (middle[1] == 'b' || middle[1] == 'B')) {
						for (size_t t = 2; t < middle.size(); ++t)
							if (middle[t] != '0' && middle[t] != '1')
								throw ERROR_THROW_IN(205, line, position);

						value = (int)strtol(middle.c_str() + 2, nullptr, 2);
					}
					else {
						value = atoi(middle.c_str());
					}

					addIntLiteral(middle.c_str(), value, lextable.size, line,
						lextable, idtable, entryIT, entryLT, literalCounter, IT::SHORT);
					indexLex = lextable.size;
				}

				LT::Entry eColon = LT::WriteEntry(entryLT, LEX_COLON, LT_TI_NULLIDX, line);
				LT::Add(lextable, eColon);
				indexLex = lextable.size;

				continue;
			}

			// 9) Если дошли сюда — неизвестный токен
			throw ERROR_THROW_IN(205, line, position);

		} // конец for

		if (!findMain) throw ERROR_THROW(302);
		if (mainCounter > 1) throw ERROR_THROW(301);

		lex.idtable = idtable;
		lex.lextable = lextable;
		LT::Entry eEnd;
		LT::WriteEntry(eEnd, '$', LT_TI_NULLIDX, line);
		LT::Add(lextable, eEnd);

		int from = max(0, lextable.size - 200);
		for (int i = from; i < lextable.size; i++) {
			auto e = LT::GetEntry(lextable, i);
		}

		if (word) {
			for (int k = 0; word[k] != nullptr; ++k)
				delete[] word[k];
			delete[] word;
		}
		lex.idtable = idtable;
		lex.lextable = lextable;

		Synchronization(lex); // <-- ОБЯЗАТЕЛЬНО, иначе idxFirstLE останется TI_NULLIDX
		return lex;

		return lex;
	}


	void Synchronization(Lex::LEX& lex) {
		bool* is_changed = new bool[lex.idtable.size] { false };

		for (int i = 0; i < lex.lextable.size; ++i) {
			int idx = lex.lextable.table[i].idxTI;

			if (idx != LT_TI_NULLIDX &&
				idx >= 0 && idx < lex.idtable.size &&
				!is_changed[idx])
			{ 
				lex.idtable.table[idx].idxFirstLE = i;
				is_changed[idx] = true;
			}
		}

		delete[] is_changed;
	}

}
