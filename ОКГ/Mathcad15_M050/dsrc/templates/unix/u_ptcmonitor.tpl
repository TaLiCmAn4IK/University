// 26-Jan-98 $$1 JJE Created
// 19-Feb-98 $$2 JJE Redirect reread
// 17-Oct-01 $$3 TWH Use PTC_D_LICENSE_FILE
#include:head.inc

setenv PTC_D_LICENSE_FILE $prodir/licensing/license.dat

$prodir/$mc/obj/lmutil lmreread >& /dev/null
$prodir/$mc/obj/plpf_monitor $prodir/licensing/ptcreport.log
