// 25-Sep-97  $$1   JJE  Recreated
// 26-Jan-98  $$2   JJE  Use head.inc
// 11-Feb-98  $$3   JJE  Use 200
// 14-Aug-98        JJE  Support spaces in directories
// 27-Jul-98        JJE  build %param% from %args% if not set
// 26-Aug-98        JJE  Reinstate the param change
// 01-Sep-98  $$4   JJE  Changes for spaces in directories
// 12-Oct-98  $$5   JJE  Use 210
// 28-Apr-03 K-01-06 $$6 TWH Add exit
#include:head.inc

if "%param%" == "" goto nextp

goto endp

:nextp
        if "%1" == "" goto endp
        set param=%param% %1
        shift
goto nextp

:endp

%start_cmd% /w "%PRO_DIRECTORY%\%MC%\obj\ncp210g.exe" %param%
:ptc_end
exit /B %ERRORLEVEL%

