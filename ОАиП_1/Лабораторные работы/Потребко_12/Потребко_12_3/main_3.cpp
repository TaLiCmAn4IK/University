#include <iostream>
#include <vector>
int main()
{
    using namespace std;
    setlocale(LC_CTYPE, "Russian");
    srand((unsigned)time(NULL));
    int i;
    int size;
    cout << "Введите размер массива: ";
    cin >> size;

    vector<int> MAX_SIZE(size);
    cout << "Элементы массива: " << endl;
    for (i = 0; i < size; i++)
    {
        MAX_SIZE[i] = rand() % 10;
        cout << MAX_SIZE[i] << endl;
    }
    int count = 0;
    for (int i = 0; i < size; i++) {
        int j;
        for (j = i - 1; j >= 0; j--)
            if (MAX_SIZE[j] == MAX_SIZE[i]) break;
        if (j < 0) count++;
    }
    cout << "Разлчных элементов: " << count << endl;
    return 0;
}