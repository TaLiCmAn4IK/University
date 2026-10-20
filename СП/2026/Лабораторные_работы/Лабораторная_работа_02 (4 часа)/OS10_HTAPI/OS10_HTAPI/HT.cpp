#include "pch.h" 
#include "HT.h"
#include <cstring>
#include <ctime>
#include <iostream>
// вся реализация API (CreateFile, CreateFileMapping, MapViewOfFile, UnmapViewOfFile, FlushViewOfFile, CloseHandle)
// --- ШАГ 1: Внутренняя структура ячейки в памяти файла ---
#pragma pack(push, 1)
struct HT_Cell {
    bool is_exists;       // true - ячейка занята, false - пустая
    int key_length;       // реальная длина ключа в этой ячейке
    int payload_length;   // реальная длина данных в этой ячейке
};
#pragma pack(pop)

namespace HT {

    // --- Вспомогательная хэш-функция DJB2 для строк и бинарных данных ---
    unsigned long long GetHash(const void* key, int keylength) {
        unsigned long long hash = 5381;
        const unsigned char* bytes = (const unsigned char*)key;
        for (int i = 0; i < keylength; i++) {
            hash = ((hash << 5) + hash) + bytes[i];
        }
        return hash;
    }

    // --- Вспомогательная функция поиска ячейки по ключу (Открытая адресация) ---
    int FindCellIndex(const HTHANDLE* ht, const void* key, int keylength, bool find_empty) {
        int cell_size = sizeof(HT_Cell) + ht->MaxKeyLength + ht->MaxPayloadLength;
        char* cells_start = (char*)ht->Addr + (sizeof(int) * 4);

        unsigned long long hash = GetHash(key, keylength);
        int initial_index = hash % ht->Capacity;
        int current_index = initial_index;

        do {
            HT_Cell* cell = (HT_Cell*)(cells_start + current_index * cell_size);

            if (!cell->is_exists) {
                if (find_empty) return current_index;
                return -1;
            }

            if (cell->key_length == keylength) {
                const char* cell_key = (const char*)cell + sizeof(HT_Cell);
                if (memcmp(cell_key, key, keylength) == 0) {
                    if (find_empty) return -1;
                    return current_index;
                }
            }

            current_index = (current_index + 1) % ht->Capacity;
        } while (current_index != initial_index);

        return -1;
    }

    // --- ШАГ 2: Реализация конструкторов ---

    HTHANDLE::HTHANDLE() {
        this->Capacity = 0;
        this->SecSnapshotInterval = 0;
        this->MaxKeyLength = 0;
        this->MaxPayloadLength = 0;
        ZeroMemory(this->FileName, 512);
        this->File = INVALID_HANDLE_VALUE;
        this->FileMapping = NULL;
        this->Addr = nullptr;
        ZeroMemory(this->LastErrorMessage, 512);
        this->lastsnaptime = 0;
    }

    HTHANDLE::HTHANDLE(int Capacity, int SecSnapshotInterval, int MaxKeyLength, int MaxPayloadLength, const char FileName[512]) {
        this->Capacity = Capacity;
        this->SecSnapshotInterval = SecSnapshotInterval;
        this->MaxKeyLength = MaxKeyLength;
        this->MaxPayloadLength = MaxPayloadLength;
        strcpy_s(this->FileName, FileName);
        this->File = INVALID_HANDLE_VALUE;
        this->FileMapping = NULL;
        this->Addr = nullptr;
        ZeroMemory(this->LastErrorMessage, 512);
        this->lastsnaptime = std::time(nullptr);
    }

    Element::Element() {
        this->key = nullptr;
        this->keylength = 0;
        this->payload = nullptr;
        this->payloadlength = 0;
    }

    Element::Element(const void* key, int keylength) {
        this->key = key;
        this->keylength = keylength;
        this->payload = nullptr;
        this->payloadlength = 0;
    }

    Element::Element(const void* key, int keylength, const void* payload, int payloadlength) {
        this->key = key;
        this->keylength = keylength;
        this->payload = payload;
        this->payloadlength = payloadlength;
    }

    Element::Element(Element* oldelement, const void* newpayload, int newpayloadlength) {
        this->key = oldelement->key;
        this->keylength = oldelement->keylength;
        this->payload = newpayload;
        this->payloadlength = newpayloadlength;
    }

    // --- ШАГ 3: Запись и обработка сообщений об ошибках ---

    void SetErrorMessage(HTHANDLE* ht, const char* msg) {
        if (ht) {
            strcpy_s(ht->LastErrorMessage, msg);
        }
    }

    char* GetLastError(HTHANDLE* ht) {
        return ht ? ht->LastErrorMessage : nullptr;
    }

    // --- ШАГ 4: Функция создания нового хранилища ---

