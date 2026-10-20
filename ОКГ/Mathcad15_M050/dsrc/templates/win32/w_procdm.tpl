// 13-Jul-98 $$1  JJE  Created
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98 $$2  JJE  Changes for spaces in directories
// 28-Apr-03 K-01-06 $$3 TWH Add exit
#include:head.inc

REM ptc-cmdtype: procdm

set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg.exe

#includeenv:PROE_START:proe_start.env
#includeenv:LM_LICENSE_FILE:lmlic.env
set PROCDM_FEATURE_NAME=__CDM_FEATURE_NAME__


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

%start_cmd% %slashb% %slashw% "%PRO_DIRECTORY%\%MC%\obj\procdm" %1 %2 %3 %4 %5
:ptc_end
exit /B %ERRORLEVEL%

