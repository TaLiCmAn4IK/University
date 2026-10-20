// 16-Sep-97  $$1  JJE  Created
// 02-Nov-98  $$2  JJE  Add ptc_end tag
// 13-Nov-98  $$3  TWH  Copy H-03 fix: fix SPR615190;
//                      Add RFTS_PASSWD_ALT_FILE
// 28-Apr-03 K-01-06 $$4 TWH Add exit
// 18-APR-05 K-03-23 $$5 MTP Use head_ilink
#include:head_ilink.inc

if NOT "%1" == "" goto fromarg
%MC%_ptc_setvars %0 fserv_startup bat
call ptc_setvars.bat
del  ptc_setvars.bat
goto prodir_set
:fromarg
set PRODIR=%1
:prodir_set

set FSERV_PORT=__FSERV_PORT__
set RFTS_PASSWD_ALT_FILE=%PRODIR%\install\etc\ilinkftpswd

net start | find /I "Intralink Fileserver" || net start "Intralink Fileserver"
start /min %PRODIR%\%MC%\obj\nfsserv %FSERV_PORT%
:ptc_end
exit /B %ERRORLEVEL%
