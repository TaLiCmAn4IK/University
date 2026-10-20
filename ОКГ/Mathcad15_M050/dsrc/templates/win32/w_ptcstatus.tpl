// 16-Sep-97  $$1  JJE  Created
// 12-Jan-98  $$2  JJE  Don't check LM set
// 23-Jan-98  $$3  JJE  Use __LICENSE_RES__
// 26-Jan-98 $$4  JJE   Move setvars call to head.inc
// 03-Feb-98 $$5  JJE   Use __LM_LICENSE_FILE__
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98 $$6  JJE   Changes for spaces in directories
// 02-Nov-98  $$7  JJE  Add ptc_end tag
// 12-Apr-01  $$8  ALG  Pause by default; Add /nopause arg
// 17-Oct-01  $$9  TWH  Use PTC_D_LICENSE_FILE
// 21-Nov-01  $$10  TWH  shift out nopause; not arg1 while pause
// 28-Apr-03 K-01-06 $$11 TWH Add exit
// 25-Jul-05 K-03-29 $$12 TWH MC fallback test x86e_win64
// 23-Jun-09 L-03-33 $$13 TWH Add forcea
// 05-Nov-09 L-03-36 $$14 TWH Fix #13
// 07-Jan-11 L-03-42 $$15 TWH Need forcea set after nopause also
#include:head.inc

if NOT "%mc%" == "x86e_win64" goto x86do
if EXIST "%prodir%\%mc%\obj\plpf_status.exe" goto x86do
set MC=i486_nt
:x86do

REM ptc-cmdtype: lmgrd

set forcea=
set PTC_D_LICENSE_FILE=__LM_LICENSE_FILE__

set PRO_LICENSE_RES=__LICENSE_RES__

set status_cmd="%PRODIR%\%MC%\obj\plpf_status"

if "%1" == "-nopause" goto nopause
if "%1" == "/nopause" goto nopause
if "%1" == "" set forcea="-a"

REM Display output page-by-page, then pause before termination
set more_flag=/E
%status_cmd% %forcea% %1 %2 %3 %4 %5 | more %more_flag%
pause
goto ptc_end
:nopause
shift
if "%1" == "" set forcea="-a"

REM Display all output, then terminate
%status_cmd% %forcea% %1 %2 %3 %4 %5

:ptc_end
exit /B %ERRORLEVEL%

