// 16-Sep-97  $$1  JJE  Created
#!__RC_SHELL__

mode=$1

case $mode in

    'start_msg')
	__RC_START_MESSAGE__
	;;

    'stop_msg')
	__RC_STOP_MESSAGE__
	;;

    'start')
	__RC_START_CMD_PATH__
	;;

    'stop')
	__RC_STOP_CMD_PATH__
	;;
    
    *)
	echo "Usage $0 { start | stop }"

esac
exit 0
