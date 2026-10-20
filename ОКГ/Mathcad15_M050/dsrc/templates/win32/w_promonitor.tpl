// 16-Sep-97  $$1  JJE  Created
// 26-Jan-98 $$2  JJE   Move setvars call to head.inc
// 14-Aug-98 $$3  JJE  Support spaces in directories
// 02-Nov-98 $$4  JJE   Add ptc_end tag
// 28-Apr-03 K-01-06 $$5 TWH Add exit
#include:head.inc

#include:nlocli.inc

"%PRO_DIRECTORY%\%MC%\obj\promonitor" %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%
