#include "stdafx.h"
#include "MFST.h"
#include <unordered_set>
#include <cstring>

int  MFST_TRACE_n = -1;
char MFST_rbuf[205]{};
char MFST_sbuf[205]{};
char MFST_lbuf[1024]{};
static const int MFST_STEP_LIMIT = 400; // поставь сколько надо

namespace MFST {

    MfstState::MfstState() {
        lenta_position = 0;
        nrule = -1;
        nrulechain = -1;
    }

    MfstState::MfstState(short pposition, MFSTSTSTACK pst, short pnrulechain) {
        lenta_position = pposition;
        st = pst;
        nrule = -1;
        nrulechain = pnrulechain;
    }

    MfstState::MfstState(short pposition, MFSTSTSTACK pst, short pnrule, short pnrulechain) {
        lenta_position = pposition;
        st = pst;
        nrule = pnrule;
        nrulechain = pnrulechain;
    }

    Mfst::MfstDiagnosis::MfstDiagnosis() {
        lenta_position = -1;
        rc_step = SURPRISE;
        nrule = -1;
        nrule_chain = -1;
    }

    Mfst::MfstDiagnosis::MfstDiagnosis(short plenta_position,
        RC_STEP prc_step,
        short pnrule,
        short pnrule_chain) {
        lenta_position = plenta_position;
        rc_step = prc_step;
        nrule = pnrule;
        nrule_chain = pnrule_chain;
    }

    Mfst::Mfst() {
        lenta = nullptr;
        lenta_size = 0;
        lenta_position = 0;
        nrule = -1;
        nrulechain = -1;
    }

    Mfst::~Mfst() {
        delete[] lenta;
        lenta = nullptr;
        lenta_size = 0;
    }

    Mfst::Mfst(const LT::LexTable& plex, GRB::Greibach pgreibach)
    {
        greibach = pgreibach;
        lex = plex;

        lenta = new short[lenta_size = lex.size];
        for (int k = 0; k < lenta_size; ++k)
            lenta[k] = GRB::Rule::Chain::T(lex.table[k].lexema);

        lenta_position = 0;
        st.push(greibach.stbottomT);
        st.push(greibach.startN);

        nrule = -1;
        nrulechain = -1;

        applied.clear();
        applied_marks.clear();
        applied.reserve((size_t)lex.size);

        for (int i = 0; i < MFST_DIAGN_NUMBER; ++i)
            diagnosis[i] = MfstDiagnosis();
    }

    bool Mfst::push_chain(GRB::Rule::Chain chain)
    {
        for (int k = chain.size - 1; k >= 0; --k)
        {
            if (chain.nt[k] == TS(EMPTY_CHAIN)) continue;
            st.push(chain.nt[k]);
        }
        return true;
    }

    bool Mfst::savestate(std::ostream& stream_out) {
        storestate.push(MfstState(lenta_position, st, nrule, nrulechain));
        MFST_TRACE6("SAVESTATE:", (int)storestate.size());
        applied_marks.push_back({ applied.size() });
        return true;
    }

    bool Mfst::resetstate(std::ostream& stream_out) {
        bool rc = false;
        if ((rc = (storestate.size() > 0)))
        {
            MfstState state = storestate.top();
            lenta_position = state.lenta_position;
            st = state.st;
            nrule = state.nrule;
            nrulechain = state.nrulechain;
            storestate.pop();

            if (!applied_marks.empty()) {
                applied.resize(applied_marks.back().applied_size);
                applied_marks.pop_back();
            }

            MFST_TRACE5("RESSTATE");
            MFST_TRACE2;
        }
        return rc;
    }

    bool Mfst::saveddiagnosis(RC_STEP prc_step) {
        bool  rc = false;
        short k = 0;

        while (k < MFST_DIAGN_NUMBER &&
            lenta_position <= diagnosis[k].lenta_position)
            ++k;

        if ((rc = (k < MFST_DIAGN_NUMBER))) {
            diagnosis[k] = MfstDiagnosis(lenta_position, prc_step, nrule, nrulechain);
            for (int i = k + 1; i < MFST_DIAGN_NUMBER; ++i)
                diagnosis[i].lenta_position = -1;
        }

        return rc;
    }

    Mfst::RC_STEP Mfst::step(std::ostream& stream_out)
    {
        if (lenta_position >= lenta_size)
            return LENTA_END;

        RC_STEP rc = SURPRISE;

        short top = st.top();
        short lookahead = lenta[lenta_position];

        if (GRB::Rule::Chain::isN(top))
        {
            GRB::Rule rule;
            nrule = greibach.getRule(top, rule);

            if (nrule < 0)
            {
                saveddiagnosis(NS_ERROR);
                return NS_ERROR;
            }

            GRB::Rule::Chain chain;
            nrulechain = rule.getNextChain(lookahead, chain, 0);

            if (nrulechain >= 0)
            {
                MFST_TRACE1;
                MFST_TRACE4(rule.getCRule(MFST_rbuf, nrulechain));
                MFST_TRACE2;

                applied.push_back({ nrule, nrulechain });

                const bool isEps = (chain.size == 1 && chain.nt[0] == TS(EMPTY_CHAIN));
                if (rule.size > 1 && !isEps)
                    savestate(stream_out);

                st.pop();
                push_chain(chain);

                nrulechain = -1;
                rc = NS_OK;
            }
            else
            {
                saveddiagnosis(NS_NORULECHAIN);
                rc = resetstate(stream_out) ? NS_NORULECHAIN : NS_NORULE;
            }
        }
        else if (top == lookahead)
        {
            MFST_TRACE1;
            MFST_TRACE2;

            ++lenta_position;
            st.pop();
            nrulechain = -1;
            rc = TS_OK;
        }
        else
        {
            saveddiagnosis(TS_NOK);
            rc = resetstate(stream_out) ? TS_NOK : NS_NORULECHAIN;
        }

        return rc;
    }

