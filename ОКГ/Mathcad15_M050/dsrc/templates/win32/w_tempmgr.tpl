// 23-Sep-98 $$1  JJE   Created
// 01-Oct-99 $$2  JJE   Add xml
// 14-Sep-00 $$3  JJE   Add quotes
// 25-Nov-02 J-03-38  JJE  $$4   Updates to java command
// 06-Dec-02 J-03-39  JJE  $$5   Use java not jre
// 28-Apr-03 K-01-06 $$6 TWH Add exit
#include:head.inc

set PTC_MFG_JAVA_SRC=%PRO_DIRECTORY%\apps\mfgapps\java

java -cp "%PTC_MFG_JAVA_SRC%\lib\template.jar;%PTC_MFG_JAVA_SRC%\lib\crimson.jar" com.ptc.mfg.template.TemplateApp

:ptc_end
exit /B %ERRORLEVEL%

