// 10-Mar-00  $$1  TWH  Created
// 10-Apr-00  $$2  TWH  Drop licensing
// 30-May-00  $$3  TWH  chg PDM_CLIENT_DIR;PRO_COMM_MSG_EXE;use full path to java_cmd
// 07-Jun-00  $$4  TWH  Force directory change to prodir; fix arg2; fix PATH
// 31-Aug-00  $$5  TWH  Change arg1 per Jovian's mail 18 Jul 2000
// 23-Jan-01 $$6  TWH   Sun4_solaris_64 changes
// 25-OCT-05 K-03-34 $$7  MTP  sun_solaris_x64 support
// 01-Nov-05 K-03-34+ $$8 MAZ  remove empty line before include line
#include:head.inc
#include:shlib.inc

setenv PDM_CLIENT_DIR $prodir/etc/`hostname`
setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg

setenv PTC_JRE __PTC_JRE__
if ( $?PATH ) then
  setenv PATH ${PTC_JRE}/bin:${prodir}/bin:${PATH}
else
  setenv PATH ${PTC_JRE}/bin:${prodir}/bin
endif

if ($mc =~ sun* ) then
  set java_cmd="java"
  set arg1="-Xms128m -Xmx512m"
  set arg2="$prodir/lib/toolkit.jar:$prodir"
endif
if ($mc == hp8k) then
  set java_cmd="jre"
  set arg1="-ms128m -mx512m"
  set arg2="$PTC_JRE/lib/rt.jar:$PTC_JRE/lib/i18n.jar:$prodir/lib/toolkit.jar:$prodir"
endif

cd $prodir

$PTC_JRE/bin/$java_cmd -Djava.security.policy="$prodir/lib/security/pas.policy" $arg1 -classpath "$arg2" com.ptc.propdm.server.PGToolkitServer
