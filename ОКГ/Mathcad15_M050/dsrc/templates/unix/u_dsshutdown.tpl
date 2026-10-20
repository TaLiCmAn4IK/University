// 23-Jul-03 K-01-11   MAZ  $$1   SPR 1025041: copy from dcadsetup
#!/bin/csh -f

#
# WARNING: please do not edit the script content below.
#

#include:head.inc

set nmsdir=$prodir/$mc/nms

$nmsdir/nmsq -quiet -off

$nmsdir/nmsq -quiet -servinfo

if ($status == 0) then
    echo "DS services disabled successfully"
    exit 0
else
    $nmsdir/nmsq -quiet -ping
    
    if ($status == 0) then
        echo "dsshutdown failed. Incompatible NMSD daemon is running."
        exit 1
    else
        echo "NMSD is not running."
        exit 0
    endif
endif
