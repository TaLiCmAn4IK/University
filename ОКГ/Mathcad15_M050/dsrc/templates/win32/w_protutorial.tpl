// 17-Jul-2000 $$1 JJE Created
// 15-Feb-01   $$2 TWH Added pro_wait 859786
// 28-Apr-03 K-01-06 $$3 TWH Add exit
#include:head.inc

#includeenv:PTCNMSPORT:ptcnmsport.env

#includeenv:PROE_START:proe_start.env

if NOT "%PROE_START%" == "" "%PROE_START%" -tutorial:_remote_helper pro_wait

:ptc_end
exit /B %ERRORLEVEL%

