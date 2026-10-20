//  29-Sep-98      JJE  Created
//  15-Apr-99 $$1  JJE  Submit to I-01
// 28-Apr-03 K-01-06 $$2 TWH Add exit
#include:head.inc

%PRO_DIRECTORY%\%MC%\jre\bin\jrew -cp %PRO_DIRECTORY%\%MC%\jms\jmyriad.jar com.infograph.jmyriad.server.MyriadServer %1 %2 %3 %4 %5 %6

:ptc_end
exit /B %ERRORLEVEL%

