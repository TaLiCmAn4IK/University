// 27-Mar-97 $$1  Pete  Created.
// 02-Nov-98 $$2  JJE   Add ptc_end tag
// 13-Nov-98 $$3  TWH   Copy H-03 fix: YWA   fix SPR615190
// 18-Dec-98 $$4  TWH   rm unnecessary code; use nfsserv
// 28-Apr-03 K-01-06 $$5 TWH Add exit
// 18-APR-05 K-03-23 $$6 MTP Use head_ilink
#include:head_ilink.inc

net stop | find /I "Intralink Fileserver" || net stop "Intralink Fileserver
start /min /w %PRODIR%\%MC%\obj\nfsserv __FSERV_PORT__ -kill
:ptc_end
exit /B %ERRORLEVEL%
