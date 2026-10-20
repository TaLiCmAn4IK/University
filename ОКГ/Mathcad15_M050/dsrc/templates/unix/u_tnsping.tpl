// 27-Mar-97 $$1  Pete  Created.
// 04-Jan-01 $$2  TWH   Add ora8_shlib
// 09-Jul-03 $$3  K-01-10  TWH  Add ORACLE_HOME
// 25-Sep-03               TWH  add TNS_ADMIN
// 25-Sep-03 $$4  K-01-16  TWH  mc fallback
// 12-May-03 $$5  K-03-02  TWH  Chg ORACLE_HOME
// 22-Aug-07 $$6  K-03-50  TWH Chg path to ORACLE9208
// 17-Jan-08 $$7  L-01-42  TWH Chg path to ORACLE10203
// 10-Mar-08 $$8  L-03-05  TWH  Intralink now support 64 bit
#include:head.inc

#include:ora8_shlib.inc
setenv ORACLE_HOME $prodir/$mc/obj/ORACLE10203
setenv TNS_ADMIN $prodir
exec $prodir/$mc/obj/tnsping $*

