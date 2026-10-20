// 17-Feb-00   1 JJE Created
// 15-Mar-00   2 JJE Fixed
// 24-Mar-00 $$1 JJE Add temp dir
// 11-Apr-00     JJE Changes
// 20-Apr-00     JJE Add dcad flag
// 21-Apr-00     JJE Add pro_comm_msg
// 02-May-00 $$2 JJE Use timeout of 500
// 26-Jun-00 $$3 prf Used -env to check compatibility
// 15-Aug-00 $$4 JJE Add PRO_DIRECTORY setting
// 06-Feb-01 $$5 prf Add cadds5atb jobman to PTCNMS_SERVICE_LIST
// 13-Mar-01 $$6 prf Add DISABLE_PRELOAD_PRO_CADDS5 setting
// 19-Feb-02 ALG $$7 SPR 925595: Fix spelling of "succeeded"
// 05-Mar-02 ALG $$8 Add service dbatchs
// 24-Sep-02 J-03-35  ALG  $$9   SPR 977187: Add "-status" argument
#include:head.inc
#includeenv:DCAD_TEMP:dcadtemp.env
#includeenv:PROE_START:proe_start.env

setenv PRO_DIRECTORY $prodir
setenv DISABLE_PRELOAD_PRO_CADDS5 yes

setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg

set nmsdir=$prodir/$mc/nms

if ("$1" == "-status") then
	$nmsdir/nmsq -quiet -ping

	if ($status != 0) then
		echo Could not connect to Name Service Daemon.
		exit -1
	else
		$nmsdir/nmsq -quiet -hostinfo

		echo The following remote services are enabled:
		$nmsdir/nmsq -quiet -servinfo

		echo Environment settings related to dbatchs:
		$nmsdir/nmsq -env PTCNMS_SERVICE_DIR PROE_START DCAD_TEMP

		exit 0
	endif
endif

if (! $?PTCNMS_SERVICE_LIST) then
	setenv PTCNMS_SERVICE_LIST "dsserver cadds5atb jobman dbatchs"
endif

$nmsdir/nmsq -quiet -env PROE_START=$PROE_START

if ($status != 0) then

	$nmsdir/nmsq -quiet -ping

	if ($status != 0) then
		goto nmsdstart
	else if ("$1" == "-force") then
		echo Shutting down Name Service Daemon ...
		$nmsdir/nmsq -quiet -shutdown
		sleep 1
		goto nmsdstart
	endif

	echo DCAD setup failed. Incompatible Name Service Daemon is running.
	echo 1. Exit all PTC applications.
	echo 2. Run $fullscrname -force

	exit -1

else

	printf "Setting up DCAD services ... \n\n"
	foreach service ($PTCNMS_SERVICE_LIST)
		$nmsdir/nmsq -quiet -on $service
		if ($status == 0) then
			printf "%15s - SUCCEEDED\n" $service
		else
			printf "%15s - FAILED\n" $service
		endif
	end
	echo " "

	goto success

endif

nmsdstart:

echo Starting Name Service Daemon ...
$nmsdir/nmsd -timeout 500 -dcad
sleep 1

success:

echo DCAD setup succeeded. The following remote services are enabled:
$nmsdir/nmsq -quiet -servinfo
