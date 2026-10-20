#include <iostream>

int defaultparam(int a, int b, int c = 30, int d = 40, int e = 50, int f = 60, int g = 70) {
    return (a + b + c + d + e + f + g) / 7;
}

int main() {
    int result1 = defaultparam(1, 2);
    int result2 = defaultparam(1, 2, 3, 4, 5);
    int result3 = defaultparam(1, 2, 3, 4, 5, 6, 7);

    std::cout << result1 << std::endl;
    std::cout << result2 << std::endl;
    std::cout << result3 << std::endl;

    return 0;
}