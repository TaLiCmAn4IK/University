// 05-Mar-00  #1  TWH  Created.
// 10-Apr-00 ##1  TWH  Preserve arg[0]
// 16-Jan-00 J-01-25  ##2  MDA  fix for args. containing space
// 25-Oct-01 J-01-39  ##3  TWH  exit w/ errorlevel
// 30-Oct-01 J-03-11+ ##4  TWH  remove #3
// 18-Dec-01 J-03-12  ##5  TWH  remove exit /b
// 31-Dec-02 J-03-40  $$1  TWH  Renamed from w_mdimpex.tpl
// 28-Apr-03 K-01-06 $$2 TWH Add exit
// Note: can't use head.inc because we need to shift and mangle the 
//       commandline args
@echo off

set MC=unset
if "%PROCESSOR%" == "INTEL_64" set MC=ia64_nt
if "%PROCESSOR%" == "INTEL_486" set MC=i486_nt
if "%PROCESSOR_ARCHITECTURE%" == "IA64" set MC=ia64_nt
if "%PROCESSOR_ARCHITECTURE%" == "x86" set MC=i486_nt
if not "%MC%" == "unset" goto mc_done
echo ERROR Cannot detect what machine type you have.
echo Please make one of the following settings:
echo.
echo set PROCESSOR=INTEL_486     - For Windows NT Intel based machines.
echo.
echo set PROCESSOR=INTEL_64      - For Windows Intel Itanium based machines.
echo.
exit
:mc_done

set params=
set argzero=%0
:ploop
set a_param=%1
if '%1' == 'apdmpi_dll' set a_param=apdmpi_dll.dll
if '%a_param%' == '' goto pldone
set params=%params% %a_param%
shift
goto ploop
:pldone

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

#include:ilinkdir.env
set path=%INTRALINK_DIR%\%MC%\illib;%PRODIR%\%MC%\mdimpex;%path%
 
if "%PDM_IMPEX_DLL_NAME%" == "" set PDM_IMPEX_DLL_NAME=apdmpi_dll.dll

%PRODIR%\%MC%\obj\mdimpex %params%
:ptc_end
exit /B %ERRORLEVEL%
