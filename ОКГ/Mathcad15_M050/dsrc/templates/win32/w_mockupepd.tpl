// 17-Jan-05 J-01-68+     Bruno  Created
// 24-Jan-05 K-03-18  $$1 MAZ    Submit to K-03
@echo off

#include:head.inc

REM ptc-cmdtype: mockup 

#includeenv:LM_LICENSE_FILE:lmlic.env
set MOCKUP_FEATURE_NAME=__MOCKUP_FEATURE_NAME__
set DIVISIONROOT=%PRO_DIRECTORY%
set DV_FILECACHE_MODE=0
%start_cmd% "%PRO_DIRECTORY%\%MC%\obj\dvs" -n dvmockup %1 %2 %3 %4 %5 %6
:ptc_end
