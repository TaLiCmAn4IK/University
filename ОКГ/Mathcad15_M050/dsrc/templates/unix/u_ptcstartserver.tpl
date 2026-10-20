// 27-Mar-97 $$1  Pete  Created.
// 09-Sep-97 $$2  JJE   lmgrd
// 09-Sep-97 $$3  TWH   removed NLO_PORT
// 09-Dec-97 $$4  JJE   support alt userid
// 12-Jan-98 $$5  JJE   Don't check for LM set
// 30-Apr-98 $$6  JJE   Check if mips4 exists
// 14-May-98 $$7  JJE   Check if hp8k exists
// 09-Jul-98 $$8  TWH   Corrected script names in warning msg;
//                      Correct pid check, add startup msg;
// 30-Dec-98 $$9  TWH   Message added per TechSupport
// 20-Apr-00 $$10 TWH   Unset LM_LICENSE_FILE
// 07-Aug-00 $$11 TWH   Check if hpux11_pa32 exists
// 02-Oct-00 $$12 TWH   Add lmgrd args
// 23-Jan-01 $$13 TWH   Sun4_solaris_64 changes
// 15-Feb-01 $$14 TWH   Sun fallback
// 26-Mar-01 $$15 TWH   Backup lmgrd logfile
// 16-May-01 $$16 TWH   hpux_pa64
// 17-Oct-01 $$17 TWH   add PTC_D_LICENSE_FILE
// 28-May-02 $$18 TWH   ulimit settings for flexlm problem
// 23-Aug-02 $$19 TWH   skip #18 for 64 bit 971672
// 12-Sep-02 $$20 MAZ   SPR 976487: fix syntax
// 05-Mar-03 K-01-02  ALG  $$21  SPR 1003705: Exit status
// 02-Oct-03 K-01-16  TWH  $$22  sgi_elf4 fallback
// 25-OCT-05 K-03-34  MTP  $$23  sun_solaris_x64 support
#!/bin/sh

PATH=$PATH:/bin:/usr/bin:/etc;export PATH

NLO_DIRECTORY=__LOADPOINT__; export NLO_DIRECTORY
mc=`$NLO_DIRECTORY/install/unix/getpmt`

LM_LICENSE_FILE=; export LM_LICENSE_FILE
PTC_D_LICENSE_FILE=; export PTC_D_LICENSE_FILE
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
if [ $mc = hp8k ]; then
  if [ ! -d $NLO_DIRECTORY/hp8k ]; then
    mc="hp700"
  fi
fi
if [ $mc = sgi_elf4 ]; then
  if [ ! -d $NLO_DIRECTORY/$mc ]; then
    mc="sgi_mips4"
  fi
fi
if [ $mc = sgi_mips4 ]; then
  if [ ! -d $NLO_DIRECTORY/$mc ]; then
    mc="sgi_elf2"
  fi
fi
if [ $mc = sun4_solaris_64 ]; then
  if [ ! -d $NLO_DIRECTORY/$mc ]; then
    mc="sun4_solaris"
  fi
fi
if [ $mc != sun4_solaris_64 ]; then
if [ $mc != hpux_pa64 ]; then
  # for flexlm error: -16,287
ulimit -n 1024
ulimit -H -n 1024
  # end flexlm error: -16,287
fi
fi

if [ -f "$NLO_DIRECTORY/licensing/license.dat" ]; then
  PTCLICFILE="-c $NLO_DIRECTORY/licensing/license.dat"
fi

if [ ! -n "$PTCLICFILE" ];then
  PTCLICFILE=""
fi

LMGRDARGS="__LMGRD_ARGS__"
MYLOGFILE="$NLO_DIRECTORY/licensing/ptclmgrd"
PTCLOGFILE="-l ${MYLOGFILE}.log"

ps_cmd="ps -ef"
case $mc in
sun*)
   ps_cmd="/bin/ps -ef"
   ;;

alpha_unix)
   ps_cmd="ps auxww"
   ;;

ibm_rs6000)
   if [ `uname -v` = 3 ] ; then
      ulimit -b 1000000
   else
      ulimit -d unlimited
   fi
   ;;

esac

pid="`$ps_cmd | egrep -v grep | grep 'ptc_d -T' | tail -1 | awk '{print $2}'`"
if [ "$pid" ]; then
   echo "WARNING: A PTC license daemon is already running"
   echo "         Use ptcshutdown to stop it then"
   echo "         restart with ptcstartserver"
   exit 1
fi

if [ -f ${MYLOGFILE}.log ]
  then
  cp ${MYLOGFILE}.log ${MYLOGFILE}.bak
  if [ "$?" != "0" ]
    then
    echo "Could not backup ${MYLOGFILE}.log to ${MYLOGFILE}.bak"
    exit 1
  fi
fi
if [ $# = 1 ]
  then
    (trap '' 1 2 15 ; su $1 -c "umask 022; $NLO_DIRECTORY/$mc/obj/lmgrd $PTCLICFILE $PTCLOGFILE $LMGRDARGS") &
  else
    (trap '' 1 2 15 ; $NLO_DIRECTORY/$mc/obj/lmgrd $PTCLICFILE $PTCLOGFILE $LMGRDARGS) &
fi

sleep 5

pid="`$ps_cmd | egrep -v grep | grep 'ptc_d -T' | tail -1 | awk '{print $2}'`"
if [ "$pid" ]; then
   echo "A PTC license daemon is now running"
   echo "( Use ptcshutdown to stop it )"
   exit 0
else
   echo "The PTC license daemon may not have started up correctly."
   echo "( check the log file: ${MYLOGFILE}.log )"
   exit 1
fi

