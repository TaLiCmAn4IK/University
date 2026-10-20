// 08-Jul-10 L-03-40 TWH $$1  Created
// 06-Jan-11 L-03-42 TWH $$2  change service name
#include:head.inc

REM ptc-cmdtype: lmadmin

set NLO_NTSERVICE=true

set PTC_D_LICENSE_FILE=%PRO_DIRECTORY%\licensing\license.dat

set PTCLOGFILE=%PRO_DIRECTORY%\logs\ptc_d.log
set BAKLOGFILE=%PRO_DIRECTORY%\logs\ptc_d.bak

set LMGRDARGS=__LMGRD_ARGS__

if NOT EXIST "%PTCLOGFILE%" goto skpc2
  copy "%PTCLOGFILE%" "%BAKLOGFILE%" > "%PRO_DIRECTORY%\logs\junknull"
  del "%PRO_DIRECTORY%\logs\junknull"
:skpc2
net start "lmadmin_ptc"
:ptc_end
exit /B %ERRORLEVEL%

