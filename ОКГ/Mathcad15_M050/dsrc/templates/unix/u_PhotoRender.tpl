// 20-Oct-99 $$1  OTT  Created.
// 23-Jan-01 $$2  TWH  Sun4_solaris_64 changes; drop /usr/lib (sun*)
// 25-May-01 $$3  JJE  Don't use ucb paths on sun
// 25-OCT-05 $$4  K-03-34  MTP  sun_solaris_x64 support
// 01-Nov-05 $$5  K-03-34+ MAZ  remove empty line before include line
#include:head.inc

switch ($PRO_MACHINE_TYPE)
case sun4_solaris_64:
case sun4_solaris:
case sun_solaris_x64:
        if ($?LD_LIBRARY_PATH) then
          setenv LD_LIBRARY_PATH /usr/openwin/lib:${LD_LIBRARY_PATH}
        else
          setenv LD_LIBRARY_PATH /usr/openwin/lib
        endif
	breaksw
endsw

exec $prodir/$mc/obj/PhotoRender $*

