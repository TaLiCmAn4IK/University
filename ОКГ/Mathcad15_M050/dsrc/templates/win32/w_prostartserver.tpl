// 16-Sep-97  $$1  JJE  Created
// 26-Jan-98 $$2  JJE   Move setvars call to head.inc
// 02-Nov-98 $$3  JJE   Add ptc_end tag
// 28-Apr-03 K-01-06 $$4 TWH Add exit
#include:head.inc

set NLO_NTSERVICE=true

start /min %PRODIR%\%MC%\obj\proserver.exe %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%

