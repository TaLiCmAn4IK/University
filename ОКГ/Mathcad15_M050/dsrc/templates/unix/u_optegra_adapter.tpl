// 18-Sep-00 $$1  MAZ  Created
// 18-Oct-00 $$2  MAZ  Fix SPR 845515 by calling java instead of jre 
#include:head.inc

java -Xms128m -Xmx256m -cp $prodir/ieadapter/OptegraIEAdapter.jar -DEPD_HOME=$prodir com.ptc.optegra.ieadapter.OptIEAdapterMain 

