//05-Nov-97 $$1 JSP     Created
#include:head.inc

#include:shlib.inc

# ptc-cmdtype: prodcs

setenv IGC_PRINT XPRINT
setenv IGC_PATH $prodir/$mc/docviewer
setenv IGC_TMP /tmp
setenv XPPATH $IGC_PATH/xprinter

$IGC_PATH/myriad.exe $*
