#include <algorithm>
#include <iostream>
#include <ctime>
#include <iomanip>
#include <cstdlib>
#include "Levenshtein.h"
#include <Windows.h>

using namespace std;

char* GenerateRandomString(int size)
{
    char* str = (char*)malloc(sizeof(char) * size);
    for (int i = 0; i < size; i++) {
        str[i] = rand() % 26 + 'a';
    }
    return str;
}

int main()
{
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    char* s1 = GenerateRandomString(300);
    cout << "S1: " << endl;
    for (int i = 0; i < 300; i++) {
        if (i % 50 == 0)
        {
            cout << "\n";
        }
        cout << s1[i];
    }
    cout << endl << endl;

    srand(time(NULL) + 1);
    char* s2 = GenerateRandomString(250);
    cout << "S2: " << endl;
    for (int i = 0; i < 250; i++) {
        if (i % 50 == 0)
        {
            cout << "\n";
        }
        cout << s2[i];
    }
    cout << endl << endl;

    clock_t t1 = 0, t2 = 0, t3 = 0, t4 = 0;

    int s1_size[]{ 300 / 25, 300 / 20, 300 / 15, 300 / 10, 300 / 5, 300 / 2, 300 };
    int s2_size[]{ 250 / 25, 250 / 20, 250 / 15, 250 / 10, 250 / 5, 250 / 2, 250 };

    cout << "\n\n-- расстояние Левенштейна -----";
    cout << "\n\n--длина --- рекурсия -- дин.програм. ---\n";

    for (int i = 0; i < 7; i++)
    {
        t1 = clock();
        levenshtein_r(s1_size[i], s1, s2_size[i], s2);
        t2 = clock();

        t3 = clock();
        levenshtein(s1_size[i], s1, s2_size[i], s2);
        t4 = clock();

        cout << right << setw(2) << s1_size[i] << "/" << setw(2) << s2_size[i]
            << "        " << left << setw(10) << (t2 - t1)
            << "   " << setw(10) << (t4 - t3) << endl;
    }

    system("pause");
    return 0;
}