//
// File: w_ptcdbatch_ae.tpl
//
// 22-Nov-02 J-03-38  ALG  $$1   SPR 971401: Created
// 28-Apr-03 K-01-06 $$2 TWH Add exit
//
#include:ptcdbatch.inc

%start_cmd% "%PRO_DIRECTORY%\%MC%\obj\dbatchc.exe" -ae %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%
