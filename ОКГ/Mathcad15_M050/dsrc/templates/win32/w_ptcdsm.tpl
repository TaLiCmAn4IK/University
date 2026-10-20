//
// File: w_ptcdsm.tpl
//
// 05-Mar-02 $$1  ALG  Created
// 28-Mar-02      MAZ  SPR 938739: Remove quotes from env var values
// 28-Mar-02 $$2  ALG  Rename file: "w_ptcdsqm.tpl" --> "w_ptcdsm.tpl"
// 15-Apr-02 $$3  ALG  Remove *_FEATURE_NAME lines
// 11-Jun-02 $$4  ALG  Add __AE_FLAG__ (Will be either " -ae" or "")
// 22-Nov-02 J-03-38  ALG  $$5   SPR 971401: Move most of contents to new
//                                 "common" file w_ptcdsm.inc
// 28-Apr-03 K-01-06 $$6 TWH Add exit
//
#include:ptcdsm.inc

%start_cmd% "%PRO_DIRECTORY%\%MC%\obj\dsq.exe" %1 %2 %3 %4 %5 %6
:ptc_end
exit /B %ERRORLEVEL%
