// 05-Jan-98 $$1  JJE  Created.
// 12-Jan-98 $$2  JJE  Use prosetup
// 26-Jan-98 $$3  JJE  Move setvars call to head.inc
// 10-Apr-98 $$4  JJE  Pass arguments
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98 $$5  JJE  Changes for spaces in directories
// 02-Nov-98 $$6  JJE  Add ptc_end tag
// 01-Nov-99      MYA  Migrate to new installer
// 16-Nov-99 $$7  JJE  Fix arg passing
// 14-Dec-99 $$8  MAZ  Add __MSI_FLAG__
// 28-Apr-03 K-01-06 $$9 TWH Add exit
// 25-Jul-05 K-03-29 $$10 TWH MC fallback test x86e_win64
// 26-Aug-05 K-03-30+$$11 TWH Fix #10
#include:head.inc

if NOT "%mc%" == "x86e_win64" goto x86do
if EXIST "%prodir%\dsrc\%mc%\obj\ptcsetup.exe" goto x86do
set MC=i486_nt
:x86do

set slashb=
if %MC% == i486_nt   set slashb=/b
if %MC% == r4000_nt  set slashb=/b
if %MC% == alpha_nt  set slashb=/b

REM=============================================================
set lp=%PRODIR%
if not exist "%lp%\dsrc\%mc%\obj\ptcsetup.exe" goto invalid_plat

set PTC_CDHOME=%lp%\
set PS_DIRECTORY=%lp%\dsrc
set DPS_DIRECTORY=%lp%\dsrc

set USE_NEW_SELECTION_TRAILING=0

%start_cmd% %slashb% "%lp%\dsrc\%mc%\obj\ptcsetup.exe" -ptcutil __MSI_FLAG__ %1 %2 %3 %4 %5

goto the_end
REM=============================================================

:bad_arch
echo Cannot detect what machine type you have.  Please set the following
echo environment variable, and run ptcsetup again:
echo.
echo set PROCESSOR=INTEL_486     - For Windows NT Intel based machines.
echo.
echo set PROCESSOR=INTEL_64      - For Windows Intel Itanium based machines.
echo.
echo set PROCESSOR_ARCHITECTURE=AMD64 - For Windows AMD 64 based machines.
echo.
goto the_end

:invalid_plat
@echo ERROR: Installation not supported on this System.
goto the_end

:the_end
:ptc_end
exit /B %ERRORLEVEL%

