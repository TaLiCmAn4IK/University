// 14-Apr-98 $$1  ZRL  Created
// 19-May-98 $$2  ZRL  Added LM_LICENSE_FILE
// 19-Mar-01 $$3  ALG  Include ptc_startup.inc
#include:head.inc

#include:shlib.inc

# ptc-cmdtype: pdmclient

if ($mc =~ sgi*) unsetenv XUSERFILESEARCHPATH
if ($mc == ibm_rs6000) then
   setenv PRO_DUMPCATIA_PATH $prodir/$mc/obj
endif
if ($mc == alpha_unix) then
    limit datasize unlimited
    limit stacksize unlimited
endif

#includeenv:PROE_START:proe_start.env

#includeenv:PTCNMSPORT:ptcnmsport.env

if (! -e /etc/pdmservers.txt && -e $prodir/etc/`hostname`/pdmservers.txt) setenv PDM_CLIENT_DIR $prodir/etc/`hostname`

setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg
setenv PROTAB protab
setenv PRO_DIRECTORY $prodir
setenv PDM_DIRECTORY $prodir/pdm
if (! $?PDM_REVCTL) setenv PDM_REVCTL $prodir/$mc/obj/pdmrnctl

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv PDM_FEATURE_NAME "__PDM_FEATURE_NAME__"

#include:ptc_startup.inc

$prodir/$mc/obj/pdm $*

