#include <iostream>
using namespace std;

int main()
{
    setlocale(LC_CTYPE, "Russian");
    int k;
    puts("Привет, давай сегодня выйдем погулять.(1-Привет, давай. , 2-Привет, извини сегодня не могу.)");
    cin >> k;
    switch (k)
    {
    case 1: { puts("Во сколько тебе удобно?(1-Утром , 2-Днём, 3-Вечером)");
             cin >> k;
             switch (k)
             {
             case 1: puts("Слишком рано."); break;
             case 2: puts("Хорошо."); break;
             case 3: puts("Слишком поздно"); break;
                   break;
             }
             break;
    }
        case 2: puts("Я заболел и сижу дома."); break;
        default: puts("Некорректный вариант"); break;
        }
        return 0;
}