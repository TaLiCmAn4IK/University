// 13-May-98 $$1  TWH   Created.
// 15-May-98 $$2  TWH   Add HW_FLAVOR, extend path w/ obj dir
// 14-Aug-98      JJE   Support spaces in directories
// 01-Sep-98      JJE   Changes for spaces in directories
// 08-Sep-98 $$3  JJE   Changes
// 18-Feb-99      JJE   Add DES_LIB
// 02-Mar-99      JJE   Add appmgr
// 05-Mar-99 $$4  JJE   Add pro_comm_msg, nms
// 19-Mar-01 $$5  ALG   Include ptc_startup.inc
// 28-Apr-03 K-01-06 $$6 TWH Add exit
// 29-Jul-05 K-03-30 $$7 TWH Drop win95 reference
#include:head.inc

REM ptc-cmdtype: 3dpaint

if "%PAINT_HOME%" == "" set PAINT_HOME=%PRO_DIRECTORY%

if NOT "%PAINTOBJPATH%" == "" goto paintobjpath
set path=%PAINT_HOME%\%MC%\obj;%path%
set PAINTOBJPATH=true
:paintobjpath

REM set PAINT_HOME and PAINT_DATA

set PAINT_DATA=%PAINT_HOME%\tutorial
#includeenv:DES_LIB:cdrs_library.env

set MACHINE=%MC%

#includeenv:PTCNMSPORT:ptcnmsport.env
set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg.exe
#includeenv:PTC_APPMGR:appmgr.env
#includeenv:LM_LICENSE_FILE:lmlic.env
set PAINT_FEATURE_NAME=__3DPAINT_FEATURE_NAME__

#include:ptc_startup.inc

%start_cmd% /b "%PAINT_HOME%\%MC%\obj\3dpaint_bin" %1 %2 %3 %4 %5

:ptc_end
exit /B %ERRORLEVEL%
