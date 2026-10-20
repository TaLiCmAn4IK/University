// 16-Sep-97  $$1  JJE  Created
// 05-Jan-98  $$2  JJE  Use -app
// 12-Jan-98  $$3  JJE  don't check LM set
// 26-Jan-98  $$4  JJE  Move setvars call to head.inc
// 03-Jun-98  $$5  JJE  Start the service on NT
// 14-Aug-98       JJE  Support spaces in directories
// 01-Sep-98  $$6  JJE  Changes for spaces in directories
// 02-Nov-98  $$7  TWH  Use ptc_end for last label
// 01-Apr-99  $$8  MAZ  Use escape char for the license path (lmgrd95 bug)
// 26-Jan-00  $$9  MAZ  Don't use lmgrd95 anymore
// 02-Oct-00  $$10 TWH  Add lmgrd args
// 26-Mar-01  $$11 TWH  Backup lmgrd logfile 
// 10-Aug-01  $$12 TWH  extend #11 for NT if binscripts used
// 17-Oct-01  $$13 TWH  Use PTC_D_LICENSE_FILE
// 28-Apr-03 K-01-06 $$14 TWH Add exit
// 29-Jul-05 K-03-30 $$15 TWH Drop win95
#include:head.inc

REM ptc-cmdtype: lmgrd

set NLO_NTSERVICE=true

set PTC_D_LICENSE_FILE=%PRO_DIRECTORY%\licensing\license.dat

set PTCLOGFILE=%PRO_DIRECTORY%\licensing\ptclmgrd.log
set BAKLOGFILE=%PRO_DIRECTORY%\licensing\ptclmgrd.bak

set LMGRDARGS=__LMGRD_ARGS__

if NOT EXIST "%PTCLOGFILE%" goto skpc2
  copy "%PTCLOGFILE%" "%BAKLOGFILE%" > "%PRO_DIRECTORY%\licensing\junknull"
  del "%PRO_DIRECTORY%\licensing\junknull"
:skpc2
net start "FLEXlm server for PTC"
:ptc_end
exit /B %ERRORLEVEL%

