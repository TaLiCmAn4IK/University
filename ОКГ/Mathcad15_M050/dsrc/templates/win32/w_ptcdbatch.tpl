//
// File: w_ptcdbatch.tpl
//
// 05-Mar-02 $$1  ALG  Created
// 28-Mar-02 $$2  MAZ  SPR 938739: Remove quotes from env var values
// 25-Apr-02 $$3  ALG  Remove DSQSERVICE_FEATURE_NAME
// 30-Apr-02 $$4  TWH  Add PRO_COMM_MSG_EXE
// 07-Nov-02 J-03-37  ALG  $$5   SPR 985495: Add NMSD_PATH for standalone mode
// 22-Nov-02 J-03-38  ALG  $$6   SPR 971401: Move most of contents to new
//                                 "common" file w_ptcdbatch.inc
// 28-Apr-03 K-01-06 $$7 TWH Add exit
//
#include:ptcdbatch.inc

%start_cmd% "%PRO_DIRECTORY%\%MC%\obj\dbatchc.exe" %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%

