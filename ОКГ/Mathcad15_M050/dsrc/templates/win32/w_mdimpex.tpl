// 05-Mar-00  #1  TWH  Created.
// 10-Apr-00 $$1  TWH  Preserve arg[0]
// 16-Jan-00 J-01-25  $$2  MDA  fix for args. containing space
// 25-Oct-01 J-01-39  $$3  TWH  exit w/ errorlevel
// 30-Oct-01 J-03-11+ $$4  TWH  remove #3
// 18-Dec-01 J-03-12  $$5  TWH  remove exit /b
// 31-Dec-02 J-03-40  $$6  TWH  Now call proe.exe w/ psf command & pro_wait
// 28-Apr-03 K-01-06  $$7  TWH  Add exit
// 25-Feb-04 K-01-26       TWH  Force no_graphics mode
// 09-Apr-04 K-01-26  $$8  TWH  Fix setvars arguments
// 06-Jan-05 K-03-16+      TWH  Add x86e_win64 check
// 28-Dec-04 K-03-17  $$9  TWH  Full path to psf file
// Note: can't use head.inc because we need to shift and mangle the 
//       commandline args
@echo off

set MC=unset
if "%PROCESSOR%" == "INTEL_64" set MC=ia64_nt
if "%PROCESSOR%" == "INTEL_486" set MC=i486_nt
if "%PROCESSOR_ARCHITECTURE%" == "IA64" set MC=ia64_nt
if "%PROCESSOR_ARCHITECTURE%" == "AMD64" set MC=x86e_win64
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
if NOT "%PRO_DIRECTORY%" == "" "%PRO_DIRECTORY%\bin\%MC%_ptc_setvars" "%argzero%" "__SCRIPT_NAME__" bat
if "%PRO_DIRECTORY%" == "" %MC%_ptc_setvars "%argzero%" "__SCRIPT_NAME__" bat
call ptc_setvars.bat
del  ptc_setvars.bat

set start_cmd=start ""

set PRO_DIRECTORY=%PRODIR%
if NOT "%PTCPATH%" == "" goto ptcpathset
set path=%PRO_DIRECTORY%\bin;%path%
set PTCPATH=true
:ptcpathset

#include:ilinkdir.env
set path=%INTRALINK_DIR%\%MC%\illib;%path%

if NOT "%tmp%" == "" goto ttmp
  if NOT "%temp%" == "" goto ttemp
    if "%homedrive%" == "" goto terr
    if NOT "%homepath%" == "" goto thome
:terr
    echo set "TMP" or "TEMP" environment variables please
    goto ptc_end
:thome
    set tmpdir=%homedrive%%homepath%
    goto ttdone
:ttemp
    set tmpdir=%temp%
    goto ttdone
:ttmp
  set tmpdir=%tmp%
:ttdone

if "%PDM_IMPEX_DLL_NAME%" == "" set PDM_IMPEX_DLL_NAME=apdmpi_dll.dll

set THE_ARG=%PRODIR%\bin\__PROE_COMMAND__.psf 0 0 -impex %params% pro_wait
set PTC_WF_ROOT=%tmpdir%\.impex_proe_wf

REM Delete the previous .wf (if any)
if exist %PTC_WF_ROOT% rmdir /s /q %PTC_WF_ROOT%

start /B /W %PRODIR%\bin\proe.exe %THE_ARG%

:ptc_end
exit /B %ERRORLEVEL%

