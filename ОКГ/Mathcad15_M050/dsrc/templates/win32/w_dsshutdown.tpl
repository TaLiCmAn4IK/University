// 23-Jul-03 K-01-11   MAZ  $$1   SPR 1025041: copy from dcadsetup
@echo off

rem WARNING: please do not edit the script content below.

#include:head.inc

set nmsdir=%PRO_DIRECTORY%\%MC%\nms

"%nmsdir%\nmsq.exe" -quiet -off

%nmsdir%\nmsq.exe -quiet -servinfo

if NOT ERRORLEVEL 0 goto nmsd_1
    echo DS services disabled successfully
    goto ptc_end
:nmsd_1
    "%nmsdir%\nmsq.exe" -quiet -ping
    
    if NOT ERRORLEVEL 0 goto nmsd_2
        echo dsshutdown failed. Incompatible NMSD daemon is running.
		goto ptc_end
:nmsd_2
        echo NMSD is not running.

:ptc_end
exit /B %ERRORLEVEL%

