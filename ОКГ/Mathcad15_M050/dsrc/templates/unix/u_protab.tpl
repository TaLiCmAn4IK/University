// 27-Mar-97 $$1  Pete  Created.
#include:head.inc

#include:shlib.inc


if ($mc =~ sgi*) unsetenv XUSERFILESEARCHPATH
if ($mc == alpha_unix) then
    limit datasize unlimited
    limit stacksize unlimited
endif

setenv PROTABLE_DIRECTORY $prodir/protable
exec $prodir/$mc/obj/protab $*

