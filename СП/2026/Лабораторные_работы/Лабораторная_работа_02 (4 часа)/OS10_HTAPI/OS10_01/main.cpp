#include <iostream>
#include "HT.h"

int main()
{
    HT::HTHANDLE* ht = nullptr;
    try
    {
        ht = HT::Create(1000, 10, 256, 256, "Hspace.ht");
        if (ht) std::cout << "Create: success\n";
        else throw "Create: error";

        if (HT::Insert(ht, new HT::Element("key222", 7, "payload", 8)))
            std::cout << "Insert: success\n";
        else throw "Insert: error";

        HT::Element* hte = HT::Get(ht, new HT::Element("key222", 7));
        if (hte) std::cout << "Get: success\n";
        else throw "Get: error";
        HT::print(hte);

        if (HT::Snap(ht)) std::cout << "Snap: success\n";
        else throw "Snap: error";

        if (HT::Update(ht, hte, "newpayload", 11))
            std::cout << "Update: success\n";
        else throw "Update: error";

        HT::Element* hte1 = HT::Get(ht, new HT::Element("key222", 7));
        if (hte1) std::cout << "Get: success\n";
        else throw "Get: error";
        HT::print(hte1);

        if (HT::Delete(ht, hte1)) std::cout << "Delete: success\n";
        else throw "Delete: error";

        if (HT::Close(ht)) std::cout << "Close: success\n";
        else throw "Close: error";
        ht = nullptr;
    }
    catch (const char* msg)
    {
        std::cout << msg << "\n";
        if (ht != nullptr) std::cout << HT::GetLastError(ht) << "\n";
    }
    return 0;
}