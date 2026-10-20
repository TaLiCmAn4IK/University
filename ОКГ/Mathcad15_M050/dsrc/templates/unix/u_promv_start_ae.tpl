// 12-Aug-97 $$1  JJE  Created.
// 15-Apr-99 $$2  JJE  Add il library path
// 23-Jan-01 $$3  TWH   Sun4_solaris_64 changes
// 19-Mar-01 $$4  TWH  Change for ilinkdir.env
// 17-Oct-01 $$5  TWH  Use PTC_D_LICENSE_FILE
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

setenv PROFLY_FEATURE_NAME FLYVIEWER_AE
setenv PTC_D_LICENSE_FILE $prodir/pronav/text/licensing/profly/promview_lic.dat

$prodir/$mc/obj/profly $*
