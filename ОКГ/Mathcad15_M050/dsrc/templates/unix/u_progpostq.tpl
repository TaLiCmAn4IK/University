// 12-Apr-99 $$1  JJE  Created.
// 20-Oct-99 $$2  JJE  Change color to navy
#include:head.inc

setenv CAMSYS $prodir/$mc/obj
setenv CAMLIB $prodir/$mc/gpost

xterm -bg navy -fg yellow -title "Optfile" -e $CAMSYS/optfile $*

