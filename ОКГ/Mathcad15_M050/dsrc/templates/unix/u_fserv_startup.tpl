// 16-Sep-97  $$1  JJE  Created
// 03-Nov-98  $$2  TWH  Add RFTS_PASSWD_ALT_FILE
#include:head.inc

setenv FSERV_PORT __FSERV_PORT__
setenv RFTS_PASSWD_ALT_FILE $prodir/install/etc/ilinkftpswd
$prodir/$mc/obj/nfsserv $FSERV_PORT &

