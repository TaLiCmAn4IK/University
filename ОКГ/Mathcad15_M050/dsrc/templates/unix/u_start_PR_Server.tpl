// 13-Apr-98 $$1  JJE   Created.
// 04-Aug-00 $$2  TWH   Add hpux11_pa32 dir check
// 16-May-01 $$3  TWH   hpux_pa64
#!/bin/csh
# set up CDRS variable if not defined in your environment
#
#setenv CDRS /usr/local/cdrs
#

if (0 == $?CDRS) then
    echo "Environment variable CDRS must first be defined"
    exit 999
endif

if (-e $CDRS/install/unix/getpmt) then
  set MACHINE_TYPE=`$CDRS/install/unix/getpmt`
else
  echo "$CDRS/install/unix/getpmt not found... exiting"
  exit -1
endif

if ( $MACHINE_TYPE == "hpux_pa64" ) then
  if ( ! -d $CDRS/hpux_pa64/obj ) then
    set MACHINE_TYPE="hpux11_pa32"
  endif
endif
if ( $MACHINE_TYPE == "hpux11_pa32" ) then
  if ( ! -d $CDRS/hpux11_pa32/obj ) then
    set MACHINE_TYPE="hp8k"
  endif
endif
#
## Set path for PhotoRender executable.
#

setenv PR_SERVER "$CDRS/$MACHINE_TYPE/obj/RenderServer -t PhotoRender"

#
# For larger models, increase this number, max of 1000000
#
setenv MAX_POLYS 100000

#
# For faster renderings, increase these numbers. Max recommended is
# 1952 width, 1040 height, uses more memory.


setenv MAX_RENDER_WIDTH  512
setenv MAX_RENDER_HEIGHT 516

# for multi-processor systems, increase to # of processors.
#

setenv MAX_PROCESSORS    1


#
### Starting up the PhotoRender Server:
#


echo "$PR_SERVER -v -m $MAX_POLYS -n $MAX_PROCESSORS -w $MAX_RENDER_WIDTH $MAX_RENDER_HEIGHT &"
$PR_SERVER -v -m $MAX_POLYS -n $MAX_PROCESSORS -w $MAX_RENDER_WIDTH $MAX_RENDER_HEIGHT &

exit
