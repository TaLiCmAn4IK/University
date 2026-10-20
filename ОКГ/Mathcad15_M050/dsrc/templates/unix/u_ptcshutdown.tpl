// 09-Sep-97 $$1 TWH Created.
// 05-Jan-98 $$2 JJE Use lmutil
// 16-Jan-98 $$3 Set lic file explicitly
// 30-Apr-98 $$4 JJE Check if mips4 exists
// 07-Jul-98 $$5 JJE Check if hp8k exists
// 30-Dec-98 $$6 TWH messages added per TechSupport
// 12-Jan-98 $$7 JJE Set var instead of passing lic file
// 07-Aug-00 $$8 TWH Check if hpux11_pa32 exists
// 23-Jan-01 $$9 TWH Sun4_solaris_64 changes
// 15-Feb-01 $$10 TWH Sun fallback
// 16-May-01 $$11 TWH hpux_pa64
// 17-Oct-01 $$12 TWH Use PTC_D_LICENSE_FILE
// 14-Dec-01 $$13 JJE Change args to lmdown
// 05-Mar-03 K-01-02  ALG  $$14  SPR 1003705: Exit status
// 02-Oct-03 K-01-16  TWH  $$15  sgi_elf4 fallback
// 25-OCT-05 K-03-34  MTP  $$16  sun_solaris_x64 support
#!/bin/sh

PATH=$PATH:/bin:/usr/bin:/etc;export PATH

NLO_DIRECTORY=__LOADPOINT__; export NLO_DIRECTORY

mc=`$NLO_DIRECTORY/install/unix/getpmt`
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
  if [ ! -d $NLO_DIRECTORY/$mc ]; then
    if [ -d $NLO_DIRECTORY/hp700 ]; then
      mc="hp700"
    fi
  fi
fi
if [ $mc = sun4_solaris_64 ]; then
  if [ ! -d $NLO_DIRECTORY/$mc ]; then
    if [ -d $NLO_DIRECTORY/sun4_solaris ]; then
      mc="sun4_solaris"
    fi
  fi
fi

PTC_D_LICENSE_FILE="__LM_LICENSE_FILE__"; export PTC_D_LICENSE_FILE

exec $NLO_DIRECTORY/$mc/obj/lmutil lmdown -c $PTC_D_LICENSE_FILE

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
pid2="`$ps_cmd | egrep -v grep | grep 'obj/lmgrd' | tail -1 | awk '{print $2}'`"
echo ""
if [ "$pid" | "$pid2" ]; then
  if [ "$pid" ]; then
    echo "A PTC license daemon is still running."
    exit 1
  fi
  if [ "$pid" ]; then
    echo "An lmgrd process is still running."
    exit 1
  fi
else
  echo "Command Completed Successfully"
  exit 0
fi

