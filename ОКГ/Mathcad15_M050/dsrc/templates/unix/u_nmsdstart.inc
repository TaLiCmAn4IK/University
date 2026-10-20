// 24-Apr-01 $$1 ksi Created
#include:head.inc
#includeenv:PTCNMSPORT:ptcnmsport.env

setenv PRO_DIRECTORY $prodir
set nmsq=$prodir/$mc/nms/nmsq
set nmsd=$prodir/$mc/nms/nmsd

$nmsq -quiet -ping
set nmsd_down=$status

if ($nmsd_down == 0) then
    $nmsq -quiet -hostinfo | grep MachineType > /dev/null
    if ($status != 0) then
        echo Shutting down Name Service ...
        $nmsq -quiet -shutdown
        set nmsd_down=1
    endif
endif

if ($nmsd_down != 0) then
    echo Starting Name Service ...
    $nmsd -dcad -timeout 600&
endif
	
