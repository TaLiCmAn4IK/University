// 24-Jun-98     JJE Created from pdm_startup template
// 13-Jul-98     TWH Not a ptc-command type
// 27-Jul-98     TWH Add db file cp
// 28-Jul-98 $$1 JJE Submit to I-01
// 10-Aug-98 $$2 TWH Cleanup PDMDB_LP setting
// 20-Aug-98 $$3 JJE Improve messages
// 17-Sep-98 $$4 TWH Chg LIBPATH settings to match intralink
// 05-Oct-98     TWH Add PDM_CLIENT_DIR
// 19-Oct-98 $$5 TWH modify hp8k check, add sgi_mips4 chk
// 22-Apr-99 $$6 TWH Set ORA_NLS32 to oracle734 per echin
// 14-Dec-99 $$7 JJE Don't set preload on sun
// 27-Jun-00 $$8 TWH reorder nlo_directory.env/pdmbgindicator.env
//                   moved pdm33orauser call into pdmbgindiator.env
// 07-Aug-00 $$9 TWH hpux11_pa32 handling
// 23-Jan-01 $$10 TWH Sun4_solaris_64 changes
// 16-May-01 $$11 TWH hpux_pa64 changes
// 25-May-01 $$12 JJE Don't use ucb paths on sun
// 10-Aug-01      TWH #10 again 894276
// 14-Aug-01      TWH #include:pdmbgindicator.env; AUTO_PORT detection
// 21-Aug-01 $$13 TWH Add xgl_stub
// 28-Dec-01 $$14 TWH Add SHT_HOSTNAME because of alpha_unix
#!/bin/sh

PATH=$PATH:/bin:/usr/bin:/etc;export PATH

#includeenv:NLO_DIRECTORY:nlo_directory.env
#include:pdmbgindicator.env
 
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
if [ "$mc" = "sun4_solaris_64" ]; then
   if [ ! -d $NLO_DIRECTORY/sun4_solaris_64 ]; then
     mc=sun4_solaris
   fi
fi

PDMDB_FLUSH_LEVEL=2; export PDMDB_FLUSH_LEVEL
PDM_PORT=391783; export PDM_PORT

PDMSIG_START=$NLO_DIRECTORY/$mc/obj/pdmsignal; export PDMSIG_START
PRO_COMM_MSG_EXE=$NLO_DIRECTORY/$mc/obj/pro_comm_msg; export PRO_COMM_MSG_EXE
PDM_SERVER_LOG_MAX_LINES=5000; export PDM_SERVER_LOG_MAX_LINES

# copy in pdmdblist, pdmpath, pdmservers files
PDMDB_LP=__PDMDB_LP__;
if [ ! -d $PDMDB_LP ]; then
  echo "Cannot find the pdm database reference files:"
  echo "Directory $PDMDB_LP does not exist."
  exit 1
fi
if [ ! -d $NLO_DIRECTORY/text ]; then
  mkdir $NLO_DIRECTORY/text
fi
if [ -f $PDMDB_LP/text/pdmdblist.txt ]; then
  cp $PDMDB_LP/text/pdmdblist.txt $NLO_DIRECTORY/text/pdmdblist.txt
  chmod 666 $NLO_DIRECTORY/text/pdmdblist.txt
else
  echo "Cannot find the pdm database reference file:"
  echo "$PDMDB_LP/text/pdmdblist.txt"
  exit 1
fi
if [ -f $PDMDB_LP/text/pdmpath.txt ]; then
  cp $PDMDB_LP/text/pdmpath.txt $NLO_DIRECTORY/text/pdmpath.txt
  chmod 666 $NLO_DIRECTORY/text/pdmpath.txt
else
  echo "Cannot find the pdm database reference file:"
  echo "$PDMDB_LP/text/pdmpath.txt"
  exit 1
fi
PDM_HOSTNAME=`hostname`; export PDM_HOSTNAME

if [ "$mc" = "alpha_unix" ]; then
SHT_HOSTNAME=`hostname -s`;
else
SHT_HOSTNAME=`hostname`;
fi

