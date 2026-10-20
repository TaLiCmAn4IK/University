// 09-Aug-01 $$1  TWH  Created.
// 28-Apr-03 K-01-06 $$2 TWH Add exit
// 18-APR-05 K-03-23 $$3 MTP Use head_ilink
#include:head_ilink.inc

set path=%ORACLE_HOME%\%mc%\obj;%path%

"%prodir%\%mc%\obj\ldbcompact.exe" %* 
:ptc_end
exit /B %ERRORLEVEL%

