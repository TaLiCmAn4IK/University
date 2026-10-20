// 10-Jul-98 $$1  JJE   Created
// 28-Jul-98 $$2  JJE   Remove stuff
// 23-Jan-01 $$3  TWH   Sun4_solaris_64 changes
#include:head.inc
#include:sun4_tmpdir.inc

# ptc-cmdtype: procdm

#includeenv:PROE_START:proe_start.env

setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv PROCDM_FEATURE_NAME "__CDM_FEATURE_NAME__"

$prodir/$mc/obj/procdm $*
