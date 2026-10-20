// 17-Jan-05 J-01-68+     Bruno  Created
// 24-Jan-05 K-03-18  $$1 MAZ    Submit to K-03
#include:head.inc
#include:shlib.inc

#include:sun4_tmpdir.inc

# ptc-cmdtype: mockup 

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
setenv WUHOME $prodir

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv MOCKUP_FEATURE_NAME "__MOCKUP_FEATURE_NAME__"

$prodir/$mc/obj/dvs -n dvremote $*

