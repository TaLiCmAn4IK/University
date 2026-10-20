// 12-Oct-00 $$1  MAZ  Created
// 27-Oct-00 $$2  MAZ  Remove "start" call
// 11-May-01 $$3  ALG  Allow spaces in TOMCAT_HOME
// 29-Nov-01      JLK  JGL.jar needed in CLASSPATH (don't know why?)
// 05-Dev-01 $$4  JLK  Set CLASSPATH only for Windchill
// 15-Jan-02 $$5  EFR  Set CLASSPATH for PartsLink
// 11-Feb-02 $$6  JAD  Set CLASSPATH for PDMLink
// 14-Mar-02      JAD  Set CLASSPATH for SupplyLink
// 01-Apr-02      DDS  Set CLASSPATH for ProjectLink R6.2
// 01-Apr-02      JAD  Set CLASSPATH for PDMLink to enable DCA
// 05-Aug-02 $$7  JMH  Merged all CLASSPATHs (other than ecatalog) into single ordered path list
@echo off

set TOMCAT_HOME=__SERVLET_HOME__
set JAVA_HOME=__WT_JVMHOMEDIR__
set PATH=%JAVA_HOME%\bin
if exist "__LOADPOINT__\codebase\wt.properties" ( set CLASSPATH=__LOADPOINT__\codebase;__LOADPOINT__\codebase\WEB-INF\lib\ie.jar;__LOADPOINT__\codebase\WEB-INF\lib\JGL.jar;__LOADPOINT__\codebase\WEB-INF\lib\xerces.jar;__LOADPOINT__\codebase\WEB-INF\lib\saxon.jar;__LOADPOINT__\codebase\WEB-INF\lib\xalan.jar;__LOADPOINT__\codebase\WEB-INF\lib\mail.jar;__LOADPOINT__\codebase\WEB-INF\lib\activation.jar;__LOADPOINT__\codebase\WEB-INF\lib\soap.jar ) else if exist "__LOADPOINT__\ecatalog\main.properties" ( set CLASSPATH=__LOADPOINT__\codebase )

"%TOMCAT_HOME%\bin\startup.bat"
