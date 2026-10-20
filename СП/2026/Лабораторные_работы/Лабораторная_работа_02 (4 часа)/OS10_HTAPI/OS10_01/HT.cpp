#include "HT.h"
#include <thread>
#include <atomic>
#include <chrono>
#include <map>
#include <mutex>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>

namespace HT
{

#pragma pack(push, 1)
    struct HTHeader
    {
        int Capacity;
        int SecSnapshotInterval;
        int MaxKeyLength;
        int MaxPayloadLength;
        int Count;
        time_t LastSnapTime;
        int SnapInProgress;
    };

    struct SlotHeader
    {
        int keylength;
        int payloadlength;
    };
#pragma pack(pop)

    static std::mutex g_mapMutex;
    static std::map<HTHANDLE*, std::atomic<bool>*> g_stops;
    static std::map<HTHANDLE*, std::thread*> g_threads;

    static int SlotStride(HTHANDLE* ht)
    {
        return (int)sizeof(SlotHeader) + ht->MaxKeyLength + ht->MaxPayloadLength;
    }

    static char* SlotPtr(HTHANDLE* ht, int index)
    {
        return (char*)ht->Addr + sizeof(HTHeader) + (size_t)index * SlotStride(ht);
    }

    static unsigned int HashKey(const void* key, int keylength)
    {
        const unsigned char* p = (const unsigned char*)key;
        unsigned int h = 2166136261u;
        for (int i = 0; i < keylength; i++)
        {
            h ^= p[i];
            h *= 16777619u;
        }
        return h;
    }

    static void SetError(HTHANDLE* ht, const char* msg)
    {
        if (!ht) return;
        strncpy_s(ht->LastErrorMessage, 512, msg, 511);
        ht->LastErrorMessage[511] = 0;
    }

    static void StartSnapshotThread(HTHANDLE* ht)
    {
        if (ht->SecSnapshotInterval <= 0) return;

        std::atomic<bool>* stop = new std::atomic<bool>(false);
        std::thread* t = new std::thread([ht, stop]()
            {
                int interval = ht->SecSnapshotInterval;
                while (!stop->load())
                {
                    for (int i = 0; i < interval * 10; i++)
                    {
                        if (stop->load()) return;
                        std::this_thread::sleep_for(std::chrono::milliseconds(100));
                    }
                    if (stop->load()) return;
                    Snap(ht);
                }
            });

        std::lock_guard<std::mutex> lock(g_mapMutex);
        g_stops[ht] = stop;
        g_threads[ht] = t;
    }

    static void StopSnapshotThread(HTHANDLE* ht)
    {
        std::thread* t = nullptr;
        std::atomic<bool>* stop = nullptr;

        {
            std::lock_guard<std::mutex> lock(g_mapMutex);
            auto itS = g_stops.find(ht);
            if (itS != g_stops.end())
            {
                stop = itS->second;
                stop->store(true);
                g_stops.erase(itS);
            }
            auto itT = g_threads.find(ht);
            if (itT != g_threads.end())
            {
                t = itT->second;
                g_threads.erase(itT);
            }
        }

        if (t)
        {
            if (t->joinable()) t->join();
            delete t;
        }
        if (stop) delete stop;
    }

