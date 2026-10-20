// 18-Sep-00 $$1  MAZ  Created
// 18-Oct-00 $$2  MAZ  Use %prodir% and java.exe: fix SPRs 845515 & 846064 
// 28-Apr-03 K-01-06 $$3 TWH Add exit
#include:head.inc

start "Optegra Adapter" /min java.exe -Xms128m -Xmx256m -classpath %prodir%\ieadapter\OptegraIEAdapter.jar -DEPD_HOME=%prodir% com.ptc.optegra.ieadapter.OptIEAdapterMain
exit /B %ERRORLEVEL%

