// 27-Mar-97 $$1  Pete  Created.
// 03-Nov-97 $$2  JJE   Rewrite for new install
// 14-Nov-97 $$3  JJE   tweak prodir before shlib.inc
// 05-Jan-98 $$4  JJE   Change ptcutil to ptcsetup
// 13-Jan-98 $$5  JJE   Use delta lib
// 10-Apr-98 $$6  JJE   Fix missing \ to pass args properly
// 06-May-98 $$7  TWH   fix machinetype if necessary
// 03-Nov-99      MYA   Migrate to new installer
// 16-Nov-99 $$8  JJE   Rm plat check
// 05-Mar-03 K-01-02  ALG  $$9   SPR 1003705: Exit status
// 31-Mar-03 K-01-04  TWH  $$10  1005485; test mc after prodir set.
// 15-Apr-03 K-01-06  TWH  $$11  handle sgi_elf4
#include:head.inc

set prodir = "${prodir}/dsrc"
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
if ( $mc == "sgi_elf4" ) then
 if ( ! -d $prodir/sgi_elf4/obj ) set mc="sgi_mips4"
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
#include:shlib.inc
set prodir=$prodir:h

@ i = 1 
while ($i < ($#argv + 1))
  switch ($argv[$i])
    case "-display"
  		@ i++
		echo "setting DISPLAY to: $argv[$i]"
		setenv DISPLAY $argv[$i]
        breaksw
  endsw
  @ i++
end

if (! $?DISPLAY) then
   echo ""
   echo 'ERROR:  The DISPLAY variable is not set - '
   echo ""
   echo "   Options:"
   echo "      o Press <cr> to set DISPLAY to `hostname`:0.0"
   echo "      o Type a value to use for the DISPLAY variable"
   echo "      o Enter 'q' to abort prosetup"
   echo "      o Specify '-display <host>:0' on the command line"
   echo ""
   echo -n "Set it to (`hostname`:0.0): [q to abort] "
   set choice="$<"
   if ($choice =~ [Qq]*) exit 1
   if ("$choice" == "") set choice=Y
   if ($choice =~ [Yy]*) then
       setenv DISPLAY `hostname`:0.0
   else
       setenv DISPLAY $choice
   endif
   echo ""
   echo "   DISPLAY has been set to: $DISPLAY"
   echo ""
endif

####### application specific settings
setenv PTC_CDHOME $prodir
setenv PS_DIRECTORY $prodir/dsrc
setenv DPS_DIRECTORY $prodir/dsrc
setenv PRO_MACHINE_TYPE $mc
setenv LC_NUMERIC C

onintr QUICK_EXIT

echo ""
echo -n "Starting PTC.Setup, please wait ..."
echo ""

if( ! $?PROSETUP_EXE ) then
  setenv PROSETUP_EXE $prodir/dsrc/$mc/obj/ptcsetup
else
  echo ""
  echo -n "Using $PROSETUP_EXE ..."
endif

if (! -e $PROSETUP_EXE ) then
   echo ""
   echo "ERROR:  $PROSETUP_EXE - file is missing."
   echo "        Check the name of the CD mount directory."
   exit 1
endif

$PROSETUP_EXE -ptcutil $*
set exitCode = $status

echo ""
exit $exitCode

############
# QUICK EXIT
############
QUICK_EXIT:
exit 1
