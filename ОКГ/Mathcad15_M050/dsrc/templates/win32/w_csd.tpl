// 24-Apr-01 $$1 ksi Created
// 24-Apr-01 $$2 ksi Csd parameter change
// 15-May-02 $$3 ACT Updated for NT
// 15-Jul-02 $$4 ksi return csd status
// 02-Dec-02 J-03-39  MAZ   $$5   SPR 969008: add startup warning to csd.bat 
// 28-Apr-03 K-01-06 $$6 TWH new std exit
@echo off

if "%1" == "-run" goto run_csd
echo Please start the Conference Server using the Conference Center application
pause 
exit

:run_csd
#include:nmsdstart.inc

"%prodir%\%mc%\obj\csd.exe" -noservice %1 %2 %3 %4 %5 %6 %7 %8 %9

:ptc_end
exit /B %ERRORLEVEL%
