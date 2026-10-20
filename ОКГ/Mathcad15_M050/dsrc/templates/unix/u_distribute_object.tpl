// 13-Apr-01 $$1 TWH Created
// 20-Jul-01 $$2 TWH Fix ora libpath, ORA_NLS
// 19-Sep-01 $$3 TWH Add hp
// 25-Sep-01 $$4 TWH Fix typo #3
// 12-Nov-01 $$5 TWH Set ORA_NLS33 to ORACLE817
// 01-Apr-02 $$6 TWH apply final_arg code per Lijun Wang 930237
// 05-Mar-03 K-01-02 ALG  $$7   SPR 1003705: Exit status
// 24-Apr-03 K-01-06 TWH  $$8   Force 32 platform type
// 12-May-04 K-03-02 TWH  $$9   Replace ORA_NLS33 w/ ORACLE_HOME
// 22-Aug-07 K-03-50 TWH  $$10  Chg path to ORACLE9208
// 17-Jan-08 L-01-42 TWH  $$11 Chg path to ORACLE10203
// 10-Mar-08 L-03-05 TWH  $$12  Intralink now support 64 bit
// 21-May-08 L-03-09 TWH  $$13  Update sun check
#include:head.inc

#include:shlib.inc

#include:ora8_shlib.inc

setenv PDM_ORA_APPL intralink
setenv PDM_ORACLE_SERVER intralink
setenv TNS_ADMIN $prodir
setenv ORACLE_HOME $prodir/$mc/obj/ORACLE10203

setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg

set n = 1 
set final_arg = "" 

while($n <= $#argv) 
    set final_arg = ($final_arg \"$argv[$n]\") 
    @ n = ($n + 1) 
end 

if ($mc =~ sun* || $mc =~ hp* ) then
  eval $prodir/$mc/obj/distribute_object $final_arg 
  exit $status
else
  echo "The executable distribute_object is supported only for SUN, HP and NT platforms"
  exit 1
endif
