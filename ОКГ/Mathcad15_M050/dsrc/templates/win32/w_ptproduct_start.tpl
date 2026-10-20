// 14-May-98 $$1  ZRL  Created.
// 26-May-98      ZRL  use PTMODELER_FEATURE_NAME as feature name
// 27-May-98      ZRL  change cmdtype to ptprod
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98 $$2  JJE  Changes for spaces in directories
// 28-Apr-03 K-01-06 $$3 TWH Add exit
// 17-May-05 K-03-25 $$4 MAZ SPR 1144354: rm PRO_PHOTORENDER 
// 28-Nov-05 K-03-37 $$5 TWH Remove GALAXYHOME ref.
#include:head.inc

REM ptc-cmdtype: ptprod

#includeenv:PRONCPOST_START:ncpstart.env

#includeenv:PTCNMSPORT:ptcnmsport.env

set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg.exe

#includeenv:CDRS_DATA:cdrsdata.env
if "%CDRS_DATA%" == "" set CDRS_DATA=%prodir%
set PRO_IMAGE_CONVERTER=%prodir%\%mc%\obj\convert_image.exe

#includeenv:PTC_APPMGR:appmgr.env

set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg.exe
set PROGUIDE_DIRECTORY=%prodir%\uifdoc
set SPG_DIRECTORY=%\prodir%
set PRO_USRMAIN=%prodir%\usrprog\umain\usrmain.c
set PROTABLE_DIRECTORY=%prodir%\protable
set PROTAB=%prodir%\%mc%\obj\protab.exe
set CLHOME=%prodir%\text\pcldata

#includeenv:LM_LICENSE_FILE:lmlic.env
set PTMODELER_FEATURE_NAME=__PTMODELER_FEATURE_NAME__

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

%start_cmd% %slashb% %slashw% "%PRO_DIRECTORY%\%MC%\obj\projr" %1 %2 %3 %4 %5
:ptc_end
exit /B %ERRORLEVEL%

