// 27-Mar-97 $$1  Pete  Created.
// 26-Jan-98 $$2  JJE   Fix.
// 17-Dec-98 $$3  TWH   Add PRO_LICENSE_RES
#include:head.inc

#include:shlib.inc

setenv PROBATCH_DIRECTORY $prodir/probatch
setenv BATCHSCRIPT_DIRECTORY $prodir/bin
setenv PRO_LICENSE_RES $prodir/text/licensing/license.res

if ("$1" == "-text") then
     exec $prodir/$mc/obj/pro_batch_t $2 $3 $4 $5 $6
else
     exec $prodir/$mc/obj/pro_batch $*
endif
