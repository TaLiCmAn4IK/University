#include <iostream>
#include <cmath>
using namespace std;
int main()
{
	float z, y, j = 2, a = 2.4 * 10 * 10 * 10 * 10, b = 0.5, m = 8;
	while (j < 3.5)
	{
		y = (m - b) / (sin(a) - exp(a));
		z = 3 * y + sqrt(a - 4 * j * b);
		cout << "j=" << j << "\t";
		cout << " z=" << z << endl;
		j = j + 0.5;
	}
}
