// 12-Apr-99  $$1   JJE  Created
// 28-Apr-03 K-01-06 $$2 TWH Add exit
#include:head.inc

if "%param%" == "" goto nextp

goto endp

:nextp
        if "%1" == "" goto endp
        set param=%param% %1
        shift
goto nextp

:endp

set CAMSYS=%PRO_DIRECTORY%\%MC%\obj
set CAMLIB=%PRO_DIRECTORY%\%MC%\gpost

%start_cmd% /w "%CAMSYS%\gpost.exe" %param%
:ptc_end
exit /B %ERRORLEVEL%

