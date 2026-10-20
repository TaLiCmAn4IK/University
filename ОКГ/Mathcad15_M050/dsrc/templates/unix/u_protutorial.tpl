// 17-Jul-2000 $$1 JJE Created
// 05-Mar-03 K-01-02  ALG  $$2   SPR 1003705: Exit status
#include:head.inc

#includeenv:PTCNMSPORT:ptcnmsport.env

#includeenv:PROE_START:proe_start.env

if( $?PROE_START ) then
  $PROE_START -tutorial:_remote_helper
  exit $status
else
  exit 1
endif
