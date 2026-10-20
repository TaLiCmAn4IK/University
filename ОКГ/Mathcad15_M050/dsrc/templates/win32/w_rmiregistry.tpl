// 18-Oct-00    $$1    TWH          New
// 26-Oct-00 J-01-20+  TWH   $$2    Fix label
// 11-May-01           ALG          Allow spaces in JDK_PATH
// 11-May-01 J-01-32+  ALG   $$3    Change locations of app.jar, gateway.jar
// 11-Jun-01 J-01-33+  MAZ          add adapter.jar to cp (SPR 876053)
// 26-Jun-01 J-03-02   ALG   $$4    Submit to J-03
// 28-Apr-03 K-01-06 $$5 TWH Add exit
// 29-Jul-05 K-03-30 $$6 TWH Drop win95
#include:head.inc

set JDK_PATH=__JDK_PATH__
set RMI_PORT=__RMI_PORT__

cd /D %PRO_DIRECTORY%

start "RMI Registry" "%JDK_PATH%\bin\rmiregistry" -J-classpath -Jcodebase\Web-inf\lib\gateway.jar;codebase\Web-inf\lib\app.jar;codebase\Web-inf\lib\adapter.jar %RMI_PORT%

exit /B %ERRORLEVEL%

