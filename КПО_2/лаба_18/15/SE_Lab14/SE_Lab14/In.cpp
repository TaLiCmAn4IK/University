#include "stdafx.h"
#include "In.h"
#include "Error.h"

using namespace std;

namespace In
{
	IN getin(wchar_t infile[])
	{
		IN in;
		in.size = 0; in.lines = 1; in.ignor = 0;
		int col = 1;
		bool space = false; // флаг для отслеживания пробелов
		bool newLine = true; // флаг новой строки

		unsigned char* text = new unsigned char[IN_MAX_LEN_TEXT];

		ifstream fin(infile);
		if (!fin.is_open())
			throw ERROR_THROW(110);

		while (in.size < IN_MAX_LEN_TEXT)
		{
			char c;
			fin.get(c);
			unsigned char x = c;
			// Проверка на конец файла
			if (fin.eof())
			{
				if (in.size > 0 && text[in.size - 1] != IN_SEP_CHAR)
					text[in.size++] = IN_SEP_CHAR;
				text[in.size] = IN_TERMINATOR;
				break;
			}

			// Обработка перевода строки
			if (x == IN_CODE_ENDL) {
				text[in.size++] = IN_SEP_CHAR; // заменяем перевод строки на '|'
				newLine = true;
				space = false;
				in.lines++;
				col = 1;
				continue;
			}


			// Обработка пробелов и табуляций
			if (x == IN_SPACE_CHAR || x == IN_TAB_CHAR)
			{
				if (!space && !newLine && in.size > 0 && text[in.size - 1] != IN_SEP_CHAR)
				{
					text[in.size++] = IN_SPACE_CHAR; 
					space = true;
				}
				in.ignor++;
				col++;
				continue;
			}

			switch (in.code[x])
			{
			case in.T:
				text[in.size++] = x;
				col++;
				space = false;
				newLine = false;
				break;
			case in.I:
				in.ignor++;
				col++;
				break;
			case in.F:
				throw ERROR_THROW_IN(111, in.lines, col);
				break;
			default:
				text[in.size++] = in.code[x];
				col++;
				space = false;
				newLine = false;
				break;
			}
		}
		in.text = text;
		return in;
	}
}