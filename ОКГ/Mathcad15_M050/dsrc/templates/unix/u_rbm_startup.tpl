// 02-Feb-98 $$1 JJE  Created.
// 14-Oct-98 $$2 TWH  Rewrite to csh
// 23-Jan-01 $$3 TWH  Sun4_solaris_64 changes
// 25-OCT-05 K-03-34 $$4  MTP  sun_solaris_x64 support
// 01-Nov-05 K-03-34+ $$5 MAZ  remove empty line before include line
#include:head.inc

#include:shlib.inc

#include:shlibproe.inc

setenv PRORB_RUNDIR __PRORB_RUNDIR__
setenv PRORB_STATFILE __PRORB_STATFILE__
setenv PRORB_PROE_START $prodir/bin/__PRORB_PROE_START__
setenv PRORB_NUM_BATCHPROES __PRORB_NUM_BATCHPROES__
setenv PRORB_HK_TIMEOUT __PRORB_HK_TIMEOUT__
setenv PRORB_RPCNUM __PRORB_RPCNUM__

setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg

# ps_cmd varies per $mc
set ps_cmd="ps -ef"
if ($mc =~ sun*) then
   set ps_cmd="/bin/ps -ef"
endif
if ($mc == alpha_unix ) then
   set ps_cmd="ps auxww"
endif 

set curruser=`whoami | awk '{print $1}'`

# see if proremd is running
set raw=`$ps_cmd | grep -v grep | grep proremd | tail -1`
set remdpid=`echo "$raw" | awk '{print $2}'`
if ("$remdpid" != "" ) then
    echo ""
    echo ""
    echo "ERROR:  You have a proremd process running.  This is likely"
    echo "due to Pro/PDM SERVER running on this machine.  Currently the"
    echo "Pro/PDM SERVER and the Remote Toolpath Computation Server"
    echo "can not be run on the same machine."
    echo ""
    echo ""
    exit 1
endif

# Start prorembd if not already running
set raw="`$ps_cmd | grep -v grep | grep prorembd | tail -1`"
set remdpid=`echo "$raw" | awk '{print $2}'`
set remduser=`echo "$raw" | awk '{print $1}'`
set uidlen=`echo "$curruser" | awk '{print length($1)}'`
if ( "$remduser" == "" ) then
   $prodir/$mc/obj/prorembd >& /dev/null &
else if ( "$remduser" != "$curruser" ) then
   set firsteight=`echo "$curruser" | awk '{print substr($1,1,8)}'`
   if (( $uidlen > 8 ) && ( "$remduser" == "$firsteight" )) then
      echo ""
      echo "WARNING: Your username exceeds eight characters. Unable to determine if"
      echo "         existing process 'prorembd' is yours. First eigth characters matched"
      echo "         your username, continuing ..."
   else
      echo ""
      echo "ERROR:  a 'prorembd' process (PID=$remdpid) owned by user '$remduser' exists."
      echo "        Stop it and retry as '$curruser'."
      exit 1
   endif
endif


set pid=`$ps_cmd | egrep -v grep | grep obj/prorembatch | tail -1 | awk '{print $2}'`
if ( "$pid" ) then
   echo "WARNING: A Remote Toolpath Computation Server is already running"
   exit 1
endif

$prodir/$mc/obj/prorembatch $* &
