// 12-Aug-97 $$1  JJE  Created.
// 02-Feb-98 $$2  JJE  Add nmsd
// 03-Feb-98 $$3  TWH  nmsd via includeenv
// 04-Feb-98 $$4  JJE  Also unset XFILESEARCHPATH
// 18-Feb-98 $$5  JJE  Remove NAME_SERVICE_EXE
// 15-Apr-99 $$6  JJE  Add il library path
// 17-Apr-00 $$7  MYA  Add FLYTHRU_FEATURE_NAME
// 23-Jan-01 $$8  TWH   Sun4_solaris_64 changes
// 19-Mar-01      ALG   Include ptc_startup.inc
// 19-Mar-01 $$9  TWH  Change for ilinkdir.env
#include:head.inc

#include:shlib.inc

#include:ilinkdir.env

#include:shlibproe.inc
#include:sun4_tmpdir.inc

# ptc-cmdtype: profly

#includeenv:PTCNMSPORT:ptcnmsport.env

if ($mc =~ sgi*) then
   unsetenv XUSERFILESEARCHPATH
   unsetenv XFILESEARCHPATH
endif
if ($mc == ibm_rs6000) then
   setenv PRO_DUMPCATIA_PATH $prodir/$mc/obj
endif
if ($mc == alpha_unix) then
    limit datasize unlimited
    limit stacksize unlimited
endif

setenv PRO_DIRECTORY $prodir
setenv PROFLY_DIRECTORY $prodir/pronav

setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg

#includeenv:PROE_START:proe_start.env

#includeenv:LM_LICENSE_FILE:lmlic.env

setenv PROFLY_FEATURE_NAME "__FLYTHRU_FEATURE_NAME__"
setenv FLYTHRU_FEATURE_NAME "__FLYTHRU_FEATURE_NAME__"

#include:ptc_startup.inc

$prodir/$mc/obj/profly $*
