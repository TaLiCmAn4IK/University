#include <iostream>
#include <vector>
using namespace std;
int main()
{
	setlocale(LC_CTYPE, "Russian");
	double b = 0.5;
	int a = 2.4 * 10 * 10 * 10 * 10;
    vector<double> m = { 0.4, -1, 1.9 };
	for (double j = 2.0; j < 3.5; j += 0.5){
		for (int i = 0; i < 3; i++){
			double y = (m[i] - b) / (sin(a) - exp(a));
		    double z = 3 * y + sqrt(a - 4 * j * b);
		    cout << "j = " << j << ", m = " << m[i] << ", y = " << y << ", z = " << z << endl;
		}
	}   
}
