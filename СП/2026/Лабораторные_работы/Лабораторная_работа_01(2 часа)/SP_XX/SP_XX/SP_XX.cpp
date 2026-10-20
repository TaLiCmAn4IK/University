#include "pch.h"
#include "SP_XX.h"

namespace SP {
    int sum(int x, int y) { return x + y; }
    int sub(int x, int y) { return x - y; }
    int mul(int x, int y) { return x * y; }
    int div(int x, int y) {
        if (y == 0) return 0;
        return x / y;
    }
}
