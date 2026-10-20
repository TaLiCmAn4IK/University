// 16-Sep-97  $$1  JJE  Created
// 05-Jan-98  $$2  JJE  Use lmutil
// 26-Jan-98 $$3  JJE   Move setvars call to head.inc
// 14-Aug-98       JJE  Support spaces in directories
// 01-Sep-98  $$4  JJE  Changes for spaces in directories
// 02-Nov-98  $$5  JJE  Add ptc_end tag
// 17-Oct-01  $$6  TWH  Use PTC_D_LICENSE_FILE
// 28-Apr-03 K-01-06 $$7 TWH Add exit
#include:head.inc

REM ptc-cmdtype: lmgrd

if NOT "%PTC_D_LICENSE_FILE%" == "" goto lmlicenseset
set PTC_D_LICENSE_FILE=%PRO_DIRECTORY%\licensing\license.dat
:lmlicenseset

"%PRODIR%\%MC%\obj\lmutil" lmdown -c "%PTC_D_LICENSE_FILE%"
:ptc_end
exit /B %ERRORLEVEL%

