// 12-Aug-97 $$1  JJE  Created.
// 19-Sep-97 $$2  JJE  __SCRIPT_NAME__
// 23-Sep-97 $$3  JJE  Change ncpost tag
// 29-Dec-97 $$4  TWH  PTC_APPMGR_DIR removed
// 26-Jan-98 $$5  JJE   Move setvars call to head.inc
// 02-Feb-98 $$6  JJE  Add nmsd
// 03-Feb-98 $$7  TWH  nms via includeenv
// 18-Feb-98 $$8  JJE  Remove NAME_SERVICE_EXE
// 26-Jun-98 $$9  JJE   Set CV_ENV_HOME
// 22-Jul-98 $$10 TWH  Add ILIBPATH code
// 09-Aug-98 $$11 ZRL  Add the sixth argument
// 14-Aug-98      JJE   Support spaces in directories
// 01-Sep-98 $$12 JJE   Changes for spaces in directories
// 19-Oct-98 $$13 JJE   Only set pro_comm once
// 13-Nov-98      TWH  Add skipifbmode above head.inc (NT only)
// 13-Nov-98 $$14 TWH  Add missing PDMCDIR
// 15-Dec-98 $$15 JJE  Use CV90
// 30-Dec-98 $$16 TWH  Add $mc/lib for Pro/JAVA
// 10-Mar-99      TWH  limit labels to 8 chars for win95
// 15-Apr-99      JJE  Use ilinkdir include file
// 22-Apr-99      JJE  Also include ilinkpath
// 10-May-99 $$17 JJE  Remove extra deflib setting
// 08-Jun-99      JJE  Include ptc_startup, mechdemo
// 27-Sep-99 $$18 JJE  Sub to new build area
// 05-Jan-00 $$19 JJE   Add mcdir setting
// 11-Jan-00 $$20 JJE   Use CV102
// 21-Jan-00 $$21 JJE   No mcdir setting
// 09-Feb-00 $$22 JJE  Start portmapper
// 01-Mar-00 $$23 TWH  Undo #22 change in plans
// 15-Aug-00 $$24 JJE   Add PRO_SURF_FOUNDATION
// 28-Aug-00 $$25 JJE  Add PROE_START setting
// 20-Feb-01 $$26 JJE  Use CV110
// 19-Mar-01 $$27 TWH  Change for ilinkdir.env
// 20-Sep-01 $$28 TWH  exit /B %errorlevel% per watras
// 04-Oct-01 $$29 JJE  Don't do #28 on win95
// 08-Nov-01 $$30 TWH  remove #28 entirely
// 04-Jan-02 $$31 MAZ  Set PTC_XALAN_BIN
// 01-Feb-02 $$32 JJE  Set VCONF_START
// 05-Feb-02 $$33 JJE  Rename to VC_VCONF_START
// 21-Mar-02 $$34 JJE  Add ZIP_EXEC setting
// 28-May-02 $$35 TWH  redo #27 to includeenv "ALWAYS_TRUE"
// 21-Aug-02 J-03-32  ALG  $$36  Change "proevconf" -> "ptcvconf"
// 28-Apr-03 K-01-06 $$37 TWH Add exit
// 17-May-05 K-03-25 $$38 MAZ SPR 1144354: rm PRO_PHOTORENDER
// 29-Jul-05 K-03-30 $$39 TWH Drop win95
// 28-Nov-05 K-03-37 $$40 TWH Remove Galaxy ref.
// 25-May-07 L-01-32 $$41 Add env var for dbghelp.dll
// 16-May-08 L-03-09 $$42 TWH Add Mozilla entry
@echo off

if NOT "%PTC_BMODE%" == "" goto skipifbmode

#include:head.inc

REM ptc-cmdtype: proe

#includeenv:ALWAYS_TRUE:ilinkdir.env
#include:ilinkpath.inc

set path=%path%;%PRO_DIRECTORY%\%MC%\lib

#includeenv:PRONCPOST_START:ncpstart.env
#includeenv:PDMCDIR:pdmcdir.env
#includeenv:PTCNMSPORT:ptcnmsport.env

set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg.exe
set PROE_START=%prodir%\bin\__SCRIPT_NAME__.bat

#includeenv:CDRS_DATA:cdrsdata.env
if "%CDRS_DATA%" == "" set CDRS_DATA=%prodir%
set PRO_IMAGE_CONVERTER=%prodir%\%mc%\obj\convert_image.exe

#includeenv:PTC_APPMGR:appmgr.env

set PROGUIDE_DIRECTORY=%prodir%\uifdoc
set SPG_DIRECTORY=%\prodir%
set PRO_USRMAIN=%prodir%\usrprog\umain\usrmain.c
set PROTABLE_DIRECTORY=%prodir%\protable
set PROTAB=%prodir%\%mc%\obj\protab.exe
set ZIP_EXEC=%prodir%\%mc%\obj\zip.exe
set MOZILLA_FIVE_HOME=%prodir%\%mc%\obj\MOZILLA
set CLHOME=%prodir%\text\pcldata
set CV_ENV_HOME=%prodir%\%mc%\CV110
set PTC_XALAN_BIN=%prodir%\%mc%\obj\Xalan\bin
set VC_VCONF_START=%prodir%\%mc%\obj\ptcvconf.exe
set DEBUG_HLP_DLL_PATH=%prodir%\%mc%\lib\dbghelp.dll
#includeenv:MECH_DEMO:mechdemo.env
#includeenv:PRO_SURF_FOUNDATION:surffnd.env

#includeenv:LM_LICENSE_FILE:lmlic.env
set PROE_FEATURE_NAME=__PROE_FEATURE_NAME__

if exist "%PRO_DIRECTORY%\install\nt\pro_mech_loc.bat" set PRO_MECH_COMMAND=%0
if exist "%PRO_DIRECTORY%\install\nt\pro_mech_loc.bat" call "%PRO_DIRECTORY%\install\nt\pro_mech_loc.bat"
if NOT exist "%PRO_DIRECTORY%\install\nt\pro_mech_loc.bat" goto nomechloc
	if exist "%MECH_LP%\install\nt\pro_mech_env.bat" call "%MECH_LP%\install\nt\pro_mech_env.bat"
:nomechloc

:skipifbmode

set slashb=
if %MC% == i486_nt    set slashb=/b
if %MC% == r4000_nt   set slashb=/b
if %MC% == alpha_nt   set slashb=/b

set slashw=
if "%1" == "pro_wait" set slashw=/w
if "%2" == "pro_wait" set slashw=/w
if "%3" == "pro_wait" set slashw=/w
if "%4" == "pro_wait" set slashw=/w
if "%5" == "pro_wait" set slashw=/w
if "%6" == "pro_wait" set slashw=/w

if "%1" == "-v" set slashw=/w
if "%2" == "-v" set slashw=/w
if "%3" == "-v" set slashw=/w
if "%4" == "-v" set slashw=/w
if "%5" == "-v" set slashw=/w
if "%6" == "-v" set slashw=/w

#include:ptc_startup.inc

REM %start_cmd% "%PRO_DIRECTORY%\%MC%\obj\portmap.exe" -noservice
%start_cmd% %slashb% %slashw% "%PRO_DIRECTORY%\%MC%\obj\xtop" %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%
