// 27-Mar-97 $$1  Pete  Created.
// 21-Apr-98 $$2  YWA   change ORACLE to 733
// 18-Aug-98 $$3  TWH   should be ORACLE733/lib
// 22-Apr-99 $$4  TWH  Set ORA_NLS32 to oracle734 per echin
// 09-Sep-99 $$5  JJE  Check for any hp platform
// 23-Jan-01 $$6  TWH   Sun4_solaris_64 changes
if ($mc =~ sun4_solaris* || $mc == sgi_elf2 || $mc == alpha_unix) then
   if ($?LD_LIBRARY_PATH) then
      setenv LD_LIBRARY_PATH $prodir/$mc/obj/ORACLE734/lib:${LD_LIBRARY_PATH}
   else
      setenv LD_LIBRARY_PATH $prodir/$mc/obj/ORACLE734/lib
   endif
endif

if ($mc =~ hp*) then
   if ($?SHLIB_PATH) then
      setenv SHLIB_PATH $prodir/$mc/obj/ORACLE734/lib:${SHLIB_PATH}
   else
      setenv SHLIB_PATH $prodir/$mc/obj/ORACLE734/lib
   endif
endif

if ($mc == ibm_rs6000) then
   if ($?LIBPATH) then
      setenv LIBPATH $prodir/$mc/obj/ORACLE734/lib:${LIBPATH}
   else
      setenv LIBPATH $prodir/$mc/obj/ORACLE734/lib
   endif
endif

