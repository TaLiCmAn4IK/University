// 16-Sep-97  $$1  JJE  Created
    __RC_NAME__.readme


    If you would like the __INSTALLABLE_NAME__ process to startup
    automatically on a system reboot, you need to modify system
    files as root.  Since the installation process was not run
    by root, Pro/SETUP could not make these changes for you.
    This file contains instructions on how to add the
    __INSTALLABLE_NAME__ startup to your system's boot
    procedure.


    Digial UNIX:
       Copy __RC_NAME__ to /sbin/rc3.d/S99__RC_NAME__

    Hitachi HI-UX:
       Add a line:
	  __RC_START_CMD_PATH__
       to the end of the /etc/localrc file.  Make sure that
       there are no duplicate references to this startup script.

    Hewlett Packard HP-UX:
       Copy __RC_NAME__ to /sbin/rc2.d/S999__RC_NAME__

    IBM AIX:
       Add a line:
	  __RC_START_CMD_PATH__
       to the end of the /etc/rc.tcpip file.  Make sure that
       there are no duplicate references to this startup script.

    NEC UNIX:
       Copy __RC_NAME__ to /etc/rc2.d/S99__RC_NAME__

    Sun Solaris:
       Copy __RC_NAME__ to /etc/rc2.d/S99__RC_NAME__

    SGI IRIX:
       Copy __RC_NAME__ to /etc/rc2.d/S99__RC_NAME__
