// 08-Jul-98 $$1  TWH  Created
// 11-Aug-98      TWH  Fix missing feat name
// 14-Aug-98      JJE  Support spaces in directories
// 17-Aug-98      TWH  Fix missing appmgr
// 18-Aug-98      TWH  Chg license env var
// 19-Aug-98      TWH  Fix appmgrdir, and feature_name
// 01-Sep-98      JJE   Changes for spaces in directories
// 03-Sep-98 $$2  TWH  Fix $prodir; add lmlic.env
// 17-Sep-98 $$3  TWH  Don't need nlocli.inc, rm -nograb
// 02-Nov-98 $$4  TWH  Add ptc_end label
// 20-Nov-98 $$5  TWH  Add PDM_LDB_PATH check
// 19-Apr-99      TWH  Use Start on command line
// 22-Apr-99 $$6  TWH  Set ORA_NLS32 to oracle734 per echin
// 13-Mar-00      TWH  Undo use of Start on command line 805815
// 16-Mar-00 $$7  TWH  move to Oracle8/Ilink 3.0
// 11-Apr-00      TWH  Fix redirect
// 18-Apr-00      TWH  Java changes
// 19-Apr-00      TWH  cmdline fixes
// 25-Apr-00      TWH  more fixes to cmdline; fix ORA_NLS33; fix cmd-type
// 27-Apr-00      TWH  Add ORACLE_HOME
// 05-May-00      TWH  Take $mc out of PTC_JRE paths; mv ORACLE_HOME
// 10-May-00 $$8  TWH  Change ORACLE_HOME value 
// 18-May-00 $$9  TWH  Drop use of pdm_proe.env
// 13-Sep-00 $$10 TWH  Change look-n-feel to Windows; mdimpex
// 20-Oct-00 $$11 TWH  fix intralinkparams 844441
// 01-Dec-00      TWH/JFU change CLASSPATH; set JAVA_HOME
// 01-Dec-00      TWH/JFU modify pro_comm_msg source
// 01-Dec-00 $$12 TWH/JFU FCS workarounds PTC_DOCS, PTC_HELP
// 08-Jan-01 $$13 TWH  check external setting of PRO_TEMPLATES
// 12-Jan-01 $$14 TWH  Add ptc_startup check 
// 22-Mar-01 $$15 TWH  Set INTRALINK_DIR
// 09-Apr-01 $$16 TWH  Add rsexplorer check 
// 11-Apr-01 $$17 TWH  chg to -Xmx256m -ss8m per yuri
// 30-Apr-01 $$18 TWH  Drop -oss4m per fung
// 27-Jun-01 $$19 TWH  Undo workarounds PTC_DOCS
// 22-Aug-01 $$20 TWH  Very ugly .proi.log file versioning
// 12-Nov-01 $$21 TWH  Set ORA_NLS33 to ORACLE817
// 11-Dec-01 $$22 TWH  rm classpath; chg run cmd per 916237
// 04-Mar-02 $$23 TWH  placeholder for license feature env name
// 03-Apr-02      TWH  fix hundred's place in .proi.log numbering 940255
// 12-Apr-02      TWH  use start /B
// 16-Apr-02 $$24 TWH  Don't quote javaw path
// 05-Feb-03 J-03-41+  $$25  TWH Reorder PROE_DIR before PROE_START
// 28-Apr-03 K-01-06 $$26 TWH Add exit
// 23-Jul-03 K-01-11 $$27 TWH Use proe wrapper scripts
// 21-Aug-03 K-01-13 $$28 TWH Fix above
// 24-May-04 K-03-02 $$29 TWH Drop NLS; Set/Chg ORACLE_HOME
// 03-Jun-04 K-03-03 $$30 TWH Drop PTC_JRE/bin from path env var.
// 06-Jan-05 K-03-17 $$31 TWH if PROE_DIR use proe_mc
// 18-APR-05 K-03-23 $$32 MTP Use head_ilink
// 29-Jul-05 K-03-30 $$33 TWH Drop win95
// 28-Nov-05 K-03-37 $$34 TWH Remove Galaxy ref.
// 13-Mar-07 K-03-47 $$35 TWH Add missing label pdmldpathset
// 22-Aug-07 K-03-50 $$36 TWH Chg path to ORACLE9208
// 17-Jan-08 L-01-42 $$37 TWH Chg path to ORACLE10203
#include:head_ilink.inc

REM ptc-cmdtype: ilink

set INTRALINK_DIR=%PRO_DIRECTORY%

