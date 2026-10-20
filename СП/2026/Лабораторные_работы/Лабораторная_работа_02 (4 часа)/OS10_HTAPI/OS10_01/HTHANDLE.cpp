#include "HT.h"
#include <string.h>

namespace HT
{

    HTHANDLE::HTHANDLE()
    {
        Capacity = 0;
        SecSnapshotInterval = 0;
        MaxKeyLength = 0;
        MaxPayloadLength = 0;
        memset(FileName, 0, 512);
        File = 0;
        FileMapping = 0;
        Addr = nullptr;
        memset(LastErrorMessage, 0, 512);
        lastsnaptime = 0;
    }

    HTHANDLE::HTHANDLE(int Capacity, int SecSnapshotInterval, int MaxKeyLength,
        int MaxPayloadLength, const char FileName[512])
    {
        this->Capacity = Capacity;
        this->SecSnapshotInterval = SecSnapshotInterval;
        this->MaxKeyLength = MaxKeyLength;
        this->MaxPayloadLength = MaxPayloadLength;
        strncpy_s(this->FileName, 512, FileName, 511);
        this->FileName[511] = 0;
        File = 0;
        FileMapping = 0;
        Addr = nullptr;
        memset(LastErrorMessage, 0, 512);
        lastsnaptime = 0;
    }

}