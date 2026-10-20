// 07-May-98 $$1  TWH   Created
// 26-Aug-98 $$2  TWH   rm shlib, add $* to cmdline, rm graphics type chk; per dottley
// 18-Feb-99      JJE   Add DES_LIB
// 02-Mar-99      JJE   Add appmgr
// 05-Mar-99 $$3  JJE   Add pro_comm_msg, nmsd
// 13-Jan-00 $$4  OTT   Added LD_LIBRARYN32_PATH
// 23-Jan-01 $$5  TWH   Sun4_solaris_64 changes
// 19-Mar-01 $$6  ALG   Include ptc_startup.inc
#include:head.inc
#include:sun4_tmpdir.inc

# ptc-cmdtype: 3dpaint

if ($mc == alpha_unix) then
    limit datasize unlimited
    limit stacksize unlimited
endif

#
# set PAINT_HOME and PAINT_DATA
#
setenv PAINT_HOME $prodir
setenv PAINT_DATA $prodir/tutorial

#
# Location of cdrs libraries
#
#includeenv:DES_LIB:cdrs_library.env

#
# Set path to find executables
#
set path = ($PAINT_HOME/bin $path)

#
# Set LD_LIBRARY_PATH
#
if ($?LD_LIBRARY_PATH) then
        setenv LD_LIBRARY_PATH $PAINT_HOME/libraries/$mc/lib_shared:$LD_LIBRARY_PATH
else
        setenv LD_LIBRARY_PATH $PAINT_HOME/libraries/$mc/lib_shared
endif

if ($?LD_LIBRARYN32_PATH) then
  setenv LD_LIBRARYN32_PATH $PAINT_HOME/libraries/$mc/lib_shared:${LD_LIBRARYN32_PATH}
else
  setenv LD_LIBRARYN32_PATH $PAINT_HOME/libraries/$mc/lib_shared
endif

#
# Start 3DPaint process
#
#includeenv:PTCNMSPORT:ptcnmsport.env
setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg
#includeenv:PTC_APPMGR:appmgr.env
#includeenv:LM_LICENSE_FILE:lmlic.env
setenv PAINT_FEATURE_NAME "__3DPAINT_FEATURE_NAME__"

#include:ptc_startup.inc

#
# disable autorepeat on keys
#
#xset r off

$prodir/$mc/obj/3dpaint_bin $*

#xset r on
#end


