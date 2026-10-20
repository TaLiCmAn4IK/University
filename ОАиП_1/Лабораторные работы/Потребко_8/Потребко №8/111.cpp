#include <iostream>
#include <vector> //динам. массив
using namespace std;

int main()
{
    setlocale(LC_ALL, "RUS");
    double t = 0.45;
    vector<int> x = { 7,2,5,7,6,11 };
    vector<double> x1 = { 1.1,6.2,3,-4,6,1 };
    double z = 0;
    double y = 0;
    double q = 0;
    q += t;
    for (int i = 0; i < x1.size(); i++) { //x.size() будет выполнять i столько раз сколько элементов в массиве x
        q += (x1[i] + 1) / x1[i];
    }
    cout << "q: " << q << endl;
    for (int i = 0; i < x.size(); i++) {
        if (x[i] > z) {
            z = x[i];
        }
    }
    cout << "z: " << z << endl;
    for (int i = 0; i < x.size() - 1; i++) {
        y += x[i] * x[i] * x[i];
    }
    y = y * t;
    cout << "y: " << y << endl;
}