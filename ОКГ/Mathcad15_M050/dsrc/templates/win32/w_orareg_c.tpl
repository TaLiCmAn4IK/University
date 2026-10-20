// 16-Sep-97       JJE  Created
// 12-Jan-98       YWA  Update to hex format
// 02-Mar-98       YWA  remove "NLS_LANG"="AMERICAN_AMERICA.WE8ISO8859P1"
//                      from ORACLE and ORACLE1
// 05-May-98       TWH  for Release 1.2 use lang dependent tpl files;
//                      many other changes
// 28-Jul-98  $$1  JJE  Submit to I-01
// 01-Sep-98       TWH  Add TCL entries, fix ORACLE_HOME
// 08-Sep-98 I-01-18 JJE   $$2   Group Submission
// 01-Sep-99 I-02-08 TWH   $$3   Add Environment setting PROI_HOME
//
//===============================================================
// Substitution list:
// __ORALP_ASCII__ - loadpoint in ascii chars
// __ORALP_HEX__   - loadpoint in comma separated hex format
// __NLS_LANG__    - Oracle NLS Lang value
//===============================================================
REGEDIT4

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment]
"ORACLE_HOME"="__ORALP_ASCII__"
"PROI_HOME"="__ORALP_ASCII__"
"ORACLE_SID"="ilnk"
"LANGUAGE"="america"
"NLS_LANG"="__NLS_LANG__"
"TCL_LIBRARY"="__ORALP_ASCII__\\intralink\\tools\\lib\\tcl"
"TK_LIBRARY"="__ORALP_ASCII__\\intralink\\tools\\lib\\tk"
"TCL_AUTO_PATH"="__ORALP_ASCII__\\intralink\\tools\\tcl_api\\sysutils"

[HKEY_LOCAL_MACHINE\SOFTWARE\ORACLE]
"ORACLE_HOME"=hex(2):__ORALP_HEX__00
"API"=hex(2):__ORALP_HEX__5c,44,42,53,00
"MSHELP"=hex(2):__ORALP_HEX__5c,6d,73,68,65,6c,70,00
"ORACLE_GROUP_NAME"=hex(2):4f,72,61,63,6c,65,20,66,6f,72,20,57,69,6e,64,6f,77,73,20,4e,54,00
"VS10"="__ORALP_ASCII__\\VS10"
"RDBMS73"=hex(2):__ORALP_HEX__5c,52,44,42,4d,53,37,33,00
"RDBMS73_CONTROL"=hex(2):__ORALP_HEX__5c,44,41,54,41,42,41,53,45,00
"RDBMS73_ARCHIVE"=hex(2):__ORALP_HEX__5c,44,41,54,41,42,41,53,45,5c,41,52,43,48,49,56,45,00
"ORA_XCPT"=hex(2):30,78,46,46,00
"ORACLE_SID"="ilnk"
"PLSQL23"=hex(2):__ORALP_HEX__5c,50,4c,53,51,4c,32,33,00
"PLSQL"=hex(2):__ORALP_HEX__5c,50,4c,53,51,4c,32,33,00
"MSHELP_TOOLS"=hex(2):__ORALP_HEX__5c,4d,53,48,45,4c,50,00
"NLS_LANG"="__NLS_LANG__"
"NLSRTL32"=hex(2):__ORALP_HEX__5c,4e,4c,53,52,54,4c,33,32,00 
"ORA_NLS32"=hex(2):__ORALP_HEX__5c,4e,4c,53,52,54,4c,33,32,5c,44,41,54,41,00
"ORA_ORCL_PWFILE"="__ORALP_ASCII__\\DATABASE\\PWDORCL.ORA"
"PRO22"="__ORALP_ASCII__\\PRO22"
"SQLPATH"=hex(2):__ORALP_HEX__5c,44,42,53,00
"PLUS33"=hex(2):__ORALP_HEX__5c,50,4c,55,53,33,33,00
"EXECUTE_SQL"=hex(2):50,4c,55,53,33,33,00
"PLUS_DFLT"=hex(2):50,4c,55,53,33,33,00

[HKEY_LOCAL_MACHINE\SOFTWARE\ORACLE\Misc_Reg_Entries]

[HKEY_LOCAL_MACHINE\SOFTWARE\ORACLE\ORACLE_HOMES]

[HKEY_LOCAL_MACHINE\SOFTWARE\ORACLE\ORACLE_HOMES\ORACLE1]
"ORACLE_HOME"=hex(2):__ORALP_HEX__,00
"API"=hex(2):__ORALP_HEX__5c,44,42,53,00
"MSHELP"=hex(2):__ORALP_HEX__5c,6d,73,68,65,6c,70,00
"NLS_LANG"="__NLS_LANG__"
"ORACLE_GROUP_NAME"="Oracle for Windows NT"

[HKEY_LOCAL_MACHINE\SOFTWARE\ORACLE\ORACLE_HOMES\ORACLE1\Misc_Reg_Entries]

[HKEY_LOCAL_MACHINE\SOFTWARE\ORACLE\SECURITY]
"ORA_ORCL_STARTMODE"="TRUE"

