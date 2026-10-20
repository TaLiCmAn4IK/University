// 25-May-99      JJE  Created.
// 02-Jul-99      JJE  Switch jre settings
// 27-Jul-99      JJE  Changes
// 10-Aug-99      JJE  Add a missing slash
// 23-Aug-99      JJE  Add some quotes
// 26-Aug-99      JJE  Changes per rembold
// 27-Sep-99 $$1  JJE   Submit
// 13-Mar-02 $$2  MAZ  Add change from Ben Fachner
// 28-Apr-03 K-01-06 $$3 TWH Add exit
// 29-Jul-05 K-03-30 $$4 TWH Drop win95
@echo off

#include:head.inc

set path=%path%;%PRO_DIRECTORY%\%MC%\lib

#includeenv:LM_LICENSE_FILE:lmlic.env
set VERICUT_FEATURE_NAME=__VERICUT_FEATURE_NAME__

set CGTECH_INSTALL=%PRO_DIRECTORY%
set CGTECH_PRODUCTS=%CGTECH_INSTALL%\%MC%\vericut
set CGTECH_CLASSES=%CGTECH_PRODUCTS%
if "%CGTECH_LOCALE%" == "" set CGTECH_LOCALE=english
set CGTECH_LANGUAGE=%CGTECH_CLASSES%\%CGTECH_LOCALE%
set CGTECH_RGB_TXT=%CGTECH_PRODUCTS%\rgbhues.txt
set CGTECH_JRE=%CGTECH_PRODUCTS%\jre
set CGTECH_DEFAULT_UNITS=INCH

set CGTECH_LIBRARY=%CGTECH_INSTALL%\text\vericut\library\
set CGTECH_SAMPLES=%CGTECH_INSTALL%\text\vericut\samples\
set CGTECH_HELP=%CGTECH_INSTALL%\html\usascii

if "%TEMP%" == "" set TEMP=C:\TEMP
if "%TMP%" == "" set TMP=C:\TEMP


if NOT "%CGTPATH%" == "" goto cgtpathset
if "%OS%" == "Windows_NT" set path=%CGTECH_PRODUCTS%;%PRO_DIRECTORY%\%MC%\obj;%PATH%
if "%OS%" == "" set path="%CGTECH_PRODUCTS%;%PRO_DIRECTORY%\%MC%\obj;%PATH%"
set cgtpath=true
:cgtpathset

set waitopt=
set argstr=
if test%1==test goto cgtchkend
if %1==BATCH set waitopt=/wait
if %1==batch set waitopt=/wait
if %1==BATCHP set waitopt=/wait
if %1==batchp set waitopt=/wait
set argstr=%1
shift
:cgtgetarg
if test%1==test goto cgtchkend
if %1==BATCH set waitopt=/wait
if %1==batch set waitopt=/wait
if %1==BATCHP set waitopt=/wait
if %1==batchp set waitopt=/wait
set argstr=%argstr% %1
shift
goto cgtgetarg
:cgtchkend

if "%OS%" == "Windows_NT" set start_cmd=start "VERICUT for Pro/Engineer" %waitopt%
if "%OS%" == "" set start_cmd=start %waitopt%

%start_cmd% "%CGTECH_JRE%\bin\javaw" -Xms16m -Xmx64m -Xss4m -classpath "%CGTECH_CLASSES%;%CGTECH_CLASSES%\CGTech.jar" Vericut %argstr%
:end_cgtech
exit /B %ERRORLEVEL%
