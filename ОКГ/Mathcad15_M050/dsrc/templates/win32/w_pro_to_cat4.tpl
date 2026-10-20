// 30-Nov-98 $$1   TWH  Created from unix scripts
// 12-May-99 $$2  TWH   Add sgi_mips4
#include:u_head.inc

set valid_catia_mc = (ibm_rs6000 sgi_elf2 sgi_mips4)
#include:u_catcheck.inc

setenv MALLOCTYPE 3.1
exec $prodir/$mc/obj/pro_to_cat4 $*

