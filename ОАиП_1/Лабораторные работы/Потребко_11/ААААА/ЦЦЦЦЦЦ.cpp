#include <iostream>
using namespace std;

void setBitsToZeroRight(int    number, int position) {
	setlocale(LC_CTYPE, "Russian");
	// Создаем маску, которая обнуляет биты вправо от заданной позиции
	int mask = (1 << position) - 1; // Создаем маску с единицами справа от позиции
	number = mask; // Обнуляем биты вправо от позиции
}


	int main() {
		int number, position;

		// Ввод числа
		std::cout << "Введите число: ";
		std::cin >> number;

		// Ввод позиции
		std::cout << "Введите позицию (0 - младший бит): ";
		std::cin >> position;

		// Обнуляем биты вправо от заданной позиции
		setBitsToZeroRight(number, position);

		// Вывод результата
		std::cout << "Результат после обнуления битов вправо от позиции " << position << ": " << number << std::endl;

		return 0;
	}
}