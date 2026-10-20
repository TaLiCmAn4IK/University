// 18-May-99 I-01-36 TWH $$1  Created
// 28-Apr-03 K-01-06 $$2 TWH Add exit
// 29-Jul-05 K-03-30 $$3 TWH Drop win95
#include:head.inc

if NOT "%PDM_LDB_PATH%" == "" goto pdmldbpathset
  if "%HOMEDRIVE%" == "" goto pdmldbpathset
    if "%HOMEPATH%" == "" goto pdmldbpathset
      set PDM_LDB_PATH=%HOMEDRIVE%%HOMEPATH%

set tkdir=__TKENV_LP__
#includeenv:PTC_APPMGR_DIR:appmgrdir.env

set PDM_ORA_APPL=intralink
set PDM_ORACLE_SERVER=intralink
set TNS_ADMIN=%tkdir%
set ORA_NLS32=%tkdir%\%mc%\obj\oracle734\NLSRTL32\DATA
set NLS_LANG=AMERICAN_AMERICA.WE8ISO8859P1
set ILINK_HOME=%prodir%

set IT_CONFIG_PATH=%prodir%\cfg\Orbix.cfg
set CS_ACCOUNT_PATH=%prodir%\admin
set CS_ROOT_PATH=__WORKSPACE_PROJECTS__
set CS_IMPEX_PATH=%prodir%\%mc%\obj\mdimpex
set PDM_ORACLE_SERVER=%OracleServer%
set NUM_OF_ENGS=__NUM_OF_ENGS__

#includeenv:ORBIX_DIRECTORY:orbix_dir.env
if NOT "%ORBIX_DIRECTORY%" == "" goto orbdone
  set ORBIX_DIRECTORY=%prodir%\orbix
:orbdone

// we do not support testing orbixd to see if it is
// running and startup otherwise here.
set IT_IMP_REP_PATH=%prodir%\ImpRep
set IT_INT_REP_PATH=%prodir%\IntRep

#includeenv:LM_LICENSE_FILE:lmlic.env
set EAS_FEATURE_NAME=__EAS_FEATURE_NAME__
set ILINK_TK_ACCESS_FEATURE_NAME=__ILINK_FEATURE_NAME__

%ORBIX_DIRECTORY%\bin\putit %prodir%\%mc%\obj\cadbrk.exe -persistent

start %prodir%\%mc%\obj\cadbrk.exe %NUM_OF_ENGS% %prodir%\%mc%\obj\cadeng %prodir%\%mc%\obj\cadproi

:ptc_end
exit /B %ERRORLEVEL%