if [ ! -d $NLO_DIRECTORY/etc/$PDM_HOSTNAME ]; then
  mkdir -p $NLO_DIRECTORY/etc/$PDM_HOSTNAME
fi
if [ -f $PDMDB_LP/etc/$PDM_HOSTNAME/pdmservers.txt ]; then
  cp $PDMDB_LP/etc/$PDM_HOSTNAME/pdmservers.txt $NLO_DIRECTORY/etc/$PDM_HOSTNAME/pdmservers.txt
  chmod 666 $NLO_DIRECTORY/etc/$PDM_HOSTNAME/pdmservers.txt
  PDM_CLIENT_DIR=$NLO_DIRECTORY/etc/$PDM_HOSTNAME; export PDM_CLIENT_DIR
else
  echo "Cannot find the pdm database reference file:"
  echo "$PDMDB_LP/etc/$PDM_HOSTNAME/pdmservers.txt"
  exit 1
fi
AUTO_PDM_PORT=`cat $PDM_CLIENT_DIR/pdmservers.txt |grep $SHT_HOSTNAME | cut -f2 -d' '`; export AUTO_PDM_PORT
SAVE_LOCALE=$LANG
CMP_PDM_PORT=`echo $AUTO_PDM_PORT | tr -d '[:alpha:]'`; export CMP_PDM_PORT
LANG=$SAVE_LOCALE
if [ "$AUTO_PDM_PORT" = "$CMP_PDM_PORT" ]; then
  if [ ! "$CMP_PDM_PORT" = "" ]; then
    PDM_PORT=$AUTO_PDM_PORT; export PDM_PORT
  fi
fi

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

case $mc in
sun4_solaris*)
whoami_cmd="/usr/ucb/whoami"
;;
esac

# EXIT_SUCCESS EXIT_WARNING
pdm_exits="0 1"
# SIGKILL(9) SIGTERM(15) SIGINT(2) SIGSTOP(24) SIGABRT(6)
sig_exits="137 143 130 151 134"
dbg=0

case $mc in
sgi*)
   ps_cmd="/bin/ps -ef"
   if [ "$LD_LIBRARY_PATH" = "" ]; then
      LD_LIBRARY_PATH=$PDM_SERVER_DIR/$mc/lib:$PDM_SERVER_DIR/$mc/obj:$PDM_SERVER_DIR/$mc/obj/ORACLE734/lib; export LD_LIBRARY_PATH
   else
      LD_LIBRARY_PATH=${LD_LIBRARY_PATH}:$PDM_SERVER_DIR/$mc/lib:$PDM_SERVER_DIR/$mc/obj:$PDM_SERVER_DIR/$mc/obj/ORACLE734/lib; export LD_LIBRARY_PATH
   fi
   ;;

alpha_unix)
   ps_cmd="ps auxww"
   if [ "$LD_LIBRARY_PATH" = "" ]; then
      LD_LIBRARY_PATH=$PDM_SERVER_DIR/$mc/obj:$PDM_SERVER_DIR/$mc/obj/ORACLE734/lib; export LD_LIBRARY_PATH
   else
      LD_LIBRARY_PATH=${LD_LIBRARY_PATH}:$PDM_SERVER_DIR/$mc/obj:$PDM_SERVER_DIR/$mc/obj/ORACLE734/lib; export LD_LIBRARY_PATH
   fi
   ;;

