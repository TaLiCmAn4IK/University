// 26-Jan-98 $$1  JJE  Created.
#include:head.inc

#include:shlib.inc

setenv BATCH_STAT_DIRECTORY $prodir/probatch

exec $prodir/$mc/obj/batch_stat $*
