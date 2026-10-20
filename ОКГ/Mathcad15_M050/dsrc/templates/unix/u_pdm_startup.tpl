// 14-Apr-98 $$1 ZRL Created
// 19-May-98     ZRL Added LD_LIBRARY_PATH for sun4_solaris
// 10-Jun-98     JJE nmsq stuff not needed here
// 16-Jun-98 $$2 JJE Group Submission
// 19-Oct-98 $$3 TWH modify hp8k chk, add sgi_mips4 chk
// 14-Dec-99 $$4 JJE Don't set preload on sun
// 09-Feb-00 $$5 MYA Change includeenv to include for BG_INDICATOR 
// 10-Mar-00 $$6 TWH Add stublib for IBM
// 26-Jun-00 $$7 TWH reorder nlo_directory.env/pdmbgindicator.env
//                   moved pdm33orauser call into pdmbgindicator.env
// 07-Aug-00 $$8 TWH Check if hpux11_pa32 exists
// 23-Jan-01 $$9  TWH   Sun4_solaris_64 changes
// 29-Mar-01 $$10 ALG Add __PDM_SERVER_PORT__ (SPR 753378)
// 11-Apr-01      TWH Add xgl_stub check for sun4
// 16-May-01 $$11 TWH hpux_pa64
// 25-May-01 $$12 JJE  Don't use ucb paths on sun
#!/bin/sh
# ptc-cmdtype: pdmserver

PATH=$PATH:/bin:/usr/bin:/etc;export PATH

#includeenv:NLO_DIRECTORY:nlo_directory.env
#include:PDM_BG_INDICATOR:pdmbgindicator.env
 
PDM_DIRECTORY=$NLO_DIRECTORY/pdm; export PDM_DIRECTORY
PRO_DIRECTORY=$NLO_DIRECTORY; export PRO_DIRECTORY
PDM_SERVER_DIR=$NLO_DIRECTORY; export PDM_SERVER_DIR
mc=`$PDM_SERVER_DIR/install/unix/getpmt`
if [ "$mc" = "hpux_pa64" ]; then
  if [ ! -d $NLO_DIRECTORY/hpux_pa64 ]; then
    mc=hpux11_pa32
  fi
fi
if [ "$mc" = "hpux11_pa32" ]; then
  if [ ! -d $NLO_DIRECTORY/hpux11_pa32 ]; then
    mc=hp8k
  fi
fi
if [ "$mc" = "hp8k" ]; then
   if [ ! -d $NLO_DIRECTORY/hp8k ]; then
      if [ -d $NLO_DIRECTORY/hp700 ]; then
         mc=hp700
      fi
   fi
fi
if [ "$mc" = "sgi_mips4" ]; then
   if [ ! -d $NLO_DIRECTORY/sgi_mips4 ]; then
      if [ -d $NLO_DIRECTORY/sgi_elf2 ]; then
         mc=sgi_elf2
      fi
   fi
fi
PDMDB_FLUSH_LEVEL=2; export PDMDB_FLUSH_LEVEL
PDM_PORT=__PDM_SERVER_PORT__; export PDM_PORT
PDMSIG_START=$NLO_DIRECTORY/$mc/obj/pdmsignal; export PDMSIG_START
PRO_COMM_MSG_EXE=$NLO_DIRECTORY/$mc/obj/pro_comm_msg; export PRO_COMM_MSG_EXE
PDM_SERVER_LOG_MAX_LINES=5000; export PDM_SERVER_LOG_MAX_LINES

pdmargs=""
mailaddr=""
norestart=0

missingpar=0
i=1
while [ $i -le $# -a $missingpar = 0 ]
do
   currpar=`echo "$*" | awk '{print $'$i'}'`

   case $currpar in
   -onetime)
      norestart=1
      ;;

   -mail)
      i=`echo $i + 1 | bc`
      if [ $i -gt $# ]; then
         missingpar=1
      else
         mailaddr=`echo "$*" | awk '{print $'$i'}'`
      fi
      ;;

   *)
      pdmargs="$pdmargs $currpar"
      ;;
   esac

   i=`echo $i + 1 | bc`
done

if [ $missingpar -eq 1 ]; then
   echo "ERROR:  you must supply a parameter after -mail"
   exit 1
fi

ps_cmd="ps -ef"
whoami_cmd="whoami"
# EXIT_SUCCESS EXIT_WARNING
pdm_exits="0 1"
# SIGKILL(9) SIGTERM(15) SIGINT(2) SIGSTOP(24) SIGABRT(6)
sig_exits="137 143 130 151 134"
dbg=0

