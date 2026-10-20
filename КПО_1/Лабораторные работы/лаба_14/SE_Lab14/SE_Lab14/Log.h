#pragma once
#include "In.h"
#include "Parm.h"
#include "Error.h"
#include "stdafx.h"

#pragma warning(disable: 4996)

using namespace std;

namespace Log
{
	struct LOG
	{
		wchar_t logfile[PARM_MAX_SIZE];
		ofstream stream;

		// Запрещаем копирование
		LOG(const LOG&) = delete;
		LOG& operator=(const LOG&) = delete;

		// Разрешаем только перемещение
		LOG(LOG&& other) noexcept : stream(std::move(other.stream)) {
			wcscpy_s(logfile, other.logfile);
		}

		LOG& operator=(LOG&& other) noexcept {
			if (this != &other) {
				stream = std::move(other.stream);
				wcscpy_s(logfile, other.logfile);
			}
			return *this;
		}

		LOG() {
			logfile[0] = L'\0';
		}
	};

	LOG getlog(wchar_t* logfile);
	void WriteLine(LOG& log, const char* c, ...);
	void WriteLine(LOG& log, const wchar_t* c, ...);
	void WriteLog(LOG& log);
	void WriteParm(LOG& log, Parm::PARM parm);
	void WriteIn(LOG& log, In::IN in);
	void WriteError(LOG& log, Error::ERROR error);
	void Close(LOG& log);
}