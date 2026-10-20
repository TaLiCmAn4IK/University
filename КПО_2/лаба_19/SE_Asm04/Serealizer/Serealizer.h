#pragma once
#include <fstream>
#include <iostream>
using namespace std;

class Serealizer
{
public:
    void Serialize(bool valBool, short valShort);

private:
    const char* FILENAME = "bin.bin";  
};