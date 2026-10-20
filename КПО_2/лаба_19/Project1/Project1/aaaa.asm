.586
.MODEL FLAT, STDCALL
includelib kernel32.lib
includelib user32.lib

ExitProcess PROTO :DWORD
MessageBoxA PROTO :DWORD, :DWORD, :DWORD, :DWORD

.STACK 4096

.DATA
    caption DB "Результат", 0
    result_msg DB "Результат сложения = "
    result_num DB "0", 0
    MB_OK EQU 0

.CODE
main PROC
    MOV EAX, 3
    ADD EAX, 4 
    
    ADD AL, '0'
    
    MOV result_num[0], AL
    
    ; Вывод результата
    PUSH MB_OK
    PUSH OFFSET caption
    PUSH OFFSET result_msg
    PUSH 0
    CALL MessageBoxA
    
    ; Завершение
    PUSH 0
    CALL ExitProcess
main ENDP
END main