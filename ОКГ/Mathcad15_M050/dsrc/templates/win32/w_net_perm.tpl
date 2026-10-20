// 20-Oct-99 $$1  OTT  Created.
// 28-Apr-03 K-01-06 $$2 TWH Add exit
#include:head.inc

"%PRODIR%\%MC%\obj\net_perm.exe" %1 %2 %3 %4 %5
:ptc_end
exit /B %ERRORLEVEL%

