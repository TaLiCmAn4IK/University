// 16-Jul-98 $$1  TWH   Created for use by Proe to append LIB settings
// 24-Jul-98 $$2  JJE   Append PTC_CVLIB
// 30-Jul-98 $$3  JJE   Set N32 path for sgi
// 30-Dec-98 $$4  TWH   Add $mc/lib for Pro/JAVA
// 25-Jun-99 $$5  JJE   Remove the prepending of mc/lib from #4
// 19-Jan-00 $$6  JJE   Xgl stubs for solaris 8
// 04-Aug-00 $$7  TWH   Change mc hp checks to hp*
// 23-Jan-01 $$8  TWH   Sun4_solaris_64 changes
// 24-Apr-01 $$9  TWH   mv #6 to shlib.inc
// 23-OCT-02 $$10 MTP   Added i486_linux
// 18-Dec-02 $$11 JJE   Change to lib64 for sgi
// 19-Dec-02 $$12 JJE   Set n32 also for elf4
// 14-Jan-03 J-03-40  ALG  $$13  SPR 927633: Use "libset" to set env var values
// 10-Oct-05 K-03-33  MAZ  $$14  SPR 1163063: add motif21 to SHLIB_PATH on HP
// 25-OCT-05 K-03-34  MTP  $$15  sun_solaris_x64 support
// 01-Nov-05 K-03-34+ MAZ  $$16 remove empty line after commments
if ( $?PTC_ILLIB ) then
   if ($mc =~ sun* || $mc == alpha_unix || $mc == i486_linux ) then
      if ($?libset) then
         setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH prepend $PTC_ILLIB`
         setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH append $prodir/$mc/deflib`
      else
         setenv LD_LIBRARY_PATH ${PTC_ILLIB}:${LD_LIBRARY_PATH}:$prodir/$mc/deflib
      endif
   endif

   if ($mc == sgi_elf4 ) then
      if ($?libset) then
         setenv LD_LIBRARY64_PATH `$libset LD_LIBRARY64_PATH prepend $PTC_ILLIB`
         setenv LD_LIBRARY64_PATH `$libset LD_LIBRARY64_PATH append $prodir/$mc/deflib`
      else
         setenv LD_LIBRARY64_PATH ${PTC_ILLIB}:${LD_LIBRARY64_PATH}:$prodir/$mc/deflib
      endif
   endif

   if ($mc =~ sgi* ) then
      if ($?libset) then
         setenv LD_LIBRARYN32_PATH `$libset LD_LIBRARYN32_PATH prepend $PTC_ILLIB`
         setenv LD_LIBRARYN32_PATH `$libset LD_LIBRARYN32_PATH append $prodir/$mc/deflib`
      else
         setenv LD_LIBRARYN32_PATH ${PTC_ILLIB}:${LD_LIBRARYN32_PATH}:$prodir/$mc/deflib
      endif
   endif

   if ($mc =~ hp* ) then
      if ($?libset) then
         setenv SHLIB_PATH `$libset SHLIB_PATH prepend $PTC_ILLIB`
         setenv SHLIB_PATH `$libset SHLIB_PATH append $prodir/$mc/deflib`
         setenv SHLIB_PATH `$libset SHLIB_PATH append $prodir/$mc/obj/JRE/lib/PA_RISC2.0/motif21`
      else
         setenv SHLIB_PATH ${PTC_ILLIB}:${SHLIB_PATH}:$prodir/$mc/deflib:$prodir/$mc/obj/JRE/lib/PA_RISC2.0/motif21
      endif
   endif

   if ($mc == ibm_rs6000) then
      if ($?libset) then
         setenv LIBPATH `$libset LIBPATH prepend $PTC_ILLIB`
         setenv LIBPATH `$libset LIBPATH append $prodir/$mc/deflib`
      else
         setenv LIBPATH ${PTC_ILLIB}:${LIBPATH}:$prodir/$mc/deflib
      endif
   endif
else
   if ($mc =~ sun* || $mc == alpha_unix || $mc == i486_linux ) then
      if ($?libset) then
         setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH append $prodir/$mc/deflib`
      else
         setenv LD_LIBRARY_PATH ${LD_LIBRARY_PATH}:$prodir/$mc/deflib
      endif
   endif

   if ($mc == sgi_elf4 ) then
      if ($?libset) then
         setenv LD_LIBRARY64_PATH `$libset LD_LIBRARY64_PATH append $prodir/$mc/deflib`
      else
         setenv LD_LIBRARY64_PATH ${LD_LIBRARY64_PATH}:$prodir/$mc/deflib
      endif
   endif

   if ($mc =~ sgi* ) then
      if ($?libset) then
         setenv LD_LIBRARYN32_PATH `$libset LD_LIBRARYN32_PATH append $prodir/$mc/deflib`
      else
         setenv LD_LIBRARYN32_PATH ${LD_LIBRARYN32_PATH}:$prodir/$mc/deflib
      endif
   endif

   if ($mc =~ hp* ) then
      if ($?libset) then
         setenv SHLIB_PATH `$libset SHLIB_PATH append $prodir/$mc/deflib`
         setenv SHLIB_PATH `$libset SHLIB_PATH append $prodir/$mc/obj/JRE/lib/PA_RISC2.0/motif21`
      else
         setenv SHLIB_PATH ${SHLIB_PATH}:$prodir/$mc/deflib:$prodir/$mc/obj/JRE/lib/PA_RISC2.0/motif21
      endif
   endif

   if ($mc == ibm_rs6000) then
      if ($?libset) then
         setenv LIBPATH `$libset LIBPATH append $prodir/$mc/deflib`
      else
         setenv LIBPATH ${LIBPATH}:$prodir/$mc/deflib
      endif
   endif
endif

if ( $?PTC_CVLIB ) then
   if ($mc =~ sun* || $mc == alpha_unix || $mc == i486_linux ) then
      if ($?libset) then
         setenv LD_LIBRARY_PATH `$libset LD_LIBRARY_PATH append $PTC_CVLIB`
      else
         setenv LD_LIBRARY_PATH ${LD_LIBRARY_PATH}:${PTC_CVLIB}
      endif
   endif

   if ($mc =~ sgi* ) then
      if ($?libset) then
         setenv LD_LIBRARY64_PATH `$libset LD_LIBRARY64_PATH append $PTC_CVLIB`
      else
         setenv LD_LIBRARY64_PATH ${LD_LIBRARY64_PATH}:${PTC_CVLIB}
      endif
   endif

   if ($mc =~ hp* ) then
      if ($?libset) then
         setenv SHLIB_PATH `$libset SHLIB_PATH append $PTC_CVLIB`
      else
         setenv SHLIB_PATH ${SHLIB_PATH}:${PTC_CVLIB}
      endif
   endif

   if ($mc == ibm_rs6000) then
      if ($?libset) then
         setenv LIBPATH `$libset LIBPATH append $PTC_CVLIB`
      else
         setenv LIBPATH ${LIBPATH}:${PTC_CVLIB}
      endif
   endif
endif
