#include <iostream>
#include <vector>
using namespace std;

int main() {
    setlocale(LC_ALL, "Russian");
    cout << "Введите N: ";
    long long N;
    cin >> N;

    long long l = 1, r = N;
    bool first = true;

    while (true) {
        long long m;
        if (first) {
            m = (l + r) / 2;
            first = false;
        }
        else {
            m = (l + r + 1) / 2;
        }

        cout << "\nПредполагаю число: " << m
            << "\n1 - Мало\n2 - Много\n3 - Угадал\n";

        int a;
        cin >> a;

        if (a == 1) {
            l = m;
        }
        else if (a == 2) {
            r = m - 1;
        }
        else if (a == 3) {
            long long count = 0;
            {
                long long t = N;
                while (true) {
                    ++count;
                    if (t == 1) break;
                    t = (t + 1) / 2;
                }
            }
            // Выделяем вектор цепочки на нужное кол-во элементов
            vector<long long> chain(count);
            {
                long long t = N;
                for (long long i = 0; i < count; ++i) {
                    chain[i] = t;
                    if (t == 1) break;
                    t = (t + 1) / 2;
                }
            }
            cout << "Максимальное количество шагов: "
                << (count - 1) << "\nЦепочка делений:\n";
            for (long long i = 0; i < count; ++i) {
                cout << chain[i] << "\n";
            }
            break;
        }
        else {
            cout << "Неправильный ввод.\n";
        }

        // Если границы пересеклись, перезапускаем
        if (l > r) {
            cout << "Ошибка. Сброс.\n";
            l = 1;
            r = N;
            first = true;
        }
    }
    return 0;
}
