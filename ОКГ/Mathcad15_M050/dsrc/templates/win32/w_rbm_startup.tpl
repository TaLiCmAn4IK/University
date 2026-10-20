// 03-Feb-98  $$1  JJE  Created
// 14-Aug-98       JJE  Support spaces in directories
// 01-Sep-98  $$2  JJE  Changes for spaces in directories
// 19-May-99  $$3  JJE  Add ilink includes
// 19-Mar-01  $$4  TWH  Change for ilinkdir.env
// 28-Apr-03 K-01-06 $$5 TWH Add exit
// 29-Jul-05 K-03-30 $$6 TWH Drop win95
#include:head.inc

#include:ilinkdir.env

#include:ilinkpath.inc

set PTC_BMODE=true
set PRO_COMM_MSG_EXE=%PRO_DIRECTORY%\%MC%\obj\pro_comm_msg.exe
set PRORB_RUNDIR=__PRORB_RUNDIR__
set PRORB_STATFILE=__PRORB_STATFILE__
set PRORB_PROE_START=__PRORB_PROE_START__
set PRORB_NUM_BATCHPROES=__PRORB_NUM_BATCHPROES__
set PRORB_HK_TIMEOUT=__PRORB_HK_TIMEOUT__
set PRORB_RPCNUM=__PRORB_RPCNUM__

set REMD_NTSERVICE=FALSE
%start_cmd% /min "%PRO_DIRECTORY%\%MC%\obj\prorembd.exe"
%start_cmd% /min "%PRO_DIRECTORY%\%MC%\obj\prorembatch.exe"
set PTC_BMODE=
:ptc_end
exit /B %ERRORLEVEL%

