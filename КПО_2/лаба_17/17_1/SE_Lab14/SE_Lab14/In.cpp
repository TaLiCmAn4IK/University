#include "In.h"
#include "Error.h"
#include"stdafx.h"

using namespace std;

namespace In
{
	IN getin(wchar_t* infile)
	{
		IN inner_text;

		// Открытие файла в бинарном режиме для корректного чтения символов Windows-1251
		ifstream file(infile, ios::binary);
		if (!file)
			throw ERROR_THROW(110);

		file.seekg(0, ios::end);
		size_t fileSize = file.tellg();
		file.seekg(0, ios::beg);

		if (fileSize > IN_MAX_LEN_TEXT)
			throw ERROR_THROW(104);

		inner_text.size = 0; // размер текста
		inner_text.lines = 1; // кол-во строк
		inner_text.ignore = 0;
		inner_text.text = new unsigned char[IN_MAX_LEN_TEXT];

		int currentLine = 1;
		int currentPosition = 0;
		bool inSingleQuotes = false; 
		bool previousWasSpace = false; // Флаг: был ли предыдущий символ пробелом

		unsigned char tempChar;
		// чтение файла посимвольно
		while (file.read(reinterpret_cast<char*>(&tempChar), 1) && inner_text.size < IN_MAX_LEN_TEXT - 1)
		{
			if (tempChar == '\r') continue;

			// Обработка открытия и закрытие стр литералов
			if (tempChar == '\'')
			{
				inSingleQuotes = !inSingleQuotes;
				inner_text.text[inner_text.size++] = tempChar;
				currentPosition++;
				previousWasSpace = false;
				continue;
			}

			// Внутри строковых литералов принимаем все символы как есть
			if (inSingleQuotes)
			{
				inner_text.text[inner_text.size++] = tempChar;
				currentPosition++;
				previousWasSpace = false;
				continue;
			}

			// Обработка символов вне строковых литералов
			switch (inner_text.code[tempChar])
			{
			case IN::T: 
			{
				if (tempChar == ' ')
				{
					if (!previousWasSpace) // Если предыдущий символ не был пробелом
					{
						inner_text.text[inner_text.size++] = tempChar;
						currentPosition++;
						previousWasSpace = true;
					}
					else
					{
						inner_text.ignore++;
					}
				}
				else
				{
					inner_text.text[inner_text.size++] = tempChar;
					currentPosition++;
					previousWasSpace = false;
				}
				break;
			}

			case IN::I:
			{
				inner_text.ignore++;
				currentPosition++;
				previousWasSpace = false;
				break;
			}

			case IN::F:
			{
				throw ERROR_THROW_IN(111, currentLine, currentPosition + 1);
				break;
			}

			default:
			{
				if (tempChar == '\n')
				{
					inner_text.lines++;
					currentLine++;
					currentPosition = 0;
				}
				inner_text.text[inner_text.size++] = (unsigned char)inner_text.code[tempChar];
				currentPosition++;
				previousWasSpace = false;
				break;
			}
			}
		}

		inner_text.text[inner_text.size] = '\0';

		if (inner_text.size == 0)
			inner_text.lines = 0;

		file.close();
		return inner_text;
	}
}