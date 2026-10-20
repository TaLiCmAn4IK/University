#include "stdafx.h"
#include "FST.h"

namespace FST {

    // -------- step --------
    bool step(FST& fst, short*& rstates) {
        bool rc = false;
        std::swap(rstates, fst.rstates);

        for (short i = 0; i < fst.nstates; ++i) {
            if (rstates[i] == fst.position) {
                for (short j = 0; j < fst.nodes[i].n_relation; ++j) {
                    if (fst.nodes[i].relations[j].symbol == fst.string[fst.position]) {
                        fst.rstates[fst.nodes[i].relations[j].nnode] = fst.position + 1;
                        rc = true;
                    }
                }
            }
        }
        return rc;
    }

    // -------- execute --------
    bool execute(FST& fst) {
        short* rstates = new short[fst.nstates];
        memset(rstates, 0xff, sizeof(short) * fst.nstates);

        // очистить текущее множество состояний
        memset(fst.rstates, 0xff, sizeof(short) * fst.nstates);

        short length = static_cast<short>(strlen(fst.string));
        bool rc = true;

        // старт: состояние 0 активно после 0 символов
        rstates[0] = 0;
        fst.rstates[0] = 0;

        fst.position = -1;
        for (short i = 0; i < length && rc; ++i) {
            fst.position++;
            rc = step(fst, rstates);
        }

        delete[] rstates;

        return rc && (fst.rstates[fst.nstates - 1] == length);
    }

} // namespace FST
