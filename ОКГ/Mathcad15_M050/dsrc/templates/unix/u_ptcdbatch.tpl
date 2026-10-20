//
// File: u_ptcdbatch.tpl
//
// 05-Mar-02 $$1  ALG  Created
// 08-Apr-02 $$2  TWH  Needs shlib.inc
// 25-Apr-02 $$3  ALG  Remove DSQSERVICE_FEATURE_NAME
// 30-Apr-02 $$4  TWH  Add PRO_COMM_MSG_EXE
// 22-Aug-02 $$5  TWH  Set DCAD_TEMP uniq per username
// 09-Sep-02 $$6  TWH  Syntax error in #5
// 07-Nov-02 J-03-37  ALG  $$7   SPR 985495: Add NMSD_PATH for standalone mode
// 22-Nov-02 J-03-38  ALG  $$8   SPR 971401: Move most of contents to new
//                                 "common" file u_ptcdbatch.inc
//
#include:ptcdbatch.inc

$prodir/$mc/obj/dbatchc $*
exit $status
