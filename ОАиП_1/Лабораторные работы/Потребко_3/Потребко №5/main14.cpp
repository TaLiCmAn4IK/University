#include <iostream>
#include <cmath>

int main()
{
	double v, w, s, j = 4, b = -8.1, a = 1.5, t = 4e-8;
	s = sqrt(t * a / t + 1) + 4 * exp(2 * b);
	w = s * a / (1 + 0.1 * a);
	v = s + j * sqrt(pow(a, 2) + pow(b, 2));
	std::cout << "w= " << w;
	std::cout << " " << "s= " << s;
	std::cout << " " << "v= " << v;
}