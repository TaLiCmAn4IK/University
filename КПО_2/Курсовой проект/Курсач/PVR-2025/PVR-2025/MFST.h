#pragma once
#include "stdafx.h"
#include "GRB.h"
#include "LT.h"
#include "LA.h"
#include "Log.h"

#include <stack>
#include <ostream>
#include <iomanip>
#include <vector>
#include <cstddef>
#include <cstring>

// Helper class to access protected std::stack container
class my_stack_SHORT : public std::stack<short> {
public:
    using std::stack<short>::c;
};

#define MFST_DIAGN_MAXSIZE (2 * ERROR_MAXSIZE_MESSAGE)
#define MFST_DIAGN_NUMBER  3

extern int  MFST_TRACE_n;
extern char MFST_rbuf[205];
extern char MFST_sbuf[205];
extern char MFST_lbuf[1024];

/*
  Trace format:
  Step : Rule                        Input                          Stack
*/
#define MFST_TRACE_START \
    stream_out << "\n\n\t\tSYNTAX ANALYSIS (MFST TRACE)\n\n" \
               << std::setw(4)  << std::setfill(' ') << std::left << "Step" << " : " \
               << std::setw(28) << std::left << "Rule" \
               << std::setw(35) << std::left << "Input" \
               << std::setw(20) << std::left << "Stack" << std::endl;

#define MFST_TRACE1 \
    stream_out << std::setw(4)  << std::left << ++MFST_TRACE_n << " : " \
               << std::setw(28) << std::left << " " \
               << std::setw(35) << std::left << getCLenta(MFST_lbuf, lenta_position, 25) \
               << std::setw(20) << std::left << getCSt(MFST_sbuf) << std::endl;

#define MFST_TRACE2 \
    stream_out << std::setw(4)  << std::left << MFST_TRACE_n << " : " \
               << std::setw(28) << std::left << " " \
               << std::setw(35) << std::left << getCLenta(MFST_lbuf, lenta_position, 25) \
               << std::setw(20) << std::left << getCSt(MFST_sbuf) << std::endl;

#define MFST_TRACE4(ruleText) \
    stream_out << std::setw(4)  << std::left << MFST_TRACE_n << " : " \
               << std::setw(28) << std::left << (ruleText) \
               << std::setw(35) << std::left << getCLenta(MFST_lbuf, lenta_position, 25) \
               << std::setw(20) << std::left << getCSt(MFST_sbuf) << std::endl;

#define MFST_TRACE5(text) \
    stream_out << std::setw(4)  << std::left << MFST_TRACE_n << " : " \
               << std::setw(28) << std::left << (text) \
               << std::setw(35) << std::left << getCLenta(MFST_lbuf, lenta_position, 25) \
               << std::setw(20) << std::left << getCSt(MFST_sbuf) << std::endl;

#define MFST_TRACE6(text, k) \
    stream_out << std::setw(4)  << std::left << MFST_TRACE_n << " : " \
               << std::setw(28) << std::left << (text) \
               << std::setw(3)  << std::left << (k) \
               << std::setw(32) << std::left << getCLenta(MFST_lbuf, lenta_position, 25) \
               << std::setw(20) << std::left << getCSt(MFST_sbuf) << std::endl;

typedef my_stack_SHORT MFSTSTSTACK;

namespace MFST {

    struct MfstState {
        short        lenta_position;
        short        nrule;
        short        nrulechain;
        MFSTSTSTACK  st;

        MfstState();
        MfstState(short pposition, MFSTSTSTACK pst, short pnrulechain);
        MfstState(short pposition, MFSTSTSTACK pst, short pnrule, short pnrulechain);
    };

    struct Mfst {

        enum RC_STEP {
            NS_OK,
            NS_NORULE,
            NS_NORULECHAIN,
            NS_ERROR,
            TS_OK,
            TS_NOK,
            LENTA_END,
            SURPRISE
        };

        struct MfstDiagnosis {
            short   lenta_position;
            RC_STEP rc_step;
            short   nrule;
            short   nrule_chain;

            MfstDiagnosis();
            MfstDiagnosis(short plenta_position, RC_STEP prc_step,
                short pnrule, short pnrule_chain);
        } diagnosis[MFST_DIAGN_NUMBER];

        class my_stack_MfstState : public std::stack<MfstState> {
        public:
            using std::stack<MfstState>::c;
        };

        struct AppliedRule { short nrule; short nrulechain; };
        struct AppliedMark { size_t applied_size; };

        std::vector<AppliedRule> applied;
        std::vector<AppliedMark> applied_marks;

        GRBALPHABET* lenta;
        short              lenta_position;
        short              nrule;
        short              nrulechain;
        short              lenta_size;

        GRB::Greibach      greibach;
        LT::LexTable       lex;

        MFSTSTSTACK        st;
        my_stack_MfstState storestate;

        Mfst();
        Mfst(const LT::LexTable& plex, GRB::Greibach pgreibach);
        ~Mfst();

        char* getCSt(char* buf);
        char* getCLenta(char* buf, short pos, short n = 25);
        char* getDiagnosis(short n, char* buf);

        bool    savestate(std::ostream& stream_out);
        bool    resetstate(std::ostream& stream_out);
        bool    push_chain(GRB::Rule::Chain chain);
        RC_STEP step(std::ostream& stream_out);
        bool    start(std::ostream& stream_out);

        bool    saveddiagnosis(RC_STEP pprc_step);

        void    printrules(std::ostream& stream_out, bool ok);

        struct Deducation {
            short  size;
            short* nrules;
            short* nrulechains;
            Deducation() : size(0), nrules(nullptr), nrulechains(nullptr) {}
        } deducation;

        bool savededucation();
    };

    bool check_syntax(const Lex::LEX& lex, Log::LOG log, std::ostream& stream_out);

} // namespace MFST
