#include "Call.h"
#include <iostream>
#include <string> 

using namespace std;

namespace Call {
    
    int __cdecl mul(int x, int y) { return x * y; }

    int __stdcall sub(int x, int y) { return x - y; }

    int __fastcall sum(int x, int y, int z, int a) { return x + y + z + a; }

    int main() {

        int rc_cdecl =  mul(2, 3);
        int rc_stdcall = sub(1, 2);
        int rc_fastcall = sum(5, 2, 1, 3);

        cout << rc_cdecl << endl;
        cout << rc_stdcall << endl;
        cout << rc_fastcall << endl;

        return 0;
    }
}

int main() {
    return Call::main();
}