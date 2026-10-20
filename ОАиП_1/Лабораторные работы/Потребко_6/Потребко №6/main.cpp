#include <iostream>
int main()
{
	setlocale(LC_CTYPE, "Russian");
    float z, y, j, a = 2.4 * 10 * 10 * 10 * 10, b = 0.5, m = 8;
	for (int n = 0; n < 4; n++)
	{
		printf("¬ведите j ");
		scanf_s("%f", &j);
		y = (m - b) / (sin(a) - exp(a));
	    z = 3 * y + sqrt(a - 4 * j * b);
		printf("j = %5.2f\t", j);
		printf("z = %5.2f\n", z);
	}
}