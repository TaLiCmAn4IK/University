// 26-Apr-00      JJE  Created.
// 28-Sep-00 $$1  MAZ  Copied from I-03 to J-01
#include:head.inc

setenv WT_HOME $prodir

__WT_JVMCOMMAND__ -classpath $prodir/codebase wt.manager.ServerLauncher
__WT_JVMCOMMAND__ -classpath $prodir/codebase wt.auth.Authentication

