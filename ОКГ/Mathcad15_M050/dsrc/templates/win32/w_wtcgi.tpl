// 24-Aug-99 $$1   JJE  Created
// 25-Apr-00 $$2   JJE  Changes for R5
#
# This file is read by the CGI programs located in the windchill CGI directory.
#

# This section defines the CGI executables that read this .ini file along with the class
# that is to be called by that program.

{KEYS}

wtcgi      "wt.httpgw.HTTPGatewayMain"
wtauthcgi  "wt.httpgw.HTTPGatewayMain"
wtsacgi    "wt.sysadm.SysAdminMain"
wtindexcgi "wt.indexloader.IndexLoaderMain"


# This section defines the java command and optional parameters that are to be used by
# the CGI program indicated by the 'key' (first field).

{COMMANDS}

wtcgi       __WT_JVMCOMMAND__  -noverify -classpath __LOADPOINT__\codebase;__WT_JVMHOMEDIR__\jre\lib\rt.jar
wtauthcgi   __WT_JVMCOMMAND__  -noverify -classpath __LOADPOINT__\codebase;__WT_JVMHOMEDIR__\jre\lib\rt.jar
wtsacgi     __WT_JVMCOMMAND__  -noverify -classpath __LOADPOINT__\codebase;__WT_JVMHOMEDIR__\jre\lib\rt.jar
wtindexcgi  __WT_JVMCOMMAND__  -noverify -classpath __LOADPOINT__\codebase;__WT_JVMHOMEDIR__\jre\lib\rt.jar
