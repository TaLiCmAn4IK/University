//  28-Sep-98 $$1  JJE  Created
// 22-Apr-99  $$2  TWH  Set ORA_NLS32 to oracle734 per echin
// 14-Dec-00  $$3  TWH  Set ORA_NLS32 to ORACLE816
// 04-Apr-01  $$4  TWH  Add oracle816\bin to path
// 23-Apr-01  $$5  TWH  ORA_NLS32 -> ORA_NLS33
// 02-May-01  $$6  TWH  fix typo
// 10-May-01  $$7  TWH  Add ORACLE_HOME
// 12-Nov-01  $$8  TWH  Set ORA_NLS33 to ORACLE817
// 28-Apr-03 K-01-06 $$9 TWH Add exit
// 19-Jul-04 K-03-06 $$10 TWH Chg to ORACLE920
// 18-APR-05 K-03-23 $$11 MTP Use head_ilink
// 29-Jul-05 K-03-30 $$12 TWH Drop win95
// 22-Aug-07 K-03-50 $$13 TWH Chg path to ORACLE9208
// 17-Jan-08 L-01-42 $$14 TWH Chg path to ORACLE10203
#include:head_ilink.inc

REM ptc-cmdtype: iasadmin

set TNS_ADMIN=%PRO_DIRECTORY%
set PDM_ORACLE_SERVER=intralink
set PDM_ORA_APPL=intralink
set ORACLE_HOME=%PRO_DIRECTORY%\%MC%\obj\ORACLE10203
set ORA_NLS33=%PRO_DIRECTORY%\%MC%\obj\ORACLE10203\ocommon\nls\admin\data

#includeenv:LM_LICENSE_FILE:lmlic.env
set ILINK_WEBCLIENT_FEATURE_NAME=__ILINK_WEBCLIENT_FEATURE_NAME__

set path=%PRO_DIRECTORY%\%MC%\obj\oracle816\bin;%path%

%start_cmd% "%PRO_DIRECTORY%\%MC%\obj\IASAdmin.exe" %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%

