// 22-Mar-00 $$1  MYA  merge cv2vdi*.tpl to cv2vdi 
// 23-Jan-01 $$2  TWH   Sun4_solaris_64 changes
#include:head.inc
#include:shlib.inc
#include:sun4_tmpdir.inc

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

$prodir/$mc/obj/cv2vdi $*

