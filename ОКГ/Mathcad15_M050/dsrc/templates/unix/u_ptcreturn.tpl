// 12-Jul-03  K-01-11  $$1 JJE Created
#include:head.inc

if ($#argv < 1) then
    echo ""
    echo 'Usage: ptcreturn <feature>'
    echo ""
	echo "Example: ptcreturn PROE_LIC"
    echo ""
    exit -1
endif

setenv LM_LICENSE_FILE __LM_LICENSE_FILE__

exec $prodir/$mc/obj/lmutil lmborrow -return $*
