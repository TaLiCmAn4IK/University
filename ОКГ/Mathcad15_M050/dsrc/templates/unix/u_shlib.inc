// 27-Mar-97 $$1  Pete  Created.
// 03-Oct-97 $$2  JJE   Add SHLIB_PATH for hitachi
// 16-Jan-98 $$3  JJE   Add memmove for sun
// 05-Mar-98 $$4  JJE   Check LD_PRELOAD first
// 05-Mar-98 $$5  JJE   Set LD_PRELOAD differently
// 06-Apr-98 $$6  JJE   Change to sgi* for mips4
// 24-Apr-98 $$7  JJE   fix sun lib path so opengl will work
// 12-May-98 $$8  JJE   Remove libmul.o
// 30-Jul-98 $$9  JJE   Set n32 path on mips4
// 12-Aug-98 $$10 JJE   Add stublib for ibm
// 16-Sep-98 $$11 JJE   Don't set system lib path for ibm
// 03-Jun-99      JJE   Set opengl in libpath on hp
// 25-Jun-99      JJE   Rearrange libpath on sun and hp
// 27-Sep-99 $$12 JJE   Sub to new build area
// 04-Oct-99 $$13 JJE   Sun changes
// 08-Mar-00 $$14 JJE   No /usr/lib on HP
// 04-Aug-00 $$15 TWH   Change hp700|hp8k to hp*
// 10-Nov-00 $$16 JJE   Chg on Sun/Hp to make sure we put our lib dir early
// 23-Jan-01 $$17 TWH   Sun4_solaris_64 changes
// 24-Apr-01 $$18 TWH   Mv Xgl stubs for solaris 8 from shlibproe.inc
// 25-May-01 $$19 JJE   Don't use ucb paths on sun
// 23-OCT-02 $$20 MTP   Added i486_linux
// 18-Dec-02 $$21 JJE   Change to lib64 for sgi_elf4, n32 for mips4
// 19-Dec-02 $$22 JJE   Set n32 also for elf4
// 14-Jan-03 J-03-40  ALG  $$23  SPR 927633: Use "libset" to set env var values
// 25-OCT-05 K-03-34  MTP  $$24  sun_solaris_x64 support
// 01-Nov-05 K-03-34+ MAZ  $$25 remove empty line after commments
if ($mc =~ sun*) then
   if ($?LD_LIBRARY_PATH) then
      if ($?libset) then
         setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH prepend /usr/openwin/lib:$prodir/$mc/lib`
         setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH append $prodir/$mc/obj`
      else
         setenv LD_LIBRARY_PATH /usr/openwin/lib:$prodir/$mc/lib:${LD_LIBRARY_PATH}:$prodir/$mc/obj
      endif
   else
      setenv LD_LIBRARY_PATH /usr/openwin/lib:$prodir/$mc/lib:$prodir/$mc/obj
   endif
   if ( !(-e /opt/SUNWits/Graphics-sw/xgl/lib/libxgl.so) ) then
      set xgl_stub = "${PRO_DIRECTORY}/${PRO_MACHINE_TYPE}/xglstub"
      if ($?libset) then
         setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH prepend $xgl_stub`
      else
         setenv LD_LIBRARY_PATH ${xgl_stub}:${LD_LIBRARY_PATH}
      endif
   endif
   if (! $?TMPDIR) then
       setenv TMPDIR /tmp
   endif
endif

if ($mc =~ hp* || $mc == hitachi) then
   if ($?SHLIB_PATH) then
      if ($?libset) then
         setenv SHLIB_PATH `$libset SHLIB_PATH prepend /opt/graphics/OpenGL/lib:$prodir/$mc/lib`
         setenv SHLIB_PATH `$libset SHLIB_PATH append /usr/lib/X11R5:/opt/graphics/common/lib:$prodir/$mc/obj`
      else
         setenv SHLIB_PATH /opt/graphics/OpenGL/lib:$prodir/$mc/lib:${SHLIB_PATH}:/usr/lib/X11R5:/opt/graphics/common/lib:$prodir/$mc/obj
      endif
   else
      setenv SHLIB_PATH /opt/graphics/OpenGL/lib:/usr/lib/X11R5:/opt/graphics/common/lib:$prodir/$mc/lib:$prodir/$mc/obj
   endif
endif

if ($mc == ibm_rs6000) then
   if ($?LIBPATH) then
      if ($?libset) then
         setenv LIBPATH `$libset LIBPATH prepend $prodir/$mc/lib`
         setenv LIBPATH `$libset LIBPATH append $prodir/$mc/obj`
      else
         setenv LIBPATH $prodir/$mc/lib:${LIBPATH}:$prodir/$mc/obj
      endif
   else
      setenv LIBPATH $prodir/$mc/lib:$prodir/$mc/obj
   endif
   if ( ! -e /usr/lib/libGL.a ) then
      if ($?libset) then
         setenv LIBPATH `$libset LIBPATH append $prodir/$mc/stublib`
      else
         setenv LIBPATH ${LIBPATH}:$prodir/$mc/stublib
      endif
   endif
endif

if ($mc == alpha_unix) then
   if (! -e /usr/shlib/libGL.so) then
      if ($?LD_LIBRARY_PATH) then
         if ($?libset) then
            setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH append $prodir/$mc/obj`
         else
            setenv LD_LIBRARY_PATH ${LD_LIBRARY_PATH}:$prodir/$mc/obj
         endif
      else
         setenv LD_LIBRARY_PATH $prodir/$mc/obj
      endif
   endif
   set os_version = `uname -r | tr -d 'V' | awk -F. '{print $1}'`
   if ($os_version < 4 ) then
      if ($?LD_LIBRARY_PATH) then
         if ($?libset) then
            setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH prepend $prodir/$mc/libcxx`
         else
            setenv LD_LIBRARY_PATH $prodir/$mc/libcxx:${LD_LIBRARY_PATH}
         endif
      else
         setenv LD_LIBRARY_PATH $prodir/$mc/libcxx
      endif
   endif
   if ($?LD_LIBRARY_PATH) then
      if ($?libset) then
         setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH prepend $prodir/$mc/lib`
      else
         setenv LD_LIBRARY_PATH $prodir/$mc/lib:${LD_LIBRARY_PATH}
      endif
   else
      setenv LD_LIBRARY_PATH $prodir/$mc/lib
   endif
