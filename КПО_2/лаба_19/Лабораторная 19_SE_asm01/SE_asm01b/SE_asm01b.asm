.586
.MODEL FLAT, STDCALL
includelib kernel32.lib
includelib msvcrt.lib
includelib "..\Debug\SE_asm01a.lib"
getmin PROTO : DWORD, : DWORD
getmax PROTO : DWORD, : DWORD

ExitProcess PROTO : DWORD
MessageBoxA PROTO : DWORD, : DWORD, : DWORD, : DWORD
SetConsoleTitleA PROTO : DWORD
GetStdHandle PROTO :DWORD
WriteConsoleA PROTO :DWORD, :DWORD, :DWORD, :DWORD, :DWORD

.STACK 4096

.CONST

.DATA

INT_ARRAY		DWORD		-9, -6, -5, -8, -3, -4, -1, -8, -2, -12
TitleCon		DB			"console",0
Text			DB			"getmin+getmax=", 0
output			DB			10 dup(0)
consolehandle	DD			0h
max				DWORD		?

.CODE			

int_to_char PROC pstr : dword, intfield : sdword                 ;преобразовани€ целого числа в строку
  mov edi, pstr        ; EDI = адрес буфера дл€ строки
  mov esi, 0           ; обнул€ем счЄтчик цифр
  mov eax, intfield    ; EAX = число, которое надо вывести
  cdq                  ; готовим EDX:EAX к делению (распростран€ем знак)
  mov ebx, 10          ; делитель = 10 (дес€тична€ система)
  idiv ebx             ; делим на 10, остаток в EDX

  test eax, 80000000h  ; провер€ем, не отрицательное ли число
  jz plus              ; если неотрицательное Ч сразу в цикл plus
  neg eax              ; делаем число положительным
  neg edx              ; и остаток тоже по модулю
  mov cl, '-'          ; готовим символ '-'
  mov [edi], cl        ; записываем '-' в начало строки
  inc edi              ; сдвигаем указатель после '-'

plus:                  ; начало цикла набора цифр
  push dx              ; сохран€ем цифру (остаток) в стеке
  inc esi              ; увеличиваем количество цифр
  test eax, eax        ; все цифры уже собраны?
  jz fin               ; если да Ч выходим из цикла делени€
  cdq                  ; снова подготавливаем EDX:EAX к делению
  idiv ebx             ; делим частное на 10, получаем следующую цифру
  jmp plus             ; повтор€ем цикл

fin:                   ; готовимс€ записывать цифры в строку
  mov ecx, esi         ; ECX = число цифр

write:                 ; цикл записи цифр из стека
  pop bx               ; берЄм очередную цифру из стека
  add bl, '0'          ; делаем из неЄ символ '0'..'9'
  mov [edi], bl        ; пишем символ в буфер
  inc edi              ; переходим к следующему символу
  loop write           ; пока ECX > 0, продолжаем

  ret                  ; выходим из процедуры
int_to_char ENDP       ; конец определени€ int_to_char


main proc 
    ;подготовка консоли
	push offset TitleCon
	call SetConsoleTitleA

	push -11
	call GetStdHandle
	mov consolehandle, eax

	push 0 
	push 0
	push sizeof Text
	push offset Text
	push consolehandle
	call WriteConsoleA

	INVOKE getmax, OFFSET INT_ARRAY, LENGTHOF INT_ARRAY
	mov max, eax
	INVOKE getmin, OFFSET INT_ARRAY, LENGTHOF INT_ARRAY
	add eax, max

	INVOKE int_to_char, OFFSET output, eax
	push 0
	push 0
	push sizeof output
	push offset output
	push consolehandle
	call WriteConsoleA
	push -1
	call ExitProcess

main ENDP

end main
