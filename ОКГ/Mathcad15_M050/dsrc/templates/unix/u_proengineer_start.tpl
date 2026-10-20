// 27-Mar-97 $$1  Pete  Created.
// 12-Jan-98 $$2  JJE   Remove set display
// 29-Dec-97 $$3  TWH   PTC_APPMGR_DIR removed
// 03-Feb-97 $$4  JJE   Add nmsd
// 03-Feb-97 $$5  TWH   nmsd via includeenv
// 18-Feb-98 $$6  JJE   Remove NAME_SERVICE_EXE
// 19-Feb-98 $$7  JJE   Remove PROGUIDE variables
// 16-Jun-98 $$8  JJE   Add GLMGRARRAYOPT for sgi
// 26-Jun-98 $$9  JJE   Set CV_ENV_HOME, CLHOME
// 16-Jul-98      TWH   include shlibproe
// 24-Jul-98 $$10 JJE   Add PTC_CVLIB
// 15-Dec-98 $$11 JJE   Use CV90
// 08-Jun-99 $$12 JJE   Include ptc_startup, mechdemo
// 05-Jan-00 $$13 JJE   Add mcdir setting
// 11-Jan-00 $$14 JJE   Use CV102
// 21-Jan-00 $$15 JJE   No mcdir setting
// 15-Aug-00 $$16 JJE   Add PRO_SURF_FOUNDATION
// 28-Aug-00 $$17 JJE   Add PROE_START
// 23-Jan-01 $$18 TWH   Sun4_solaris_64 changes
// 20-Feb-01 $$19 JJE   Use CV110
// 01-Mar-01 $$20 JJE   Add stubs dir for CV
// 19-Mar-01 $$21 TWH   Change for ilinkdir.env
// 20-Sep-01 $$22 TWH   Exit $status
// 18-Dec-01 $$23 JJE   Set _VFORK_NO_CMA for hp8k
// 04-Jan-02 $$24 MAZ   include wildfire.inc 
// 28-Jan-02 $$25 JJE   Set _VFORK_NO_CMA for all hp
// 01-Feb-02 $$26 JJE   Set VCONF_START
// 05-Feb-02 $$27 JJE   Rename to VC_VCONF_START
// 20-Mar-02      MAZ   SPR 936895: set MOZ_PLUGIN_PATH 
// 21-Mar-02 $$28 JJE   Add ZIP_EXEC setting
// 01-Apr-02 $$29 JJE   Fix MOZ_PLUGIN_PATH setting
// 05-Apr-02 $$30 MAZ   Include mechdir.env 
// 28-May-02 $$31 TWH   redo #21 to includeenv "ALWAYS_TRUE"
// 21-Jun-02 $$32 MAZ   SPR 960895: set PVX_PATH
// 22-Jul-02 $$33 JJE   Add PVIEW_HOME
// 21-Aug-02 J-03-32  ALG  $$34  Change "proevconf" -> "ptcvconf"
// 11-Nov-02 J-03-37  ECW  $$35  Change PVIEW_HOME to PVIEW_PVEXPORT_HOME
// 31-Dec-02 J-03-40  TWH  $$36  Add includeenv:ILINK_CMD_NAME:ilinkcmd.env
// 05-Jun-03 K-01-08  JJE  $$37  Update MOZ_PLUGIN_PATH for JRE1.4
// 21-Jul-03 K-01-11  JJE  $$38  Add case for sgi_elf4, fix PLUGIN_PATH
// 08-Sep-03 K-01-14  JJE  $$39  Update sun to ns610
// 23-Sep-03 K-01-15  JJE  $$40  Add setting of JAVA_HOME on sgi
// 12-Dec-03 K-01-21  TWH  $$41  Fix appmgr handling
// 16-Mar-04 K-01-25+ MAZ  $$42  SPR 1075618: update linux moz_plug to ns610
// 04-Oct-04 K-03-11  TWH  $$43  Add PVIEW_PVIMPORT_HOME
// 13-Apr-05 K-03-22+ MAZ  $$44  SPR 1139819: update MOZ_PLUGIN_PATH to JRE 1.5 
// 17-May-05 K-03-25  MAZ  $$45  SPR 1144354: rm PRO_PHOTORENDER
// 25-OCT-05 K-03-34  MTP  $$46  sun_solaris_x64 support
// 01-Nov-05 K-03-34+ MAZ  $$47  SPR 1169567: rm empty line before include line
// 28-Nov-05 K-03-37  TWH  $$48  Remove Galaxy ref.
// 18-Jan-08 L-01-42  TWH  $$49  Update PVIEW env var.
// 13-Apr-09 L-03-30  TWH  $$50  include preset MOZ_PLUGIN_PATH
// 05-Apr-10 L-03-38  TWH  $$51  Update CV version
#include:head.inc

#include:shlib.inc

#includeenv:ALWAYS_TRUE:ilinkdir.env
#includeenv:ILINK_CMD_NAME:ilinkcmd.env

#include:mechdir.env

setenv CV_ENV_HOME $prodir/$mc/CV140
setenv PTC_CVLIB ${CV_ENV_HOME}/usr/apl/cvdors/lib:${CV_ENV_HOME}/usr/apl/cvdors/stubs

#include:shlibproe.inc
#include:wildfire.inc
#include:sun4_tmpdir.inc

# ptc-cmdtype: proe

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
if ($mc =~ hp*) then
    setenv _VFORK_NO_CMA true
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

#include:appmgr.inc

setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg
setenv PROE_START $0
setenv SPG_DIRECTORY $prodir
setenv PRO_USRMAIN $prodir/usrprog/umain/usrmain.c
setenv PROTABLE_DIRECTORY $prodir/protable
setenv PROTAB $prodir/$mc/obj/protab
setenv CLHOME $prodir/text/pcldata
setenv PVIEW_PROEPVIEW_HOME $prodir/apps/prodview
setenv VC_VCONF_START $prodir/$mc/obj/ptcvconf
setenv ZIP_EXEC $prodir/$mc/obj/zip
setenv PVX_PATH $prodir/xplatform/jar/nodebug
#includeenv:MECH_DEMO:mechdemo.env
#includeenv:PRO_SURF_FOUNDATION:surffnd.env

set moz_plug = "J2RE14/plugin/platform/ns600"
switch ($PRO_MACHINE_TYPE)
  case "hpux_pa64"
    set moz_plug = "JRE/plugin/PA_RISC2.0/mozilla"
  breaksw
  case "i486_linux"
    set moz_plug = "JRE/plugin/i386/ns7"
  breaksw
  case "sun4_solaris_64"
    set moz_plug = "JRE/plugin/sparc/ns7"
  breaksw
  case "sun_solaris_x64"
    set moz_plug = "JRE/plugin/i386/ns7"
  breaksw
  default
  breaksw
endsw

if ($?MOZ_PLUGIN_PATH) then
setenv MOZ_PLUGIN_PATH $PRO_DIRECTORY/$PRO_MACHINE_TYPE/obj/${moz_plug}:${MOZ_PLUGIN_PATH}
else
setenv MOZ_PLUGIN_PATH $PRO_DIRECTORY/$PRO_MACHINE_TYPE/obj/$moz_plug
endif

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv PROE_FEATURE_NAME "__PROE_FEATURE_NAME__"

#include:ptc_startup.inc

$prodir/$mc/obj/pro $*
exit $status
