// 23-Sep-97 $$1  JJE  Created.
// 26-Jan-98 $$2  JJE   Move setvars call to head.inc
// 11-Feb-98 $$3  JJE  Use 200
// 18-Feb-98 $$4  JJE  Remove NAME_SERVICE_EXE
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98 $$5  JJE   Changes for spaces in directories
// 12-Oct-98 $$6  JJE  Use 210
// 07-Aug-00 $$7  TWH  fix ptc-cmdtype line
// 28-Apr-03 K-01-06 $$8 TWH Add exit
// 17-May-05 K-03-25 $$9 MAZ SPR 1144354: rm PRO_PHOTORENDER
// 28-Nov-05 K-03-37 $$10 TWH Remove GALAXYHOME ref.
#include:head.inc

REM ptc-cmdtype: nc_post

set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg.exe

if "%CDRS_DATA%" == "" set CDRS_DATA=%prodir%
set PRO_IMAGE_CONVERTER=%prodir%\%mc%\obj\convert_image.exe

#includeenv:PTC_APPMGR:appmgr.env

#includeenv:LM_LICENSE_FILE:lmlic.env
set NCPOST_FEATURE_NAME=__NCPOST_FEATURE_NAME__

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

%start_cmd% %slashb% %slashw% "%PRO_DIRECTORY%\%MC%\obj\ncp210q.exe" %1 %2 %3 %4 %5
:ptc_end
exit /B %ERRORLEVEL%
