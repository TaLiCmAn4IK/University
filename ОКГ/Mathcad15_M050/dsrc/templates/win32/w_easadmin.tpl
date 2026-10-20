// 05-May-99  I-01-35  $$1  TWH   Created
// 28-Apr-03 K-01-06 $$2 TWH Add exit
// 29-Jul-05 K-03-30 $$3 TWH Drop win95
#include:head.inc

set CLASSPATH=%CLASSPATH%;%PRO_DIRECTORY%\jar\AdminClient.jar;%PRO_DIRECTORY%\jar\OrbixClasses.jar;%PRO_DIRECTORY%\jar\symbeans.jar;%PRO_DIRECTORY%\cfg

java com.ptc.cadadmin.LaunchCadadmin

:ptc_end
exit /B %ERRORLEVEL%