case $mc in
alpha_unix)
   ps_cmd="ps auxww"
   if [ "$LD_LIBRARY_PATH" = "" ]; then
      LD_LIBRARY_PATH=$PDM_SERVER_DIR/$mc/obj; export LD_LIBRARY_PATH
   else
      LD_LIBRARY_PATH=${LD_LIBRARY_PATH}:$PDM_SERVER_DIR/$mc/obj; export LD_LIBRARY_PATH
   fi
   ;;

sun4_solaris*)
   ps_cmd="/bin/ps -ef"
   if [ "$LD_LIBRARY_PATH" = "" ]; then
      LD_LIBRARY_PATH=$PDM_SERVER_DIR/$mc/obj; export LD_LIBRARY_PATH
   else
      LD_LIBRARY_PATH=${LD_LIBRARY_PATH}:$PDM_SERVER_DIR/$mc/obj; export LD_LIBRARY_PATH
   fi
   if [ ! -f "/opt/SUNWits/Graphics-sw/xgl/lib/libxgl.so" ]; then
     xgl_stub="$PRO_DIRECTORY/$mc/xglstub";
     LD_LIBRARY_PATH=${xgl_stub}:${LD_LIBRARY_PATH}
   fi

   TWO_TASK=p:; export TWO_TASK

   descr_limit=`ulimit -n`
   if [ ${descr_limit} -le 400 ]; then
	ulimit -n 400
   fi
   ;;

hp*)
   if [ "$SHLIB_PATH" = "" ]; then
      SHLIB_PATH=/usr/lib/X11R5:/usr/lib:$PDM_SERVER_DIR/$mc/obj; export SHLIB_PATH
   else
      SHLIB_PATH=${SHLIB_PATH}:$PDM_SERVER_DIR/$mc/obj; export SHLIB_PATH
   fi
   ;;

ibm_rs6000)
   if [ `uname -v` = 3 ] ; then
      ulimit -b 1000000
   else
      ulimit -d unlimited
   fi
   if [ "$LIBPATH" = "" ]; then
      LIBPATH=/usr/lib:/lib:$PDM_SERVER_DIR/$mc/obj; export LIBPATH
   else
      LIBPATH=${LIBPATH}:$PDM_SERVER_DIR/$mc/obj; export LIBPATH
   fi
   if [ ! -e "/usr/lib/libGL.a" ] ; then
      LIBPATH=${LIBPATH}:$PDM_SERVER_DIR/$mc/stublib; export LIBPATH
   fi 
   ;;
esac

case $mc in
sgi_elf2|nec_mips|alpha_unix)
ulimit -s 32768
ulimit -n 350
;;
sun4_solaris|ibm_rs6000)
ulimit -s 32768
;;
esac

pdmstop_exits="$pdm_exits $sig_exits"

pdmpid="`$ps_cmd | egrep -v grep | grep obj/pdmserver | tail -1 | awk '{print $2}'`"
if [ "$pdmpid" ]; then
   echo "ERROR:  pdmserver (PID=$pdmpid) is already running - stop it and retry"
   exit 1
fi

curruser=`$whoami_cmd | awk '{print $1}'`

# Start proremd if not already running
raw="`$ps_cmd | grep -v grep | grep proremd | tail -1`"
remdpid=`echo "$raw" | awk '{print $2}'`
remduser=`echo "$raw" | awk '{print $1}'`
uidlen=`echo "$curruser" | awk '{print length($1)}'`
if [ "$remduser" = "" ]; then
   $PDM_SERVER_DIR/$mc/obj/proremd 1> /dev/null 2>&1 &
elif [ "$remduser" != "$curruser" ]; then
   firsteight=`echo "$curruser" | awk '{print substr($1,1,8)}'`
   if [ $uidlen -gt 8 ] && [ "$remduser" = "$firsteight" ]; then
      echo ""
      echo "WARNING: Your username exceeds eight characters. Unable to determine if"
      echo "         existing process 'proremd' is yours. First eigth characters matched"
      echo "         your username, continuing ..."
   else
      echo ""
      echo "ERROR:  a 'proremd' process (PID=$remdpid) owned by user '$remduser' exists."
      echo "        Stop it and retry as '$curruser'."
      exit 1
   fi
fi

. $NLO_DIRECTORY/pdm/pdmtools/unix/convert_shenv

#includeenv:PDM_ORACLE:pdmoracle.env
#includeenv:PDM_NO_ORACLE:pdmnooracle.env
