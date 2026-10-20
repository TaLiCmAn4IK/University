#include <iostream>
#include <cmath> // Для математических функций
#include <iomanip> // Для форматирования вывода

void displayMenu() {
    setlocale(LC_CTYPE, "Russian");
    std::cout << "============================\n";
    std::cout << "     Инженерный калькулятор  \n";
    std::cout << "============================\n";
    std::cout << "Выберите операцию:\n";
    std::cout << "1. Синус\n";
    std::cout << "2. Косинус\n";
    std::cout << "3. Тангенс\n";
    std::cout << "4. Котангенс\n";
    std::cout << "5. Сложение\n";
    std::cout << "6. Вычитание\n";
    std::cout << "7. Умножение\n";
    std::cout << "8. Деление\n";
    std::cout << "============================\n";
}


int main() {
    setlocale(LC_CTYPE, "Russian");
    double angle, num1, num2; // Угол и числа для операций
    int choice; // Выбор операции

    displayMenu();
    std::cout << "Введите номер операции (1-8): ";
    std::cin >> choice;

    if (choice >= 1 && choice <= 4) {
        std::cout << "Введите угол в радианах: ";
        std::cin >> angle;
    }
    else if (choice >= 5 && choice <= 8) {
        std::cout << "Введите первое число: ";
        std::cin >> num1;
        std::cout << "Введите второе число: ";
        std::cin >> num2;
    }

    switch (choice) {
    case 1:
        std::cout << "Синус(" << angle << ") = " << std::setprecision(4) << std::fixed << sin(angle) << std::endl;
        break;
    case 2:
        std::cout << "Косинус(" << angle << ") = " << std::setprecision(4) << std::fixed << cos(angle) << std::endl;
        break;
    case 3:
        if (cos(angle) != 0) {
            std::cout << "Тангенс(" << angle << ") = " << std::setprecision(4) << std::fixed << tan(angle) << std::endl;
        }
        else {
            std::cout << "Тангенс не определен для угла " << angle << " (деление на ноль)." << std::endl;
        }
        break;
    case 4:
        if (sin(angle) != 0) {
            std::cout << "Котангенс(" << angle << ") = " << std::setprecision(4) << std::fixed << 1 / tan(angle) << std::endl;
        }
        else {
            std::cout << "Котангенс не определен для угла " << angle << " (деление на ноль)." << std::endl;
        }
        break;
    case 5:
        std::cout << "Результат: " << num1 << " + " << num2 << " = " << std::setprecision(4) << std::fixed << (num1 + double(num2)) << std::endl;
        break;
    case 6:
        std::cout << "Результат: " << num1 << " - " << num2 << " = " << std::setprecision(4) << std::fixed << (num1 - double(num2)) << std::endl;
        break;
    case 7:
        std::cout << "Результат: " << num1 << " * " << num2 << " = " << std::setprecision(4) << std::fixed << (num1 * double(num2)) << std::endl;
        break;
    case 8:
        if (num2 != 0) {
            std::cout << "Результат: " << num1 << " / " << num2 << " = " << std::setprecision(4) << std::fixed << (num1 / double(num2)) << std::endl;
        }
        else {
            std::cout << "Ошибка: деление на ноль." << std::endl;
        }
        break;
    default:
        std::cout << "Неверный выбор операции." << std::endl;
        break;
    }

    return 0;
}