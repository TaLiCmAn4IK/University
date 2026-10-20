// 23-Sep-98 $$1  JJE   Created
// 01-Oct-99 $$2  JJE   Add xml
// 25-Nov-02 J-03-38  JJE  $$3   Updates to java command
// 06-Dec-02 J-03-39  JJE  $$4   Use java not jre
#include:head.inc

set PTC_MFG_JAVA_SRC=$PRO_DIRECTORY/apps/mfgapps/java

java -cp $PTC_MFG_JAVA_SRC/lib/template.jar:$PTC_MFG_JAVA_SRC/lib/crimson.jar com.ptc.mfg.template.TemplateApp
