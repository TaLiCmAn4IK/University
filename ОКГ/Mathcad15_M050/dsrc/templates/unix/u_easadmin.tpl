// 21-Jun-99  I-02-04  $$1  TWH  Redirect stderr/stdout
#include:head.inc

if ($?CLASSPATH) then
	setenv CLASSPATH "${CLASSPATH}:${PRO_DIRECTORY}/jar/AdminClient.jar:${PRO_DIRECTORY}/jar/OrbixClasses.jar:${PRO_DIRECTORY}/jar/symbeans.jar:${PRO_DIRECTORY}/cfg/"
else
	setenv CLASSPATH ".:${PRO_DIRECTORY}/jar/AdminClient.jar:${PRO_DIRECTORY}/jar/OrbixClasses.jar:${PRO_DIRECTORY}/jar/symbeans.jar:${PRO_DIRECTORY}/cfg/"
endif

java com.ptc.cadadmin.LaunchCadadmin >& /dev/null
