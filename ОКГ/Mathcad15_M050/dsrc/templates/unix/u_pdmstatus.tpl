// 18-Jan-99 $$1  JJE  Created
#include:head.inc

#include:shlib.inc

#includeenv:PTCNMSPORT:ptcnmsport.env

setenv PRO_DIRECTORY $prodir
if (! -e /etc/pdmservers.txt && -e $prodir/etc/`hostname`/pdmservers.txt) setenv PDM_CLIENT_DIR $prodir/etc/`hostname`

setenv PDM_DIRECTORY $prodir/pdm
setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg
exec $prodir/$mc/obj/pdmstatus $*
