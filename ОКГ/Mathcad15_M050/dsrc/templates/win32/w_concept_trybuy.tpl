// 11-Mar-03 K-01-03 $$1  OTT   Created.
// 28-Apr-03 K-01-06 $$2 TWH Add exit
// 29-Jul-05 K-03-30 $$3 TWH Drop win95
// 18-Jan-08 L-01-42 $$4 TWH Update PVIEW settings
#include:head.inc

REM ptc-cmdtype: concept_trybuy

if "%PAINT_HOME%" == "" set PAINT_HOME=%PRO_DIRECTORY%

if NOT "%PAINTOBJPATH%" == "" goto paintobjpath
set path=%PAINT_HOME%\%MC%\obj;%path%
set PAINTOBJPATH=true
:paintobjpath

REM set PVIEW_PROEPVIEW_HOME and PAINT_DATA

set PVIEW_PROEPVIEW_HOME=%PAINT_HOME%\apps\prodview
set PAINT_DATA=%PAINT_HOME%\tutorial

set MACHINE=%MC%

#includeenv:PTCNMSPORT:ptcnmsport.env
set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg.exe

#includeenv:LM_LICENSE_FILE:lmlic.env
set CONCEPT_FEATURE_NAME=CONCEPT_TRYBUY

#include:ptc_startup.inc

%start_cmd% /b "%PAINT_HOME%\%MC%\obj\concept" %1 %2 %3 %4 %5

:ptc_end
exit /B %ERRORLEVEL%
