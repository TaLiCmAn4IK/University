// 08-Sep-97 $$1  JJE  Created.
// 29-Dec-97 $$2  TWH  Added __APPMGR_OLD_FONTS__
// 02-Feb-98 $$3  JJE  Add nmsd
// 22-Jan-98 $$4  TWH  Add nms support
// 18-Feb-98 $$5  JJE  Remove NAME_SERVICE_EXE
// 23-Jan-01 $$6  TWH   Sun4_solaris_64 changes
// 28-Nov-05 K-03-37 $$7 TWH  Remove Galaxy ref
#include:head.inc
#include:shlib.inc
#include:sun4_tmpdir.inc

# ptc-cmdtype: proappmgr 

#includeenv:PTCNMSPORT:ptcnmsport.env

if ($mc =~ sgi*) unsetenv XUSERFILESEARCHPATH
if ($mc == ibm_rs6000) then
   setenv PRO_DUMPCATIA_PATH $prodir/$mc/obj
endif
if ($mc == alpha_unix) then
    limit datasize unlimited
    limit stacksize unlimited
endif

setenv PTC_APPMGR_DIR $prodir/appmgr
setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg
setenv PTC_APPMGR_OLD_FONTS __APPMGR_OLD_FONTS__

$prodir/$mc/obj/appmgr -laf motif $*
