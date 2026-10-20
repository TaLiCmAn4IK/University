// 11-Aug-98      TWH  Created
// 14-Aug-98      JJE  Support spaces in directories
// 18-Aug-98      TWH  Chg license env var
// 01-Sep-98      TWH  Fix feature name
// 04-Sep-98 $$1  TWH  add lmlic.env,
// 10-Sep-98 $$2  JJE  rm nmsd logic, add path setting
// 17-Sep-98 $$3  TWH  Don't need nlocli.inc
// 02-Nov-98 $$4  TWH  add ptc_end label; rm nlocli.inc
// 20-Nov-98 $$5  TWH  Add PDM_LDB_PATH check
// 02-Feb-99      JJE  Fix comment markers
// 22-Apr-99 $$6  TWH  Set ORA_NLS32 to oracle734 per echin
// 01-May-00      TWH  Add ORACLE_HOME
// 05-May-00 $$7  TWH  Change ptc-cmdtype
// 18-May-00 $$8  TWH  Drop use of pdm_proe.env
// 19-Mar-01 $$9  ALG  Include ptc_startup.inc
// 11-May-01 $$10 TWH  Update path
// 27-Jun-01 $$11 TWH  Fix path line
// 12-Nov-01 $$12 TWH  Set ORA_NLS33 to ORACLE817
// 28-Apr-03 K-01-06 $$13 TWH Add exit
// 23-Jul-03 K-01-11 $$14 TWH Use Proe wrapper scripts
// 21-Aug-03 K-01-13 $$15 TWH Fix above
// 17-Feb-04 K-01-26 $$16 TWH Chg comment line 1071392
// 19-Jul-04 K-03-06 $$17 TWH Chg to ORACLE920
// 18-APR-05 K-03-23 $$18 MTP Use head_ilink
// 29-Jul-05 K-03-30 $$19 TWH Drop win95
// 22-Aug-07 K-03-50 $$20 TWH Chg path to ORACLE9208
// 08-Nov-07 K-03-52 $$21 TWH fix #19 missing label
// 17-Jan-08 L-01-42 $$22 TWH Chg path to ORACLE10203
#include:head_ilink.inc

REM ptc-cmdtype: tkenv 

if NOT "%PDM_LDB_PATH%" == "" goto pdmldbpathset
  if "%HOMEDRIVE%" == "" goto pdmldbpathset
    if "%HOMEPATH%" == "" goto pdmldbpathset
      set PDM_LDB_PATH=%HOMEDRIVE%%HOMEPATH%
:pdmldbpathset

#includeenv:PROE_DIR:f_proe_dir.env
#includeenv:PROE_START:f_proe_start.env

#includeenv:LM_LICENSE_FILE:lmlic.env
set ILINK_TK_ACCESS_FEATURE_NAME=__ILINK_FEATURE_NAME__

set PDM_ORA_APPL=intralink
set PDM_ORACLE_SERVER=intralink
set TNS_ADMIN=%prodir%

set ORACLE_HOME=%prodir%\%mc%\obj\oracle10203
set EPC_DISABLED=TRUE
set ORA_NLS33=%ORACLE_HOME%\ocommon\nls\admin\data
set path=%ORACLE_HOME%\bin;%path%;%prodir%\%mc%\obj;%prodir%\pro_intlnk_tk\%mc%\lib
set NLS_LANG=AMERICAN_AMERICA.WE8ISO8859P1

#include:ptc_startup.inc

REM Insert full path to your Pro/INTRALINK Toolkit Application below
REM Example:  c:\local\apps\MyToolkitApp %*

:ptc_end
exit /B %ERRORLEVEL%
