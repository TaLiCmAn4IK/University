// 27-Mar-97 $$1  Pete  Created.
// 12-Jun-98 $$2  ZRL   added MALLOCTYPE
// 13-Jul-98 $$3  TWH   Add ibm_only check
// 30-Nov-98 $$4  TWH   set valid list; inc catcheck
// 12-May-99 $$5  TWH   Add sgi_mips4
#include:head.inc

set valid_catia_mc = (ibm_rs6000 sgi_elf2 sgi_mips4)
#include:catcheck.inc

setenv MALLOCTYPE 3.1
exec $prodir/$mc/obj/pro_to_cat $*

