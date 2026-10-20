// 12-Aug-97 $$1  JJE  Created.
// 26-Jan-98 $$2  JJE   Move setvars call to head.inc
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98 $$3  JJE   Changes for spaces in directories
// 02-Nov-98 $$4  JJE   Add ptc_end tag
// 28-Apr-03 K-01-06 $$5 TWH Add exit
#include:head.inc

REM ptc-cmdtype: netscape 

%start_cmd% "%prodir%\%mc%\netscape\netscape" %1 %2 %3 %4 %5
:ptc_end
exit /B %ERRORLEVEL%

