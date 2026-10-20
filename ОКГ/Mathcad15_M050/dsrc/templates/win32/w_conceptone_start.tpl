// 28-Nov-01 $$1  ott   Created.
// 10-Apr-02 $$2  ott   Added PVIEW_HOME
// 10-Jul-02 $$3  ott   Changed name to ProCONCEPT
// 28-Apr-03 K-01-06 $$4 TWH Add exit
// 06-Oct-03 K-01-16 $$5 TWH Replace cdrs_library.env
// 01-Jul-04 K-03-06 $$6 TWH Fix label to match goto
// 29-Jul-05 K-03-30 $$7 TWH Drop win95
// 18-Jan-08 L-01-42 $$8 TWH Update PVIEW env var
#include:head.inc

REM ptc-cmdtype: concept

if "%PAINT_HOME%" == "" set PAINT_HOME=%PRO_DIRECTORY%

if NOT "%PAINTOBJPATH%" == "" goto paintobjpath
set path=%PAINT_HOME%\%MC%\obj;%path%
set PAINTOBJPATH=true
:paintobjpath

REM set PVIEW_PROEPVIEW_HOME and PAINT_DATA

set PVIEW_PROEPVIEW_HOME=%PAINT_HOME%\apps\prodview
set PAINT_DATA=%PAINT_HOME%\tutorial
set DES_LIB=__DES_LIB__
if "%DES_LIB%" == "__!!DES_LIB!!__" goto skpdes
  set CDRS_LIBRARY=__DES_LIB__\graphic-library
:skpdes

set MACHINE=%MC%

#includeenv:PTCNMSPORT:ptcnmsport.env
set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg.exe

#includeenv:LM_LICENSE_FILE:lmlic.env
set CONCEPT_FEATURE_NAME=__CONCEPT_FEATURE_NAME__

#include:ptc_startup.inc

%start_cmd% /b "%PAINT_HOME%\%MC%\obj\concept" %1 %2 %3 %4 %5

:ptc_end
exit /B %ERRORLEVEL%

