#include <iostream>
#include "SP_XX.h" 

#pragma comment(lib, "SP_XX.lib") // линкер автоматически привяжет SP_XX.lib

int main() {
    setlocale(LC_ALL, "Russian");

    int a = 12;
    int b = 4;

    std::cout << "=== Демонстрация работы SP_01.exe (Статический вызов) ===\n\n";
    // Добавили SP:: чтобы вызывать именно наши функции
    std::cout << "Вызов sum(" << a << ", " << b << ") = " << SP::sum(a, b) << "\n";
    std::cout << "Вызов sub(" << a << ", " << b << ") = " << SP::sub(a, b) << "\n";
    std::cout << "Вызов mul(" << a << ", " << b << ") = " << SP::mul(a, b) << "\n";
    std::cout << "Вызов div(" << a << ", " << b << ") = " << SP::div(a, b) << "\n\n";

    system("pause");
    return 0;
}
