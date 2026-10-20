// 06-Jan-00 $$1  JJE  Created.
// 21-Jan-00 $$2  JJE   No mcdir setting
// 28-Apr-03 K-01-06 $$3 TWH Add exit
#include:head.inc

"%prodir%\%MC%\obj\modelcheck.exe" %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%
