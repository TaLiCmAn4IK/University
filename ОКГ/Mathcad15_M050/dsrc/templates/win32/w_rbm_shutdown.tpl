// 03-Feb-98  $$1  JJE  Created
// 14-Aug-98 $$2  JJE  Support spaces in directories
// 28-Oct-98 $$3  TWH  use PRORB_RPCNUM for shutdown
// 28-Apr-03 K-01-06 $$4 TWH Add exit
#include:head.inc

if NOT "%PRORB_RPCNUM%" == "" goto skiprpcnum
set PRORB_RPCNUM=__PRORB_RPCNUM__
:skiprpcnum

"%PRO_DIRECTORY%\%MC%\obj\rbm_shutdown.exe" -rpc %PRORB_RPCNUM% %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%

