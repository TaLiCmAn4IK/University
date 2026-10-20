// 01-Feb-02 $$1  MAZ   Copy from w_cpu_id.tpl (SPR 921507)
// 28-Apr-03 K-01-06 $$2 TWH Add exit
// 25-Jul-05 K-03-29 $$3 TWH MC fallback test for x86e_win64
#include:head.inc

if NOT "%mc%" == "x86e_win64" goto x86do
if EXIST "%prodir%\%mc%\obj\cpu_id.exe" goto x86do
set MC=i486_nt
:x86do

"%PRODIR%\%MC%\obj\cpu_id.exe" %1 %2 %3 %4 %5
:ptc_end
exit /B %ERRORLEVEL%

