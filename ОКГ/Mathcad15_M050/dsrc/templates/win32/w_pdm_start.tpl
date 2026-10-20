//  28-Apr-98 $$1  ZRL  Created
//  03-Jun-98 $$2  ZRL  use PDM_FEATURE_NAME
// 14-Aug-98       JJE  Support spaces in directories
// 01-Sep-98  $$3  JJE  Changes for spaces in directories
// 29-Sep-98  $$4  JJE  Fix comment line to use rem
// 19-Mar-01  $$5  ALG  Include ptc_startup.inc
// 28-Apr-03 K-01-06 $$6 TWH Add exit
#include:head.inc

REM ptc-cmdtype: pdmclient

#includeenv:PTCNMSPORT:ptcnmsport.env

#includeenv:PROE_START:proe_start.env
 
set PRO_COMM_MSG_EXE=%PRO_DIRECTORY%\%MC%\obj\pro_comm_msg.exe
set PROTAB=protab
set PDM_DIRECTORY=%PRO_DIRECTORY%\pdm

set PDM_CLIENT_DIR=%PRO_DIRECTORY%\etc\%PTC_HOSTNAME%

if NOT "%PDM_REVCTL%" == "" goto revctl_ok
set PDM_REVCTL=%PRO_DIRECTORY%\%MC%\obj\pdmrnctl.exe
:revctl_ok

#includeenv:LM_LICENSE_FILE:lmlic.env
set PDM_FEATURE_NAME=__PDM_FEATURE_NAME__

#include:ptc_startup.inc

%start_cmd% "%PRO_DIRECTORY%\%MC%\obj\pdm.exe" %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%

