// 26-Jan-98  $$1  JJE  Created
// 14-Aug-98 $$2  JJE  Support spaces in directories
// 28-Apr-03 K-01-06 $$3 TWH Add exit
#include:head.inc

"%PRO_DIRECTORY%\%MC%\obj\pro_mcs_comp" %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%

