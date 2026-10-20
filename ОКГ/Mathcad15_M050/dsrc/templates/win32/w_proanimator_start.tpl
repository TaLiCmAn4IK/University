// 03-Jun-98  $$1  TWH   Created
// 14-Aug-98       JJE  Support spaces in directories
// 01-Sep-98  $$2  JJE  Changes for spaces in directories
// 24-Feb-99  $$3  JJE  Fix include typo
// 20-Oct-99  $$4  OTT  Changed to network_animator.
// 19-Mar-01  $$5  ALG  Include ptc_startup.inc
// 28-Apr-03 K-01-06 $$6 TWH Add exit
// 29-Jul-05 K-03-30 $$7 TWH Drop win95
#include:head.inc

REM ptc-cmdtype: network_animator

if NOT "%NAPATH%" == "" goto napathset

set path=%PRODIR%\bin;%PRODIR%\%mc%\obj;%path%

set NAPATH=true
:napathset

set MACHINE=%MC%

#includeenv:LM_LICENSE_FILE:lmlic.env

set ANIMATOR_FEATURE_NAME=__ANIMATOR_FEATURE_NAME__

set args=
:loopa
if .%1==. goto endloopa
set args=%args% %1
shift
goto loopa

:endloopa

#include:ptc_startup.inc

%start_cmd% /b "%PRODIR%\%MC%\obj\network_animator.exe" %args%

:ptc_end
exit /B %ERRORLEVEL%

