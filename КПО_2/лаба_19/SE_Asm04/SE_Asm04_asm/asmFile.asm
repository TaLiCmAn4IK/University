.586P
.MODEL FLAT, STDCALL
includelib kernel32.lib
includelib user32.lib

ExitProcess PROTO : DWORD
MessageBoxA PROTO : DWORD, : DWORD, : DWORD, : DWORD

.STACK 4096

.CONST

.DATA

OK EQU 0
TEXT_MESSAGE DB "SE_Asm04 (с метаданными)", 0
HW DD ?

; === ДАННЫЕ С МЕТАИНФОРМАЦИЕЙ ===
; Тип 0x01 = bool, Тип 0x02 = short

FILEBOOL DB 0      ; значение bool
BOOLTYPE DB 1               ; метаданные: тип bool (0x01)
BOOLSTR DB "bool (тип 01):   ", 0

SHORTTYPE DB 2              ; метаданные: тип short (0x02)
FILESHORT DD 1, 2   ; цифры числа 12
SHORTSTR DB "short (тип 02):  ", 0

.CODE

main PROC

; Вывод bool с метаданными
mov al, BOOLTYPE        ; тип bool (0x01)
add al, 30h             ; в ASCII
mov BOOLSTR + 11, al    ; помещаем тип в строку

mov al, FILEBOOL
add al, 30h
mov BOOLSTR + 14, al    ; помещаем значение
invoke MessageBoxA, 0, offset BOOLSTR, ADDR TEXT_MESSAGE, OK

; Вывод short с метаданными
mov al, SHORTTYPE       ; тип short (0x02)
add al, 30h             ; в ASCII
mov SHORTSTR + 12, al   ; помещаем тип в строку

; Вывод цифр числа
mov eax, FILESHORT 
add eax, 30h
mov SHORTSTR +14, al
mov eax, FILESHORT  + type FILESHORT
add eax, 30h
mov SHORTSTR +15, al

invoke MessageBoxA, 0, offset SHORTSTR, ADDR TEXT_MESSAGE, OK

; Завершение программы
push - 1
call ExitProcess
main ENDP
end main
