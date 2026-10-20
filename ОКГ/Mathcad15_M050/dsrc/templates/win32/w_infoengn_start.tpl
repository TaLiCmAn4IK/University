// 29-Feb-00 $$1  MAZ  Created.
// 10-Apr-00      MAZ  Sub in full path to JVM cmd
// 11-Apr-00 $$2  MAZ  Use head.inc
// 25-Oct-00 $$3  MAZ  Use the new ie.jar path
// 28-Apr-03 K-01-06 $$4 TWH Add exit
#include:head.inc

cmd.exe /C start "Info*Engine Naming Service" /MIN __JVM_CMD__ -cp %PRO_DIRECTORY%\codebase\WEB-INF\lib\ie.jar -DpropFile=%PRO_DIRECTORY%\ie.properties com.infoengine.au.NamingService exit
exit /B %ERRORLEVEL%

