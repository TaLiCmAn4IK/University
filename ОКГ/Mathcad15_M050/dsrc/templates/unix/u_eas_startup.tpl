// 19-Jan-99 I-01-31 TWH  Created
// 22-Apr-99               TWH  Set ORA_NLS32 to oracle734 per echin
// 05-May-99 I-01-35 JJE   $$1   Group Submission
// 22-Oct-99         JJE   $$2   Update from I-02
#include:head.inc

#include:shlib.inc

#include:ora_shlib.inc

set tkdir=__TKENV_LP__
#include:itk_shlib.inc

#includeenv:PTC_APPMGR_DIR:appmgrdir.env

setenv PDM_ORA_APPL intralink
setenv PDM_ORACLE_SERVER intralink
setenv TNS_ADMIN $tkdir 
setenv ORA_NLS32 $tkdir/$mc/obj/oracle734/ocommon/nls/admin/data
setenv NLS_LANG AMERICAN_AMERICA.WE8ISO8859P1

setenv ILINK_HOME $PRO_DIRECTORY

// user name for process
setenv EAS_USER __EAS_USER__
// path to and including cfg file
setenv IT_CONFIG_PATH ${PRO_DIRECTORY}/cfg/Orbix.cfg
// Broker db acct directory
setenv CS_ACCOUNT_PATH ${PRO_DIRECTORY}/admin
// Proj storage directory
setenv CS_ROOT_PATH __WORKSPACE_PROJECTS__

// mdimpex - in ilink client lp also.. reuse ?
setenv CS_IMPEX_PATH   ${PRO_DIRECTORY}/${mc}/obj/mdimpex

setenv PDM_ORACLE_SERVER $OracleServer

set NUM_OF_ENGS=__NUM_OF_ENGS__

#includeenv:ORBIX_DIRECTORY:orbix_dir.env
if (! $?ORBIX_DIRECTORY) then
  setenv ORBIX_DIRECTORY ${PRO_DIRECTORY}/orbix
endif

#
# Check if orbixd is running.
#
${ORBIX_DIRECTORY}/bin/pingit
if ($status != 0) then
  setenv IT_IMP_REP_PATH ${PRO_DIRECTORY}/ImpRep
  setenv IT_INT_REP_PATH ${PRO_DIRECTORY}/IntRep
  # Attempt to start orbixd.
  if ($?EAS_USER) then
    ${ORBIX_DIRECTORY}/bin/orbixd -t -u -o $EAS_USER &
  else
    ${ORBIX_DIRECTORY}/bin/orbixd -t -u &
  endif
  if ($status != 0) then
    echo "Could not start orbixd"
    exit 1
  endif
endif

#includeenv:LM_LICENSE_FILE:lmlic.env
setenv EAS_FEATURE_NAME "__EAS_FEATURE_NAME__"
setenv ILINK_TK_ACCESS_FEATURE_NAME "__ILINK_TK_ACCESS_FEATURE_NAME__"

#
# Check if CadBroker is registered and register if neccessary
#
${ORBIX_DIRECTORY}/bin/catit ${PRO_DIRECTORY}/${mc}/obj/cadbrk
if ($status != 0) then
  ${ORBIX_DIRECTORY}/bin/putit ${PRO_DIRECTORY}/${mc}/obj/cadbrk -persistent
endif

echo "Starting EAS Server..."
${PRO_DIRECTORY}/${mc}/obj/cadbrk $NUM_OF_ENGS ${PRO_DIRECTORY}/${mc}/obj/cadeng ${PRO_DIRECTORY}/${mc}/obj/cadproi


