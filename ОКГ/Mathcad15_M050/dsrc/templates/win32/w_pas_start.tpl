// 10-Mar-00  $$1  TWH  Created.
// 14-Apr-00  $$2  TWH  Drop licensing
// 30-May-00  $$3  TWH  Rm start /b from cmd line; Add %mc%\obj to path
//                      Chg PDM_CLIENT_DIR, PRO_COMM_MSG_EXE; full path to java
// 28-Apr-03 K-01-06 $$4 TWH Add exit
#include:head.inc

if NOT "%mc%" == "i486_nt" goto platerr

set PDM_CLIENT_DIR=%PRO_DIRECTORY%\etc\%PTC_HOSTNAME%
set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg.exe

set PTC_JRE=__PTC_JRE__
// add %mc%\obj\ for ptc dll's loaded by JVM
set path="%PTC_JRE%\bin;%PRO_DIRECTORY%\%mc%\obj\;%path%"

cd /d "%PRO_DIRECTORY%"

%PTC_JRE%\bin\java.exe -Djava.security.policy="%PRO_DIRECTORY%\lib\security\pas.policy" -Xms4m -classpath "%PRO_DIRECTORY%\lib\toolkit.jar;%PRO_DIRECTORY%" com.ptc.propdm.server.PGToolkitServer

goto ptc_end

:platerr

echo "Pro/PDM Application Server is supported on i486_nt platforms only."
goto ptc_end

:ptc_end
exit /B %ERRORLEVEL%

