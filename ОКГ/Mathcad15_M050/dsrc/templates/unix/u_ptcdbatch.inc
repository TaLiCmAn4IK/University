//
// File: u_ptcdbatch.inc
//
// 22-Nov-02 J-03-38  ALG  $$1   SPR 971401: Created from u_ptcdbatch.tpl
// 10-Nov-05 K-03-36  TWH  $$2   Add wildfire.inc
//
#include:head.inc

#include:shlib.inc
#include:wildfire.inc

if (! $?TEMP ) then
    set TEMP=/tmp
endif
if (! $?USER) then
    set USER=`whoami`
endif
setenv DCAD_TEMP ${TEMP}/${USER}_dbatchc
setenv NMSD_PATH $prodir/$mc/nms/nmsd
#includeenv:PROE_START:proe_start.env

setenv DB_CLIENT_DIRECTORY $prodir
setenv DBC_TEXT_RESOURCE $prodir/text/resource
setenv DBC_UITOOLS_RESOURCE $prodir/proe/uitools/text/resource

#includeenv:LM_LICENSE_FILE:lmlic.env

setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg

#include:ptc_startup.inc
