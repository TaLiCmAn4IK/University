// 23-Sep-97  $$1  JJE  Created
// 26-Jan-98  $$2  JJE   Move setvars call to head.inc
// 11-May-98  $$3  ZRL  should call pro_batch.exe as well
// 14-Aug-98       JJE  Support spaces in directories
// 01-Sep-98  $$4  JJE  Changes for spaces in directories
// 17-Dec-98  $$5  TWH  Add PRO_LICENSE_RES
// 28-Apr-03 K-01-06 $$6 TWH Add exit
#include:head.inc

set PROBATCH_DIRECTORY=%PRO_DIRECTORY%\probatch
set BATCHSCRIPT_DIRECTORY=%PRO_DIRECTORY%\bin
set PRO_LICENSE_RES=%PRO_DIRECTORY%\text\licensing\license.res

if "%1" == "-text" "%PRO_DIRECTORY%\%MC%\obj\pro_batch_t.exe" %2 %3 %4 %5 %6
if NOT "%1" == "-text" "%PRO_DIRECTORY%\%MC%\obj\pro_batch.exe" %1 %2 %3 %4 %5 %6
 
:ptc_end
exit /B %ERRORLEVEL%

