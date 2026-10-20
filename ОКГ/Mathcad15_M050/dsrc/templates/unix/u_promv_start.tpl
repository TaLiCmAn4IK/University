// 12-Aug-97 $$1  JJE  Created.
// 18-Feb-98 $$2  JJE  New licensing
// 09-Mar-98 $$3  JJE  Also unset XFILESEARCHPATH
// 15-Apr-99 $$4  JJE  Add il library path
// 17-Apr-00      MYA  Add MODVIEW_FEATURE_NAME
// 02-May-00 $$5  MYA  get rid of empty line before head.inc, bug 821021
// 23-Jan-01 $$6  TWH   Sun4_solaris_64 changes
// 19-Mar-01      ALG   Include ptc_startup.inc
// 19-Mar-01 $$7  TWH  Change for ilinkdir.env
#include:head.inc

#include:shlib.inc

#include:ilinkdir.env

#include:shlibproe.inc
#include:sun4_tmpdir.inc

# ptc-cmdtype: promodelview

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

setenv PROFLY_FEATURE_NAME "__MODELVIEW_FEATURE_NAME__"
setenv MODELVIEW_FEATURE_NAME "__MODELVIEW_FEATURE_NAME__"

#includeenv:LM_LICENSE_FILE:lmlic.env

#include:ptc_startup.inc

$prodir/$mc/obj/profly $*