sun4_solaris*)
   ps_cmd="/bin/ps -ef"
   if [ "$LD_LIBRARY_PATH" = "" ]; then
      LD_LIBRARY_PATH=/usr/openwin/lib:$PDM_SERVER_DIR/$mc/obj:/usr/dt/lib:$PDM_SERVER_DIR/$mc/lib:$PDM_SERVER_DIR/$mc/obj/ORACLE734/lib; export LD_LIBRARY_PATH
   else
      LD_LIBRARY_PATH=${LD_LIBRARY_PATH}:/usr/openwin/lib:$PDM_SERVER_DIR/$mc/obj:/usr/dt/lib:$PDM_SERVER_DIR/$mc/lib:$PDM_SERVER_DIR/$mc/obj/ORACLE734/lib; export LD_LIBRARY_PATH
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
      SHLIB_PATH=/usr/lib/X11R5:/usr/lib:$PDM_SERVER_DIR/$mc/lib:$PDM_SERVER_DIR/$mc/obj:$PDM_SERVER_DIR/$mc/obj/ORACLE734/lib:/opt/graphics/common/lib; export SHLIB_PATH
   else
      SHLIB_PATH=${SHLIB_PATH}:/usr/lib/X11R5:/usr/lib:$PDM_SERVER_DIR/$mc/lib:$PDM_SERVER_DIR/$mc/obj:$PDM_SERVER_DIR/$mc/obj/ORACLE734/lib:/opt/graphics/common/lib; export SHLIB_PATH
   fi
   ;;

ibm_rs6000)
   if [ `uname -v` = 3 ] ; then
      ulimit -b 1000000
   else
      ulimit -d unlimited
   fi
   if [ "$LIBPATH" = "" ]; then
      LIBPATH=/usr/lib:/lib:$PDM_SERVER_DIR/$mc/obj:$PDM_SERVER_DIR/$mc/lib:$PDM_SERVER_DIR/$mc/obj/ORACLE734/lib; export LIBPATH
   else
      LIBPATH=${LIBPATH}:/usr/lib:/lib:$PDM_SERVER_DIR/$mc/obj:$PDM_SERVER_DIR/$mc/lib:$PDM_SERVER_DIR/$mc/obj/ORACLE734/lib; export LIBPATH
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

initdelaysecs=900
loopsecs=5
while [ 1 ]
do
   dhms=`date '+%d %H %M %S'`
   set $dhms
   startsecs=`echo "$1*24*60*60 + $2*60*60 + $3*60 + $4" | bc`

   $PDM_SERVER_DIR/$mc/obj/pdmserver -uc -f $pdmargs
   saved_status=$?
   if [ $dbg = 1 ]; then
      echo DEBUG exited - status $saved_status
   fi

   if [ $norestart = 1 ]; then
      if [ $dbg = 1 ]; then
         echo DEBUG norestart indicated
      fi
      break
   fi

   dhms=`date '+%d %H %M %S'`
   set $dhms
   currsecs=`echo "$1*24*60*60 + $2*60*60 + $3*60 + $4" | bc`
   deltasecs=`echo "$currsecs - $startsecs" | bc`

   if [ $deltasecs -lt $initdelaysecs ]; then
      if [ $dbg = 1 ]; then
         echo DEBUG within init time - $initdelaysecs secs
      fi

      pdmmsg="Pro/PDM Server inactive (no restart within initialization time) on `hostname` on `date`"
      echo "$pdmmsg" >> $PDM_SERVER_DIR/text/pdmlog.txt
      if [ "$mailaddr" != "" ]; then
         echo "$pdmmsg" | mail $mailaddr
      fi
      break
   else
      exitnow=`echo " $pdmstop_exits " | grep " $saved_status " | wc -l | tr -d ' '`
      if [ $exitnow = 1 ]; then
         if [ $dbg = 1 ]; then
            echo DEBUG stop value found
         fi
         break
      fi
   fi

   log=$NLO_DIRECTORY/text/server_log
   if [ -w $log ]; then
      mv $log.4 $log.5 1> /dev/null 2>&1
      mv $log.3 $log.4 1> /dev/null 2>&1
      mv $log.2 $log.3 1> /dev/null 2>&1
      mv $log.1 $log.2 1> /dev/null 2>&1
      mv $log $log.1   1> /dev/null 2>&1
   fi

   pdmmsg="Restarting the Pro/PDM Server on `hostname` on `date`"
   echo "$pdmmsg" >> $PDM_SERVER_DIR/text/pdmlog.txt
   if [ "$mailaddr" != "" ]; then
      echo "$pdmmsg" | mail $mailaddr
   fi

   sleep $loopsecs
done

exit 0
