// 10-Mar-00  $$1  TWH  Created.
// 30-May-00  $$2  TWH  Drop start /b from cmd line
// 07-Jun-00  $$3  TWH  pass third argument
// 28-Apr-03 K-01-06 $$4 TWH Add exit
#include:head.inc

if NOT "%mc%" == "i486_nt" goto platerr

set PTC_JRE=__PTC_JRE__

%PTC_JRE%\bin\java.exe -classpath "%PRO_DIRECTORY%\lib\toolkit.jar" com.ptc.propdm.client.ToolkitTest %1 %2 %3

goto ptc_end

:platerr

echo "Pro/PDM Application Server is supported on i486_nt platforms only."
goto ptc_end

:ptc_end
exit /B %ERRORLEVEL%

