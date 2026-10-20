// 04-Mar-02 ##1  TWH  Created
// 07-Mar-02 $$1  TWH  Add u_ora8_shlib.inc & ORACLE_HOME
// 28-May-02 $$2  TWH  Fix #1
// 16-Apr-03 K-01-06 $$3  TWH  64 bit fallback
// 02-Oct-03 K-01-16 $$4 TWH  rm PGL_DEBUG_GRAPHICS hack
// 25-Feb-04 K-01-26 $$5 TWH  java chg'd sgi arguments
// 12-May-04 K-03-02 $$6 TWH  Chg ORACLE_HOME value
// 22-Aug-07 K-03-50 $$7 TWH  Chg path to ORACLE9208
// 17-Jan-08 L-01-42 $$8 TWH  Chg path to ORACLE10203
// 10-Mar-08 L-03-05 $$9 TWH  Intralink now support 64 bit
// 21-Apr-08 L-03-07 $$10 TWH Chg setting of "format"
// 29-May-08 L-03-10 $$11 TWH  Add -DPTC_MRC_HOST=.. to run cmd
#include:head.inc

set libset="$prodir/$mc/obj/ps_libset"
if ( ! -x $libset ) unset libset

setenv INTRALINK_DIR $prodir
setenv TNS_ADMIN $prodir
setenv ORACLE_HOME $prodir/$mc/obj/ORACLE10203

set intralink_params = "$*"
setenv PTC_JRE __PTC_JRE__
setenv JAVA_HOME __PTC_JRE__

#include:ora8_shlib.inc
#include:jre_shlib.inc

if ($mc =~ sgi*) then
  if (! $?LD_LIBRARYN32_PATH ) then
    if ($?LD_LIBRARY64_PATH ) then
      setenv LD_LIBRARYN32_PATH ${LD_LIBRARY64_PATH}
    endif
  endif
  if ($?LD_LIBRARYN32_PATH) then
    if ($?LD_LIBRARY_PATH ) then
      setenv LD_LIBRARY_PATH ${LD_LIBRARYN32_PATH}:${LD_LIBRARY_PATH}
    else
      setenv LD_LIBRARY_PATH ${LD_LIBRARYN32_PATH}
    endif
  endif
endif

set native=""
set format=""

if ($mc == sun4_solaris_64) then
  set format="-d64"
endif

#include:ptc_startup.inc

__JVM_CMD__ $native $format -DPTC_MRC_HOST=$PTC_MRC_HOST -DPTC_MRC_PORT=$PTC_MRC_PORT -DPTC_MRC_SID=$PTC_MRC_SID -jar ${prodir}/java/lib/mrc.jar $intralink_params

