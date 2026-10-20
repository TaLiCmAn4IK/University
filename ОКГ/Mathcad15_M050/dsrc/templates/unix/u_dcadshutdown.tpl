// 13-Mar-00 $$1 JJE Created
// 13-Apr-00 $$2 JJE Changes
// 05-Mar-03 K-01-02  ALG  $$3   SPR 1003705: Exit status
#include:head.inc

set nmsdir=$prodir/$mc/nms

$nmsdir/nmsq -quiet -off

if ($status == 0) then
    echo "DCAD disabled successfully"
    exit 0
else
    $nmsdir/nmsq -quiet -ping
    
    if ($status == 0) then
        echo "dcad_shutdown failed. Incompatible nmsd daemon is running."
        exit 1
    else
        echo "nmsd is not running. DCAD was not enabled"
        exit 0
    endif
endif
