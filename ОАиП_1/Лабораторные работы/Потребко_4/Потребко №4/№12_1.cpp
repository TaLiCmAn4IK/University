#include <iostream>
#include <iomanip>

int main()
{
	setlocale(LC_CTYPE, "Russian");
	using namespace std;
	char c, probel; probel = ' ';
	cout << "Введите символ "; cin >> c;
	cout << setw(23) << setfill(probel) << probel;
	cout << setw(2) << setfill(c) << c << endl;
	cout << setw(22) << setfill(probel) << probel; //количество пробелов до символов
	cout << setw(4) << setfill(c) << c << endl; //колличество символов
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
	
	cout << setw(2) << setfill(probel) << probel; 
	cout << setw(44) << setfill(c) << c << endl; 
	cout << setw(1) << setfill(probel) << probel;
	cout << setw(46) << setfill(c) << c << endl;
	cout << setw(2) << setfill(probel) << probel;
	cout << setw(44) << setfill(c) << c << endl;

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



