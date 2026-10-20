#include <iostream>
using namespace std;
int main() {

    setlocale(LC_CTYPE, "Russian");
    unsigned int A, B;
    int p, n, q, m;
    char tmp[33];

    cout << "Введите число A: ";
    cin >> A;
    cout << "Введите число B: ";
    cin >> B;
    cout << "Введите позицию p (0 - индикатор младшего бита): ";
    cin >> p;
    cout << "Введите n (количество битов для установки в 0): ";
    cin >> n;
    cout << "Введите позицию q: ";
    cin >> q;
    cout << "Введите m (количество битов для замены): ";
    cin >> m;

    // Создание маски для обнуления n битов в A начиная с позиции p
    unsigned int maskA = ((1U << n) - 1) << p; // Создаем маску, где n правых битов равны 1
    A &= ~maskA; //Применяем побитовую операцию или с инвертированной маской, чтобы установить n битов в 0

    // Создание маски для извлечения m битов из B, начиная с позиции q
    unsigned int maskB = ((1U << m) - 1) << q; // Создаем маску для m битов
    unsigned int bitsToReplace = (B & maskB) >> q; // Извлекаем m битов из B

    // Размещение извлеченных битов в позиции p в A
    A |= (bitsToReplace << p); // 3) Заменяем m битов числа B в A начиная с позиции p
    _itoa_s(A , tmp, 2);
    cout << "Результат (A): " << tmp << endl;

    return 0;
}