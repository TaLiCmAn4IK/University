// 23-Sep-97  $$1  JJE  Created
// 28-Jul-98  $$2  TWH  Rm duplicate stuff from head
// 14-Aug-98  $$3  JJE  Support spaces in directories
// 02-Nov-98  $$4  JJE  Add ptc_end tag
// 28-Apr-03 K-01-06 $$5 TWH Add exit
#include:head.inc

"%PRODIR%\%MC%\obj\purge.exe" %1 %2 %3 %4 %5
:ptc_end
exit /B %ERRORLEVEL%

