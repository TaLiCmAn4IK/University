#include "HT.h"

namespace HT
{

    Element::Element()
    {
        key = nullptr;
        keylength = 0;
        payload = nullptr;
        payloadlength = 0;
    }

    Element::Element(const void* key, int keylength)
    {
        this->key = key;
        this->keylength = keylength;
        this->payload = nullptr;
        this->payloadlength = 0;
    }

    Element::Element(const void* key, int keylength, const void* payload, int payloadlength)
    {
        this->key = key;
        this->keylength = keylength;
        this->payload = payload;
        this->payloadlength = payloadlength;
    }

    Element::Element(Element* oldelement, const void* newpayload, int newpayloadlength)
    {
        this->key = oldelement->key;
        this->keylength = oldelement->keylength;
        this->payload = newpayload;
        this->payloadlength = newpayloadlength;
    }

}