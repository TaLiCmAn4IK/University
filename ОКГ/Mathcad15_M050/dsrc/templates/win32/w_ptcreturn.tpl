// 12-Jul-03  K-01-11  $$1 JJE Created
#include:head.inc

set LM_LICENSE_FILE=__LM_LICENSE_FILE__

set missargs=false
if "%1" == "" set missargs=true
if "%missargs%" == "false" goto ok_to_run
   echo Usage: ptcreturn [feature]
   echo Example: ptcreturn PROE_LIC
   goto ptc_end
:ok_to_run
"%PRODIR%\%MC%\obj\lmutil" lmborrow -return %*
:ptc_end
exit /B %ERRORLEVEL%
