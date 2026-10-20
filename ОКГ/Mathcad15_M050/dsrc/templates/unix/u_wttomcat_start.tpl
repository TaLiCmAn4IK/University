// 12-Oct-00 $$1  MAZ  Created.
// 29-Nov-01      JLK  Clear CLASSPATH and create symlink
// 03-Dec-01      JLK  Clear with unsetenv instead of setenv
// 05-Dec-01 $$2  JLK  Add xerces to classpath for Windchill only
#include:head.inc

setenv TOMCAT_HOME __SERVLET_HOME__ 
setenv JAVA_HOME __WT_JVMHOMEDIR__

if ( -e __LOADPOINT__/codebase/wt.properties ) then
   setenv CLASSPATH __LOADPOINT__/codebase/WEB-INF/lib/xerces.jar
else
   unsetenv CLASSPATH
endif

setenv PATH $JAVA_HOME/bin:$PATH

if ( ! -e __LOADPOINT__/codebase/WEB-INF/classes ) then
   ln -s __LOADPOINT__/codebase __LOADPOINT__/codebase/WEB-INF/classes
endif

$TOMCAT_HOME/bin/startup.sh
