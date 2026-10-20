// 29-Feb-00 $$1  MAZ  Created.
// 10-Apr-00      MAZ  Sub in full path to JVM cmd
// 11-Apr-00 $$2  MAZ  Use head.inc
// 02-May-00 $$3  MAZ  Use classpath instead of cp
// 25-Oct-00 $$4  MAZ  Use the new ie.jar path
#include:head.inc

xterm -title "name service" -e __JVM_CMD__ -classpath $prodir/codebase/WEB-INF/lib/ie.jar -DpropFile=$prodir/ie.properties com.infoengine.au.NamingService
exit
