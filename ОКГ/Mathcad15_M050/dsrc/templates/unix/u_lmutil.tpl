// 23-Apr-98 $$1 JJE Created
// 26-Dec-01 $$2 TWH Add PTC_D_LICENSE_FILE 919916
// 07-Jan-02 $$3 TWH move args infront of PTC_D 919916
// 18-Sep-02 J-03-34  $$4   MAZ  SPR 969593: rm -c arg and set LM_LICENSE_FILE 
#include:head.inc

#includeenv:LM_LICENSE_FILE:lmlic.env

setenv LM_LICENSE_FILE ${PTC_D_LICENSE_FILE}

$prodir/$mc/obj/lmutil $* 
