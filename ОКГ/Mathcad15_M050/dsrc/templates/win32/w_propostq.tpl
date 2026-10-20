// 25-Sep-97  $$1   JJE  Recreated
// 11-Feb-98  $$2   JJE  Use 200
// 14-Aug-98       JJE  Support spaces in directories
// 01-Sep-98  $$3  JJE   Changes for spaces in directories
// 12-Oct-98  $$4  JJE  Use 210
// 28-Apr-03 K-01-06 $$5 TWH Add exit
#include:head.inc

%start_cmd% /w "%PRO_DIRECTORY%\%MC%\obj\ncp210q.exe" -n %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%

