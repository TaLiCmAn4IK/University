// 21-Nov-03  K-01-24  ##1  TWH  Create
// 07-Apr-04  K-01-26  $$1  TWH  Drop oracle references
// 24-May-04  K-03-02  $$2  TWH  Add PDM_ORACLE_SERVER;fix TCLLIBPATH
//
//===============================================================
// Substitution list:
// __PROI_HOME__   - Proi_HOME loadpoint
// __NLS_LANG__    - Oracle NLS Lang value
// TCLLIBPATH value must be in braces else tcl assumes \t is tabs etc.
//            don't you just love it.
//===============================================================
REGEDIT4

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment]
"PROI_HOME"="__PROI_HOME__"
"PDM_ORA_APPL"="intralink"
"PDM_ORACLE_SERVER"="INTRALINK"
"LANGUAGE"="america"
"TCL_LIBRARY"="__PROI_HOME__\\tools\\lib"
"TCLLIBPATH"="{__PROI_HOME__\\tools\\tcl_api\\sysutils}"
"TCL_AUTO_PATH"="__PROI_HOME__\\tools\\tcl_api\\sysutils"