    bool Mfst::start(std::ostream& stream_out) {
        MFST_TRACE_START;

        bool    rc = false;
        RC_STEP rc_step = SURPRISE;
        char    buf[MFST_DIAGN_MAXSIZE]{};

        rc_step = step(stream_out);

        int steps = 0;
        while ((rc_step == SURPRISE || rc_step == NS_OK || rc_step == TS_OK || rc_step == TS_NOK) &&
            steps < MFST_STEP_LIMIT)
        {
            rc_step = step(stream_out);
            ++steps;
        }

        if (steps >= MFST_STEP_LIMIT) {
            stream_out << "------------------------------------------------------------------------------------------   ------\n";
            stream_out << "[MFST] STEP LIMIT reached (" << MFST_STEP_LIMIT << "). Output is partial.\n";
            return false; // синтаксис считаем неуспешным, но без зависания
        }


        switch (rc_step) {
        case LENTA_END:
            stream_out << "------------------------------------------------------------------------------------------   ------\n";
            stream_out << std::setw(4) << std::left << 0
                << "total lines " << lex.table[lex.size - 1].line
                << ", syntax analysis completed without errors\n";
            rc = true;
            break;

        default:
            stream_out << "------------------------------------------------------------------------------------------   ------\n";
            stream_out << getDiagnosis(0, buf) << '\n';
            stream_out << getDiagnosis(1, buf) << '\n';
            stream_out << getDiagnosis(2, buf) << '\n';
            rc = false;
            break;
        }

        return rc;
    }

    char* Mfst::getCSt(char* buf) {
        short p;
        for (int k = (short)st.size() - 1; k >= 0; --k) {
            p = st.c[k];
            buf[st.size() - 1 - k] = GRB::Rule::Chain::alphabet_to_char(p);
        }
        buf[st.size()] = '\0';
        return buf;
    }

    char* Mfst::getCLenta(char* buf, short pos, short n) {
        short end = (pos + n < lenta_size) ? (pos + n) : lenta_size;
        short j = 0;

        for (short i = pos; i < end; ++i)
            buf[j++] = GRB::Rule::Chain::alphabet_to_char(lenta[i]);

        buf[j] = '\0';
        return buf;
    }

    char* Mfst::getDiagnosis(short n, char* buf)
    {
        buf[0] = 0;
        int errid = 0;
        int lpos = -1;

        if (n < MFST_DIAGN_NUMBER &&
            (lpos = diagnosis[n].lenta_position) >= 0)
        {
            errid = greibach.getRule(diagnosis[n].nrule).iderror;
            Error::ERROR err = Error::geterror(errid);

            sprintf_s(buf, MFST_DIAGN_MAXSIZE,
                "%d: Line %d, %s",
                err.id, lex.table[lpos].line, err.message);
        }

        return buf;
    }

    struct RuleKeyHash {
        size_t operator()(const unsigned int& x) const noexcept {
            return std::hash<unsigned int>{}(x);
        }
    };

    void Mfst::printrules(std::ostream& out, bool ok)
    {
        out << "\n\t\tSYNTAX RULES (UNIQUE)\n";
        out << "total lines " << lex.table[lex.size - 1].line
            << (ok ? ", syntax analysis completed without errors\n\n"
                : ", syntax analysis failed\n\n");

        std::unordered_set<unsigned int, RuleKeyHash> seen;
        seen.reserve(applied.size() * 2 + 1);

        int outIndex = 0;
        for (size_t i = 0; i < applied.size(); ++i)
        {
            unsigned int key =
                ((unsigned int)(unsigned short)applied[i].nrule << 16) |
                (unsigned int)(unsigned short)applied[i].nrulechain;

            if (seen.find(key) != seen.end())
                continue;

            seen.insert(key);

            GRB::Rule r = greibach.getRule(applied[i].nrule);
            out << std::setw(4) << std::left << outIndex++ << " : "
                << r.getCRule(MFST_rbuf, applied[i].nrulechain)
                << "\n";
        }
    }

    bool check_syntax(const Lex::LEX& lex, Log::LOG /*log*/, std::ostream& stream_out) {
        MFST::Mfst mfst(lex.lextable, GRB::getGreibach());
        bool ok = mfst.start(stream_out);
        mfst.printrules(stream_out, ok);
        return ok;
    }

} // namespace MFST