    HTHANDLE* Create(int Capacity, int SecSnapshotInterval, int MaxKeyLength, int MaxPayloadLength, const char FileName[512]) {
        HTHANDLE* ht = new HTHANDLE(Capacity, SecSnapshotInterval, MaxKeyLength, MaxPayloadLength, FileName);

        ht->File = CreateFileA(ht->FileName, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
        if (ht->File == INVALID_HANDLE_VALUE) {
            SetErrorMessage(ht, "CreateFile Error");
            return ht;
        }

        int cell_size = sizeof(HT_Cell) + MaxKeyLength + MaxPayloadLength;
        DWORD file_size = (sizeof(int) * 4) + (cell_size * Capacity);

        ht->FileMapping = CreateFileMappingA(ht->File, NULL, PAGE_READWRITE, 0, file_size, NULL);
        if (ht->FileMapping == NULL) {
            CloseHandle(ht->File);
            SetErrorMessage(ht, "CreateFileMapping Error");
            return ht;
        }

        ht->Addr = MapViewOfFile(ht->FileMapping, FILE_MAP_ALL_ACCESS, 0, 0, 0);
        if (ht->Addr == NULL) {
            CloseHandle(ht->FileMapping);
            CloseHandle(ht->File);
            SetErrorMessage(ht, "MapViewOfFile Error");
            return ht;
        }

        int* metadata = (int*)ht->Addr;
        metadata[0] = Capacity;
        metadata[1] = SecSnapshotInterval;
        metadata[2] = MaxKeyLength;
        metadata[3] = MaxPayloadLength;

        char* cells_start = (char*)ht->Addr + (sizeof(int) * 4);
        ZeroMemory(cells_start, cell_size * Capacity);

        ht->lastsnaptime = std::time(nullptr);
        return ht;
    }

    // --- ШАГ 5: Функция открытия существующего хранилища ---

    HTHANDLE* Open(const char FileName[512]) {
        HTHANDLE* ht = new HTHANDLE();
        strcpy_s(ht->FileName, FileName);

        ht->File = CreateFileA(ht->FileName, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
        if (ht->File == INVALID_HANDLE_VALUE) {
            SetErrorMessage(ht, "OpenFile Error: File not found");
            return ht;
        }

        ht->FileMapping = CreateFileMappingA(ht->File, NULL, PAGE_READWRITE, 0, 0, NULL);
        if (ht->FileMapping == NULL) {
            CloseHandle(ht->File);
            SetErrorMessage(ht, "OpenFileMapping Error");
            return ht;
        }

        ht->Addr = MapViewOfFile(ht->FileMapping, FILE_MAP_ALL_ACCESS, 0, 0, 0);
        if (ht->Addr == NULL) {
            CloseHandle(ht->FileMapping);
            CloseHandle(ht->File);
            SetErrorMessage(ht, "Open MapViewOfFile Error");
            return ht;
        }

        int* metadata = (int*)ht->Addr;
        ht->Capacity = metadata[0];
        ht->SecSnapshotInterval = metadata[1];
        ht->MaxKeyLength = metadata[2];
        ht->MaxPayloadLength = metadata[3];

        ht->lastsnaptime = std::time(nullptr);
        return ht;
    }

    // --- ШАГ 6: Функции Snap и Close ---

    BOOL Snap(const HTHANDLE* hthandle) {
        if (!hthandle || hthandle->Addr == nullptr) return FALSE;

        int cell_size = sizeof(HT_Cell) + hthandle->MaxKeyLength + hthandle->MaxPayloadLength;
        DWORD file_size = (sizeof(int) * 4) + (cell_size * hthandle->Capacity);

        if (FlushViewOfFile(hthandle->Addr, file_size)) {
            ((HTHANDLE*)hthandle)->lastsnaptime = std::time(nullptr);
            return TRUE;
        }
        return FALSE;
    }

    BOOL Close(const HTHANDLE* hthandle) {
        if (!hthandle) return FALSE;
        Snap(hthandle);

        if (hthandle->Addr) UnmapViewOfFile(hthandle->Addr);
        if (hthandle->FileMapping) CloseHandle(hthandle->FileMapping);
        if (hthandle->File && hthandle->File != INVALID_HANDLE_VALUE) CloseHandle(hthandle->File);

        delete hthandle;
        return TRUE;
    }

    // --- ШАГ 7: Реализация функций манипуляции данными (CRUD) ---

    BOOL Insert(const HTHANDLE* hthandle, const Element* element) {
        if (!hthandle || !element) return FALSE;
        if (element->keylength > hthandle->MaxKeyLength || element->payloadlength > hthandle->MaxPayloadLength) {
            SetErrorMessage((HTHANDLE*)hthandle, "Insert Error: Превышена максимальная длина ключа или данных.");
            return FALSE;
        }

        int index = FindCellIndex(hthandle, element->key, element->keylength, true);
        if (index == -1) {
            SetErrorMessage((HTHANDLE*)hthandle, "Insert Error: Хранилище переполнено или ключ уже существует.");
            return FALSE;
        }

        int cell_size = sizeof(HT_Cell) + hthandle->MaxKeyLength + hthandle->MaxPayloadLength;
        char* cells_start = (char*)hthandle->Addr + (sizeof(int) * 4);
        HT_Cell* cell = (HT_Cell*)(cells_start + index * cell_size);

        cell->is_exists = true;
        cell->key_length = element->keylength;
        cell->payload_length = element->payloadlength;

        char* key_ptr = (char*)cell + sizeof(HT_Cell);
        char* payload_ptr = key_ptr + hthandle->MaxKeyLength;

        memcpy(key_ptr, element->key, element->keylength);
        memcpy(payload_ptr, element->payload, element->payloadlength);

        time_t now = std::time(nullptr);
        if (now - hthandle->lastsnaptime >= hthandle->SecSnapshotInterval) {
            Snap(hthandle);
        }

        return TRUE;
    }

    BOOL Delete(const HTHANDLE* hthandle, const Element* element) {
        if (!hthandle || !element) return FALSE;

        int index = FindCellIndex(hthandle, element->key, element->keylength, false);
        if (index == -1) {
            SetErrorMessage((HTHANDLE*)hthandle, "Delete Error: Элемент с таким ключом не найден.");
            return FALSE;
        }

        int cell_size = sizeof(HT_Cell) + hthandle->MaxKeyLength + hthandle->MaxPayloadLength;
        char* cells_start = (char*)hthandle->Addr + (sizeof(int) * 4);
        HT_Cell* cell = (HT_Cell*)(cells_start + index * cell_size);

        cell->is_exists = false;

        return TRUE;
    }

    Element* Get(const HTHANDLE* hthandle, const Element* element) {
        if (!hthandle || !element) return nullptr;

        int index = FindCellIndex(hthandle, element->key, element->keylength, false);
        if (index == -1) {
            SetErrorMessage((HTHANDLE*)hthandle, "Get Error: Элемент не найден.");
            return nullptr;
        }

        int cell_size = sizeof(HT_Cell) + hthandle->MaxKeyLength + hthandle->MaxPayloadLength;
        char* cells_start = (char*)hthandle->Addr + (sizeof(int) * 4);
        HT_Cell* cell = (HT_Cell*)(cells_start + index * cell_size);

        char* key_ptr = (char*)cell + sizeof(HT_Cell);
        char* payload_ptr = key_ptr + hthandle->MaxKeyLength;

        void* res_key = malloc(cell->key_length);
        void* res_payload = malloc(cell->payload_length);

        if (res_key) memcpy(res_key, key_ptr, cell->key_length);
        if (res_payload) memcpy(res_payload, payload_ptr, cell->payload_length);

        return new Element(res_key, cell->key_length, res_payload, cell->payload_length);
    }

    BOOL Update(const HTHANDLE* hthandle, const Element* oldelement, const void* newpayload, int newpayloadlength) {
        if (!hthandle || !oldelement) return FALSE;
        if (newpayloadlength > hthandle->MaxPayloadLength) {
            SetErrorMessage((HTHANDLE*)hthandle, "Update Error: Превышена максимальная длина новых данных.");
            return FALSE;
        }

        int index = FindCellIndex(hthandle, oldelement->key, oldelement->keylength, false);
        if (index == -1) {
            SetErrorMessage((HTHANDLE*)hthandle, "Update Error: Изменяемый элемент не найден.");
            return FALSE;
        }

        int cell_size = sizeof(HT_Cell) + hthandle->MaxKeyLength + hthandle->MaxPayloadLength;
        char* cells_start = (char*)hthandle->Addr + (sizeof(int) * 4);
        HT_Cell* cell = (HT_Cell*)(cells_start + index * cell_size);

        char* key_ptr = (char*)cell + sizeof(HT_Cell);
        char* payload_ptr = key_ptr + hthandle->MaxKeyLength;

        cell->payload_length = newpayloadlength;
        memcpy(payload_ptr, newpayload, newpayloadlength);

        return TRUE;
    }

    void print(const Element* element) {
        if (!element) {
            std::cout << "Element is nullptr" << std::endl;
            return;
        }

        std::cout << "Element: Key='";
        const char* k = (const char*)element->key;
        for (int i = 0; i < element->keylength; i++) {
            if (k[i] == '\0') break;
            std::cout << k[i];
        }

        std::cout << "', Payload='";
        const char* p = (const char*)element->payload;
        for (int i = 0; i < element->payloadlength; i++) {
            if (p[i] == '\0') break;
            std::cout << p[i];
        }
        std::cout << "'" << std::endl;
    }
}
