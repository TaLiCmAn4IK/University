// 27-Mar-97 $$1  Pete  Created.
// 09-Feb-99 $$2  TWH   change $prodir -> $tkdir
// 05-May-00 $$3  TWH   Add pro_intlnk_tk/$mc/lib
// 04-Aug-00 $$4  TWH   change hp700 to hp*
// 23-Jan-01 $$5  TWH   Sun4_solaris_64 changes
// 11-May-01 $$6  TWH   Add mips4 LD_LIBRARYN32_PATH
// this must be included after shlib.inc

if ($mc =~ sun4_solaris*) then
    setenv LD_LIBRARY_PATH $tkdir/pro_intlnk_tk/$mc/obj:$tkdir/pro_intlnk_tk/$mc/lib:${LD_LIBRARY_PATH}
endif

if ($mc =~ hp*) then
    setenv SHLIB_PATH $tkdir/pro_intlnk_tk/$mc/obj:$tkdir/pro_intlnk_tk/$mc/lib:${SHLIB_PATH}
endif

if ($mc == ibm_rs6000) then
    setenv LIBPATH $tkdir/pro_intlnk_tk/$mc/obj:$tkdir/pro_intlnk_tk/$mc/lib:${LIBPATH}
endif

if ($mc == alpha_unix) then
    setenv LD_LIBRARY_PATH $tkdir/pro_intlnk_tk/$mc/obj:$tkdir/pro_intlnk_tk/$mc/lib:${LD_LIBRARY_PATH}
endif

if ($mc == "sgi_elf2") then
    setenv LD_LIBRARY_PATH $tkdir/pro_intlnk_tk/$mc/obj:$tkdir/pro_intlnk_tk/$mc/lib:${LD_LIBRARY_PATH}
endif

if ($mc == "sgi_mips4") then
    setenv LD_LIBRARYN32_PATH $tkdir/pro_intlnk_tk/$mc/obj:$tkdir/pro_intlnk_tk/$mc/lib:${LD_LIBRARYN32_PATH}
endif
