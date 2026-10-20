// 12-Aug-97 $$1  JJE  Created.
// 26-Jan-98 $$2  JJE   Move setvars call to head.inc
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98 $$3  JJE   Changes for spaces in directories
// 02-Nov-98 $$4  JJE   Add ptc_end tag
// 15-Apr-99      JJE   Add illib stuff
// 22-Apr-99      JJE  Also include ilinkpath
// 10-May-99 $$5  TWH  Add PDM_LDB_PATH setting
// 19-Mar-01 $$6  TWH  Change for ilinkdir.env
// 17-Oct-01 $$7  TWH  Use PTC_D_LICENSE_FILE
// 28-Apr-03 K-01-06 $$8 TWH Add exit
// 29-Jul-05 K-03-30 $$9 TWH Drop win95
#include:head.inc

REM ptc-cmdtype: promodelview

if NOT "%PDM_LDB_PATH%" == "" goto pdmldbpathset
  if "%HOMEDRIVE%" == "" goto pdmldbpathset
    if "%HOMEPATH%" == "" goto pdmldbpathset
      set PDM_LDB_PATH=%HOMEDRIVE%%HOMEPATH%

#include:ilinkdir.env

#include:ilinkpath.inc

set PROFLY_DIRECTORY=%prodir%\pronav

set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg

set PROFLY_FEATURE_NAME=FLYVIEWER_DEMO
set PTC_D_LICENSE_FILE=%prodir%\pronav\text\licensing\profly\promview_lic.dat

%start_cmd% "%prodir%\%mc%\obj\profly" %1 %2 %3 %4 %5
:ptc_end
exit /B %ERRORLEVEL%

