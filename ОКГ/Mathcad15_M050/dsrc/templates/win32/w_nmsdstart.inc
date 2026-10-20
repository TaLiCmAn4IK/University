// 24-Apr-01 $$1 ksi Created
// 15-May-02 $$2 ACT Updated logic for NT
#include:head.inc

set nmsdir=%PRO_DIRECTORY%\%MC%\nms

"%nmsdir%\nmsq.exe" -quiet -ping

if ERRORLEVEL 1 goto nmsd_start
%start_cmd% /min "%nmsdir%\nmsq.exe" -hostinfo -quiet 

if ERRORLEVEL 1 goto nmsd_shutdown
goto nmsd_up_and_running 
	
:nmsd_shutdown
echo Shutting down Name Service ...
"%nmsdir%\nmsq.exe" -quiet -shutdown

:nmsd_start
echo Starting Name Service ...
%start_cmd% /min "%nmsdir%\nmsd.exe"  -noservice -dcad -timeout 600&

:nmsd_up_and_running

