// 26-Jan-98  $$1  JJE  Created
// 14-Aug-98  $$2  JJE  Support spaces in directories
// 16-Oct-98  $$3  TWH  Increase Argument count
// 28-Apr-03 K-01-06 $$4 TWH Add exit
#include:head.inc

"%PRO_DIRECTORY%\%MC%\obj\convert_image" %1 %2 %3 %4 %5 %6 %7 %8 %9
:ptc_end
exit /B %ERRORLEVEL%

