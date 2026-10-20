#include <iostream>
using namespace std;

void hanoi(int n, int from, int to, int aux, unsigned long long& moves_counter) { //n = кол. дисков для переноса, from = 1 , to = 2 or 3, aux = вспомогательный, ссылка на кол. перемещений
    if (n <= 0) return;            // ничего делать не надо
    if (n == 1) {                  // базовый случай: один диск
        cout << "Переместить диск 1 с " << from << " на " << to << " стержень\n";
        ++moves_counter;
        return;
    }
    // 1) Перенести n-1 дисков с from на aux (используя to как вспомогательный)
    hanoi(n - 1, from, aux, to, moves_counter);

    // 2) Переместить самый большой диск (номер n) с from на to
    cout << "Переместить диск " << n << " с " << from << " на " << to << " стержень\n";
    ++moves_counter;

    // 3) Перенести n-1 дисков с aux на to
    hanoi(n - 1, aux, to, from, moves_counter);
}

int main() {
    setlocale(LC_ALL, "Russian");
    int N, K; //N(количество дисков) и K(номер целевого стержня)
    cout << "Введите количество дисков N: ";
    if (!(cin >> N) || N < 0) {
        cerr << "Ошибка: N должно быть неотрицательным целым числом.\n";
        return 1;
    }

    cout << "Введите номер целевого стержня K (2 или 3): ";
    if (!(cin >> K) || (K != 2 && K != 3)) {
        cerr << "Ошибка: K должен быть равен 2 или 3.\n";
        return 1;
    }

    const int from = 1;
    const int to = K;
    const int aux = 6 - from - to; // 1+2+3 = 6, поэтому третий = 6 - from - to

    cout << "Пошаговое решение:\n";
    unsigned long long moves_counter = 0;
    hanoi(N, from, to, aux, moves_counter);

    cout << "Всего ходов (посчитано при выводе): " << moves_counter << '\n';
    return 0;
}
