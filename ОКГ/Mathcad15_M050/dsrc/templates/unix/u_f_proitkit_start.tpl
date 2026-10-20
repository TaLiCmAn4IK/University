// 23-Apr-98 $$1  TWH  Created 
// 01-Sep-98 $$2  TWH  fix typo, Chg license env var, fix feature name
// 17-Sep-98 $$3  TWH  nmsd not needed
// 09-Feb-99      TWH  Added set tkdir=
// 22-Apr-99      TWH  Set ORA_NLS32 to oracle734 per echin
// 05-May-99 $$4  JJE  Group Submission
// 01-May-00      TWH  Mv to Oracle816
// 05-May-00 $$5  TWH  Use ora8_shlib; cmdtype tkenv; Add ORACLE_HOME
// 18-May-00 $$6  TWH  drop inc pdm_proe.env
// 19-Mar-01 $$7  ALG  Include ptc_startup.inc
// 12-Nov-01 $$8  TWH  Set ORA_NLS33 to ORACLE817
// 09-Dec-02 J-03-40 $$9  TWH  Cp NLS_LANG setting logic from intralink_start
// 20-Jan-03 J-03-40+ $$10 TWH  Add NO_J2D_DGA 987638
// 13-Feb-03 J-03-41+ $$11 TWH  Intralink does not support 64 bit
// 15-Apr-03 K-01-06  $$12 TWH  fallback to mip4; set LD_LIBRARY_PATH also
// 23-Jul-03 K-01-11  $$13 TWH  fix missing endif
// 19-Jul-04 K-03-06  $$14 TWH  Chg to ORACLE920
// 22-Aug-07 K-03-50  $$15 TWH  Chg path to ORACLE9208
// 17-Jan-08 L-01-42  $$16 TWH  Chg path to ORACLE10203
// 10-Mar-08 L-03-05  $$17 TWH  Intralink now support 64 bit
#include:head.inc

set libset="$prodir/$mc/obj/ps_libset"
if ( ! -x $libset ) unset libset

if ($mc =~ sun4_solaris*) then
    setenv NO_J2D_DGA true
endif

#include:shlib.inc

#include:ora8_shlib.inc

set tkdir=$prodir
#include:itk_shlib.inc

if ($mc =~ sgi*) then
  if (! $?LD_LIBRARYN32_PATH ) then
    if ($?LD_LIBRARY64_PATH ) then
      setenv LD_LIBRARYN32_PATH ${LD_LIBRARY64_PATH}
    endif
  endif
  if ($?LD_LIBRARYN32_PATH ) then
    if ($?LD_LIBRARY_PATH ) then
      setenv LD_LIBRARY_PATH ${LD_LIBRARYN32_PATH}:${LD_LIBRARY_PATH}
    else
      setenv LD_LIBRARY_PATH ${LD_LIBRARYN32_PATH}
    endif
  endif
endif

# ptc-cmdtype: tkenv

#includeenv:PROE_START:proe_start.env
if ($?PROE_START) then
  setenv PDM_PROE $PROE_START
  setenv PDM_PROE_PATH $PDM_PROE
endif

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv ILINK_TK_ACCESS_FEATURE_NAME "__ILINK_FEATURE_NAME__"

setenv PDM_ORA_APPL intralink
setenv PDM_ORACLE_SERVER intralink
setenv TNS_ADMIN $prodir
setenv ORA_NLS33 $prodir/$mc/obj/ORACLE10203/ocommon/nls/admin/data
setenv ORACLE_HOME $prodir/$mc/obj/ORACLE10203

if ( $?LC_NUMERIC || $?LC_ALL ) then
  if ($?LC_COLLATE) then
    setenv PTC_COLLATE $LC_COLLATE
  endif
# LC_COLLATE effects how "tr" performs
  setenv LC_COLLATE C
  set resetnls=false
  if ( $?LC_NUMERIC ) then
    if ( $LC_NUMERIC == "C" ) then
      set resetnls=true
        else if ( `echo $LC_NUMERIC | cut -c3` == '_' ) then
          if (`echo $LC_NUMERIC |cut -c1-2 | tr '[A-Z]' '[a-z]'` == "en") then
        set resetnls=true
      endif
    endif
  endif
  if ( $?LC_ALL ) then
    if ( $LC_ALL == "C") then
      set resetnls=true
    else if ($?LC_NUMERIC ) then
      if ( `echo $LC_NUMERIC | cut -c3` == '_' ) then
        if (`echo $LC_NUMERIC |cut -c1-2 | tr '[A-Z]' '[a-z]'` == "en") then
          set resetnls=true
        endif
      endif
    endif
  endif
  if ( "$resetnls" == "true" ) then
    setenv NLS_LANG AMERICAN_AMERICA.WE8ISO8859P1
    set i18n = 0
    if ( `echo $LANG | cut -c3` == '_' ) set i18n = 1
    if ($i18n) then
      set runlang = `echo $LANG | cut -c1-2 | tr '[A-Z]' '[a-z]'`
    else
      set runlang=$LANG
    endif
    switch ($runlang)
      # csh in japanese does not like cascading case statements...
      # so don't change.
      case "japan" :
          setenv NLS_LANG AMERICAN_AMERICA.JA16EUC
        breaksw
      case "japanese" :
          setenv NLS_LANG AMERICAN_AMERICA.JA16EUC
        breaksw
      case "japanese.euc" :
          setenv NLS_LANG AMERICAN_AMERICA.JA16EUC
        breaksw
      case "ja" :
          setenv NLS_LANG AMERICAN_AMERICA.JA16EUC
        breaksw
    endsw
  endif
  if ($?PTC_COLLATE) then
    setenv LC_COLLATE $PTC_COLLATE
  endif
endif

#include:ptc_startup.inc

# Insert full path to your Pro/INTRALINK Toolkit Application below
# Example:  /usr/local/apps/MyToolkitApp $*
