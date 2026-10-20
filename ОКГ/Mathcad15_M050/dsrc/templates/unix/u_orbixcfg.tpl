// 20-Jan-99  I-01-31  $$1  TWH  Created.
//
# This file contains the default values for Orbix 2.3c system configuration.
# =========================================================================
#
# Below are listed the main orbix environment configuration variables
# and associated default values. An Orbix client, server or daemon will
# use these values if, and only if, the relevant unix environment
# variable is not defined.

# the port number for the Orbix daemon:
IT_DAEMON_PORT          1570

# the starting port number for daemon-run servers:
IT_DAEMON_SERVER_BASE   1590

# the full path name of the error messages _file_:
IT_ERRORS               __ORBIX_DIRECTORY__/cfg/ErrorMsgs

# the full path name of the Implementation Repository _directory_:
IT_IMP_REP_PATH         __PRO_DIRECTORY__/ImpRep

# the full path name of the Interface Repository _directory_:
IT_INT_REP_PATH         __PRO_DIRECTORY__/IntRep

# the full path name of the _directory_ holding the locator files:
IT_LOCATOR_PATH         __ORBIX_DIRECTORY__/cfg

# the local DNS domain name:
IT_LOCAL_DOMAIN         __LOCAL_DOMAIN__

