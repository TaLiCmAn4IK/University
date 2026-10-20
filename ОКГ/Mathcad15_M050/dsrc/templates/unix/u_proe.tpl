// 07-Nov-03 K-01-18 $$1  TWH  Created.
// 21-Nov-03 K-01-18+ $$2 MAZ  Fix include line
#include:head.inc
setenv FORCE_PMT $MC
$PRO_DIRECTORY/$MC/obj/proe.exe $*
exit $status
