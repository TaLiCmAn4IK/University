// 16-Sep-97  $$1  JJE  Created
// 23-Jan-97  $$2  JJE  Count args
// 26-Jan-97  $$3  JJE  Use __LM_LICENSE_FILE__
// 26-Jan-98  $$4  JJE  Move setvars call to head.inc
// 07-Jul-98  $$5  JJE  Change arguments
// 14-Aug-98       JJE  Support spaces in directories
// 01-Sep-98  $$6  JJE  Changes for spaces in directories
// 19-Oct-98  $$7  TWH  Remove -c %LM_LICENSE_FILE%
// 02-Nov-98  $$8  TWH  Use ptc_end for last label
// 17-Oct-01  $$9  TWH  Use PTC_D_LICENSE_FILE
// 05-May-03       TWH  Add exit
// 05-May-03  K-01-06 $$10 JJE  Use LM_LICENSE_FILE
// 25-Jul-05  K-03-29 $$11 TWH  MC fallback test for x86e_win64
#include:head.inc

if NOT "%mc%" == "x86e_win64" goto x86do
if EXIST "%prodir%\%mc%\obj\lmutil.exe" goto x86do
set MC=i486_nt
:x86do

REM ptc-cmdtype: lmgrd

set LM_LICENSE_FILE=__LM_LICENSE_FILE__
set PRO_LICENSE_RES=__LICENSE_RES__

set missargs=false
if "%1" == "" set missargs=true
if "%2" == "" set missargs=true
if "%3" == "" set missargs=true
if "%4" == "" set missargs=true
if "%missargs%" == "false" goto ok_to_run
   echo Usage: ptcflush [feature] [license server] [port] [handle]
   echo Example: ptcflush PROE_1234 ptchost 7788 213
   echo   All arguments to ptcflush can be taken from the
   echo   output of ptcstatus
   goto ptc_end
:ok_to_run
"%PRODIR%\%MC%\obj\lmutil" lmremove -h %1 %2 %3 %4 %5
:ptc_end
exit /B %ERRORLEVEL%
