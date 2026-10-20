// 4-Apr-00  I-03-27  #1   TWH  Created for Intralink mdimpex
// 7-Apr-00  I-03-27+ #2   TWH  Quote entity for hyphenated args
// 10-Apr-00 I-03-27+ ##1  TWH  fix typo (shlib.env -> shlib.inc)
// 16-Jan-00 J-01-25  ##2  MDA  fix for args. containing space
// 23-Jan-01 J-01-26+ ##3  TWH  sun4_solaris_64 changes
// 06-Sep-01 J-01-36+ ##4  TWH  add hpux* check
// 23-OCT-02 J-03-35  ##5  MTP  Added i486_linux
// 31-Dec-02 J-03-40  $$1  TWH  renamed from u_mdimpex.tpl
#include:head.inc

set params=""
foreach entity ($*:q)
if ("$entity" == "apdmpi_dll") then
  set params=($params:q apdmpi_dll.dll)
else
  set params=($params:q $entity:q)
endif
end

#include:ilinkdir.env

#include:shlib.inc

if ( $?PTC_ILLIB ) then
   if ($mc =~ sun4_solaris* || $mc == alpha_unix || $mc =~ sgi*  || $mc == i486_linux) then
      setenv LD_LIBRARY_PATH ${PTC_ILLIB}:$prodir/$mc/mdimpex:${LD_LIBRARY_PATH}
   endif
   if ($mc == sgi_mips4) then
      setenv LD_LIBRARYN32_PATH ${PTC_ILLIB}:$prodir/$mc/mdimpex:${LD_LIBRARYN32_PATH}
   endif

   if ($mc == hp700 || $mc == hp8k || $mc =~ hpux* ) then
      setenv SHLIB_PATH ${PTC_ILLIB}:$prodir/$mc/mdimpex:${SHLIB_PATH}
   endif

   if ($mc == ibm_rs6000) then
      setenv LIBPATH ${PTC_ILLIB}:$prodir/$mc/mdimpex:${LIBPATH}
   endif
else
   echo "Intralink Directory not setup. Please Define."
endif

if (! $?PDM_IMPEX_DLL_NAME) then
  setenv PDM_IMPEX_DLL_NAME apdmpi_dll.dll
endif

$prodir/$mc/obj/mdimpex $params:q

exit($status)

