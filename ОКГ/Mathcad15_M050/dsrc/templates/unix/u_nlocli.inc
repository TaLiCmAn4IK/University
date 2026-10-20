// 27-Mar-97 $$1  Pete  Created.
if (! -e /etc/nloservers.txt && -e $prodir/etc/`hostname`/nloservers.txt) setenv NLO_CLIENT_DIR $prodir/etc/`hostname`

