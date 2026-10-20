// 04Jun97 H-03-13  Pete    $$1 created new or from H-01-XX
// 27-Mar-97 ##1  Pete  Created.
// 12-Jan-98 $$2 TWH  'if [!' - add a space
// 23-Jan-01 $$3  TWH   Sun4_solaris_64 changes
// 25-OCT-05 K-03-34 $$4  MTP  sun_solaris_x64 support
#!/bin/sh

PATH=$PATH:/bin:/usr/bin:/etc;export PATH

NLO_DIRECTORY=__LOADPOINT__; export NLO_DIRECTORY
NLO_PORT=__NLO_PORT__; export NLO_PORT
mc=`$NLO_DIRECTORY/install/unix/getpmt`

ps_cmd="ps -ef"

case $mc in
hp8k)
   if [ ! -d $NLO_DIRECTORY/hp8k ]; then
      mc="hp700"
   fi
   ;;
esac

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

pid="`$ps_cmd | egrep -v grep | grep obj/proserver | tail -1 | awk '{print $2}'`"
if [ "$pid" ]; then
   echo "WARNING: A Pro/SERVER is already running"
   echo "         Use proshutdown to kill it then"
   echo "         restart with prostartserver"
   exit 1
fi

(trap '' 1 2 15 ; $NLO_DIRECTORY/$mc/obj/proserver $*) &
