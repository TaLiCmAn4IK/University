// 02-Feb-98 $$1 JJE  Created.
// 28-Oct-98 $$2 TWH  use $PRORB_RPCNUM
#include:head.inc
if ( ! $?PRORB_RPCNUM) then
  setenv PRORB_RPCNUM __PRORB_RPCNUM__
endif
$prodir/$mc/obj/rbm_shutdown -rpc $PRORB_RPCNUM $*
