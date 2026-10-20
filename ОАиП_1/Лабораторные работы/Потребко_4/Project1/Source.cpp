#include <iostream>

int main() {
    int number;

    std::cout << "Введите трехзначное число: ";
    std::cin >> number;

    // Проверка, что число трехзначное
    if (number < 100 || number > 999) {
        std::cout << "Ошибка: введите трехзначное число." << std::endl;
        return 1;
    }

    // Получение цифр числа
    int hundreds = number / 100;         // Сотни
    int tens = (number / 10) % 10;      // Десятки
    int units = number % 10;             // Единицы

    // Формирование реверсной записи
    int reversedNumber = units * 100 + tens * 10 + hundreds;

    std::cout << "Реверсная запись числа: " << reversedNumber << std::endl;

    return 0;
}