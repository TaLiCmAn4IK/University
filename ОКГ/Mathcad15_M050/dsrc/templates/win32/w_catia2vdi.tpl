// 09-Sep-99 $$1  MAZ  Created.
// 22-Mar-00 $$2  MYA  change MOCKUP_CONVERTER to MOCKUPCONVERTER
// 28-Apr-03 K-01-06 $$3 TWH Add exit
@echo off

#include:head.inc

#includeenv:LM_LICENSE_FILE:lmlic.env
set MOCKUPCONVERTER_FEATURE_NAME=__MOCKUP_CONV_FEATURE_NAME__
set DIVISIONROOT=%PRO_DIRECTORY%

%start_cmd% "%PRO_DIRECTORY%\%MC%\obj\catia2vdi" %1 %2 %3 %4 %5 %6 %7 %8 %9
:ptc_end
exit /B %ERRORLEVEL%
