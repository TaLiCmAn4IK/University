// 17-Jul-2000 $$1 JJE Created
// 11-Aug-2000 $$2 JJE Add nmsd startup
// 14-Mar-2001 $$3 Idan Added alpha_unix condition
// 07-May-2001 $$4 Asaf Removed extra empty line
#include:head.inc

#includeenv:PTCNMSPORT:ptcnmsport.env

setenv PRO_TUTORIAL_START $prodir/bin/protutorial

if ($mc == alpha_unix) then
   uac p reset
endif

$prodir/$mc/obj/ptc_launcher "-trb:$1"
