// 23-Apr-98 $$1 JJE Created
// 14-Aug-98 $$2  JJE  Support spaces in directories
// 02-Nov-98 $$3  JJE   Add ptc_end tag
// 26-Dec-01 $$4  TWH  Add PTC_D_LICENSE_FILE 919916
// 07-Jan-02 $$5  TWH  put arg1 before PTC_D_LICENSE_FILE
// 07-Jun-02 $$6  TWH  Fix #5 ; Add set others
// 09-Aug-02 $$7  JJE  Put quotes around lic file arg
// 18-Sep-02 J-03-34  $$8   MAZ  SPR 969593: rm -c arg and set LM_LICENSE_FILE
// 28-Apr-03 K-01-06 $$9 TWH Add exit
#include:head.inc

#includeenv:LM_LICENSE_FILE:lmlic.env

set LM_LICENSE_FILE=%PTC_D_LICENSE_FILE%

"%PRODIR%\%MC%\obj\lmutil" %* 

:ptc_end
exit /B %ERRORLEVEL%

