// 27-Mar-97 $$1  TWH  Created.
// 22-Oct-97 $$2  TWH  Support passed args
// 23-Oct-97 $$3  TWH  Rm PDM_KEY dependancy
// 28-Jul-98 $$4  TWH  Rm dupelicate code, not a cmd-type
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98 $$5  JJE   Changes for spaces in directories
// 17-Sep-98 $$6  TWH  Don't need nlocli.inc
// 02-Nov-98 $$7  JJE  Add ptc_end tag
// 22-Apr-99 $$8  TWH  Set ORA_NLS32 to oracle734 per echin
// 14-Dec-00 $$9  TWH  Set ORA_NLS32 to ORACLE816
// 20-Jul-01 $$10 TWH  Fix ORA_NLS, PATH, existance check
// 12-Nov-01 $$11 TWH  Set ORA_NLS33 to ORACLE817
// 03-Jan-01 $$12 TWH  Fix ORACLE_HOME setting 916278
// 28-Apr-03 K-01-06 $$13 TWH Add exit
// 12-May-04 K-03-02 $$14 TWH Drop ORA_NLS33; chg to ORACLE920
// 18-APR-05 K-03-23 $$15 MTP Use head_ilink
// 29-Jul-05 K-03-30 $$16 TWH Drop win95
// 22-Aug-07 K-03-50 $$17 TWH Chg path to ORACLE9208
// 17-Jan-08 L-01-42 $$18 TWH Chg path to ORACLE10203
#include:head_ilink.inc

set PDM_ORA_APPL=intralink
set PDM_ORACLE_SERVER=intralink
set TNS_ADMIN=%prodir%
set ORACLE_HOME=%prodir%\%mc%\obj\ORACLE10203
set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg

set path=%ORACLE_HOME%\bin;%path%

if NOT EXIST "%prodir%\%mc%\obj\replicate_folder.exe" goto mcerr

"%prodir%\%mc%\obj\replicate_folder.exe" %* 
goto ptc_end
:mcerr
echo "This utility is not supported on %mc%"
:ptc_end
exit /B %ERRORLEVEL%
