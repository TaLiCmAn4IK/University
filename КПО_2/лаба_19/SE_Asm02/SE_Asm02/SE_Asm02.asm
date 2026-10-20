.586P													; система подкоманд(процессор Pentium)
.MODEL FLAT, STDCALL									; модель пам€ти, соглашение о вызове
includelib kernel32.lib									

ExitProcess PROTO :DWORD								
MessageBoxA PROTO :DWORD, :DWORD, :DWORD, :DWORD		

.STACK 4096											

.CONST												

.DATA													
		a db 4	                                        ; define double word 
		b db 2                                          ; define byte
		str0 db "Lab2ASM", 0 
		str1 db "–≈«”Ћ№“ј“ = < >", 0 

.CODE													; сегмент кода

main PROC												
START :													; метка
		mov al, a									
		add al, b										
		add al, 30h 								
		
		mov str1+13, al									
		
		invoke MessageBoxA, 0, offset str1, offset str0, 0

		push 0											;код возврата процесса Windows (параметр ExitProcess)
		call ExitProcess								;так завершаетс€ любой процесс Windows
main ENDP

end main								