endif

if ($mc == i486_linux) then
   if ($?LD_LIBRARY_PATH) then
      if ($?libset) then
         setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH prepend $prodir/$mc/lib`
         setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH append $prodir/$mc/obj`
      else
         setenv LD_LIBRARY_PATH $prodir/$mc/lib:${LD_LIBRARY_PATH}:$prodir/$mc/obj
      endif
   else
      setenv LD_LIBRARY_PATH $prodir/$mc/lib:$prodir/$mc/obj
   endif
endif

if ($mc == sgi_elf4) then
   if ($?LD_LIBRARY64_PATH) then
      if ($?libset) then
         setenv LD_LIBRARY64_PATH `$libset LD_LIBRARY64_PATH prepend $prodir/$mc/lib`
         setenv LD_LIBRARY64_PATH `$libset LD_LIBRARY64_PATH append $prodir/$mc/obj`
      else
         setenv LD_LIBRARY64_PATH $prodir/$mc/lib:${LD_LIBRARY64_PATH}:$prodir/$mc/obj
      endif
   else
      setenv LD_LIBRARY64_PATH $prodir/$mc/lib:$prodir/$mc/obj
   endif
endif

if ($mc =~ sgi* ) then
   if ($?LD_LIBRARYN32_PATH) then
      if ($?libset) then
         setenv LD_LIBRARYN32_PATH `$libset LD_LIBRARYN32_PATH prepend $prodir/$mc/lib`
         setenv LD_LIBRARYN32_PATH `$libset LD_LIBRARYN32_PATH append $prodir/$mc/obj`
      else
         setenv LD_LIBRARYN32_PATH $prodir/$mc/lib:${LD_LIBRARYN32_PATH}:$prodir/$mc/obj
      endif
   else
      setenv LD_LIBRARYN32_PATH $prodir/$mc/lib:$prodir/$mc/obj
   endif
endif
