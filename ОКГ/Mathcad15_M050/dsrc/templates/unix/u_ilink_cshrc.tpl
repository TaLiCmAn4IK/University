// 05-Jun-98  $$1  TWH  Created from system .ilink_cshrc
// 14-Aug-98  $$2  SVN  Setting ORACLE_HOME without any conditions.
// 17-Aug-98  $$3  SVN  Fix bug in setting M_TYPE
// 01-Sep-98  $$4  TWH  Patch
// 16-Nov-98  $$5  TWH  Add ORACLE_HOME/lib libpath
// 16-Dec-98  $$6  TWH  mv ilink_lib under M_TYPE switch
// 02-Feb-99  $$7  TWH  Add /usr/lib for hp
// 21-Jun-99       TWH  modify M_TYPE setting (| tail -1)
// 25-Jun-99       SVN  Add new variable PROI_HOME;
//                      new placeholders: PROI_HOME,TNS_ADMIN
// 27-Sep-99  $$8  JJE  Sub to new build area
// 11-Apr-00  $$9  TWH  Add $PROI_HOME/tools/lib to LIBPATH's
//                      Fix TCL* and PATH for PROI_HOME
// 15-Aug-00  $$10 TWH  Add hpux11_pa32
// 04-Dec-00       TWH  Add ORACLE_HOME/lib32 to default(sgi) section only
// 05-Dec-00       TWH  fix default to use LD_LIBRARYN32_PATH (mips4)
// 08-Dec-00  $$11 TWH  use both for mips4 (why ?)
// 25-May-01  $$12 JJE  Don't use ucb paths on sun
#!/bin/csh -f
#Intralink .cshrc

setenv PROI_HOME __PROI_HOME__
setenv ORACLE_HOME __ORACLE_HOME__
setenv TNS_ADMIN __TNS_ADMIN__

if ($?ORACLE_SID == 0) then
   setenv ORACLE_SID ilnk
endif

setenv NLS_LANG __NLS_LANG__

unsetenv TWO_TASK
setenv ORACLE_TERM   vt100
setenv ORACLE_SERVER ""

setenv TCL_LIBRARY   ${PROI_HOME}/tools/lib/tcl
setenv TK_LIBRARY    ${PROI_HOME}/tools/lib/tk
setenv TCL_AUTO_PATH ${PROI_HOME}/tools/tcl_api/sysutils
setenv PDM_ORA_APPL intralink

# The following removes trailing colon from PATH and adds ORACLE_HOME/bin
echo $PATH | grep $ORACLE_HOME/bin > /dev/null
if ($status) then
    setenv PATH `echo $PATH | sed 's/\(.*\):$/\1/'`:$ORACLE_HOME/bin
endif

# The following adds PROI_HOME dependent directories
echo $PATH | grep $PROI_HOME/bin > /dev/null
if ($status) then
    setenv PATH `echo $PATH | sed 's/\(.*\):$/\1/'`:$PROI_HOME/bin:$PROI_HOME/diag:$PROI_HOME/tools/bin
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

set M_TYPE = `\ls $PROI_HOME | grep INTRALINK_ORA | tail -1`
switch ($M_TYPE)
	case INTRALINK_ORA*sun4_solaris
	     if ($?path) then
		set path=($path /opt/SUNWspro/bin /usr/ccs/bin /usr/sbin /etc /usr/openwin/bin /usr/openwin/bin/xview /usr/openwin/demo /bin /usr/bin)
	     else
		set path=(/opt/SUNWspro/bin /usr/ccs/bin /usr/sbin /etc /usr/openwin/bin /usr/openwin/bin/xview /usr/openwin/demo /bin /usr/bin)
	     endif
	     if ($?LD_LIBRARY_PATH) then
		setenv LD_LIBRARY_PATH ${LD_LIBRARY_PATH}:/opt/SUNWspro/lib:/opt/SUNWits/Graphics-sw/xgl-3.0/lib:/usr/openwin/lib:/usr/dt/lib:/opt/SUNWmotif/lib:/usr/lib:${ORACLE_HOME}/lib:$PROI_HOME/ilink_lib:$PROI_HOME/tools/lib
	     else
		setenv LD_LIBRARY_PATH /opt/SUNWspro/lib:/opt/SUNWits/Graphics-sw/xgl-3.0/lib:/usr/openwin/lib:/usr/dt/lib:/opt/SUNWmotif/lib:/usr/lib:${ORACLE_HOME}/lib:$PROI_HOME/ilink_lib:$PROI_HOME/tools/lib
	     endif
	breaksw
	case INTRALINK_ORA*hp700
	case INTRALINK_ORA*hp8k
	case INTRALINK_ORA*hpux11_pa32
	     if ($?SHLIB_PATH) then
		setenv SHLIB_PATH ${SHLIB_PATH}:$ORACLE_HOME/lib:$PROI_HOME/ilink_lib:/usr/lib:$PROI_HOME/tools/lib
	     else
		setenv SHLIB_PATH $ORACLE_HOME/lib:$PROI_HOME/ilink_lib:/usr/lib:$PROI_HOME/tools/lib
             endif
	breaksw
	case INTRALINK_ORA*ibm_rs6000
	     if ($?LIBPATH) then
		setenv LIBPATH ${LIBPATH}:$ORACLE_HOME/lib:$PROI_HOME/ilink_lib:$PROI_HOME/tools/lib
	     else
		setenv LIBPATH $ORACLE_HOME/lib:$PROI_HOME/ilink_lib:$PROI_HOME/tools/lib
             endif
	breaksw
	default
	    if ($?LD_LIBRARYN32_PATH) then
		setenv LD_LIBRARYN32_PATH ${LD_LIBRARYN32_PATH}:$ORACLE_HOME/lib32:$ORACLE_HOME/lib:$PROI_HOME/ilink_lib:$PROI_HOME/tools/lib
	    else
		setenv LD_LIBRARYN32_PATH $ORACLE_HOME/lib32:$ORACLE_HOME/lib:$PROI_HOME/ilink_lib:$PROI_HOME/tools/lib
	    endif
	    if ($?LD_LIBRARY_PATH) then
	       setenv LD_LIBRARY_PATH ${LD_LIBRARY_PATH}:$ORACLE_HOME/lib32:$ORACLE_HOME/lib:$PROI_HOME/ilink_lib:$PROI_HOME/tools/lib
	    else
	       setenv LD_LIBRARY_PATH $ORACLE_HOME/lib32:$ORACLE_HOME/lib:$PROI_HOME/ilink_lib:$PROI_HOME/tools/lib
	    endif
	breaksw
endsw

unset noclobber

unalias rm
unalias mv
unalias cp
unalias ls
unalias ps
