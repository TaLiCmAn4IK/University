// 13-Mar-00  $$1  JJE  Created
// 28-Mar-00  $$2  JJE  Shorten the batch file tag
// 11-Mar-00  $$3  JJE  Change
// 21-Aug-00  $$4  JJE  Fix formatting
// 28-Apr-03 K-01-06 $$5 TWH Add exit
#include:head.inc

set nmsdir=%PRO_DIRECTORY%\%MC%\nms

"%nmsdir%\nmsq.exe" -quiet -off

if NOT ERRORLEVEL 0 goto nmsd_1
    echo DCAD disabled successfully
    goto ptc_end
:nmsd_1
    "%nmsdir%\nmsq.exe" -quiet -ping
    
    if NOT ERRORLEVEL 0 goto nmsd_2
        echo dcad_shutdown failed. Incompatible nmsd daemon is running.
		goto ptc_end
:nmsd_2
        echo nmsd is not running. DCAD was not enabled

:ptc_end
exit /B %ERRORLEVEL%

