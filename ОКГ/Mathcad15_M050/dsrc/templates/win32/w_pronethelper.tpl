// 17-Jul-2000 $$1 JJE Created
// 11-Aug-2000 $$2 JJE Add nmsd startup
// 14-Aug-2000 $$3 JJE Fix start line
// 07-Nov-2000 $$4 JJE Pass more args, add comments
// 28-Apr-03 K-01-06 $$5 TWH Add exit
#include:head.inc

#includeenv:PTCNMSPORT:ptcnmsport.env

REM This script gets one only parameter -  A trb file ( Tutorial resource bundle ).
REM Under no circumstances send more than one parameter to this script, because
REM the script will treat it as part of the trb file path.

REM The "-trb:%1 .. %9" is used in order to support trb files which contain blanks in their path.
REM Batch files don't know how to handle blanks. Using this syntax there can be  up to 8
REM blanks in the trb's path.

set PRO_TUTORIAL_START="%PRODIR%\bin\protutorial.bat"
%start_cmd% "%PRODIR%\%MC%\obj\ptc_launcher" "-trb:%1 %2 %3 %4 %5 %6 %7 %8 %9"

:ptc_end
exit /B %ERRORLEVEL%
