// 26-Sep-97 $$1  TWH  Created.
// 16-Oct-97 $$2  TWH  Pass all args
// 21-Oct-97 $$3  TWH  Change to format of passed args
// 23-Oct-97 $$4  TWH  Rm PDM_KEY dependancy
// 13-Jul-98 $$5  TWH  Not a ptc-cmdtype
// 17-Sep-98 $$6  TWH  Don't need nlocli.inc
// 22-Apr-99 $$7  TWH  Set ORA_NLS32 to oracle734 per echin
// 14-Dec-00 $$8  TWH  Set ORA_NLS32 to ORACLE816 per fung
// 20-Jul-01 $$9  TWH  Fix ora libpath, ORA_NLS, existance check
// 12-Nov-01 $$10 TWH  Set ORA_NLS33 to ORACLE817
// 25-Sep-03 $$11 K-01-16  TWH  mc fallback
// 12-May-04 $$12 K-03-02  TWH  replace ORA_NLS33 w/ ORACLE_HOME
// 22-Aug-07 $$13 K-03-50  TWH Chg path to ORACLE9208
// 17-Jan-08 $$14 L-01-42  TWH Chg path to ORACLE10203
// 10-Mar-08 $$15 L-03-05  TWH  Intralink now support 64 bit
#include:head.inc

#include:shlib.inc

#include:ora8_shlib.inc

setenv PDM_ORA_APPL intralink
setenv PDM_ORACLE_SERVER intralink
setenv TNS_ADMIN $prodir
setenv ORACLE_HOME $prodir/$mc/obj/ORACLE10203

setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg

if (! -e $prodir/$mc/obj/replicate_folder ) then
  echo "This utility is not supported on $mc"
  exit 0
endif

if ($1 == "") then
$prodir/$mc/obj/replicate_folder $*
else
$prodir/$mc/obj/replicate_folder $1 $2 "$3" $4 $5 $6 $7
endif
