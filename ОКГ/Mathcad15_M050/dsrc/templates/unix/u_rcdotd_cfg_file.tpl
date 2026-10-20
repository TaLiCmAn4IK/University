// 23-Sep-99  $$1  MAZ  Created
#!__RC_SHELL__

# using HP template file: /sbin/init.d/template
# Allowed exit values:
#   0 = success; causes "OK" to show up in checklist.
#   1 = failure; causes "FAIL" to show up in checklist.
#   2 = skip; causes "N/A" to show up in the checklist.
#           Use this value if execution of this script is overridden
#       by the use of a control variable, or if this script is not
#       appropriate to execute for some other reason.
#       3 = reboot; causes the system to be rebooted after execution.

PATH=/usr/sbin:/usr/bin:/sbin
export PATH
rval=0

case $1 in
'start_msg')
	__RC_START_MESSAGE__
	;;

'stop_msg')
	__RC_STOP_MESSAGE__
	;;

'start')
	# source the system configuration variables
	if [ -f /etc/rc.config ] ; then
		. /etc/rc.config
	else
		echo "ERROR: /etc/rc.config defaults file MISSING"
	fi

	# Check to see if this script is allowed to run...
	if [ "$__RC_CMD_NAME__" != 1 ]; then
		rval=2
	else
		# Execute the commands to start your subsystem
		__RC_START_CMD_PATH__		
	fi
	;;

'stop')
	# source the system configuration variables
	if [ -f /etc/rc.config ] ; then
		. /etc/rc.config
	else
		echo "ERROR: /etc/rc.config defaults file MISSING"
	fi

    # Check to see if this script is allowed to run...
    if [ "$__RC_CMD_NAME__" != 1 ]; then
        rval=2
    else
        # Execute the commands to start your subsystem
        __RC_STOP_CMD_PATH__       
    fi
    ;;

*)

	echo "usage: $0 {start|stop|start_msg|stop_msg}"
	rval=1
	;;
esac
exit $rval
