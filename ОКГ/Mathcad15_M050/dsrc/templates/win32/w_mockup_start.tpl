// 09-Sep-99 $$1  MAZ  Created.
// 29-Sep-99 $$2  MAZ  Call dvmockup instead of dvreality
// 22-Mar-00 $$3  MYA  Change dvmockup to dvremote64
// 19-Mar-01 $$4  ALG  Include ptc_startup.inc
// 28-Apr-03 K-01-06 $$5 TWH Add exit
// 17-Jan-05 J-01-68+    Bruno Update dvs args
// 24-Jan-05 K-03-18  $$6 MAZ    Submit to K-03
@echo off

#include:head.inc

REM ptc-cmdtype: mockup 

#includeenv:LM_LICENSE_FILE:lmlic.env
set MOCKUP_FEATURE_NAME=__MOCKUP_FEATURE_NAME__
set DIVISIONROOT=%PRO_DIRECTORY%

#include:ptc_startup.inc

%start_cmd% "%PRO_DIRECTORY%\%MC%\obj\dvs" -n dvmockup %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%

