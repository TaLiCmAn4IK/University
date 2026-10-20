// 13-Apr-98 $$1  JJE   Created.
// 15-Apr-98 $$2  JJE   Fixed.
// 22-Apr-98      JJE   Include PROE_START
// 13-May-98      TWH   Fix CDRS_LIBRARY env
// 18-May-98      TWH   Change MACHINE env 
// 02-Jun-98      TWH   Add PAINT_CMD,DES_LIB
// 16-Jun-98 $$3  JJE   Group Submission
// 01-Sep-98 $$4  TWH   rm animate and lisp lines per dottley
// 14-Sep-98 $$5  TWH   includeenv CDRS_DIR
// 05-Oct-98 $$6  JJE   check mips4
// 06-Oct-98 $$7  TWH   move PROE_START; add if defined check
// 02-Mar-99      JJE   Add appmgr
// 08-Mar-99 $$8  JJE   Add nmsd
// 20-Oct-99 $$9  OTT   Changed env var CDRS_SOCKET, CDRS_POPEN_SOCKET
// 22-Nov-99 $$10 OTT   Added DIVISIONROOT
// 13-Jan-00 $$11 OTT   Added LD_LIBRARYN32_PATH for mips4; rm sgi_elf2
// 23-Jan-01 $$12 TWH   Sun4_solaris_64 changes; drop /usr/lib (sun*)
// 19-Mar-01 $$13 ALG   Include ptc_startup.inc
// 25-May-01 $$14 JJE   Don't use ucb paths on sun
// 25-OCT-05 K-03-34 $$15  MTP  sun_solaris_x64 support
// 01-Nov-05 K-03-34+ $$16 MAZ  remove empty line before include line
#include:head.inc

# ptc-cmdtype: cdrs
#
# Description:  This shell sets up the appropriate environment
#               for running Pro/DESIGNER.
#

if ( ! $?CDRS ) then
setenv CDRS $PRODIR
#includeenv:CDRS_DIR:cdrs.env 
endif


if ( ! $?PROE_START ) then
#includeenv:PROE_START:proe_start.env
endif

#
# Location of model directories
#
if ( ! $?CDRS_DATA) then
        setenv CDRS_DATA $cwd
endif

#
# Location of cdrs libraries
#
#includeenv:DES_LIB:cdrs_library.env
if ( ! $?CDRS_LIBRARY) then
        setenv CDRS_LIBRARY $CDRS/graphic-library
endif

#includeenv:DIVROOT:divisionroot.env

#
# Set the MACHINE environment variable ...
#
setenv MACHINE $mc

# 
# Platform dependent environment variables, etc...
#
switch ($MACHINE)
case sun4_solaris_64:
case sun_solaris_x64:
case sun4_solaris:
        setenv FREE_SWAP_SPACE `/etc/swap -l | sed '/swaplo/d' | awk '{total += $5} END {print 512 * total}'`
        if ($?LD_LIBRARY_PATH) then
          setenv LD_LIBRARY_PATH /usr/openwin/lib:/usr/dt/lib:$CDRS/libraries/$MACHINE/lib_shared:${LD_LIBRARY_PATH}
        else
          setenv LD_LIBRARY_PATH /usr/openwin/lib:/usr/dt/lib:$CDRS/libraries/$MACHINE/lib_shared
        endif

        # Franz requirement ...
        breaksw
case ibm_rs6000:
        pushd /etc > /dev/null
        setenv FREE_SWAP_SPACE `lsps -a | awk '{ s += $4 - 0.01 * $5 * $4 } END {print s * 1000000 }'`
        popd > /dev/null
        #
        # Deny cdrs-lisp no resources ...
        unlimit datasize

        breaksw
case sgi_mips4:
        if ($?LD_LIBRARYN32_PATH) then
          setenv LD_LIBRARYN32_PATH $prodir/libraries/$mc/lib_shared:${LD_LIBRARYN32_PATH}
        else
          setenv LD_LIBRARYN32_PATH $prodir/libraries/$mc/lib_shared
        endif
        setenv FREE_SWAP_SPACE `/etc/swap -l | sed '/swaplo/d' | awk '{total += $5} END {print 512 * total}'`
        setenv MEMORY `/bin/hinv | awk 'BEGIN {s = 67108864} ($1 == "Main") && ($2 == "memory") { s = $4 * 1048576 } END {print s}'`
        if ($?LD_LIBRARY_PATH) then
          setenv LD_LIBRARY_PATH $CDRS/libraries/$MACHINE/lib_shared:${LD_LIBRARY_PATH}
        else
          setenv LD_LIBRARY_PATH $CDRS/libraries/$MACHINE/lib_shared
        endif
        
        #
        # Deny cdrs-lisp no resources ...
        limit datasize unlimited
                
        breaksw
case hp8k:
        if (! $?SHLIB_PATH) then
          setenv SHLIB_PATH /opt/graphics/common/lib:/usr/lib:$CDRS/libraries/$MACHINE/lib_shared
        else
          setenv SHLIB_PATH ${SHLIB_PATH}:/opt/graphics/common/lib:/usr/lib:$CDRS/libraries/$MACHINE/lib_shared
        endif
        breaksw
default
        echo "Machine $MACHINE is not known in Pro/DESIGNER script file"
        breaksw
endsw


#
# Augment path with location of CDRS executables
#
set path = ($CDRS/bin $CDRS/$MACHINE/obj $path)


#
# Build unique names for sockets ...
#
set DATE_STRING = `date | sed 's/[: ]/_/g'`
setenv CDRS_SOCKET ".CDRS_SOCKET_$DATE_STRING"
setenv CDRS_POPEN_SOCKET ".CDRS_POCKET_$DATE_STRING"
setenv GSERV_DEFAULT_SOCK "gcp_socket_$DATE_STRING"
if ( ! $?GCP_SOCKET_NAME ) setenv GCP_SOCKET_NAME $GSERV_DEFAULT_SOCK

#includeenv:PAINT_CMD:paint_cmd.env
#includeenv:PTC_APPMGR:appmgr.env

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv DESIGNER_FEATURE_NAME "__DESIGNER_FEATURE_NAME__"

#includeenv:PTCNMSPORT:ptcnmsport.env
setenv PRO_COMM_MSG_EXE $CDRS/$MACHINE/obj/pro_comm_msg

#include:ptc_startup.inc

#
# Start cdrs manager process
#

$CDRS/$MACHINE/obj/dmgr $*

#end
