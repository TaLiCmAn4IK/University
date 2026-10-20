#pragma once
#define ASMHEADER \
<< ".586P" << endl \
<< ".MODEL FLAT, STDCALL" << endl \
<< "includelib kernel32.lib\n" \
<< "includelib user32.lib\n" << endl \
<< "ExitProcess PROTO : DWORD" << endl \
<< "MessageBoxA PROTO : DWORD, : DWORD, : DWORD, : DWORD\n" << endl \
<< ".STACK 4096\n" << endl \
<< ".CONST\n" << endl \
<< ".DATA\n" << endl \
<< "OK EQU 0" << endl \
<< "TEXT_MESSAGE DB \"SE_Asm04 (с метаданными)\", 0" << endl \
<< "HW DD ?\n" << endl \

class AsmGen
{
public:
    void CreateFile(bool valBool, short valShort);

private:
    const char* ASMFILE = "../SE_Asm04_asm/asmFile.asm";
};