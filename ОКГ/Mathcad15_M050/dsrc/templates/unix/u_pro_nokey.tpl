// 27-Mar-97 $$1  Pete  Created.
// 16-Jun-98 $$2  JJE   Add GLMGRARRAYOPT for sgi
// 07-Jul-98 $$3  JJE   Set CLHOME,CV_ENV_HOME
// 17-May-05 K-03-25 $$4 MAZ SPR 1144354: rm PRO_PHOTORENDER
#include:head.inc

#include:shlib.inc

#include:nlocli.inc


if ($mc =~ sgi*) then
   unsetenv XUSERFILESEARCHPATH
   setenv GLMGRARRAYOPT 1
endif
if ($mc == ibm_rs6000) then
   setenv PRO_DUMPCATIA_PATH $prodir/$mc/obj
endif
if ($mc == alpha_unix) then
    limit datasize unlimited
    limit stacksize unlimited
endif

if ( ! $?CDRS_DATA) then
    setenv CDRS_DATA $prodir
endif
setenv PRO_IMAGE_CONVERTER $prodir/$mc/obj/convert_image
setenv PRO_RENDERSERVER $prodir/$mc/obj/RenderServer
setenv PRO_AR_EXE $prodir/$mc/obj/pro_ar

setenv PROGUIDE_DIRECTORY $prodir/uifdoc
setenv PROUSERGUIDE $prodir/$mc/obj/proguide
setenv SPG_DIRECTORY $prodir
setenv PRO_USRMAIN $prodir/usrprog/umain/usrmain.c
setenv PROTABLE_DIRECTORY $prodir/protable
setenv PROTAB $prodir/$mc/obj/protab
setenv CV_ENV_HOME $prodir/$mc/CV80
setenv CLHOME $prodir/text/pcldata

$prodir/$mc/obj/pro $*

