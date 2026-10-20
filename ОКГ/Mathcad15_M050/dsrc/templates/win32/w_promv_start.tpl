// 12-Aug-97 $$1  JJE  Created.
// 19-Sep-97 $$2  JJE  __SCRIPT_NAME__
// 26-Jan-98 $$3  JJE   Move setvars call to head.inc
// 18-Feb-98 $$4  JJE  Change licensing
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98 $$5  JJE   Changes for spaces in directories
// 02-Nov-98 $$6  JJE   Add ptc_end tag
// 15-Apr-99      JJE   Add illib stuff
// 22-Apr-99      JJE  Also include ilinkpath
// 10-May-99 $$7  TWH  Add PDM_LDB_PATH setting
// 17-Apr-00 $$8  MYA  Add MODELVIEW_FEATURE_NAME
// 19-Mar-01      ALG  Include ptc_startup.inc
// 19-Mar-01 $$9  TWH  Change for ilinkdir.env
// 28-Apr-03 K-01-06 $$10 TWH Add exit
// 29-Jul-05 K-03-30 $$11 TWH Drop win95
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

set PROFLY_FEATURE_NAME=__MODELVIEW_FEATURE_NAME__

set MODELVIEW_FEATURE_NAME=__MODELVIEW_FEATURE_NAME__

#includeenv:LM_LICENSE_FILE:lmlic.env

#include:ptc_startup.inc

%start_cmd% "%prodir%\%mc%\obj\profly" %1 %2 %3 %4 %5
:ptc_end
exit /B %ERRORLEVEL%

