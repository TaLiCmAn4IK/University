// 23-Apr-98 $$1  TWH  Created 
// 13-Jul-98 $$2  TWH  Chg license env var.
// 01-Sep-98 $$3  TWH  Add PTC_APPMGR_DIR, fix feature env var, NLS_LANG support
// 17-Oct-98 $$4  TWH  rm -nograb arg
// 14-Oct-98 $$5  TWH  LC check more verbose
// 23-Mar-99      TWH  add mips4 to elf2 switch
// 22-Apr-99      TWH  Set ORA_NLS32 to oracle734 per echin
// 05-May-99      JJE  Group Submission
// 10-May-99 $$6  TWH  fix nlslang scripting
// 17-Mar-00      TWH  Move to Oracle8 Ilink 3.0
// 21-Mar-00 $$7  TWH  Comment out multiple luknfeel options
// 18-Apr-00      TWH  Java changes
// 19-Apr-00      TWH  Cmdline fixes
// 26-Apr-00      TWH  Fix ORA_NLS33; fix cmd-type
// 05-May-00 $$8  TWH  rm $mc in PTC_JRE paths
// 18-May-00 $$9  TWH  drop inc pdm_proe.env
// 13-Sep-00 $$10 TWH  tweaks & mdimpex
// 01-Dec-00      TWH/JFU change CLASSPATH; Add JAVA_HOME
// 01-Dec-00      TWH/JFU modify pro_comm_msg source
// 01-Dec-00      TWH  FCS workarounds; PGL..., PTC_DOCS, PTC_HELP, XFILE..
// 05-Dec-00      TWH  more of #14
// 05-Dec-00 $$11 TWH  cancel above
// 08-Jan-01 $$12 TWH  check set of PRO_TEMPLATES external
// 12-Feb-01 $$13 TWH  Add ptc_startup check
// 19-Mar-01 $$14 TWH  Set INTRALINK_DIR
// 09-Apr-01 $$15 TWH  Fix #12; Add rsexplorer check
// 11-Apr-01 $$16 TWH  Chg to -Xmx256m -ss8m per yuri
// 20-Apr-01 $$17 TWH  Hack in sgi/jap block attempt
// 24-Apr-01 $$18 TWH  fix #17
// 30-Apr-01 $$19 TWH  drop -oss4m per fung
// 27-Jun-01 $$20 TWH  Undo workarounds PTC_DOCS, PTC_HELP #11 853155
// 06-Aug-01 $$21 TWH  LC_NUMERIC chk 893079
// 19-Sep-01      TWH  Undo #17 (sgi/jap hack)
// 19-Sep-01 $$22 TWH  Undo mips4 to elf2 switch (head.inc handles now)
// 12-Nov-01 $$23 TWH  Set ORA_NLS33 to ORACLE817
// 11-Dec-01 $$24 TWH  Drop CLASSPATH;Chg run line per 916237
// 04-Mar-02 $$25 TWH  placeholder for license feature env name
// 20-Jan-03 J-03-40+ $$26 TWH  Add NO_J2D_DGA 987638
// 05-Feb-03 J-03-41+ $$27 TWH  Reorder PROE_DIR before PROE_START
// 13-Feb-03 J-03-41+ $$28 TWH  Intralink does not support 64 bit
// 15-Apr-03 K-01-06  $$29 TWH  Fallback to mips4; also set LD_LIBRARY_PATH
// 23-Jul-03 K-01-11  $$30 TWH  Fix missing endif
// 02-Oct-03 K-01-16  $$31 TWH  undo PGL hack
// 25-Feb-04 K-01-26  $$32 TWH  java chg'd sgi arguments
// 24-May-04 K-03-02  $$33 TWH  Drop NLS; Set ORACLE_HOME
// 03-Jun-04 K-03-03  $$34 TWH  Drop PTC_JRE/bin from path env var.
// 22-Aug-07 K-03-50  $$35 TWH  Chg path to ORACLE9208
// 17-Jan-08 L-01-42  $$36 TWH  Chg path to ORACLE10203 
// 10-Mar-08 L-03-05  $$37 TWH  Intralink now support 64 bit
// 21-Apr-08 L-03-07  $$38 TWH  Chg setting of "format"
// 15-Jul-09 L-03-34  $$39 TWH  drop luknfeel
#include:head.inc

set libset="$prodir/$mc/obj/ps_libset"
if ( ! -x $libset ) unset libset

// set for use by proe and others started by INTRALINK
setenv INTRALINK_DIR $prodir

set luknfeel="Metal"

foreach i ($*)
  if ("$i" != "--") then
    shift
  else
    shift
    break
  endif
end

set intralink_params = "$*"

#include:shlib.inc

if ($mc == alpha_unix) then
  if ($?LD_LIBRARY_PATH) then
    setenv LD_LIBRARY_PATH ${LD_LIBRARY_PATH}:$prodir/$mc/obj
  else
    setenv LD_LIBRARY_PATH $prodir/$mc/obj
  endif
endif
if ($mc == ibm_rs6000) then
      setenv LIBPATH ${LIBPATH}:$prodir/$mc/stublib
endif
if ($mc =~ sun4_solaris*) then
    setenv NO_J2D_DGA true
endif

#include:ora8_shlib.inc

setenv PTC_JRE __PTC_JRE__
setenv JAVA_HOME __PTC_JRE__

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

set path=($prodir/$mc/lib $prodir/$mc/obj $path)

# ptc-cmdtype: ilink

#includeenv:PROE_DIR:mdimpex.env
#includeenv:PROE_START:proe_start.env
if ($?PROE_START) then
  setenv PDM_PROE $PROE_START
  setenv PDM_PROE_PATH $PDM_PROE
endif

#includeenv:PTCNMSPORT:ptcnmsport.env

#includeenv:PTC_APPMGR:appmgr.env
#includeenv:PTC_APPMGR_DIR:appmgrdir.env 

if ($?PROE_DIR) then
  setenv PRO_COMM_MSG_EXE $PROE_DIR/$proe_mc/obj/pro_comm_msg
else
  setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg
endif

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv __FEAT_ENV__ "__ILINK_FEATURE_NAME__"

setenv PDM_ORA_APPL intralink
setenv PDM_ORACLE_SERVER intralink
setenv TNS_ADMIN $prodir
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

set native=""
set format=""

if ($mc == sun4_solaris_64) then
  set format="-d64"
endif

if (! $?PRO_TEMPLATES) then
	setenv PRO_TEMPLATES $prodir/pdm/templates
endif
setenv EPC_DISABLE TRUE
if ($?XFILESEARCHPATH) then
	setenv XFILESEARCHPATH_SAVE $XFILESEARCHPATH
endif

if (-e $prodir/rsexplorer.ini) then
  setenv RSDB_INI_FILE $prodir/rsexplorer.ini
endif

#include:ptc_startup.inc

__JVM_CMD__ $native $format -Xmx256m -ss8m -jar ${prodir}/java/lib/Intralink.jar $intralink_params >& .proi.log

