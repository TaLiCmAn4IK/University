// 09-Sep-97 $$1 TWH Created
// 06-Apr-98 $$2 JJE Check for sgi_mips4
// 07-Aug-00 $$3 TWH Check if hpux11_pa32 exists
// 23-Jan-01 $$4  TWH   Sun4_solaris_64 changes
// 15-Feb-01 $$5  TWH Sun fallback
// 16-May-01 $$6  TWH  hpux_pa64
// 25-May-01 $$7  JJE  Don't use ucb paths on sun
// 19-Sep-01 $$8  TWH  pass through args; modify mc fallback chk
// 17-Oct-01 $$9  TWH  Use PTC_D_LICENSE_FILE
// 03-Nov-03 K-01-17 $$10 TWH  Use -e chk for intralink
#!/bin/csh -f

set rundir="$cwd"
set fullscrname="$0"
set fullscrname=`ls -l $fullscrname | awk '{print $NF}'`
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
if ( $mc == "hpux_pa64" ) then
 if ( ! -e $prodir/hpux_pa64/obj/plpf_status ) then
  set mc="hpux11_pa32"
 endif
endif
if ( $mc == "hpux11_pa32" ) then
 if ( ! -e $prodir/hpux11_pa32/obj/plpf_status ) then
  set mc="hp8k"
 endif
endif
if ( $mc == "hp8k" ) then
 if ( ! -e $prodir/hp8k/obj/plpf_status ) then
  if ( -d $prodir/hp700 ) set mc="hp700"
 endif
endif
if ( $mc == "sgi_mips4" ) then
 if ( ! -e $prodir/sgi_mips4/obj/plpf_status ) then
  if ( -d $prodir/sgi_elf2 ) set mc="sgi_elf2"
 endif
endif
if ( $mc == "sun4_solaris_64" )then
 if ( ! -e $prodir/sun4_solaris_64/obj/plpf_status ) then
  if ( -d $prodir/sun4_solaris ) set mc="sun4_solaris"
 endif
endif

setenv MC $mc

setenv PRO_DIRECTORY $prodir

setenv PRO_LICENSE_RES __LICENSE_RES__
setenv PTC_D_LICENSE_FILE __LM_LICENSE_FILE__

exec $prodir/$mc/obj/plpf_status $*
