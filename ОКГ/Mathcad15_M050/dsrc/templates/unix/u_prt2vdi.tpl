// 22-Mar-00 $$1  MYA  merge prt2vdi* to pro2vdi 
// 23-Jan-01 $$2  TWH   Sun4_solaris_64 changes
// 25-OCT-05 K-03-34 $$3  MTP  sun_solaris_x64 support
// 01-Nov-05 K-03-34+ $$4 MAZ  remove empty line before include line
#include:head.inc
#include:shlib.inc

if ($mc =~ sun*) then
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

setenv DIVISIONROOT $prodir
setenv PRO_DIRECTORY $prodir

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv MOCKUPCONVERTER_FEATURE_NAME "__MOCKUP_CONV_FEATURE_NAME__"

$prodir/$mc/obj/prt2vdi $*

