// 22-Mar-00 $$1  MYA  Created
// 28-Apr-03 K-01-06 $$2 TWH Add exit
@echo off

#include:head.inc

REM ptc-cmdtype: mockup 

#includeenv:LM_LICENSE_FILE:lmlic.env
set MOCKUP_FEATURE_NAME=__MOCKUP_FEATURE_NAME__
set DIVISIONROOT=%PRO_DIRECTORY%

%start_cmd% "%PRO_DIRECTORY%\%MC%\obj\dvs" -n dvremote %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%

