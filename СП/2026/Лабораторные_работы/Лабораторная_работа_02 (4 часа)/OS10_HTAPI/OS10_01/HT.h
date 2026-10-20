#pragma once
#include <windows.h>
#include <time.h>

namespace HT
{

    struct HTHANDLE
    {
        HTHANDLE();
        HTHANDLE(int Capacity, int SecSnapshotInterval, int MaxKeyLength,
            int MaxPayloadLength, const char FileName[512]);

        int Capacity;
        int SecSnapshotInterval;
        int MaxKeyLength;
        int MaxPayloadLength;
        char FileName[512];
        HANDLE File;
        HANDLE FileMapping;
        LPVOID Addr;
        char LastErrorMessage[512];
        time_t lastsnaptime;
    };

    struct Element
    {
        Element();
        Element(const void* key, int keylength);
        Element(const void* key, int keylength, const void* payload, int payloadlength);
        Element(Element* oldelement, const void* newpayload, int newpayloadlength);

        const void* key;
        int keylength;
        const void* payload;
        int payloadlength;
    };

    HTHANDLE* Create(int Capacity, int SecSnapshotInterval, int MaxKeyLength,
        int MaxPayloadLength, const char FileName[512]);
    HTHANDLE* Open(const char FileName[512]);
    BOOL Snap(const HTHANDLE* hthandle);
    BOOL Insert(HTHANDLE* hthandle, Element* element);
    BOOL Delete(HTHANDLE* hthandle, Element* element);
    Element* Get(HTHANDLE* hthandle, Element* element);
    BOOL Update(HTHANDLE* hthandle, Element* element, const void* newpayload, int newpayloadlength);
    BOOL Close(HTHANDLE* hthandle);
    const char* GetLastError(const HTHANDLE* hthandle);
    void print(Element* element);

}