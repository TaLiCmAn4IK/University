// Generation.h
#pragma once
#include "stdafx.h"
#include "Parm.h"
#include "LT.h"
#include "IT.h"
#include <fstream>

namespace Gen {

    struct Generator {
        LT::LexTable& lexT;
        IT::IdTable& idT;
        std::ofstream out;

        Generator(LT::LexTable& plexT, IT::IdTable& pidT);

        void Header();
        void Vars();
        void Code();

    private:
        void GenerateOutput(int posOutput);
        void GenerateSwitch(int posSwitch);

        // IMPORTANT: принимаем позицию идентификатора и позицию '='
        void GenerateAssignment(int posId, int eqPos);

        // helpers
        int  findRightPar(int leftParPos);
        int  findSemicolon(int from);
    };

}
