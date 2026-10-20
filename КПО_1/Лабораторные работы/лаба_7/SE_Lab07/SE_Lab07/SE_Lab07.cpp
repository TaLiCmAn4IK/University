#include <iostream>
#include "Call.h"

int main() {
    int a = 2, b = 3, c = 4, d = 5;
    int x = 6, y = 7;

    std::cout << "cdecl(2, 3, 4) = " << cdecl(a, b, c) << std::endl;
    std::cout << "cstd(&6, 7) = " << cstd(&x, y) << std::endl;
    std::cout << "cfst(2, 3, 4, 5) = " << cfst(a, b, c, d) << std::endl;

    return 0;
}
