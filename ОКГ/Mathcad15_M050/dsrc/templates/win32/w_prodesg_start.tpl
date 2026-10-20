// 13-Apr-98 $$1  JJE   Created.
// 15-Apr-98 $$2  JJE   Fixed.
// 13-May-98 $$3  TWH   Fixed CDRS_LIBRARY, FEATURE_NAME
// 15-May-98 $$4  TWH   Rm NMSD
// 03-Jun-98      TWH   Add PAINT_CMD,PROE_START,DES_LIB
// 28-Jul-98      TWH   Change label to cdrspathset
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98      JJE   Changes for spaces in directories
// 08-Sep-98 $$5  JJE   Remove animate line
// 14-Sep-98 $$6  TWH   includeenv:CDRS_DIR
// 06-Oct-98      TWH   move PROE_START;Add if defined check
// 14-Oct-98 $$7  TWH   Add: set CDRS=%PRODIR%
// 16-Dec-98 $$8  TWH   Rm CDRS_DATA line per dottley
// 25-Feb-99      TWH   Fix % on PROE_START
// 02-Mar-99      JJE   Add appmgr
// 05-Mar-99 $$9  JJE   Add nmsd
// 20-Oct-99 $$10 OTT   Changed env var CDRS_SOCKET, CDRS_POPEN_SOCKET
// 22-Nov-99 $$11 OTT   Added DIVISIONROOT
// 19-Mar-01 $$12 ALG   Include ptc_startup.inc
// 28-Apr-03 K-01-06 $$13 TWH Add exit
// 29-Jul-05 K-03-30 $$14 TWH Drop win95
#include:head.inc

REM ptc-cmdtype: cdrs

#includeenv:CDRS_DIR:cdrs.env
if "%CDRS%" == "" set CDRS=%PRODIR%

if NOT "%PROE_START%" == "" goto proestartset
#includeenv:PROE_START:proe_start.env
:proestartset

// REM Location of model directories
// if "%CDRS_DATA%" == "" set CDRS_DATA=%CDRS%

#includeenv:DES_LIB:cdrs_library.env
if "%CDRS_LIBRARY%" == "" set CDRS_LIBRARY=%CDRS%\graphic-library

#includeenv:DIVROOT:divisionroot.env

if NOT "%CDRSPATH%" == "" goto cdrspathset

set path=%CDRS%\bin;%CDRS%\%MC%\obj;%path%

set CDRSPATH=true
:cdrspathset

if "%ALLEGRO_CL_HOME%" == "" set ALLEGRO_CL_HOME=%CDRS%\%MC%\obj

set PRO_DIRECTORY=%CDRS%
set MACHINE=%MC%

#includeenv:PAINT_CMD:paint_cmd.env
#includeenv:PTC_APPMGR:appmgr.env

#includeenv:LM_LICENSE_FILE:lmlic.env
set DESIGNER_FEATURE_NAME=__DESIGNER_FEATURE_NAME__

#includeenv:PTCNMSPORT:ptcnmsport.env
set PRO_COMM_MSG_EXE=%CDRS%\%MC%\obj\pro_comm_msg.exe

#include:ptc_startup.inc

REM Start cdrs manager process
%start_cmd% /b "%CDRS%\%MC%\obj\dmgr" %1 %2 %3 %4 %5

REM  End of script
:ptc_end
exit /B %ERRORLEVEL%

