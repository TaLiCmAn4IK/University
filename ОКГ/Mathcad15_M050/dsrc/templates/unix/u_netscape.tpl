// 16-Sep-97  $$1  JJE  Created
// 06-Mar-98  $$2  JJE  Check if hp8k exists
// 09-Mar-98  $$3  JJE  Also unset XFILESEARCHPATH
// 06-Apr-98  $$4  JJE  Check if sgi_mips4 exists
// 07-Aug-00  $$5  TWH  Check if hpux11_pa32 exists
// 23-Jan-01  $$6  TWH   Sun4_solaris_64 changes
// 16-May-01  $$7  TWH   hpux_pa64
// 25-May-01  $$8  JJE  Don't use ucb paths on sun
#!/bin/csh -f

set flag = ""

#command
set fullscrname="$0"

#path to "prohelp"
set fullscrpath=$fullscrname:h

if ( $fullscrname == $fullscrpath ) set fullscrpath=$cwd

set old = $cwd
cd $fullscrpath
set fullscrpath = `pwd`
cd $old

set prodir="$fullscrpath:h"
set mc=`$prodir/bin/getpmt.csh`
if ( $mc == "hpux_pa64" ) then
 if ( ! -d $prodir/hpux_pa64 ) then
  set mc="hpux11_pa32"
 endif
endif
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

if ($mc =~ sgi*) then
	unsetenv XUSERFILESEARCHPATH
	unsetenv XFILESEARCHPATH
endif
if ( $mc == ibm_rs6000 ) then
   setenv LANG C
   setenv XUSERFILESEARCHPATH ""
   set appdir = ""
endif

set exe=realnetscape

$prodir/$mc/netscape/$exe $*
