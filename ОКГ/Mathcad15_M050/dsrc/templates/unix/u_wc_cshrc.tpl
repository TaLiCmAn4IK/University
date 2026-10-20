// 03-Apr-00  TWH   $$1  Made into PTC.Setup template
// 02-Oct-00  TWH   $$2  modify TNS_ADMIN
//
#!/bin/csh -f
#Windchill .cshrc

setenv WTDB_HOME __WTDB_HOME__
setenv ORACLE_HOME __ORACLE_HOME__
// TNS_ADMIN chg from WTDB_HOME to ORACLE_HOME/network/admin 840415
setenv TNS_ADMIN $ORACLE_HOME/network/admin

if ($?ORACLE_SID == 0) then
   setenv ORACLE_SID __ORACLE_SID__
endif

setenv NLS_LANG "AMERICAN_AMERICA.WE8ISO8859P1"

unsetenv TWO_TASK
setenv ORACLE_TERM   vt100
setenv ORACLE_SERVER ""

setenv TCL_LIBRARY   ${WTDB_HOME}/tools/lib/tcl
setenv TCL_AUTO_PATH ${WTDB_HOME}/dbs/Tcl

# The following removes trailing colon from PATH and adds ORACLE_HOME/bin
echo $PATH | grep $ORACLE_HOME/bin > /dev/null
if ($status) then
    setenv PATH `echo $PATH | sed 's/\(.*\):$/\1/'`:$ORACLE_HOME/bin
endif

# The following adds WTDB_HOME dependent directories
echo $PATH | grep $WTDB_HOME/tools/bin > /dev/null
if ($status) then
    setenv PATH `echo $PATH | sed 's/\(.*\):$/\1/'`:$WTDB_HOME/tools/bin
endif

# Make sure the local bin directory is in the path
echo $PATH | grep /usr/local/bin > /dev/null
if ($status) then
    setenv PATH `echo $PATH | sed 's/\(.*\):$/\1/'`:/usr/local/bin
endif

echo $PATH | grep /usr/sbin > /dev/null
if ($status) then
    setenv PATH `echo $PATH | sed 's/\(.*\):$/\1/'`:/usr/sbin
endif

if ($?LD_LIBRARY_PATH) then
   setenv LD_LIBRARY_PATH ${LD_LIBRARY_PATH}:$ORACLE_HOME/lib:$WTDB_HOME/tools/lib
else
   setenv LD_LIBRARY_PATH $ORACLE_HOME/lib:$WTDB_HOME/tools/lib
endif

unset noclobber

unalias rm
unalias mv
unalias cp
unalias ls
unalias ps
