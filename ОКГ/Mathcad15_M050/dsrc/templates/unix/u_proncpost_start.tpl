// 27-Mar-97 $$1  Pete  Created.
// 11-Feb-98 $$2  JJE  Use 200
// 12-Oct-98 $$3  JJE  Use 210
// 07-Aug-00 $$4  TWH  fix ptc-cmdtype line
#include:head.inc

# ptc-cmdtype: nc_post

#include:nlocli.inc

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv NCPOST_FEATURE_NAME "__NCPOST_FEATURE_NAME__"

$prodir/$mc/obj/ncp210q.exe $*
