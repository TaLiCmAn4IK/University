// 09-Sep-99  $$1  MAZ  Created
// 28-Apr-03 K-01-06 $$2 TWH Add exit
#include:head.inc

set DIVISIONROOT=%PRODIR%

"%PRODIR%\%MC%\obj\bgf2flt" %1 %2 %3 %4 %5 %6 %7 %8 %9
:ptc_end
exit /B %ERRORLEVEL%
