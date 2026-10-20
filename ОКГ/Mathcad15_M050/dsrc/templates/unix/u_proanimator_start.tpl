// 03-Jun-98 $$1  TWH   Created.
// 20-Oct-99 $$2  OTT   Changed to network_animator.
// 13-Jan-00 $$3  OTT   Added LD_LIBRARYN32_PATH for mips4
// 23-Jan-01 $$4  TWH   Sun4_solaris_64 changes; drop /usr/lib (sun*)
// 19-Mar-01 $$5  ALG   Include ptc_startup.inc
// 25-May-01 $$6  JJE  Don't use ucb paths on sun
// 25-OCT-05 K-03-34 $$7  MTP  sun_solaris_x64 support
// 01-Nov-05 K-03-34+ $$8 MAZ  remove empty line before include line
#include:head.inc

# ptc-cmdtype: network_animator

#
# Set path to find executables
#
set path = ($PRO_DIRECTORY/bin $path)

setenv MACHINE $mc

#
# Set up LD_LIBRARY_PATH 
#
switch ($PRO_MACHINE_TYPE)
case sun4_solaris_64:
case sun_solaris_x64:
case sun4_solaris:
        if ($?LD_LIBRARY_PATH) then
          setenv LD_LIBRARY_PATH /usr/openwin/lib:${LD_LIBRARY_PATH}
        else
          setenv LD_LIBRARY_PATH /usr/openwin/lib
        endif
	breaksw
case sgi_mips4:
        if ($?LD_LIBRARYN32_PATH) then
          setenv LD_LIBRARYN32_PATH ${PRO_DIRECTORY}/libraries/$mc/lib_shared:${LD_LIBRARYN32_PATH}
        else
          setenv LD_LIBRARYN32_PATH ${PRO_DIRECTORY}/libraries/$mc/lib_shared
        endif
        if ($?LD_LIBRARY_PATH) then
          setenv LD_LIBRARY_PATH ${PRO_DIRECTORY}/libraries/sgi_mips4/lib_shared:${LD_LIBRARY_PATH}
        else
          setenv LD_LIBRARY_PATH ${PRO_DIRECTORY}/libraries/sgi_mips4/lib_shared
        endif
	breaksw
endsw

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv ANIMATOR_FEATURE_NAME "__ANIMATOR_FEATURE_NAME__"

#include:ptc_startup.inc

$prodir/$mc/obj/network_animator $*

#end
