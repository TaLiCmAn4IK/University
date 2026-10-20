// 10-Mar-00  $$1  TWH  Created
// 30-May-00  $$2  TWH  Use full path to java_cmd
// 07-Jun-00  $$3  TWH  pass third argument; fix arg2; fix PATH
// 23-Jan-01 $$4  TWH   Sun4_solaris_64 changes
// 25-OCT-05 K-03-34 $$5  MTP  sun_solaris_x64 support
// 01-Nov-05 K-03-34+ $$6 MAZ  remove empty line before include line
#include:head.inc

setenv PTC_JRE __PTC_JRE__
if ( $?PATH ) then
  setenv PATH ${PTC_JRE}/bin:${prodir}/bin:${PATH}
else
  setenv PATH ${PTC_JRE}/bin:${prodir}/bin
endif

if ($mc =~ sun* ) then
  set java_cmd="java"  
  set arg2="$prodir/lib/toolkit.jar"
endif
if ($mc == hp8k) then
  set java_cmd="jre"
  set arg2="$PTC_JRE/lib/rt.jar:$PTC_JRE/lib/i18n.jar:$prodir/lib/toolkit.jar"
endif

$PTC_JRE/bin/$java_cmd -classpath "$arg2" com.ptc.propdm.client.ToolkitTest $1 $2 $3

