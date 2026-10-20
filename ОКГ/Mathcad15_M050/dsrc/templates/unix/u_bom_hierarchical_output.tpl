// 06-Sep-01 $$1 TWH Provided by Fung 898070
// 17-Sep-01     TWH Use ora8_shlib
// 20-Sep-01 $$2 TWH Fix ORA_NLS33
// 12-Nov-01 $$3 TWH Set ORA_NLS33 to ORACLE817
// 29-Nov-01 $$4 TWH Add ORACLE_HOME 916044
// 19-Jul-04 K-03-06 $$5 TWH chg to ORACLE920
// 22-Aug-07 K-03-50 $$6 TWH Chg path to ORACLE9208
// 17-Jan-08 L-01-42 $$7 TWH Chg path to ORACLE10203
#include:head.inc 

#include:shlib.inc 

#include:ora8_shlib.inc 

setenv PDM_ORA_APPL intralink 
setenv PDM_ORACLE_SERVER intralink 
setenv TNS_ADMIN $prodir 
setenv ORA_NLS33 $prodir/$mc/obj/ORACLE10203/ocommon/nls/admin/data 
setenv ORACLE_HOME $prodir/$mc/obj/ORACLE10203

setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg 

$prodir/$mc/obj/bom_output $* 
