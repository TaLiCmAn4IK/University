// 11-Jun-98 $$1  ZRL  Created.
// 07-Jul-98 $$2  JJE  Add LD_LIBRARY_PATH for sun
// 23-Jan-01 $$3  TWH   Sun4_solaris_64 changes
// 25-May-01 $$4  JJE  Don't use ucb paths on sun
// 25-OCT-05 K-03-34 $$5  MTP  sun_solaris_x64 support
// 01-Nov-05 K-03-34+ $$6 MAZ  remove empty line before include line
#include:head.inc

if ($mc =~ sun*) then
	if ($?LD_LIBRARY_PATH) then
		setenv LD_LIBRARY_PATH ${LD_LIBRARY_PATH}:$prodir/$mc/lib
	else
		setenv LD_LIBRARY_PATH /usr/openwin/lib:$prodir/$mc/lib:/usr/dt/lib
	endif
endif

exec $prodir/$mc/obj/pro_from_feas $*
