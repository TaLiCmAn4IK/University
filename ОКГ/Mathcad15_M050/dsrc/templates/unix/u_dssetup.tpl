// 23-Jul-03 K-01-11   MAZ  $$1   SPR 1025041: copy from dcadsetup
// 01-Oct-03 K-01-16   MAZ  $$2   Sub __LOADPOINT__ and __ARCH__
// 19-Jan-04 K-01-22   MAZ  $$3   SPR 1069176: env vars for ds_protoolkit_svc
#!/bin/csh -f

#
# User Input Area: Please customize the following variables 
#                  according to your setup. 
#
# PTCNMS_SERVICE_DIR is the path to the DS services directory. 
#    Only one directory is allowed in this variable. All service 
#    executables need to be located in this directory.
#
# PTCNMS_SERVICE_LIST is a space delimited list of DS service names. 
#    Surround the list with double quotes if it contains more than one item.
#
#
# The following environment variables are required for the
# DS sample Pro/TOOLKIT service (ds_protoolkit_svc). If they
# are not set, the sample Pro/TOOLKIT service will not be available.
# Uncomment and modify the following lines to use ds_protoolkit_svc
#
# if (! $?PROE_START) then
#    setenv PROE_START <PROE_LOADPOINT>/bin/<PROE_STARTUP_SCRIPT>
# endif
# if (! $?PRO_COMM_MSG_EXE) then
#    setenv PRO_COMM_MSG_EXE <PROE_LOADPOINT>\__ARCH__\obj\pro_comm_msg
# endif
#

# You can set these variables outside of the script using your shell command
if (! $?PTCNMS_SERVICE_DIR) then
    setenv PTCNMS_SERVICE_DIR __LOADPOINT__/ds_api/c_api/__ARCH__/obj 
endif

if (! $?PTCNMS_SERVICE_LIST) then
    setenv PTCNMS_SERVICE_LIST "ds_simple_svc ds_protoolkit_svc"  
endif

#
#
# WARNING: please do not edit the script content below. 
#
#

#include:head.inc

set nmsdir=$prodir/$mc/nms

if ("$1" == "-status") then
	$nmsdir/nmsq -quiet -ping

	if ($status != 0) then
		echo Could not connect to Name Service Daemon.
		exit -1
	else
		$nmsdir/nmsq -quiet -hostinfo

		echo The following remote services are registered:
		$nmsdir/nmsq -quiet -servinfo

		exit 0
	endif
endif

$nmsdir/nmsq -quiet -env PTCNMS_SERVICE_DIR=$PTCNMS_SERVICE_DIR

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

	echo DS setup failed. Incompatible Name Service Daemon is running.
	echo 1. Exit all PTC applications.
	echo 2. Run $fullscrname -force

	exit -1

else

	printf "Setting up DS services ... \n\n"
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

echo DS setup succeeded. The following remote services are enabled:
$nmsdir/nmsq -quiet -servinfo

