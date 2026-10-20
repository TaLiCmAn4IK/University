// 01-Feb-02 $$1  MAZ   Copy from u_cpu_id.tpl (SPR 921507)
// 25-Sep-03 $$2  K-01-16  TWH  mc fallback
#include:head.inc

if ( $mc == "hpux_pa64" ) then
  if ( ! -e $prodir/$mc/obj/cpu_id ) then
    set mc="hpux11_pa32"
  endif
endif
if ( $mc == "sun4_solaris_64" )then
  if ( ! -e $prodir/$mc/obj/cpu_id ) then
    set mc="sun4_solaris"
  endif
endif
if ( $mc == "sgi_elf4" ) then
  if ( ! -e $prodir/$mc/obj/cpu_id ) then
    set mc="sgi_mips4"
  endif
endif
exec $prodir/$mc/obj/cpu_id $*

