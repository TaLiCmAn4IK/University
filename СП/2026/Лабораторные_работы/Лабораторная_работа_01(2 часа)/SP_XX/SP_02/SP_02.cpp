#include <iostream>
#include <windows.h> 

// Определяем тип указателя на функцию
typedef int (*FuncType)(int, int);

int main() {
    setlocale(LC_ALL, "Russian");

    std::cout << "=== Демонстрация работы SP_02.exe (Динамический вызов) ===\n\n";

    HMODULE hDll = LoadLibrary(L"SP_XX.dll");

    if (hDll == NULL) {
        std::cout << "Ошибка: Не удалось загрузить библиотеку SP_XX.dll!\n";
        std::cout << "Убедитесь, что файл SP_XX.dll лежит рядом с SP_02.exe\n";
        system("pause");
        return 1;
    }

    // Получаем адреса функций из DLL по их именам
    FuncType sumFunc = (FuncType)GetProcAddress(hDll, "sum");
    FuncType subFunc = (FuncType)GetProcAddress(hDll, "sub");
    FuncType mulFunc = (FuncType)GetProcAddress(hDll, "mul");
    FuncType divFunc = (FuncType)GetProcAddress(hDll, "div");

    if (!sumFunc || !subFunc || !mulFunc || !divFunc) {
        std::cout << "Ошибка: Не удалось найти одну или несколько функций в DLL!\n";
        FreeLibrary(hDll);
        system("pause");
        return 1;
    }

    int a = 12;
    int b = 4;

    // Вызываем функции через полученные указатели
    std::cout << "Вызов динамически sum(" << a << ", " << b << ") = " << sumFunc(a, b) << "\n";
    std::cout << "Вызов динамически sub(" << a << ", " << b << ") = " << subFunc(a, b) << "\n";
    std::cout << "Вызов динамически mul(" << a << ", " << b << ") = " << mulFunc(a, b) << "\n";
    std::cout << "Вызов динамически div(" << a << ", " << b << ") = " << divFunc(a, b) << "\n\n";

    // Выгружаем библиотеку из памяти, когда она больше не нужна
    FreeLibrary(hDll);

    system("pause");
    return 0;
}
