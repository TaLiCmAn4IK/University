// 14-Apr-98 $$1  ZRL  Created
// 05-Jan-00 $$2  VEG  Fixed passing args
#include:head.inc

#include:shlib.inc

#includeenv:PTCNMSPORT:ptcnmsport.env

setenv PRO_DIRECTORY $prodir
if (! -e /etc/pdmservers.txt && -e $prodir/etc/`hostname`/pdmservers.txt) setenv PDM_CLIENT_DIR $prodir/etc/`hostname`

setenv PDM_DIRECTORY $prodir/pdm
setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg
exec $prodir/$mc/obj/pdmshell $*:q
