// 12-Apr-99 $$1  JJE  Created.
// 25-May-99 $$2  JJE  Add param
// 21-Mar-01 $$3  JJE  Add xterm per Raman
#include:head.inc

setenv CAMSYS $prodir/$mc/obj
setenv CAMLIB $prodir/$mc/gpost

if( $?param ) then
  xterm -bg navy -fg yellow -title "GPOST" -e $CAMSYS/gpost $param
else
  xterm -bg navy -fg yellow -title "GPOST" -e $CAMSYS/gpost $*
endif
