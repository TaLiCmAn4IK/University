#include "FST.h"
#include <iostream>
using namespace std;

int main() {
    setlocale(LC_ALL, "rus");

    const char* testStrings[] = {
        "begin;return;abs(calc);end;",
        "begin; return; abs(pr);end;",
        "begin;  return;    abs(calc);  end;",
        "begin; return; abs(pr);abs(pr); end;",
        "begin;  return;abs(calc);abs(pr);abs(pr);  end;",
        "begin;return; abs(calc);abs(calc);end;",
        "begin; return;  abs(calc);  abs(calc); abs(pr);  end;",
        "begin;abs(calc);end;",
        "begin;return;abs(calc)end;"  
    };

    cout << "Тестирование автомата:" << endl;

    for (int i = 0; i < 9; i++) {
        FST::FST testFst(
            (char*)testStrings[i],
            32,  

            
            FST::NODE(1, FST::RELATION('b', 1)),           // 0
            FST::NODE(1, FST::RELATION('e', 2)),           // 1
            FST::NODE(1, FST::RELATION('g', 3)),           // 2
            FST::NODE(1, FST::RELATION('i', 4)),           // 3
            FST::NODE(1, FST::RELATION('n', 5)),           // 4
            FST::NODE(1, FST::RELATION(';', 6)),           // 5

           
            FST::NODE(2, FST::RELATION(' ', 6), FST::RELATION('r', 7)), // 6

        
            FST::NODE(1, FST::RELATION('e', 8)),           // 7
            FST::NODE(1, FST::RELATION('t', 9)),           // 8
            FST::NODE(1, FST::RELATION('u', 10)),          // 9
            FST::NODE(1, FST::RELATION('r', 11)),          // 10
            FST::NODE(1, FST::RELATION('n', 12)),          // 11
            FST::NODE(1, FST::RELATION(';', 13)),          // 12

         
            FST::NODE(3, FST::RELATION(' ', 13), FST::RELATION('a', 14), FST::RELATION('e', 28)), // 13 

           
            FST::NODE(1, FST::RELATION('b', 15)),          // 14
            FST::NODE(1, FST::RELATION('s', 16)),          // 15
            FST::NODE(1, FST::RELATION('(', 17)),          // 16

        
            FST::NODE(2, FST::RELATION('c', 18), FST::RELATION('p', 24)), // 17

          
            FST::NODE(1, FST::RELATION('a', 19)),          // 18
            FST::NODE(1, FST::RELATION('l', 20)),          // 19
            FST::NODE(1, FST::RELATION('c', 21)),          // 20
            FST::NODE(1, FST::RELATION(')', 22)),          // 21
            FST::NODE(1, FST::RELATION(';', 23)),          // 22

         
            FST::NODE(3, FST::RELATION(' ', 23), FST::RELATION('a', 14), FST::RELATION('e', 28)), // 23

          \
            FST::NODE(1, FST::RELATION('r', 25)),          // 24
            FST::NODE(1, FST::RELATION(')', 26)),          // 25
            FST::NODE(1, FST::RELATION(';', 27)),          // 26

        
            FST::NODE(3, FST::RELATION(' ', 27), FST::RELATION('a', 14), FST::RELATION('e', 28)), // 27

       
            FST::NODE(1, FST::RELATION('n', 29)),          // 28
            FST::NODE(1, FST::RELATION('d', 30)),          // 29
            FST::NODE(1, FST::RELATION(';', 31)),          // 30

    
            FST::NODE()                                   // 31
        );

        cout << "Тест " << (i + 1) << ": Цепочка \"" << testStrings[i] << "\" - ";
        if (FST::execute(testFst))
            cout << "распознана" << endl;
        else
            cout << "не распознана" << endl;
    }

    return 0;
}