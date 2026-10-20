// 26-May-99      JJE  Created.
// 16-Jun-99      JJE  Add quotes
// 27-Jul-99      JJE  Changes
// 26-Aug-99      JJE  Changes per rembold
// 27-Sep-99 $$1  JJE   Submit
// 23-Jan-01 $$2  TWH   Sun4_solaris_64 changes
// 29-Jan-02 $$3  MAZ   Use new script from Jeff
// 13-Mar-02 $$4  MAZ  Add change from Ben Fachner
// 16-Oct-02 $$5  AKG  Modified SHLIB_PATH for HP_UX #974653
// 16-OCT-02      SNK  Added argument "-Xoss4m" for SunOS 
// 30-MAR-04  K-01-26  $$6  SNK  Modified LD_LIBRARY_PATH and LIBPATH
// 30-OCT-06  L-01-19  $$7 ksamanta Updated for Vericut 6
// 08-NOV-06  L-01-19+ $$8 ksamanta Typo fix
#include:head.inc
#include:sun4_tmpdir.inc

setenv PRO_COMM_MSG_EXE $prodir/$mc/obj/pro_comm_msg

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv VERICUT_FEATURE_NAME Embedded

setenv CGTECH_INSTALL $PRO_DIRECTORY

setenv CGTECH_PRODUCTS $CGTECH_INSTALL/$MC/vericut
setenv CGTECH_CLASSES $CGTECH_PRODUCTS
if ( $?CGTECH_LOCALE == "0") then
   setenv CGTECH_LOCALE english
endif
setenv CGTECH_LANGUAGE $CGTECH_CLASSES/$CGTECH_LOCALE
setenv CGTECH_DEFAULT_UNITS INCH
setenv CGTECH_RGB_TXT $CGTECH_PRODUCTS/rgbhues.txt
setenv CGTECH_BROWSER netscape

setenv CGTECH_LIBRARY $CGTECH_INSTALL/text/vericut/library/
setenv CGTECH_SAMPLES $CGTECH_INSTALL/text/vericut/samples/
setenv CGTECH_HELP $CGTECH_INSTALL/html/usascii/vericut

if ( `uname` == "HP-UX" ) then
   if ( $?SHLIB_PATH ) then
      setenv SHLIB_PATH $CGTECH_INSTALL/$MC/obj:$SHLIB_PATH
   else
      setenv SHLIB_PATH $CGTECH_INSTALL/$MC/obj
   endif
  setenv LC_CTYPE C
else if ( `uname` == "IRIX" || `uname` == "IRIX64" ) then
  unset SGI_ABI
  if ( $?LD_LIBRARYN32_PATH == "1") then
     setenv LD_LIBRARYN32_PATH ${CGTECH_PRODUCTS}:${CGTECH_INSTALL}/${MC}/obj:$LD_LIBRARYN32_PATH
  else if ( $?LD_LIBRARY_PATH == "1") then
     setenv LD_LIBRARY_PATH ${CGTECH_PRODUCTS}:${CGTECH_INSTALL}/${MC}/obj:$LD_LIBRARY_PATH
  endif
  setenv LC_CTYPE C
else if ( `uname` == "SunOS" ) then
  setenv LD_LIBRARY_PATH ${CGTECH_PRODUCTS}:${CGTECH_INSTALL}/${MC}/obj:$LD_LIBRARY_PATH
  setenv LC_CTYPE C
else if ( `uname` == "AIX" ) then
  setenv LIBPATH ${CGTECH_PRODUCTS}:${CGTECH_INSTALL}/${MC}/obj:$LIBPATH
  setenv LANG C
endif

if ( $?CGTECH_PATH != "0" ) then
  setenv PATH $CGTECH_PATH
endif

java -fullversion
if ( `uname` == "SunOS" ) then
  java -Xms24m -Xmx128m -Xss6m -Xoss4m -classpath "${CGTECH_CLASSES}:$CGTECH_CLASSES/CGTech.jar" Vericut $*
else
  java -Xms24m -Xmx128m -Xss6m -classpath "${CGTECH_CLASSES}:$CGTECH_CLASSES/CGTech.jar" Vericut $*
endif
