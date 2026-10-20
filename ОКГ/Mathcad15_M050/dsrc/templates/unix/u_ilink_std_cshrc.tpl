// 25-Feb-04 K-01-26 $$1  TWH  Create in K-01
// 12-May-04 K-03-02 $$2  TWH  Add PDM_ORACLE_SERVER
#!/bin/csh -f
#Intralink .cshrc

setenv PROI_HOME __PROI_HOME__

setenv PDM_ORA_APPL intralink
setenv PDM_ORACLE_SERVER INTRALINK

setenv TCL_LIBRARY   ${PROI_HOME}/tools/lib

setenv TCL_AUTO_PATH ${PROI_HOME}/tools/tcl_api/sysutils
setenv TCLLIBPATH    ${PROI_HOME}/tools/tcl_api/sysutils

# The following adds PROI_HOME dependent directories
echo $PATH | grep $PROI_HOME/bin > /dev/null
if ($status) then
    setenv PATH `echo $PATH | sed 's/\(.*\):$/\1/'`:$PROI_HOME/bin:$PROI_HOME/diag:$PROI_HOME/tools/bin
endif

# Make sure specific directories are in the path
foreach req_path ( /usr/local/bin /usr/sbin )
    echo ":$PATH" | grep ":$req_path" > /dev/null
    if ($status) then
	if ((-e $req_path) && (-d $req_path)) then
	    setenv PATH `echo $PATH | sed 's/\(.*\):$/\1/'`:$req_path
	endif
    endif
end

switch (`uname -s`)
    case "HP-UX":
	if ($?SHLIB_PATH) then
	   setenv SHLIB_PATH ${SHLIB_PATH}:$PROI_HOME/ilink_lib:/usr/lib:$PROI_HOME/tools/lib
	else
	   setenv SHLIB_PATH $PROI_HOME/ilink_lib:/usr/lib:$PROI_HOME/tools/lib
	endif
    breaksw

    case "AIX":
	if ($?LIBPATH) then
	    setenv LIBPATH ${LIBPATH}:$PROI_HOME/ilink_lib:$PROI_HOME/tools/lib
	else
	    setenv LIBPATH $PROI_HOME/ilink_lib:$PROI_HOME/tools/lib
	endif
    breaksw

    case "SunOS":
	if ($?LD_LIBRARY_PATH) then
	    setenv LD_LIBRARY_PATH ${LD_LIBRARY_PATH}:$PROI_HOME/ilink_lib:$PROI_HOME/tools/lib
	else
	    setenv LD_LIBRARY_PATH $PROI_HOME/ilink_lib:$PROI_HOME/tools/lib
	endif
    breaksw
endsw

unset noclobber

unalias rm
unalias mv
unalias cp
unalias ls
unalias ps
