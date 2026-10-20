// 05-Jan-98 $$1  JJE  Created.
// 06-Jan-98 $$2  JJE  fixes
// 07-Jan-98 $$3  JJE  Change to cshell
// 08-Jan-98 $$4  JJE  Add Olly's cshell version
// 16-Jan-98 $$5  JSP  Update
// 21-Jan-98 $$6  TWH  New cshell version from Olly
// 22-Jan-98 $$7  JSP  Tweaks
// 19-Feb-98 $$8  JJE  Set nlo cli dir
// 02-Sep-98 $$9  JJE  Take Olly's changes
// 12-Nov-98      Olly fix of 80-character ps output limitation 
// 16-Mar-99      Olly PATH fix for jms
// 22-Apr-99      TWH  Set ORA_NLS32 to oracle734 per echin
// 12-May-99 $$10 TWH  Update from I-01 src
// 29-Oct-99 $$11 TWH  Add ora_shlib.inc
// 14-Dec-00 $$12 TWH  Set ORA_NLS32 to ORACLE816
// 23-Jan-01 $$13 TWH   Sun4_solaris_64 changes
// 08-Mar-01      TWH  remove license references
// 08-Mar-01 $$14 TWH  Change to ora8_shlib; Add itk_shlib
// 09-Mar-26 $$15 jwong Remove dvs, dcs related functions
// 23-Apr-01      TWH  Hpux adjustments
// 23-Apr-01 $$15 TWH  ORA_NLS32 -> ORA_NLS33
// 10-May-01 $$16 TWH  Add ORACLE_HOME
// 16-May-01 $$17 TWH  Hpux_pa64
// 12-Nov-01 $$18 TWH  Set ORA_NLS33 to ORACLE817
// 19-Jul-04 K-03-06  $$19 TWH Chg to ORACLE920
// 22-Aug-07 L-01-38  $$20 TWH Chg path to ORACLE9208
// 17-Jan-08 L-01-42  $$21 TWH Chg path to ORACLE10203
#include:head.inc

#include:shlib.inc
#include:ora8_shlib.inc
set tkdir=$prodir
#include:itk_shlib.inc

# ptc-cmdtype: iasadmin

setenv NLO_CLIENT_DIR $prodir/etc
setenv TNS_ADMIN $prodir
setenv PDM_ORACLE_SERVER intralink
setenv PDM_ORA_APPL intralink
setenv ORACLE_HOME $prodir/$mc/obj/ORACLE10203
setenv ORA_NLS33 $prodir/$mc/obj/ORACLE10203/ocommon/nls/admin/data

//#includeenv:LM_LICENSE_FILE:lmlic.env
//setenv ILINK_WEBCLIENT_FEATURE_NAME "__ILINK_WEBCLIENT_FEATURE_NAME__"

unsetenv CLASSPATH
setenv PATH "$prodir/$mc/jre/bin:$prodir/$mc/jms:$PATH"

set ILSERVER="iaserver"
set HOST_NAME=`hostname`
set CFG_FILE_NAME="${ILSERVER}.conf"
set LOG_FILE_NAME="${ILSERVER}.log"

set WRONG_CHOICE="Illegal choice..."

### define configuration file entries names
#
set SRV_HOST='ServerHost'
set SRV_PORT='ServerPort'
set LOG_FILE='LogFile'
set LOG_LEVEL='LogLevel'
set IDLE='IdleTimeout'
set WUPI='WakeupInterval'
set FROOT='FileRoot'
set FAN='EnableFileAccess'
set HLP_BROWSER='Browser'
#
###########################################

### check configuration file
#
set CONF_FILE="${prodir}/${HOST_NAME}/${CFG_FILE_NAME}"

if ( ! -r $CONF_FILE ) then
   echo "Can not find configuration file: $CONF_FILE"
   exit 1
else
   set BROWSER=`grep "Browser" $CONF_FILE | awk -F: '{printf $2}'`
   if ( $BROWSER =~ ?* ) then
      set USE_BROWSER="true"
   else
      set USE_BROWSER="false"
   endif
endif
#
### conf. file checked

