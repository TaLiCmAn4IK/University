#include <iostream>
#include <iomanip>

int main()
{
	setlocale(LC_CTYPE, "Russian");
	using namespace std;
	char c, probel; probel = ' ';
	cout << "Введите символ "; cin >> c;
	cout << setw(23) << setfill(probel) << probel;
	cout << setw(1) << setfill(c) << c << endl;
	cout << setw(22) << setfill(probel) << probel; 
	cout << setw(3) << setfill(c) << c << endl; 
	cout << setw(21) << setfill(probel) << probel;
	cout << setw(5) << setfill(c) << c << endl;
	cout << setw(20) << setfill(probel) << probel;
	cout << setw(7) << setfill(c) << c << endl;
	cout << setw(19) << setfill(probel) << probel;
	cout << setw(9) << setfill(c) << c << endl;
	cout << setw(18) << setfill(probel) << probel;
	cout << setw(11) << setfill(c) << c << endl;
	cout << setw(17) << setfill(probel) << probel;
	cout << setw(13) << setfill(c) << c << endl;
	cout << setw(16) << setfill(probel) << probel;
	cout << setw(15) << setfill(c) << c << endl;
	cout << setw(15) << setfill(probel) << probel;
	cout << setw(17) << setfill(c) << c << endl;
	cout << setw(14) << setfill(probel) << probel;
	cout << setw(19) << setfill(c) << c << endl;

	cout << setw(13) << setfill(probel) << probel;
	cout << setw(21) << setfill(c) << c << endl;
	cout << setw(14) << setfill(probel) << probel;
	cout << setw(19) << setfill(c) << c << endl;
	cout << setw(15) << setfill(probel) << probel;
	cout << setw(17) << setfill(c) << c << endl;

	cout << setw(16) << setfill(probel) << probel;
	cout << setw(15) << setfill(c) << c << endl;
	cout << setw(17) << setfill(probel) << probel;
	cout << setw(13) << setfill(c) << c << endl;
	cout << setw(18) << setfill(probel) << probel;
	cout << setw(11) << setfill(c) << c << endl;
	cout << setw(19) << setfill(probel) << probel;
	cout << setw(9) << setfill(c) << c << endl;
	cout << setw(20) << setfill(probel) << probel;
	cout << setw(7) << setfill(c) << c << endl;
	cout << setw(21) << setfill(probel) << probel;
	cout << setw(5) << setfill(c) << c << endl;
	cout << setw(22) << setfill(probel) << probel;
	cout << setw(3) << setfill(c) << c << endl;
	cout << setw(23) << setfill(probel) << probel;
	cout << setw(1) << setfill(c) << c << endl;
}