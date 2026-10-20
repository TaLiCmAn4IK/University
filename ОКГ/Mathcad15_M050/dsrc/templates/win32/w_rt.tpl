// 20-Oct-99 $$1  OTT  Created.
// 28-Apr-03 K-01-06 $$2 TWH Add exit
#include:head.inc

set args=
:loopa
if .%1==. goto endloopa
set args=%args% %1
shift
goto loopa

:endloopa

"%PRODIR%\%MC%\obj\rt.exe" %args%
:ptc_end
exit /B %ERRORLEVEL%

