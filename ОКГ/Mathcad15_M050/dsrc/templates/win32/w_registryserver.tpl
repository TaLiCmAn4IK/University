// 24-Apr-00      MAZ  Created. (SPR 819658)
// 28-Sep-00 $$1  MAZ  Copied from I-03 to J-01
// 28-Apr-03 K-01-06 $$2 TWH Add exit
#include:head.inc

start "Registry" /min __WT_JVMCOMMAND__ -Dmode_list_file=modes.ini -Dregistry_path=%PRO_DIRECTORY%\cfg -DWORKGROUPMGR_HOME=%PRO_DIRECTORY%\logs -DSiteDir=%PRO_DIRECTORY%\codebase\cfg\site com.ptc.framework.registry.RegistryServer

exit /B %ERRORLEVEL%
