// 17-Mar-00  $$1    St.Patrick Created.
// 13-Sep-00  $$2    fix paths
// 23-Jan-01 $$3  TWH   Sun4_solaris_64 changes
// 14-Mar-01 $$4  TWH   Reorder IBM
// 15-Apr-03 K-01-06 $$5  TWH  sgi use LD_LIBRARYN32_PATH
// 25-OCT-05 K-03-34 $$6  MTP  sun_solaris_x64 support
//
if ($mc =~ sun*) then
  setenv LD_LIBRARY_PATH ${PTC_JRE}/$mc/lib/native_threads:${LD_LIBRARY_PATH}
endif
if ($mc =~ sgi*) then
  setenv LD_LIBRARYN32_PATH ${PTC_JRE}/$mc/lib/native_threads:${LD_LIBRARYN32_PATH}
endif
if ($mc == ibm_rs6000) then
// native_threads must be appended to the end because a lib
// within that directory conflicts with the X11 system.
  setenv LIBPATH ${LIBPATH}:${PTC_JRE}/$mc/lib/native_threads
endif

