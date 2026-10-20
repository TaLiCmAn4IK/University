// 17-Nov-98      TWH   mv PRO_MACHINE_TYPE/MC settings
// 30-Nov-98 $$1  TWH   Created from unix/u_head.inc begining with above comment.
// 17-Jan-01 $$2  TWH   Updated from unix/u_head.inc
#!/bin/csh -f

set rundir="$cwd"
set fullscrname="$0"
set fullscrname=`ls -l $fullscrname | awk '{print $NF}'`
set scrname="$fullscrname:t"
set fullscrpath=$fullscrname:h
if ($fullscrpath == $fullscrname) then
    set fullscrpath=$cwd
else
    cd $fullscrpath
    set fullscrpath=$cwd
endif
cd $rundir
set prodir="$fullscrpath:h"
set mc=`$prodir/install/unix/getpmt`
setenv PRODIR $prodir

if ( $mc == "hpux11_pa32" ) then
 if ( ! -d $prodir/hpux11_pa32 ) then
  set mc="hp8k"
 endif
endif
if ( $mc == "hp8k" ) then
 if ( ! -d $prodir/hp8k ) then
  if ( -d $prodir/hp700 ) set mc="hp700"
 endif
endif
if ( $mc == "sgi_mips4" ) then
 if ( ! -d $prodir/sgi_mips4 ) then
  if ( -d $prodir/sgi_elf2 ) set mc="sgi_elf2"
 endif
endif
if ( $mc == "sun4_solaris_64" )then
 if ( ! -d $prodir/sun4_solaris_64 ) then
  if ( -d $prodir/sun4_solaris ) set mc="sun4_solaris"
 endif
endif

setenv MC $mc
setenv PRO_MACHINE_TYPE $mc

if ($mc =~ sun4_solaris*) then
   set path=($path /usr/ucb)
endif

setenv PRO_DIRECTORY $prodir

