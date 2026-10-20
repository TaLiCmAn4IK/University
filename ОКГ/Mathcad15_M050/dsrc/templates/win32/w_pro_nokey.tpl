// 23-Sep-97 $$1  JJE  Created.
// 23-Sep-97 $$2  JJE  Change ncpost tag
// 26-Jan-98 $$3  JJE   Move setvars call to head.inc
// 18-Feb-98 $$4  JJE  Remove NAME_SERVICE_EXE
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98 $$5  JJE  Changes for spaces in directories
// 28-Apr-03 K-01-06 $$6 TWH Add exit
// 17-May-05 K-03-25 $$7 MAZ SPR 1144354: rm PRO_PHOTORENDER
// 28-Nov-05 K-03-37 $$8 TWH Remove GALAXYHOME ref.
#include:head.inc

REM ptc-cmdtype: proe

set PRONCPOST_START=__NCPOST_CMD__

if "%CDRS_DATA%" == "" set CDRS_DATA=%prodir%
set PRO_IMAGE_CONVERTER=%prodir%\%mc%\obj\convert_image.exe

#includeenv:PTC_APPMGR:appmgr.env
#includeenv:PTC_APPMGR_DIR:appmgrdir.env
set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg.exe
set PROGUIDE_DIRECTORY=%prodir%\uifdoc
set SPG_DIRECTORY=%\prodir%
set PRO_USRMAIN=%prodir%\usrprog\umain\usrmain.c
set PROTABLE_DIRECTORY=%prodir%\protable
set PROTAB=%prodir%\%mc%\obj\protab.exe
set CLHOME=%prodir%\text\pcldata

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

%start_cmd% %slashb% %slashw% "%PRO_DIRECTORY%\%MC%\obj\xtop.exe" %1 %2 %3 %4 %5
:ptc_end
exit /B %ERRORLEVEL%

