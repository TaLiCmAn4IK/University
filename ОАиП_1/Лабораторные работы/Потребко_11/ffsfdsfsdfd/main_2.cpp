#include <iostream> 
using namespace std;
void main()
{
	setlocale(LC_CTYPE, "Russian");
	int A;
	cin >> A;
	char tmp[33];
	_itoa_s(A, tmp, 2);
	cout << " Число А: " << tmp << endl;
	_itoa_s(A ^ (((1 << 5) - 1) << 3), tmp, 2);
	cout << " Результат: " << tmp << endl << endl;
}