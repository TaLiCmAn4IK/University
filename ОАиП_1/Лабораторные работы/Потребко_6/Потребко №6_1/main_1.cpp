#include <iostream>
#include <vector>
using namespace std;
int main()
{
	setlocale(LC_CTYPE, "Russian");
	double b = 0.5;
	int a = 2.4 * 10 * 10 * 10 * 10;
	int m = 8;
	vector <double> j = { 1.7, 2.9, 8 };
	for (int i = 0; i < 2.9; i++)
	{
		printf("¬ведите j ");
		scanf_s("%f", &j);
		double y = (m - b) / (sin(a) - exp(a));
		double z = 3 * y + sqrt(a - 4 * j[i] * b);
		printf("j = %5.2f\t", j);
		printf("z = %5.2f\n", z);
	}
}