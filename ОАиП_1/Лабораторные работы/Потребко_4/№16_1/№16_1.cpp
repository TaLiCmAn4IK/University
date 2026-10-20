#include <iostream>
#include <iomanip>

int main()
{
	setlocale(LC_CTYPE, "Russian");
	using namespace std;
	char c, probel; probel = ' ';
	cout << "Введите символ "; cin >> c;
	cout << setw(23) << setfill(probel) << probel;
	cout << setw(3) << setfill(c) << c << endl;
	cout << setw(20) << setfill(probel) << probel; //количество пробелов до символов
	cout << setw(9) << setfill(c) << c << endl; //колличество символов
	cout << setw(18) << setfill(probel) << probel;
	cout << setw(13) << setfill(c) << c << endl;
	cout << setw(16) << setfill(probel) << probel;
	cout << setw(17) << setfill(c) << c << endl;
	cout << setw(14) << setfill(probel) << probel;
	cout << setw(21) << setfill(c) << c << endl;
	cout << setw(12) << setfill(probel) << probel;
	cout << setw(25) << setfill(c) << c << endl;
	cout << setw(10) << setfill(probel) << probel;
	cout << setw(29) << setfill(c) << c << endl;
	cout << setw(8) << setfill(probel) << probel;
	cout << setw(33) << setfill(c) << c << endl;
	cout << setw(6) << setfill(probel) << probel;
	cout << setw(37) << setfill(c) << c << endl;
	cout << setw(4) << setfill(probel) << probel;
	cout << setw(41) << setfill(c) << c << endl;

	cout << setw(2) << setfill(probel) << probel;
	cout << setw(45) << setfill(c) << c << endl;
	cout << setw(1) << setfill(probel) << probel;
	cout << setw(47) << setfill(c) << c << endl;
	cout << setw(2) << setfill(probel) << probel;
	cout << setw(45) << setfill(c) << c << endl;

	cout << setw(21) << setfill(probel) << probel;
	cout << setw(6) << setfill(c) << c << endl;
	cout << setw(21) << setfill(probel) << probel;
	cout << setw(6) << setfill(c) << c << endl;
	cout << setw(21) << setfill(probel) << probel;
	cout << setw(6) << setfill(c) << c << endl;
	cout << setw(21) << setfill(probel) << probel;
	cout << setw(6) << setfill(c) << c << endl;
	cout << setw(21) << setfill(probel) << probel;
	cout << setw(6) << setfill(c) << c << endl;
	cout << setw(21) << setfill(probel) << probel;
	cout << setw(6) << setfill(c) << c << endl;
	cout << setw(21) << setfill(probel) << probel;
	cout << setw(6) << setfill(c) << c << endl;
	cout << setw(21) << setfill(probel) << probel;
	cout << setw(6) << setfill(c) << c << endl;
	cout << setw(22) << setfill(probel) << probel;
	cout << setw(4) << setfill(c) << c << endl;
	cout << setw(23) << setfill(probel) << probel;
	cout << setw(2) << setfill(c) << c << endl;
}