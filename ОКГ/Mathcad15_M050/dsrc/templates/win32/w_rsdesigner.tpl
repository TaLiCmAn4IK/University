// 19-Oct-00      JJE  Created.
// 24-Oct-00      JJE  Add ilinkpath stuff
// 19-Mar-01      TWH  Change for ilinkdir.env
// 28-Apr-03 K-01-06     TWH  Add exit
// 24-Feb-05 K-01-36     JJE  Pass all args
// 27-Feb-05 K-01-36     Hila Modified if statements that involves args to 
//                            work with args with spaces.
// 10-May-05 K-01-38     JJE  Add PDM_LDB_PATH setting
// 18-Jan-07 L-01-25 $$1 JJE  New to L-01 and Add EWGM_PATH setting
// 12-Jun-07 L-01-33 $$2 TWH  Alternate header
@echo off

#include:rsd_head.inc

REM ptc-cmdtype: rsd

set BROWSER="__WEBBROWSER__"

#include:ilinkdir.env

#include:ilinkpath.inc

if NOT "%EWGM_PATH%" == "" goto ewgmpath
  set EWGM_PATH=__EWGM_PATH__
  if "%EWGM_PATH%" == "__!!EWGM_PATH!!__" set EWGM_PATH=
:ewgmpath

if "%mc%" == "i486_win95" goto pdmldbpathset
  if NOT "%PDM_LDB_PATH%" == "" goto pdmldbpathset
    if "%HOMEDRIVE%" == "" goto pdmldbpathset
      if "%HOMEPATH%" == "" goto pdmldbpathset
        set PDM_LDB_PATH=%HOMEDRIVE%%HOMEPATH%
:pdmldbpathset

#includeenv:LM_LICENSE_FILE:lmlic.env
set RSDESIGNER_FEATURE_NAME=__RSDESIGNER_FEATURE_NAME__

set slashb=
if %MC% == i486_nt    set slashb=/b
if %MC% == r4000_nt   set slashb=/b
if %MC% == alpha_nt   set slashb=/b

set slashw=
if "pro_wait" == "%1" set slashw=/w
if "pro_wait" == "%2" set slashw=/w
if "pro_wait" == "%3" set slashw=/w
if "pro_wait" == "%4" set slashw=/w
if "pro_wait" == "%5" set slashw=/w
if "pro_wait" == "%6" set slashw=/w
 
if "-v" == "%1" set slashw=/w
if "-v" == "%2" set slashw=/w
if "-v" == "%3" set slashw=/w
if "-v" == "%4" set slashw=/w
if "-v" == "%5" set slashw=/w
if "-v" == "%6" set slashw=/w

%start_cmd% %slashb% %slashw% "%PRO_DIRECTORY%\%MC%\rsdesigner\bin\uiengine" %*%
:ptc_end
exit /B %ERRORLEVEL%

