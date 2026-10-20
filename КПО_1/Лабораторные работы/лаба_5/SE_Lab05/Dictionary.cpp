#include "Dictionary.h"
#include <iostream>
#include <cstring>
#include <algorithm>

using namespace Dictionary;

Instance Dictionary::Create(const char name[DICTNAMEMAXSIZE], int size)
{
    if (strlen(name) >= DICTNAMEMAXSIZE)
        throw std::runtime_error(THROW01);
    if (size > DICTMAXSIZE)
        throw std::runtime_error(THROW02);

    Instance inst;
    strncpy_s(inst.name, name, DICTNAMEMAXSIZE);
    inst.maxsize = size;
    inst.size = 0;
    inst.dictionary = new Entry[size];

    return inst;
}

void Dictionary::AddEntry(Instance& inst, Entry ed)
{
    if (inst.size >= inst.maxsize) // Исправлено == на >= для всех случаев переполнения
        throw std::runtime_error(THROW03);

    for (int i = 0; i < inst.size; i++)
    {
        if (inst.dictionary[i].id == ed.id)
            throw std::runtime_error(THROW04);
    }

    inst.dictionary[inst.size++] = ed;
}



void Dictionary::DelEntry(Instance& inst, int id)
{
    bool found = false;
    for (int i = 0; i < inst.size; i++)
    {
        if (inst.dictionary[i].id == id)
        {
            // Сдвигаем все элементы после удаляемого на одну позицию влево
            for (int j = i; j < inst.size - 1; j++)
            {
                inst.dictionary[j] = inst.dictionary[j + 1];
            }
            inst.size--;
            found = true;
            break;
        }
    }

    if (!found)
        throw std::runtime_error(THROW06);
}

void Dictionary::UpdEntry(Instance& inst, int id, Entry new_ed)
{
    bool found = false;
    for (int i = 0; i < inst.size; i++)
    {
        if (inst.dictionary[i].id == id)
        {
            // Проверяем, что новый ID не дублируется (если он изменился)
            if (new_ed.id != id)
            {
                for (int j = 0; j < inst.size; j++)
                {
                    if (inst.dictionary[j].id == new_ed.id)
                        throw std::runtime_error(THROW08);
                }
            }

            inst.dictionary[i] = new_ed;
            found = true;
            break;
        }
    }

    if (!found)
        throw std::runtime_error(THROW07);
}

Entry Dictionary::GetEntry(Instance inst, int id)
{
    for (int i = 0; i < inst.size; i++)
    {
        if (inst.dictionary[i].id == id)
            return inst.dictionary[i];
    }

    throw std::runtime_error(THROW05);
}

void Dictionary::Print(Instance d)
{
    std::cout << "----- " << d.name << " -----" << std::endl;
    for (int i = 0; i < d.size; i++)
    {
        std::cout << d.dictionary[i].id << " " << d.dictionary[i].name << std::endl;
    }
    std::cout << "---------------------" << std::endl;
}

void Dictionary::Delete(Instance& d)
{
    delete[] d.dictionary;
    d.dictionary = nullptr;
    d.size = 0;
}