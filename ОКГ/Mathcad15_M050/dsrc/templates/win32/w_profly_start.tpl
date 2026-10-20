// 12-Aug-97 $$1  JJE  Created.
// 19-Sep-97 $$2  JJE  __SCRIPT_NAME__
// 26-Jan-98 $$3  JJE   Move setvars call to head.inc
// 02-Feb-98 $$4  JJE  Add nmsd
// 03-Feb-98 $$5  TWH  nmsd via includeenv
// 18-Feb-98 $$6  JJE  Remove NAME_SERVICE_EXE
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98 $$7  JJE   Changes for spaces in directories
// 02-Nov-98 $$8  JJE   Add ptc_end tag
// 15-Apr-99      JJE  Add ILIBPATH stuff
// 22-Apr-99      JJE  Also include ilinkpath
// 04-May-99 $$9  TWH  Add PDM_LDB_PATH setting
// 17-Apr-00 $$10 MYA  add FLYTHU_FEATURE_NAME 
// 19-Mar-01      ALG  Include ptc_startup.inc
// 19-Mar-01 $$11 TWH  Change for ilinkdir.env
// 28-Apr-03 K-01-06 $$12 TWH Add exit
// 29-Jul-05 K-03-30 $$13 TWH Drop win95
#include:head.inc

REM ptc-cmdtype: profly

if NOT "%PDM_LDB_PATH%" == "" goto pdmldbpathset
  if "%HOMEDRIVE%" == "" goto pdmldbpathset
    if "%HOMEPATH%" == "" goto pdmldbpathset
      set PDM_LDB_PATH=%HOMEDRIVE%%HOMEPATH%

#include:ilinkdir.env

#include:ilinkpath.inc

#includeenv:PTCNMSPORT:ptcnmsport.env

set PROFLY_DIRECTORY=%prodir%\pronav

set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg

#includeenv:PROE_START:proe_start.env

#includeenv:LM_LICENSE_FILE:lmlic.env
set PROFLY_FEATURE_NAME=__FLYTHRU_FEATURE_NAME__
set FLYTHRU_FEATURE_NAME=__FLYTHRU_FEATURE_NAME__

#include:ptc_startup.inc

%start_cmd% "%prodir%\%mc%\obj\profly" %1 %2 %3 %4 %5
:ptc_end
exit /B %ERRORLEVEL%
