// 10-Jun-99  $$1  TWH   Created from req.doc.
#include:head.inc

########################################
##    PTC Pro/E Update Init Script    ##
########################################
##   LCD Multimedia Creations, 1998   ##
########################################

set ECHO="echo"

# Command-line help info

if ($1 == "-h")  then
	$ECHO " "
	$ECHO " PTC Pro/E Update Training 1.0 -  Application Launcher"
	$ECHO "     usage: $0 (-volume range)"
	$ECHO "     Supported options:"
	$ECHO " 	-volume N	audio volume (0-100)"
	$ECHO " "
	exit 1
endif

set arg1=$1
set arg2=$2
set arg3=$3
set arg4=$4

# Welcome message

$ECHO " "
$ECHO "PTC Pro/E Update Training"
$ECHO " configuring environment variables..."

########

set OS=`uname` 

setenv TRAINING_FEATURE_NAME "__TRAINING_FEATURE_NAME__"

setenv CONTENTDIR "__CONTENTDIR__"

#includeenv:PROE_START:proe_start.env

switch ($OS)
case IRIX:
	setenv LIBPATH $fullscrpath/irix
	$ECHO "  LIBPATH set"
	$fullscrpath/update.sgi $arg1 $arg2 
	breaksw
case AIX:
	$ECHO "  Extracting shared libraries..."
	tar -xf $fullscrpath/aix/aixlibs.tar
	setenv LD_LIBRARY_PATH /usr/openwin/lib:/usr/dt/lib:/tmp
	$ECHO "  LD_LIBRARY_PATH set"
	setenv LIBPATH /tmp
	$ECHO "  LIBPATH set"
	$fullscrpath/update.aix $arg1 $arg2
	breaksw
case HP-UX:
	$fullscrpath/update.hp $arg1 $arg2
	breaksw
case OSF1:	
	setenv LIBPATH $fullscrpath/osf1
	$ECHO "  LIBPATH set"
	$fullscrpath/update.osf $arg1 $arg2
	breaksw
default:
	setenv LD_LIBRARY_PATH /usr/openwin/lib:/usr/dt/lib:$fullscrpath/sunos
	$fullscrpath/update.sun $arg1 $arg2
endsw


cd $rundir
exit 0
