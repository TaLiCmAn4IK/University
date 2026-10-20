// 10-Feb-99 $$1  MAZ  Created
#!/bin/csh -f
# =====================================
# name:    mfuname
# args:    none
# version: 2.0.2
# last modified: PMB 23/02/98
# created: CJS 30/09/93
# desc:    mfuname script to echo a system name denoting the
#          platform being run on.
#
# usage:   mfuname [ -s | -l | -b ]
#          Returns Moldflow system code.
#          -s option returns a more specific code based on
#          system and OS level. Currently only affects SGI, IBM.
#          -l option returns new long name (up to 5 chars).
#          -b option returns long name of build that supports this
#             platform.
# eg. SGI IRIX 6.3 is supported by SG62 build.  So on IRIX 6.3:
#          mfuname    --> sgi
#          mfuname -s --> s63
#          mfuname -l --> sg63
#          mfuname -b --> sg62
#
# Notes:   be careful changing this.  Other scripts depend on the
#          output of mfuname.
# =====================================

set oldcode = True
set sysos = False
set longname = False
set buildname = False

if ( $#argv == 1 ) then

  set oldcode = False

  if ( "$1" == "-s" ) then
    set sysos = True
  else if ( "$1" == "-l" ) then
    set longname = True
  else if ( "$1" == "-b" ) then
    set buildname = True
    set longname = True
  else
    echo "usage: mfuname [ -s | -l | -b ]"
    exit 1
  endif

endif

# test if uname is present
( set res = `uname` ) >& /dev/null

if ( $status == 0 ) then # have the uname command
   if (`uname` == "AIX") then
     set mach = `uname -n`
     set syst = `uname -s`
     set ver  = `uname -v`.`uname -r`
     set res = "${mach}:${syst}:${ver}"
   else
     set mach = `uname -m`
     set syst = `uname -s`
     set ver  = `uname -r`
     set res = "${mach}:${syst}:${ver}"
   endif
else
   # Removed obsolete APO and CVX code.

# at this stage, should have res set, if not, print error and exit
   if ( $?res == 0 ) then
      echo "ERROR:  Cannot determine cpu architecture"
      exit 1
   endif
endif

# now, test results of uname command to get architecture
switch ("$res")

  case "*IRIX*":

    if ( "$oldcode" == "True" ) then
      echo "sgi"
      exit 0
    endif

    if ( "$longname" == "True" ) then
      set bname = "NULL"
      switch ("$ver")
        case "5.3":
          set lname = sg53
          set bname = sg53
          breaksw
        case "6.1":
          set lname = sg61
          set bname = sg53
          breaksw
        case "6.2":
          set lname = sg62
          set bname = sg62
          breaksw
        case "6.3":
          set lname = sg63
          set bname = sg62
          breaksw
        case "6.4":
          set lname = sg64
          # Don't set bname here, as we want it to be derived later.
          # i.e. sg64l
          breaksw
      endsw

      # Modify for 64-bit machines - add an "l" to the end.
      switch ("$res")
        case "*IRIX64*":
          switch ("$ver")
            case "6.1":
            case "6.2":
            case "6.4":
              set lname = ${lname}l
              breaksw
          endsw
        breaksw
      endsw

      if ( "$bname" == "NULL" ) set bname = $lname

      if ( "$buildname" == "True" ) then
        echo "$bname"
        exit 0
      else
        echo "$lname"
        exit 0
      endif

    endif # /* longname == True */

    if ( "$sysos" == "True" ) then
    # Must be more specific...
    # Current possible machines are:
    # sg8 - Power 8000
    # s10 - Indigo2 IRIX64 R10000
    # s62 - R4x000 IRIX 6.2
    # s63 - O2 R5000 or R10000 IRIX 6.3
    # sgi - IRIX 5.3
    switch ("$res")
       case "*IRIX64*":
          if ("$ver" == "6.4" ) then
            echo "s64"
            exit 0
          endif
          # Determine chip using hinv command
          set chip = `/bin/hinv -t cpu | sed -e "s/ Processor Chip.*//" | sed -e "s/^.* //"`
          switch ("$chip")
             case "R8*":
                echo "sg8"
                exit 0
                breaksw
             case "R10*":
                echo "s10"
                exit 0
                breaksw
          endsw
          # Fallback for some cases where IRIX64 is installed on a R4K chip
          switch ("$ver")
                case "6.2":
                echo "s62"
                exit 0
                breaksw
             case "6.3":
                echo "s63"
                exit 0
                breaksw
             default:
                echo "sgi"
                exit 0
                breaksw
          endsw
          breaksw
       case "*IRIX:4*":
          # No longer supported - fall thru to error
          # echo "sg4"
          # exit 0
          breaksw
       case "*IRIX*":
          switch ("$ver")
             case "6.2":
                echo "s62"
                exit 0
                breaksw
             case "6.3":
                echo "s63"
                exit 0
                breaksw
             default:
                echo "sgi"
                exit 0
                breaksw
          endsw
          echo "PROGRAMMER ERROR"
          breaksw
    endsw
    endif # /* sysos == True */

    echo "PROGRAMMER ERROR"
    breaksw
  case "alpha:OSF1:1.*":
  case "alpha:OSF1:V2.*":
    # No longer supported - fall thru to error
    # echo "dco"
    # exit 0
    breaksw
  case "alpha:OSF1:V3*":
  case "alpha:OSF1:V4*":
    if ( "$oldcode" == "True" || "$sysos" == "True" ) then
      echo "dau"
      exit 0
    endif
    if ( "$longname" == "True" ) then
      set lname = "NULL"
      set bname = "NULL"
      switch ("$ver")
        case "V3.2*":
          set lname = du32g
          set bname = du32g
          breaksw
        case "V4.0*":
          set lname = du40b
          set bname = du40b
          breaksw
      endsw
      if ( "$lname" == "NULL" ) then
        breaksw     # This will fall thru to "OS level not supported"
      endif
      if ( "$bname" == "NULL" ) then
        set bname = $lname
      endif

      if ( "$buildname" == "True" ) then
        echo "$bname"
        exit 0
      else
        echo "$lname"
        exit 0
      endif

    endif
    echo "PROGRAMMER ERROR"
    breaksw
  case "sun4*:SunOS:[5-9]*":
    if ( "$oldcode" == "True" || "$sysos" == "True" ) then
      echo "so2"
      exit 0
    endif
    if ( "$longname" == "True" ) then
      set lname = "NULL"
      set bname = "NULL"
      switch ("$ver")
        case "5.6*":
          set lname = su26
          set bname = su251
          breaksw
        case "5.4*":
          set lname = su24
          set bname = su24
          breaksw
        case "5.5*":
          set lname = su251
          set bname = su251
          breaksw
      endsw
      if ( "$lname" == "NULL" ) then
        breaksw     # This will fall thru to "OS level not supported"
      endif
      if ( "$bname" == "NULL" ) then
        set bname = $lname
      endif

      if ( "$buildname" == "True" ) then
        echo "$bname"
        exit 0
      else
        echo "$lname"
        exit 0
      endif

    endif
    echo "PROGRAMMER ERROR"
    breaksw
  case "sun4*:SunOS:*":
    # No longer supported - fall thru to error
    # echo "sun"
    # exit 0
    breaksw
  # Removed unsupported DEC-ULTRIX and VAX-ULTRIX
  case "*:AIX:*":
    if ( "$oldcode" == "True" ) then
      echo "rs6"
      exit 0
    endif
    if ( "$longname" == "True" ) then
      set lname = "NULL"
      set bname = "NULL"
      switch ("$ver")
        case "3.2*":
          set lname = ib325
          set bname = ib325
          breaksw
        case "4.1*":
          set lname = ib41
          set bname = ib41
          breaksw
        case "4.2*":
          set lname = ib42
          set bname = ib41
          breaksw
      endsw

      if ( "$lname" == "NULL" ) then
        breaksw     # This will fall thru to "OS level not supported"
      endif
      if ( "$bname" == "NULL" ) then
        set bname = $lname
      endif

      if ( "$buildname" == "True" ) then
        echo "$bname"
        exit 0
      else
        echo "$lname"
        exit 0
      endif

    endif
    if ( "$sysos" == "True" ) then
      # Must be more specific...
      switch ("$res")
        case "*AIX:4*":
          echo "ia4"
          exit 0
          breaksw
        case "*AIX:3*":
          echo "rs6"
          exit 0
          breaksw
      endsw
      breaksw  # This will fall thru to "OS level not supported"
    endif
    echo "PROGRAMMER ERROR"
    breaksw

  # Removed obsolete 9000/3* 9000/4*
  case "*:HP-UX:*":
    if ( "$oldcode" == "True" || "$sysos" == "True" ) then
      switch ("$mach")
        case "9000/7*":
          echo "hp7"
          exit 0
          breaksw
        case "9000/8*":
          echo "hp8"
          exit 0
          breaksw
      endsw
    endif

    if ( "$longname" == "True" ) then
       set lname = "NULL"
       set bname = "NULL"
       switch ("$ver")
         case "*.09.01*":
           set lname = hp901
           set bname = hp907
           breaksw
         case "*.09.03*":
           set lname = hp903
           set bname = hp907
           breaksw
         case "*.09.05*":
           set lname = hp905
           set bname = hp907
           breaksw
         case "*.09.07*":
         case "*.09.*":
           set lname = hp907
           set bname = hp907
           breaksw
         case "*.10.0*":
           set lname = hp100
           set bname = hp907
           breaksw
         case "*.10.1*":
           set lname = hp101
           set bname = hp907
           breaksw
         case "*.10.2*":
           set lname = hp102
           set bname = hp102
           breaksw
         case "*.10.3*":
           set lname = hp103
           set bname = hp102
           breaksw
       endsw

       if ( "$lname" == "NULL" ) then
         breaksw     # This will fall thru to "OS level not supported"
       endif
       if ( "$bname" == "NULL" ) then
         set bname = $lname
       endif

       if ( "$buildname" == "True" ) then
         echo "$bname"
         exit 0
       else
         echo "$lname"
         exit 0
       endif

    endif    # /* longname == True */

    echo "PROGRAMMER ERROR"
    breaksw

  case "CRAY*":
    if ( "$oldcode" == "True" ) then
      echo "cry"
      exit 0
    endif
    if ( "$longname" == "True" ) then
       set lname = "NULL"
       set bname = "NULL"
       switch ("$mach")
         case "CRAY TS":
           set mname = ct
         case "CRAY C*":
         case "CRAY Y-MP":
         default:
           set mname = cr
           breaksw
       endsw
       switch ("$ver")
         case "8.0*":
           set lname = ${mname}80
           set bname = $lname
           breaksw
         case "9.0*":
           set lname = ${mname}90
           set bname = $lname
           breaksw
       endsw

       if ( "$lname" == "NULL" ) then
         breaksw     # This will fall thru to "OS level not supported"
       endif
       if ( "$bname" == "NULL" ) then
         set bname = $lname
       endif

       if ( "$buildname" == "True" ) then
         echo "$bname"
         exit 0
       else
         echo "$lname"
         exit 0
       endif

    endif    # /* longname == True */

    if ( "$sysos" == "True" ) then
      # Must be more specific...
      switch ("$res")
        case "*CRAY TS:*":
          echo "ct9"
          exit 0
          breaksw
        case "*:8*":
          echo "cry"
          exit 0
          breaksw
        # Removed obsolete cr7
      endsw
      breaksw  # This will fall thru to "OS level not supported"
    endif
    breaksw
  # Removed obsolete Intergraph CLIX
  default:
    echo "ERROR:  Cannot determine cpu architecture"
    exit 1
    breaksw
  endsw

  echo "ERROR:  OS level not supported"
  exit 1

exit 0