if NOT "%PDM_LDB_PATH%" == "" goto pdmldbpathset
  if "%HOMEDRIVE%" == "" goto pdmldbpathset
    if "%HOMEPATH%" == "" goto pdmldbpathset
      set PDM_LDB_PATH=%HOMEDRIVE%%HOMEPATH%
:pdmldbpathset

set intralinkparams=""
:aloop
if "%1" == "" goto bkloop
if NOT "%1" == "--" goto doloop
  shift
  set intralinkparams=%1 %2 %3 %4 %5 %6 %7 %8 %9
:doloop
  shift
  goto aloop
:bkloop

set logfile=".proi.log"
if [%E2%]==[] set E2=0
if [%E1%]==[] set E1=0
if [%E0%]==[] set E0=0
set E2=0
set E1=0
set E0=0
:bloop
:E0
if %E0%==9 goto E1
if %E0%==8 set E0=9
if %E0%==7 set E0=8
if %E0%==6 set E0=7
if %E0%==5 set E0=6
if %E0%==4 set E0=5
if %E0%==3 set E0=4
if %E0%==2 set E0=3
if %E0%==1 set E0=2
if %E0%==0 set E0=1
goto edone
:E1
set E0=0
if %E1%==9 goto E2
if %E1%==8 set E1=9
if %E1%==7 set E1=8
if %E1%==6 set E1=7
if %E1%==5 set E1=6
if %E1%==4 set E1=5
if %E1%==3 set E1=4
if %E1%==2 set E1=3
if %E1%==1 set E1=2
if %E1%==0 set E1=1
goto edone
:E2
set E1=0
if %E2%==9 set E2=0
if %E2%==8 set E2=9
if %E2%==7 set E2=8
if %E2%==6 set E2=7
if %E2%==5 set E2=6
if %E2%==4 set E2=5
if %E2%==3 set E2=4
if %E2%==2 set E2=3
if %E2%==1 set E2=2
if %E2%==0 set E2=1
REM goto E0
:edone
if %E2%==0 goto et1
set count=%E2%%E1%%E0%
goto et
:et1
if %E1%==0 goto et0
set count=%E1%%E0%
goto et
:et0
set count=%E0%
:et
set logfile=".proi.log.%count%"
if EXIST "%logfile%" goto d2loop
goto bzloop
:d2loop
goto bloop
:bzloop

set PTC_JRE=__PTC_JRE__
set JAVA_HOME=__PTC_JRE__

set path=%PRO_DIRECTORY%\%mc%\lib;%PRO_DIRECTORY%\%mc%\obj;%path%

#includeenv:PROE_DIR:mdimpex.env
#includeenv:PROE_START:f_proe_start.env

#includeenv:PTCNMSPORT:ptcnmsport.env

#includeenv:PTC_APPMGR:appmgr.env
#includeenv:PTC_APPMGR_DIR:appmgrdir.env

set PDM_ORA_APPL=intralink
set PDM_ORACLE_SERVER=intralink
set TNS_ADMIN=%prodir%

set ORACLE_HOME=%prodir%\%mc%\obj\ORACLE10203
set EPC_DISABLED=TRUE
set path=%ORACLE_HOME%\bin;%path%

#includeenv:LM_LICENSE_FILE:lmlic.env
set __FEAT_ENV__=__ILINK_FEATURE_NAME__

if "%PROE_DIR%" == "" goto pimsg
set PRO_COMM_MSG_EXE=%PROE_DIR%\%proe_mc%\obj\pro_comm_msg
goto pstmsg
:pimsg
set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg
:pstmsg

set NAME_SERVICE_EXE=%prodir%\%mc%\obj\name_service

if EXIST "%prodir%\%mc%\obj\pdm_edit_copy.exe" set PDM_ED_CP_PATH=%prodir%\%mc%\obj\pdm_edit_copy.exe

if EXIST "%prodir%\install\nt\docmgr_loc.bat" call "%prodir%\install\nt\docmgr_loc.bat"

if NOT "%PRO_TEMPLATES%" == "" goto pstemp
set PRO_TEMPLATES=%prodir%\pdm\templates
:pstemp
set EPC_DISABLE=TRUE

if EXIST "%prodir%\rsexplorer.ini" set RSDB_INI_FILE=%prodir%\rsexplorer.ini

#include:ptc_startup.inc

start /B %prodir%\%mc%\jre\bin\javaw -Xmx256m -Xss8m -jar %PRO_DIRECTORY%\java\lib\Intralink.jar -l %luknfeel% %intralinkparams% >> "%logfile%"

:ptc_end
exit /B %ERRORLEVEL%
