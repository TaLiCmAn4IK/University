// 27-Mar-97 $$1  Pete  Created.
// 10-Feb-98 $$2  JJE   Check if hp8k exists
// 06-Mar-98 $$3  JJE   Modify the 8k check
// 06-Apr-98 $$4  JJE   Check if sgi_mips4 exists
// 27-Aug-98 $$5  JJE   Set pmt for java
// 17-Nov-98 $$6  TWH   mv PRO_MACHINE_TYPE/MC settings
// 04-Aug-00 $$7  TWH   Add hpux11_pa32 dir check
// 01-Dec-00 $$8  TWH   sun4_solaris_64 fallback
// 23-Jan-01 $$9  TWH   Sun4_solaris_64 changes
// 16-May-01 $$10 TWH   hpux_pa64 changes/fallback
// 25-May-01 $$11 JJE   Don't use ucb paths on sun
// 19-Sep-01 $$12 TWH   Mc fallback must check $mc/obj
// 14-Jan-03 J-03-40  ALG  $$13  SPR 927633: Define var "libset" if exe exists
// 02-Oct-03 K-01-16 TWH  $$14  sgi_elf4 fallback
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

if ( $mc == "hpux_pa64" ) then
 if ( ! -d $prodir/hpux_pa64/obj ) then
  set mc="hpux11_pa32"
 endif
endif
if ( $mc == "hpux11_pa32" ) then
 if ( ! -d $prodir/hpux11_pa32/obj ) then
  set mc="hp8k"
 endif
endif
if ( $mc == "hp8k" ) then
 if ( ! -d $prodir/hp8k/obj ) then
  if ( -d $prodir/hp700 ) set mc="hp700"
 endif
endif
if ( $mc == "sgi_elf4" ) then
 if ( ! -d $prodir/sgi_elf4/obj ) then
   set mc="sgi_mips4"
 endif
endif
if ( $mc == "sgi_mips4" ) then
 if ( ! -d $prodir/sgi_mips4/obj ) then
  if ( -d $prodir/sgi_elf2 ) set mc="sgi_elf2"
 endif
endif
if ( $mc == "sun4_solaris_64" )then
 if ( ! -d $prodir/sun4_solaris_64/obj ) then
  if ( -d $prodir/sun4_solaris ) set mc="sun4_solaris"
 endif
endif

set libset="$prodir/$mc/obj/ps_libset"
if ( ! -x $libset ) unset libset

setenv MC $mc
setenv PRO_MACHINE_TYPE $mc

setenv PRO_DIRECTORY $prodir

