// 12-Jun-98 $$1  ZRL   Created
// 13-Jul-98 $$2  TWH   Add ibm_only check
// 30-Nov-98 $$3  TWH   set valid list; inc catcheck
// 28-Jan-99 $$4  TWH   Added sgi_mips4
#include:head.inc

set valid_catia_mc = (ibm_rs6000 sgi_elf2 sgi_mips4)
#include:catcheck.inc

setenv MALLOCTYPE 3.1
exec $prodir/$mc/obj/progeo_to_cat4 $*

