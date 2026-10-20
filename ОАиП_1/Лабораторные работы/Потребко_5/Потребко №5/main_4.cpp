#include <iostream>

int main()
{
	setlocale(LC_CTYPE, "Russian");
	using namespace std;
	double min, max, a, b, r, x, y, z, t;
	cout << "Введите через пробел значения x, y и z: ";
	cin >> x >> y >> z;
	a = x + y + z / 2;
	b = x * y * z;
	r = x * y;
	if (a > b)
		min = a;
	else
		min = b;
	if (r > y)
		max = r;
	else
		max = y;
	t = max * min;
    cout << t << endl;
}
