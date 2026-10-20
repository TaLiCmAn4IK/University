// 09-Sep-99 $$1  MAZ  Created.
// 22-Oct-99 $$2  JJE  Synch w/ 02
// 28-MAr-00 $$3  MYA  change MOCKUP_CONVERTER to MOCKUPCONVERTER
#include:head.inc
#include:shlib.inc

if ($mc == sun4_solaris) then
    if (! $?TMPDIR) then
        setenv TMPDIR /tmp
    endif
endif

if ($mc =~ sgi*) then
   unsetenv XUSERFILESEARCHPATH
   setenv GLMGRARRAYOPT 1
endif

if ($mc == alpha_unix) then
    limit datasize unlimited
    limit stacksize unlimited
endif

#include:add_path.inc

setenv DIVISIONROOT $prodir
setenv PRO_DIRECTORY $prodir

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv MOCKUPCONVERTER_FEATURE_NAME "__MOCKUP_CONV_FEATURE_NAME__"

$prodir/$mc/obj/iges2vdi $*