### check command line arguments
# (currently, -f, -ae and -c config-file name are supported by IAS itself,
# IAS_START/IAS_STOP and -ae -- by this script)
#
set IAS_START="false"
set IAS_STOP="false"
set IAS_AE_MODE="false"
if ( ${#argv} > 0 ) then
   set ias_start=`echo $* | grep IAS_START`
   if ( ${#ias_start} > 0 ) then
      set IAS_START="true"
   endif
   set ias_stop=`echo $* | grep IAS_STOP`
   if ( ${#ias_stop} > 0 ) then
      set IAS_STOP="true"
   endif
   set ae_mode=`echo $* | grep "ae"`
   if ( ${#ae_mode} > 0 ) then
      set IAS_AE_MODE="true"
   endif
endif
#
### arguments checked

### start/stop server if it's told so by command line
#
if ( $IAS_START != "false" || $IAS_STOP != "false" ) then

   if ( $IAS_START == "true" ) then
      ### save log file with an incremented name
      set log_file=`grep -h $LOG_FILE $CONF_FILE | awk -F: '{print $2}'`

      if ( ${#log_file} == 0 ) then
         set $log_file=$LOG_FILE_NAME
      endif
      if ( -e $log_file ) then
         set n=`ls -l $log_file:h | grep $log_file:t | awk -F. '{ print $NF }' | sort -n | tail -1`
         set n=`echo $n | awk '{ print($1 + 1) }'`
         mv $log_file ${log_file}.$n
      endif
      ### log file saved

      ${prodir}/${MC}/obj/$ILSERVER -c $CONF_FILE
      exit 0
   endif

   if ( $IAS_STOP == "true" ) then
      if( $mc =~ sun4_solaris* ) then
            set servers=`/usr/bin/ps -e | grep iaserver | grep -v grep | awk '{print $1}'`
            set servers=`/usr/proc/bin/pwdx $servers | grep $prodir | awk -F: '{print $1}'`
      else if ( $mc =~ hpux* ) then
         setenv UNIX95 true
         set servers=`/usr/bin/ps -aef -o 'pid ppid args' | grep -v grep | grep ias | awk '{print $1 " " $2 " " $3}' | grep $prodir | awk '{print $1}'`
         unsetenv UNIX95
      else
         set servers=`/usr/bin/ps -aef -o 'pid ppid args' | grep -v grep | grep iaserver | awk '{print $1 " " $2 " " $3}' | grep $prodir | awk '{print $1}'`
      endif

if( ${?servers} ) then
   if( ${#servers} > 0 ) then
      kill -9 $servers
      echo "Server stopped"
   else
      echo "No server instances found"
   endif
else
   echo "No server instances found"
endif
      exit 0
   endif

endif
#
### end of non-interactive start/stop section

while(1)
   set show_prompt="true"
   clear
   ### draw interactive menu ###############
   #
   echo '-IAS Server Administration Utility-'
   if ( $IAS_AE_MODE == "true" ) then
      echo "____________ AE mode ______________"
   else
      echo "___________________________________"
   endif
   echo ""
   echo '  0. Exit'
   echo '  1. Start server'
   echo '  2. Stop server'
   echo '  3. Check server status'
   echo '  4. View configuration settings'
   echo '  5. Edit configuration settings'
   echo '  6. Configuration help'
#   echo '  ..............................'
#   echo '  7. Start viewing server'
#   echo '  8. Stop viewing server'
#   echo '  9. Check viewing server status'
   echo "___________________________________"
   echo "Choose menu item and press <Enter>"
   #########################################

      set in=$<
      switch ( $in )

         case 1:
            ### start server
            ### save log file with an incremented name
            set log_file=`grep -h $LOG_FILE $CONF_FILE | awk -F: '{print $2}'`
            if ( ${#log_file} == 0 ) then
               set $log_file=$LOG_FILE_NAME
            endif
            if ( -e $log_file ) then
               set n=`ls -l $log_file:h | grep $log_file:t | awk -F. '{ print $NF }' | sort -n | tail -1`
               set n=`echo $n | awk '{ print($1 + 1) }'`
               mv $log_file ${log_file}.$n
            endif
            ### log file saved

            if ( $IAS_AE_MODE == "true" ) then
               ${prodir}/${MC}/obj/$ILSERVER -c $CONF_FILE -ae
            else
               ${prodir}/${MC}/obj/$ILSERVER -c $CONF_FILE
            endif
            breaksw

         case 2:
######################## stop server
            echo ""

      if( $mc =~ sun4_solaris* ) then
            set servers=`/usr/bin/ps -e | grep iaserver | grep -v grep | awk '{print $1}'`
            set servers=`/usr/proc/bin/pwdx $servers | grep $prodir | awk -F: '{print $1}'`
      else if ( $mc =~ hpux* ) then
         setenv UNIX95 true
         set servers=`/usr/bin/ps -aef -o 'pid ppid args' | grep -v grep | grep ias | awk '{print $1 " " $2 " " $3}' | grep $prodir | awk '{print $1}'`
         unsetenv UNIX95
      else
         set servers=`/usr/bin/ps -aef -o 'pid ppid args' | grep -v grep | grep iaserver | awk '{print $1 " " $2 " " $3}' | grep $prodir | awk '{print $1}'`
      endif

if( ${?servers} ) then
   if( ${#servers} > 0 ) then
      kill -9 $servers
      echo "Server stopped"
   else
      echo "No server instances found"
   endif
else
   echo "No server instances found"
endif
            echo ""
            breaksw
         case 3:
######################### check status
            echo ""
      if( $mc =~ sun4_solaris* ) then
            set servers=`/usr/bin/ps -e | grep iaserver | grep -v grep | awk '{print $1}'`
            set servers=`/usr/proc/bin/pwdx $servers | grep $prodir | awk -F: '{print $1}'`
      else if ( $mc =~ hpux* ) then
         setenv UNIX95 true
         set servers=`/usr/bin/ps -aef -o 'pid ppid args' | grep -v grep | grep ias | awk '{print $1 " " $2 " " $3}' | grep $prodir | awk '{print $1}'`
         unsetenv UNIX95
      else
         set servers=`/usr/bin/ps -aef -o 'pid ppid args' | grep -v grep | grep iaserver | awk '{print $1 " " $2 " " $3}' | grep $prodir | awk '{print $1}'`
      endif

if( ${?servers} ) then
   if( ${#servers} > 0 ) then
      echo "Server running with PID: $servers"
   else
      echo "No server instances found"
   endif
else
   echo "No server instances found"
endif
            echo ""
            breaksw

         case 4:
            clear
            echo "______ Current IAS settings ______"
            echo ""
            cat $CONF_FILE | tr "\011" ' ' | tr -d ' ' | awk -F: '{ printf("%s : %s\n", $1, $2) }'
            echo "__________________________________"
            echo ""
            breaksw

         case 5:
### edit configuration items ##############
#
while (1)
   clear
   echo "IAS configuration edit"
### display configuration items ###########
#
   echo "__________________________________"
   echo ""
   echo '  0. Return to main menu'
   echo "  1. $SRV_HOST"
   echo "  2. $SRV_PORT"
   echo "  3. $LOG_FILE"
   echo "  4. $LOG_LEVEL"
   echo "  5. $IDLE"
   echo "  6. $WUPI"
   echo "  7. $FROOT"
   echo "  8. $FAN"
   echo "  9. $HLP_BROWSER"
   echo "__________________________________"
   echo "Choose menu item and press <Enter>"
#
############################################

   unset ITEM_LINE
   unset ITEM_2_EDIT
   set ed_in=$<

   switch ( $ed_in )
      case 0:
         set show_prompt="false"
         break
         breaksw
      case 1:
         set ITEM_2_EDIT=$SRV_HOST
         breaksw
      case 2:
         set ITEM_2_EDIT=$SRV_PORT
         breaksw
      case 3:
         set ITEM_2_EDIT=$LOG_FILE
         breaksw
      case 4:
         set ITEM_2_EDIT=$LOG_LEVEL
         breaksw
      case 5:
         set ITEM_2_EDIT=$IDLE
         breaksw
      case 6:
         set ITEM_2_EDIT=$WUPI
         breaksw
      case 7:
         set ITEM_2_EDIT=$FROOT
         breaksw
      case 8:
         set ITEM_2_EDIT=$FAN
         breaksw
      case 9:
         set ITEM_2_EDIT=$HLP_BROWSER
         breaksw
      default:
         unset ITEM_2_EDIT
         echo ""
         echo $WRONG_CHOICE
         breaksw
   endsw

   if ( ${?ITEM_2_EDIT} ) then
      set CONF_ITEM_VALUE=`grep -h $ITEM_2_EDIT $CONF_FILE | awk -F: '{print $2}'`
      set ITEM_LINE=`grep -n $ITEM_2_EDIT $CONF_FILE | awk -F: '{print $1}'`
      echo ""

      if ( ! ${#ITEM_LINE} ) then
         echo "$ITEM_2_EDIT currently unset. Enter its value now :"
      else
         echo "Current value : $CONF_ITEM_VALUE"
         echo 'Enter new value : '
      endif

      set new_value=$<

      if ( $new_value =~ ?* ) then
         echo "New value: $new_value"
         set new_line="${ITEM_2_EDIT}:  $new_value"

         if ( ! ${#ITEM_LINE} ) then ################## this is a fresh value
            cat $CONF_FILE >${CONF_FILE}.$$
            echo "$new_line" >>${CONF_FILE}.$$
         else #################### edit an old value
            sed "${ITEM_LINE}c\\
$new_line" $CONF_FILE >${CONF_FILE}.$$
         endif

         mv $CONF_FILE ${CONF_FILE}.bak
         mv ${CONF_FILE}.$$ $CONF_FILE
         echo "Restart server for the changes to take an effect..."
      else
         echo "Ignored..."
      endif
   endif
   echo ""
   echo "Press <Enter> to continue..."
   set response=$<
   clear
end
#
### end of edit
            breaksw

         case 6:
            ### display configuration items help topics
            # set HLP_FILE=${prodir}/docs/`echo ${0} | awk -F\/ '{print $NF}'`.html
            # we will not provide html-parsed item-by-item help anymore
            set HLP_FILE=${prodir}/docs/index.htm
            unset HLP_START_LINE
            unset HLP_STOP_LINE
            unset HELP_ITEM_VALUE

   set $USE_BROWSER="true"   # we will not provide html-parsed item-by-item help anymore

   if ( "$USE_BROWSER" == "true" ) then
      if( -r $HLP_FILE ) then
         echo "Starting browser..."
         $BROWSER ${HLP_FILE}&
         set show_prompt="false"
         clear
      else
         set show_prompt="true"
         echo "Can not find help file $HLP_FILE"
      endif
   else

while (1)
   clear
   echo "IAS configuration help"
### display configuration items ###########
#
   echo "__________________________________"
   echo ""
   echo '  0. Return to main menu'
   echo "  1. $SRV_HOST"
   echo "  2. $SRV_PORT"
   echo "  3. $LOG_FILE"
   echo "  4. $LOG_LEVEL"
   echo "  5. $IDLE"
   echo "  6. $FROOT"
   echo "  7. $FAN"
   echo "  8. $HLP_BROWSER"
   echo "__________________________________"
   echo "Choose menu item and press <Enter>"
#
############################################
   set hlp_in=$<
   echo ""
   switch ( $hlp_in )
      case 0:
         set show_prompt="false"
         break
         breaksw
      case 1:
         set from=START_ITEM."$SRV_HOST"
         set to=END_ITEM."$SRV_HOST"
         breaksw
      case 2:
         set from=START_ITEM."$SRV_PORT"
         set to=END_ITEM."$SRV_PORT"
         breaksw
      case 3:
         set from=START_ITEM."$LOG_FILE"
         set to=END_ITEM."$LOG_FILE"
         breaksw
      case 4:
         set from=START_ITEM."$LOG_LEVEL"
         set to=END_ITEM."$LOG_LEVEL"
         breaksw
      case 5:
         set from=START_ITEM."$IDLE"
         set to=END_ITEM."$IDLE"
         breaksw
      case 6:
         set from=START_ITEM."$FROOT"
         set to=END_ITEM."$FROOT"
         breaksw
      case 7:
         set from=START_ITEM."$FAN"
         set to=END_ITEM."$FAN"
         breaksw
      case 8:
         set from=START_ITEM."$HLP_BROWSER"
         set to=END_ITEM."$HLP_BROWSER"
         breaksw
      default:
         echo $WRONG_CHOICE
         breaksw
   endsw

   if ( $hlp_in <= 9 && $hlp_in >= 1 ) then
      set HLP_START_LINE=`grep -n $from $HLP_FILE | awk -F: '{print $1}'`
      set HLP_STOP_LINE=`grep -n $to $HLP_FILE | awk -F: '{print $1}'`
      set HELP_ITEM_VALUE=`sed -n "$HLP_START_LINE,${HLP_STOP_LINE}p" $HLP_FILE | grep -v _ITEM` breaksw
      sed -n "$HLP_START_LINE,${HLP_STOP_LINE}p" $HLP_FILE | grep -v _ITEM | sed 's/<B>/***/g' | sed 's/<\/B>/***/g' | more
      echo ""
      echo "Press <Enter> to continue..."
      set response=$<
   endif
end
endif
#
### end of help
            breaksw

         case 0:
            break
            breaksw

#         case 7:
#            set JMS_LOG_FILE=ViewServerLog
#            set jms_log_file=`grep -h $JMS_LOG_FILE $CONF_FILE | awk -F: '{print $2}'`
#            if ( ${#jms_log_file} > 0 ) then
#               set jms_log_file="-logfile $jms_log_file"
#            else
#               set jms_log_file=""
#            endif
#
#            set JMS_PORT=ViewServerPort
#            set jms_port=`grep -h $JMS_PORT $CONF_FILE | awk -F: '{print $2}'`
#            if ( ${#jms_port} > 0 ) then
#               set jms_port="-clientport $jms_port"
#            else
#               set jms_port=""
#            endif
#
#            set JMS_PING_PORT=ViewServerPingPort
#            set jms_ping_port=`grep -h $JMS_PING_PORT $CONF_FILE | awk -F: '{print $2}'`
#            if ( ${#jms_ping_port} > 0 ) then
#               set jms_ping_port="-pingport $jms_ping_port"
#            else
#               set jms_ping_port=""
#            endif
#
#            echo ""
#            echo "Viewing server starting..."
#            set curr_dir=`pwd` 
#            cd $prodir/$mc/jms
#            ulimit -n 1024
#            jre -mx64m -cp jmyriad.jar com.infograph.jmyriad.server.MyriadServer $jms_log_file $jms_port $jms_ping_port&
#            cd $curr_dir
#            breaksw
#
#         case 8:
#            set JMS_PING_PORT="ViewServerPingPort"
#            set jms_ping_port=`grep -h $JMS_PING_PORT $CONF_FILE | awk -F: '{print $2}'`
#            echo ""
#            set jms_status=`jre -cp $prodir/bin:$prodir/mc/jre/lib JMSPing $jms_ping_port | head -1 | grep PING`
#            if ( ${#jms_status} > 0 ) then
#               set jms_status=`jre -cp $prodir/bin JMSStop $jms_ping_port | head -1 | grep PING`
#               if ( ${#jms_status} == 0 ) then
#                  echo "Viewing server stopped" 
#               endif
#            else
#               echo "Viewing server is down" 
#            endif
#            breaksw
#
#         case 9:
#            set JMS_PING_PORT="ViewServerPingPort"
#            set jms_ping_port=`grep -h $JMS_PING_PORT $CONF_FILE | awk -F: '{print $2}'`
#            echo ""
#            set jms_status=`jre -cp $prodir/bin JMSPing $jms_ping_port | head -1 | grep PING`
#            if ( ${#jms_status} > 0 ) then
#               echo "Viewing server is alive" 
#            else
#               echo "Viewing server is down" 
#            endif
#            breaksw

         default:
            echo ""
            echo $WRONG_CHOICE
      endsw

   if( $show_prompt == "true" ) then
      echo ""
      echo "Press <Enter> to continue..."
      set response=$<
   endif

end

clear

#
#
################## End of iasadmin ###################
