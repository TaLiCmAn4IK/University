// 10-Dec-01 J-01-40+ EFR   $$1    Created
// 28-Apr-03 K-01-06 $$2 TWH Add exit
// 03-Aug-05 K-03-30 $$3 TWH drop i486_win95
#include:head.inc

set JDK_PATH=__JDK_PATH__

cd /D %PRO_DIRECTORY%

rem if exist ".\bin\rmiregistry.bat" call .\bin\rmiregistry.bat -EXIT

if exist ".\bin\rnwebsupport.bat" call .\bin\rnwebsupport.bat -EXIT

if exist ".\bin\rngateway.bat" call .\bin\rngateway.bat -EXIT

if exist ".\bin\rnportal.bat" call .\bin\rnportal.bat -EXIT

if exist ".\bin\rnecatalog.bat" call .\bin\rnecatalog.bat -EXIT

echo "Shutdown Servers"
exit /B %ERRORLEVEL%
