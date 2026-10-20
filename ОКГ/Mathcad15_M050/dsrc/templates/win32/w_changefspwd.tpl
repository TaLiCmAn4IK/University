// 06-Aug-98 $$1 YWA	created
// 17-Aug-99     LL	    fixed SPR769554
// 06-Oct-99 $$2 MAZ    Update from I-02
// 14-Nov-00 $$3 TWH    drop /min
// 28-Apr-03 K-01-06 $$4 TWH Add exit
#include:head.inc

if NOT "%1" == "" goto fromarg
%MC%_ptc_setvars %0 changefspwd bat
call ptc_setvars.bat
del  ptc_setvars.bat
goto prodir_set
:fromarg
set PRODIR=%1
:prodir_set

start %PRODIR%\%MC%\obj\fsrvpswd %PRODIR%
exit /B %ERRORLEVEL%
