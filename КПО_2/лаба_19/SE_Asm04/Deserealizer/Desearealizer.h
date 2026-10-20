#pragma once
class Desearealizer
{
public:
    void Deserialize(bool* valBool, short* valShort);

private:
    const char* FILENAME = "../Serealizer/bin.bin";
};
