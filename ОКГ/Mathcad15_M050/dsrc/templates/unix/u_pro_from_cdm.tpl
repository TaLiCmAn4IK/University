// 27-Mar-97 $$1  Pete  Created.
#include:head.inc

#include:shlib.inc

if ($mc != "ibm_rs6000") then
    echo "ERROR:  You must run pro_from_cdm on an ibm_rs6000 machine, exiting."
    exit(1)
endif

set iue_path=`which iue`
if (! -x "$iue_path") then
    echo "ERROR:  iue is not installed on `hostname`, check CADAM installation."
    exit(1)
endif

set CDMDUMPEXEC=""

if (`uname -v` == "3") then
    set CDMDUMPEXEC="$PRO_DIRECTORY/ibm_rs6000/obj/_produmpcadam_"
else
    set CDMDUMPEXEC="$PRO_DIRECTORY/ibm_rs6000/obj/_produmpcadam36_"
endif

if ($CDMDUMPEXEC == "") then
    echo "ERROR:  $CDMDUMPEXEC:t does not exist, exiting."
    exit(1)
endif

if (! -x $CDMDUMPEXEC) then
    echo "ERROR:  $CDMDUMPEXEC:t is not executable, exiting."
    exit(1)
endif

exec $iue_path -pgm $CDMDUMPEXEC -parms $*
