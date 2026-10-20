// 16-Sep-97  $$1  JJE  Created
// 26-Jan-98  $$2  JJE  Move in call to ptcsetvars
// 10-Feb-98  $$3  JJE  Set PRO_MACHINE_TYPE
// 14-Aug-98       JJE  Support spaces in directories
// 01-Sep-98  $$4  JJE  Changes for spaces in directories
// 14-Oct-98  $$5  TWH  Win98 & env4096 changes
// 20-Oct-98  $$6  JJE  Must pass 6 arguments
// 02-Nov-98       TWH  Must pass 9 arguments
// 13-Nov-98  $$7  TWH  rm skipifbmode from std header
// 12-Dec-00  $$8  TWH  Add IA64/INTEL_64
// 28-Apr-03 K-01-06 $$9 TWH remove win95
// 03-Jun-04 K-03-03 $$10 ASRS added x86
// 25-Jul-05 K-03-29 $$11 TWH  Add setup echo for x86e_win64
// 04-Aug-11 L-03-45 $$12 TWH  Add cygwin check
@echo off

set MC=unset
if "%PROCESSOR%" == "INTEL_64" set MC=ia64_nt
if "%PROCESSOR%" == "INTEL_486" set MC=i486_nt
if "%PROCESSOR_ARCHITECTURE%" == "IA64" set MC=ia64_nt
if "%PROCESSOR_ARCHITECTURE%" == "AMD64" set MC=x86e_win64
if "%PROCESSOR_ARCHITECTURE%" == "x86" set MC=i486_nt
REM cygwin sets this on 64bit OS 
if "%PROCESSOR_ARCHITEW6432%" == "AMD64" set MC=x86e_win64
if not "%MC%" == "unset" goto mc_done
echo ERROR Cannot detect what machine type you have.
echo Please make one of the following settings:
echo.
echo set PROCESSOR=INTEL_486     - For Windows NT Intel based machines.
echo.
echo set PROCESSOR=INTEL_64      - For Windows Intel Itanium based machines.
echo.
echo set PROCESSOR_ARCHITECTURE=AMD64 - For Windows AMD 64 based machines.
echo.
exit
:mc_done

set PRO_MACHINE_TYPE=%MC%

if "%PROOBJ_START_DIRECTORY%" == "" set PRO_DIRECTORY=
if NOT "%PRO_DIRECTORY%" == "" "%PRO_DIRECTORY%\bin\%MC%_ptc_setvars" %0 "__SCRIPT_NAME__" bat
if "%PRO_DIRECTORY%" == "" %MC%_ptc_setvars %0 "__SCRIPT_NAME__" bat
call ptc_setvars.bat
del  ptc_setvars.bat

set start_cmd=start ""

set PRO_DIRECTORY=%PRODIR%
if NOT "%PTCPATH%" == "" goto ptcpathset

set path=%PRO_DIRECTORY%\bin;%path%

set PTCPATH=true
:ptcpathset

