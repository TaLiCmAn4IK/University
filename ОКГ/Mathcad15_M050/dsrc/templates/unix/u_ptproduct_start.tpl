// 14-May-98      ZRL  Created.
// 26-May-98      ZRL  use PTMODELER_FEATURE_NAME as feature name
// 27-May-98      ZRL  change cmdtype to ptprod
// 16-Jun-98 $$1  JJE  Group Submission
// 22-Jun-98 $$2  JJE  Add GLMGRARRAYOPT for sgi
// 07-Jul-98 $$3  JJE   Set CLHOME
// 23-Jan-01 $$4  TWH   Sun4_solaris_64 changes
// 17-May-05 K-03-25 $$5 MAZ SPR 1144354: rm PRO_PHOTORENDER
// 28-Nov-05 K-03-37 $$6 TWH Remove Galaxy Ref
#include:head.inc

#include:shlib.inc
#include:sun4_tmpdir.inc

# ptc-cmdtype: ptprod

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

#includeenv:PRONCPOST_START:ncpstart.env

#includeenv:PDMCDIR:pdmcdir.env

#includeenv:CDRS_DATA:cdrsdata.env

#includeenv:PTCNMSPORT:ptcnmsport.env

if ( ! $?CDRS_DATA) then
    setenv CDRS_DATA $prodir
endif
setenv PRO_IMAGE_CONVERTER $prodir/$mc/obj/convert_image

if (-e $prodir/install/unix/pro_mech_loc.csh) then
    setenv PRO_MECH_COMMAND $0
    source $prodir/install/unix/pro_mech_loc.csh
    if (-e $MECH_LP/install/unix/pro_mech_env.csh) then
        source $MECH_LP/install/unix/pro_mech_env.csh
    endif
endif

#includeenv:PTC_APPMGR:appmgr.env

setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg
setenv SPG_DIRECTORY $prodir
setenv PRO_USRMAIN $prodir/usrprog/umain/usrmain.c
setenv PROTABLE_DIRECTORY $prodir/protable
setenv PROTAB $prodir/$mc/obj/protab
setenv CLHOME $prodir/text/pcldata

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv PTMODELER_FEATURE_NAME "__PTMODELER_FEATURE_NAME__"

$prodir/$mc/obj/projr $*
