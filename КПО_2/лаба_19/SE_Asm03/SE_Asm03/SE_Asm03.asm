.586P
.MODEL FLAT, STDCALL
includelib kernel32.lib
includelib user32.lib

ExitProcess PROTO :DWORD
MessageBoxA PROTO :DWORD, :DWORD, :DWORD, :DWORD

.STACK 4096

.DATA
    myBytes BYTE 10h, 20h, 30h, 40h                          ; массив байт (1 байт)  
    myWords WORD 8Ah, 3Bh, 44h, 5Fh, 99h                     ; массив слов (2 байта) 
    myDoubles DWORD 1, 2, 3, 4, 5, 6                         ; массив двойных слов (4 байта) 
    myPointer DWORD myDoubles                                ; указатель на myDoubles (4 байта) 

    massive DWORD 1, 2, 3, 4, 5, 6, 7                        ; массив чисел для задания c)
    sumOfElements DWORD 0                                    ; Переменная для хранения суммы
    
    StrTitle DB "Potrebko Vadim - Variant 12", 0
    StrZero DB "Zero element found in array", 0
    StrNoZero DB "No zero element found", 0

.CODE

main PROC
    mov ESI, OFFSET myWords                                  ; помещаем в регистр адрес начала массива myWords
    mov AX, [ESI + 4]                                        ; загружает в регистр AX значение (слово = 2 байта), находящееся по адресу ESI + 4
    mov BX, [ESI + 2]
    
    mov ESI, OFFSET massive 
    mov ECX, 7                                               ; ECX = 7 (количество элементов)
    mov EAX, 0                                               ; EAX = 0 (обнуляем сумму)
    
sum_loop:
    add EAX, [ESI]
    add ESI, 4
    loop sum_loop
    
    mov sumOfElements, EAX
    
    mov ESI, OFFSET massive
    mov ECX, 7
    mov EBX, 1
    
check_zero_loop:                                            ; Метка начала цикла проверки
    cmp DWORD PTR [ESI], 0                                  ; Сравнить значение указателя с 0
    je zero_found                                           ; Если результат сравнения равен 0 - перейти на zero_found
    add ESI, 4                                              ; ESI = ESI + 4 (следующий элемент)
    loop check_zero_loop                                    ; команда цикла если ECX≠0 - перейти к check_zero_loop
    jmp show_result                                         ; Перейти к выводу результата
    
zero_found:
    mov EBX, 0
    
show_result:
    cmp EBX, 0
    je show_zero_msg                                        ; Если EBX=0 - перейти к show_zero_msg
    
    invoke MessageBoxA, 0, OFFSET StrNoZero, OFFSET StrTitle, 0
    jmp exit_program
    
show_zero_msg:
    invoke MessageBoxA, 0, OFFSET StrZero, OFFSET StrTitle, 0
    
exit_program:
    push 0
    call ExitProcess

main ENDP
end main

