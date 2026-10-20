// 26-Jan-98 $$1 JJE Created
// 14-Aug-98     JJE  Support spaces in directories
// 01-Sep-98 $$2  JJE  Changes for spaces in directories
// 02-Nov-98 $$3  JJE  Add ptc_end tag
// 17-Oct-01 $$4  TWH  Use PTC_D_LICENSE_FILE
// 28-Apr-03 K-01-06 $$5 TWH Add exit
#include:head.inc

set PTC_D_LICENSE_FILE=%PRO_DIRECTORY%\licensing\license.dat

"%PRODIR%\%MC%\obj\lmutil" lmreread
"%PRODIR%\%MC%\obj\plpf_monitor" "%PRODIR%\licensing\ptcreport.log"
:ptc_end
exit /B %ERRORLEVEL%

