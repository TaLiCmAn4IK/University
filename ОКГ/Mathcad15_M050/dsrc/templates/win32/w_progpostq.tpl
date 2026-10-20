// 12-Apr-99  $$1   JJE  Created
// 28-Apr-03 K-01-06 $$2 TWH Add exit
#include:head.inc

set CAMSYS=%PRO_DIRECTORY%\%MC%\obj
set CAMLIB=%PRO_DIRECTORY%\%MC%\gpost

%start_cmd% /w "%CAMSYS%\optfile.exe" %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%

