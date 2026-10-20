// 06-Sep-01 $$1 TWH Created from distribute_object
// 12-Nov-01 $$2 TWH Set ORA_NLS33 to ORACLE817; NLS32 -> NLS33
// 29-Nov-01 $$3 TWH Add ORACLE_HOME
// 28-Apr-03 K-01-06 $$4 TWH Add exit
// 19-Jul-04 K-03-06 $$5 TWH Chg to ORACLE920
// 18-APR-05 K-03-23 $$6 MTP Use head_ilink
// 22-Aug-07 K-03-50 $$7 TWH Chg path to ORACLE9208
// 17-Jan-08 L-01-42 $$8 TWH Chg path to ORACLE10203
#include:head_ilink.inc 

set PDM_ORA_APPL=intralink 
set PDM_ORACLE_SERVER=intralink 
set TNS_ADMIN=%prodir% 
set ORA_NLS33=%prodir%\%mc%\obj\ORACLE10203\ocommon\nls\admin\data
set ORACLE_HOME=%prodir%\%mc%\obj\ORACLE10203

set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg 

"%prodir%\%mc%\obj\bom_output.exe" %* 
:ptc_end 
exit /B %ERRORLEVEL%
