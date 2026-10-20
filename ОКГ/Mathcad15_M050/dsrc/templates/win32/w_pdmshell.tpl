// 28-Apr-98 $$1  ZRL  Created
// 14-Aug-98      JJE  Support spaces in directories
// 01-Sep-98 $$2  JJE  Changes for spaces in directories
// 05-Jan-00 $$3  VEG  Passed more args
// 28-Apr-03 K-01-06 $$4 TWH Add exit
#include:head.inc

#includeenv:PTCNMSPORT:ptcnmsport.env

set PDM_DIRECTORY=%PRO_DIRECTORY%\pdm
set PRO_COMM_MSG_EXE=%PRO_DIRECTORY%\%MC%\obj\pro_comm_msg.exe
if EXIST "%PRO_DIRECTORY%\etc\%PTC_HOSTNAME%\pdmservers.txt" set PDM_CLIENT_DIR=%PRO_DIRECTORY%\etc\%PTC_HOSTNAME%

"%PRO_DIRECTORY%\%MC%\obj\pdmshell" %1 %2 %3 %4 %5 %6 %7 %8 %9
:ptc_end
exit /B %ERRORLEVEL%

