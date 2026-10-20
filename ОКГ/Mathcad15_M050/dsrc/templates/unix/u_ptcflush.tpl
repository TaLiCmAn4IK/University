// 09-Sep-97 $$1 TWH Created
// 13-Jan-98 $$2 JJE Use lmremove
// 23-Jan-98 $$3 JJE Check num args
// 27-May-98 $$4 JJE Change arguments
// 01-Oct-98 $$5 TWH Remove "-c $LM_LICENSE_FILE" from cmd line
// 17-Oct-01 $$6 TWH Use PTC_D_LICENSE_FILE
// 05-May-03 K-01-06 $$7 JJE Use LM_LICENSE_FILE
// 25-Sep-03 K-01-16 $$8 TWH MC fallback
#include:head.inc

if ($#argv < 4) then
    echo ""
    echo 'Usage: ptcflush <feature> <license server> <port> <handle>'
    echo ""
	echo "Example: ptcflush PROE_1234 ptchost 7788 213"
    echo ""
    echo "  All arguments to ptcflush can be taken from the"
    echo "  output of ptcstatus"
    echo ""
    exit -1
endif

setenv PRO_LICENSE_RES __LICENSE_RES__
setenv LM_LICENSE_FILE __LM_LICENSE_FILE__

if ( $mc == "hpux_pa64" ) then
  if ( ! -e $prodir/$mc/obj/lmutil ) then
    set mc="hpux11_pa32"
  endif
endif
if ( $mc == "sun4_solaris_64" )then
  if ( ! -e $prodir/$mc/obj/lmutil ) then
    set mc="sun4_solaris"
  endif
endif
if ( $mc == "sgi_elf4" ) then
  if ( ! -e $prodir/$mc/obj/lmutil ) then
    set mc="sgi_mips4"
  endif
endif

exec $prodir/$mc/obj/lmutil lmremove -h $1 $2 $3 $4 $5
