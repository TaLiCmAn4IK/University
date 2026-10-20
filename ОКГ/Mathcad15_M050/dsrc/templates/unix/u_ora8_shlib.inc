// 17-Mar-00 $$1  TWH  Create from u_ora_shlib.inc
// 04-Dec-00  #2  TWH  Add sgi_mips4 section
// 05-Dec-00 $$2  TWH  fix #2
// 23-Jan-01 $$3  TWH   Sun4_solaris_64 changes
// 12-Nov-01 $$4  TWH  Set to ORACLE817
// 12-May-04 K-03-02      TWH  Set to ORACLE920
// 24-May-04 K-03-02 $$5  TWH  Add hp lib32 oracle path
// 22-Aug-07 K-03-50 $$6  TWH  Chg path to ORACLE9208
// 17-Jan-08 L-01-42 $$7  TWH  Chg path to ORACLE10203
//
if ($mc =~ sun4_solaris* || $mc == sgi_elf2 || $mc == alpha_unix) then
   if ($?LD_LIBRARY_PATH) then
      setenv LD_LIBRARY_PATH $prodir/$mc/obj/ORACLE10203/lib:${LD_LIBRARY_PATH}
   else
      setenv LD_LIBRARY_PATH $prodir/$mc/obj/ORACLE10203/lib
   endif
endif

if ($mc == sgi_mips4 ) then
   if ($?LD_LIBRARYN32_PATH) then
      setenv LD_LIBRARYN32_PATH $prodir/$mc/obj/ORACLE10203/lib:${LD_LIBRARYN32_PATH}
   else
      setenv LD_LIBRARYN32_PATH $prodir/$mc/obj/ORACLE10203/lib
   endif
endif

if ($mc =~ hp*) then
   if ($?SHLIB_PATH) then
      setenv SHLIB_PATH $prodir/$mc/obj/ORACLE10203/lib32:$prodir/$mc/obj/ORACLE10203/lib:${SHLIB_PATH}
   else
      setenv SHLIB_PATH $prodir/$mc/obj/ORACLE10203/lib32:$prodir/$mc/obj/ORACLE10203/lib
   endif
endif

if ($mc == ibm_rs6000) then
   if ($?LIBPATH) then
      setenv LIBPATH $prodir/$mc/obj/ORACLE10203/lib:${LIBPATH}
   else
      setenv LIBPATH $prodir/$mc/obj/ORACLE10203/lib
   endif
endif

