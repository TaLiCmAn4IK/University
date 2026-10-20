// 04-Jan-02 $$1  MAZ   Wildfire settings 
// 04-Apr-02 $$2  MAZ   SPR 939393: Fix sgi setting
// 02-Jul-02 $$3  MAZ   SPR 962331: prepend MOZILLA_FIVE_HOME to libpath
// 09-Aug-02 $$4  JJE   PTC_XALAN_BIN no longer needed
// 23-OCT-02 $$5  MTP   Added i486_linux
// 18-Dec-02 $$6  JJE   Use N32 on sgi
// 14-Jan-03 J-03-40  ALG  $$7   SPR 927633: Use "libset" to set env var values
// 25-OCT-05 K-03-34  MTP  $$8   sun_solaris_x64 support

setenv MOZILLA_FIVE_HOME $prodir/$mc/obj/MOZILLA

if ($mc =~ sun*) then
    if ($?libset) then
        setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH prepend $MOZILLA_FIVE_HOME`
    else
        setenv LD_LIBRARY_PATH ${MOZILLA_FIVE_HOME}:${LD_LIBRARY_PATH}
    endif
endif

if ($mc == i486_linux) then
    if ($?libset) then
        setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH prepend $MOZILLA_FIVE_HOME`
    else
        setenv LD_LIBRARY_PATH ${MOZILLA_FIVE_HOME}:${LD_LIBRARY_PATH}
    endif
endif

if ($mc =~ sgi* ) then
	if( $?LD_LIBRARYN32_PATH ) then
        if ($?libset) then
            setenv LD_LIBRARYN32_PATH `$libset LD_LIBRARYN32_PATH prepend $MOZILLA_FIVE_HOME`
        else
            setenv LD_LIBRARYN32_PATH ${MOZILLA_FIVE_HOME}:${LD_LIBRARYN32_PATH}
        endif
	else
		setenv LD_LIBRARYN32_PATH ${MOZILLA_FIVE_HOME}
	endif
endif

if ($mc =~ hp* ) then
    if ($?libset) then
        setenv SHLIB_PATH `$libset SHLIB_PATH prepend $MOZILLA_FIVE_HOME`
    else
        setenv SHLIB_PATH ${MOZILLA_FIVE_HOME}:${SHLIB_PATH}
    endif
endif

if ($mc == ibm_rs6000) then
    if ($?libset) then
        setenv LIBPATH `$libset LIBPATH prepend $MOZILLA_FIVE_HOME`
    else
        setenv LIBPATH ${MOZILLA_FIVE_HOME}:${LIBPATH}
    endif
endif