    HTHANDLE* Create(int Capacity, int SecSnapshotInterval, int MaxKeyLength,
        int MaxPayloadLength, const char FileName[512])
    {
        if (Capacity <= 0 || MaxKeyLength <= 0 || MaxPayloadLength <= 0)
            return nullptr;

        HTHANDLE* ht = new HTHANDLE(Capacity, SecSnapshotInterval, MaxKeyLength,
            MaxPayloadLength, FileName);

        int stride = (int)sizeof(SlotHeader) + MaxKeyLength + MaxPayloadLength;
        DWORD size = (DWORD)sizeof(HTHeader) + (DWORD)Capacity * stride;

        ht->File = CreateFileA(FileName, GENERIC_READ | GENERIC_WRITE, 0, NULL,
            CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
        if (ht->File == INVALID_HANDLE_VALUE)
        {
            SetError(ht, "Create: cannot create file");
            ht->File = 0;
            delete ht;
            return nullptr;
        }

        LONG high = 0;
        SetFilePointer(ht->File, size, &high, FILE_BEGIN);
        SetEndOfFile(ht->File);

        ht->FileMapping = CreateFileMappingA(ht->File, NULL, PAGE_READWRITE, 0, size, NULL);
        if (!ht->FileMapping)
        {
            SetError(ht, "Create: cannot create mapping");
            CloseHandle(ht->File);
            ht->File = 0;
            delete ht;
            return nullptr;
        }

        ht->Addr = MapViewOfFile(ht->FileMapping, FILE_MAP_ALL_ACCESS, 0, 0, size);
        if (!ht->Addr)
        {
            SetError(ht, "Create: cannot map view");
            CloseHandle(ht->FileMapping);
            CloseHandle(ht->File);
            ht->FileMapping = 0;
            ht->File = 0;
            delete ht;
            return nullptr;
        }

        memset(ht->Addr, 0, size);
        HTHeader* h = (HTHeader*)ht->Addr;
        h->Capacity = Capacity;
        h->SecSnapshotInterval = SecSnapshotInterval;
        h->MaxKeyLength = MaxKeyLength;
        h->MaxPayloadLength = MaxPayloadLength;
        h->Count = 0;
        h->LastSnapTime = time(nullptr);
        h->SnapInProgress = 0;
        ht->lastsnaptime = h->LastSnapTime;

        StartSnapshotThread(ht);
        return ht;
    }

    HTHANDLE* Open(const char FileName[512])
    {
        HANDLE f = CreateFileA(FileName, GENERIC_READ | GENERIC_WRITE, 0, NULL,
            OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
        if (f == INVALID_HANDLE_VALUE)
            return nullptr;

        DWORD fileSize = GetFileSize(f, NULL);
        if (fileSize < sizeof(HTHeader))
        {
            CloseHandle(f);
            return nullptr;
        }

        HANDLE fm = CreateFileMappingA(f, NULL, PAGE_READWRITE, 0, 0, NULL);
        if (!fm)
        {
            CloseHandle(f);
            return nullptr;
        }

        LPVOID addr = MapViewOfFile(fm, FILE_MAP_ALL_ACCESS, 0, 0, 0);
        if (!addr)
        {
            CloseHandle(fm);
            CloseHandle(f);
            return nullptr;
        }

        HTHeader* h = (HTHeader*)addr;
        HTHANDLE* ht = new HTHANDLE(h->Capacity, h->SecSnapshotInterval,
            h->MaxKeyLength, h->MaxPayloadLength, FileName);
        ht->File = f;
        ht->FileMapping = fm;
        ht->Addr = addr;
        ht->lastsnaptime = h->LastSnapTime;

        StartSnapshotThread(ht);
        return ht;
    }

    BOOL Snap(const HTHANDLE* hthandle)
    {
        HTHANDLE* ht = const_cast<HTHANDLE*>(hthandle);
        if (!ht || !ht->Addr) return FALSE;

        HTHeader* h = (HTHeader*)ht->Addr;
        time_t now = time(nullptr);
        h->LastSnapTime = now;
        ht->lastsnaptime = now;

        FlushViewOfFile(ht->Addr, 0);
        FlushFileBuffers(ht->File);
        return TRUE;
    }

    BOOL Insert(HTHANDLE* ht, Element* element)
    {
        if (!ht || !ht->Addr || !element || !element->key)
        {
            SetError(ht, "Insert: bad args");
            return FALSE;
        }
        if (element->keylength <= 0 || element->keylength > ht->MaxKeyLength)
        {
            SetError(ht, "Insert: bad key length");
            return FALSE;
        }
        if (element->payloadlength < 0 || element->payloadlength > ht->MaxPayloadLength)
        {
            SetError(ht, "Insert: bad payload length");
            return FALSE;
        }

        HTHeader* h = (HTHeader*)ht->Addr;
        unsigned int start = HashKey(element->key, element->keylength) % h->Capacity;
        int firstTomb = -1;

        for (int i = 0; i < h->Capacity; i++)
        {
            unsigned int cur = (start + i) % h->Capacity;
            char* slot = SlotPtr(ht, cur);
            SlotHeader* sh = (SlotHeader*)slot;

            if (sh->keylength == 0)
            {
                int target = (firstTomb != -1) ? firstTomb : (int)cur;
                char* tslot = SlotPtr(ht, target);
                SlotHeader* tsh = (SlotHeader*)tslot;
                tsh->keylength = element->keylength;
                tsh->payloadlength = element->payloadlength;
                memcpy(tslot + sizeof(SlotHeader), element->key, element->keylength);
                if (element->payloadlength > 0)
                    memcpy(tslot + sizeof(SlotHeader) + ht->MaxKeyLength,
                        element->payload, element->payloadlength);
                h->Count++;
                return TRUE;
            }

            if (sh->keylength == -1)
            {
                if (firstTomb == -1) firstTomb = (int)cur;
                continue;
            }

            if (sh->keylength == element->keylength &&
                memcmp(slot + sizeof(SlotHeader), element->key, element->keylength) == 0)
            {
                SetError(ht, "Insert: key already exists");
                return FALSE;
            }
        }

        SetError(ht, "Insert: table full");
        return FALSE;
    }

    BOOL Delete(HTHANDLE* ht, Element* element)
    {
        if (!ht || !ht->Addr || !element || !element->key)
        {
            SetError(ht, "Delete: bad args");
            return FALSE;
        }

        HTHeader* h = (HTHeader*)ht->Addr;
        unsigned int start = HashKey(element->key, element->keylength) % h->Capacity;

        for (int i = 0; i < h->Capacity; i++)
        {
            unsigned int cur = (start + i) % h->Capacity;
            char* slot = SlotPtr(ht, cur);
            SlotHeader* sh = (SlotHeader*)slot;

            if (sh->keylength == 0) break;

            if (sh->keylength == element->keylength &&
                memcmp(slot + sizeof(SlotHeader), element->key, element->keylength) == 0)
            {
                sh->keylength = -1;
                sh->payloadlength = 0;
                h->Count--;
                return TRUE;
            }
        }

        SetError(ht, "Delete: not found");
        return FALSE;
    }

    Element* Get(HTHANDLE* ht, Element* element)
    {
        if (!ht || !ht->Addr || !element || !element->key)
        {
            SetError(ht, "Get: bad args");
            return nullptr;
        }

        HTHeader* h = (HTHeader*)ht->Addr;
        unsigned int start = HashKey(element->key, element->keylength) % h->Capacity;

        for (int i = 0; i < h->Capacity; i++)
        {
            unsigned int cur = (start + i) % h->Capacity;
            char* slot = SlotPtr(ht, cur);
            SlotHeader* sh = (SlotHeader*)slot;

            if (sh->keylength == 0) break;

            if (sh->keylength == element->keylength &&
                memcmp(slot + sizeof(SlotHeader), element->key, element->keylength) == 0)
            {
                void* keyBuf = malloc(sh->keylength);
                memcpy(keyBuf, slot + sizeof(SlotHeader), sh->keylength);

                void* payBuf = nullptr;
                if (sh->payloadlength > 0)
                {
                    payBuf = malloc(sh->payloadlength);
                    memcpy(payBuf, slot + sizeof(SlotHeader) + ht->MaxKeyLength, sh->payloadlength);
                }

                return new Element(keyBuf, sh->keylength, payBuf, sh->payloadlength);
            }
        }

        SetError(ht, "Get: not found");
        return nullptr;
    }

    BOOL Update(HTHANDLE* ht, Element* element, const void* newpayload, int newpayloadlength)
    {
        if (!ht || !ht->Addr || !element || !element->key)
        {
            SetError(ht, "Update: bad args");
            return FALSE;
        }
        if (newpayloadlength < 0 || newpayloadlength > ht->MaxPayloadLength)
        {
            SetError(ht, "Update: bad payload length");
            return FALSE;
        }

        HTHeader* h = (HTHeader*)ht->Addr;
        unsigned int start = HashKey(element->key, element->keylength) % h->Capacity;

        for (int i = 0; i < h->Capacity; i++)
        {
            unsigned int cur = (start + i) % h->Capacity;
            char* slot = SlotPtr(ht, cur);
            SlotHeader* sh = (SlotHeader*)slot;

            if (sh->keylength == 0) break;

            if (sh->keylength == element->keylength &&
                memcmp(slot + sizeof(SlotHeader), element->key, element->keylength) == 0)
            {
                sh->payloadlength = newpayloadlength;
                if (newpayloadlength > 0)
                    memcpy(slot + sizeof(SlotHeader) + ht->MaxKeyLength,
                        newpayload, newpayloadlength);
                return TRUE;
            }
        }

        SetError(ht, "Update: not found");
        return FALSE;
    }

    BOOL Close(HTHANDLE* ht)
    {
        if (!ht) return FALSE;

        StopSnapshotThread(ht);
        Snap(ht);

        if (ht->Addr)
        {
            UnmapViewOfFile(ht->Addr);
            ht->Addr = nullptr;
        }
        if (ht->FileMapping)
        {
            CloseHandle(ht->FileMapping);
            ht->FileMapping = 0;
        }
        if (ht->File)
        {
            CloseHandle(ht->File);
            ht->File = 0;
        }

        delete ht;
        return TRUE;
    }

    const char* GetLastError(const HTHANDLE* hthandle)
    {
        if (!hthandle) return "no handle";
        return hthandle->LastErrorMessage;
    }

    void print(Element* element)
    {
        if (!element) return;
        if (element->key)
            printf("key: %.*s\n", element->keylength, (const char*)element->key);
        if (element->payload)
            printf("payload: %.*s\n", element->payloadlength, (const char*)element->payload);
    }

}