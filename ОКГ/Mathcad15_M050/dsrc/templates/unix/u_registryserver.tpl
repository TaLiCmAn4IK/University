// 24-Apr-00      MAZ  Created.
// 28-Sep-00 $$1  MAZ  Copied from I-03 to J-01
#include:head.inc

setenv WT_HOME $prodir
setenv CLASSPATH $prodir/codebase

__WT_JVMCOMMAND__ -DWORKGROUPMGR_HOME=$prodir/logs -Dmode_list_file=modes.ini -Dregistry_path=$prodir/cfg -DSiteDir=$prodir/codebase/cfg/site com.ptc.framework.registry.RegistryServer

