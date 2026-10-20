// 09-Sep-99 $$1  MAZ  Created.
// 29-Sep-99 $$2  MAZ  Rm empty line before head.inc
// 22-Mar-00 $$3  MYA  change dvmockup to dvremote64
// 23-Jan-01 $$4  TWH   Sun4_solaris_64 changes
// 19-Mar-01 $$5  ALG   Include ptc_startup.inc
// 17-Jan-05 J-01-68+     Bruno Update dvs args
// 24-Jan-05 K-03-18  $$6 MAZ    Submit to K-03
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

#include:ptc_startup.inc

$prodir/$mc/obj/dvs -n dvmockup $*

