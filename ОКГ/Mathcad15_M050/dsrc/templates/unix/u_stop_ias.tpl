// 20-Nov-01  $$1  TWH  Created
// 28-Nov-01  $$2  TWH  fix servers value
#include:head.inc

set ILSERVER="iaserver"

if ( $mc =~ sun4_solaris* ) then
  set servers=`/usr/bin/ps -e | grep iaserver | grep -v grep | grep -v stop_ias | awk '{print $1}'`
else if ( $mc =~ hpux* ) then
  setenv UNIX95 true
  set servers=`/usr/bin/ps -aef -o 'pid ppid args' | grep -v grep | grep -v stop_ias | grep ias | awk '{print $1 " " $2 " " $3}' | grep $prodir | awk '{print $1}'`
  unsetenv UNIX95
else
  set servers=`/usr/bin/ps -aef -o 'pid ppid args' | grep -v grep | grep -v stop_ias | grep iaserver | awk '{print $1 " " $2 " " $3}' | grep $prodir | awk '{print $1}'`
endif

if (${?servers} ) then
   if( ${#servers} > 0 ) then
      kill -9 $servers
      echo "Server stopped"
   else
      echo "No server instances found"
   endif
else
   echo "No server instances found"
endif

