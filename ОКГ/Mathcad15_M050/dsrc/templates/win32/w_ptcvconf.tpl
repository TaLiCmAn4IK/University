// 01-Feb-2002 $$1 JJE Created
// 29-Apr-2002 $$2 ksi made changes
// 15-May-2002 $$3 ACT Launch csd before proevconf
// 21-May-2002 $$4 ksi don't launch csd, remove /min
// 21-Aug-02 J-03-32  ALG  $$5   Change "proevconf" -> "ptcvconf"; Rename file
// 28-Apr-03 K-01-06 $$6 TWH Add exit
// 16-May-08 L-03-09 $$7 TWH Add Mozilla entry
#include:nmsdstart.inc

set MOZILLA_FIVE_HOME=%PRO_DIRECTORY%\%mc%\obj\MOZILLA

%start_cmd% "%PRO_DIRECTORY%\%mc%\obj\ptcvconf.exe" %1 %2 %3 %4 %5 %6 %7 %8 %9 &

:ptc_end
exit /B %ERRORLEVEL%

