#include <iostream>                         
#include <locale.h>                         
#include "D:\Документы\КПО_1\лаба_6\SE_Lab06S\SE_Lab06L\Dictionary.h"        // Подключаем заголовок нашей статической библиотеки

using namespace std;                        
using namespace Dictionary;                 

int main() {
    setlocale(LC_ALL, "Russian");           

    try {                                   // Блок try: здесь основной код, ошибки будут ловиться в catch
        cout << " Тест статической библиотеки Dictionary \n";

        // Создание словаря преподавателей
        Instance teachers = Create("Преподаватели", 10); 
        AddEntry(teachers, { 1, "Иванов И.И." });        
        AddEntry(teachers, { 2, "Петров П.П." });        
        AddEntry(teachers, { 3, "Сидоров С.С." });       
        AddEntry(teachers, { 4, "Кузнецов К.К." });    
        AddEntry(teachers, { 5, "Смирнов С.С." });    

 
        // Создание словаря студентов
        Instance students = Create("Студенты", 10);     
        AddEntry(students, { 101, "Алексеев А.А." });
        AddEntry(students, { 102, "Борисов Б.Б." });
        AddEntry(students, { 103, "Владимиров В.В." });
        AddEntry(students, { 104, "Григорьев Г.Г." });
        AddEntry(students, { 105, "Дмитриев Д.Д." });

        // Печать словарей
        cout << "\nИсходные словари:\n";   
        Print(teachers);                    
        Print(students);                    


        // Поиск, обновление и удаление
        Entry e = GetEntry(teachers, 3);    
        cout << "Найден преподаватель: " << e.name << endl; 

        UpdEntry(teachers, 3, { 3, "Сидоров С.С. (преподаватель)" }); 
        DelEntry(students, 104);            

   
        // Итоговый вывод после изменений
        cout << "\nПосле изменений:\n";
        Print(teachers);                    
        Print(students);                   

        // Очистка памяти
        Delete(teachers);                   
        Delete(students);                 

        cout << "\n Завершено успешно \n"; 
    }
    catch (const exception& e) {            // Если возникнет исключение
        cerr << "Ошибка: " << e.what() << endl; 
        return 1;                          
    }
    return 0;                               
}
