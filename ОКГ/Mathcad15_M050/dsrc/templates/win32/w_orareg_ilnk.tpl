// 16-Sep-97       JJE  Created
// 12-Jan-98       YWA  Update to hex format
// 02-Mar-98       YWA  remove "NLS_LANG"="AMERICAN_AMERICA.WE8ISO8859P1"
//                      from ORACLE and ORACLE1
// 05-May-98       TWH  for Release 1.2 use lang dependent tpl files;
//                      many other changes
// 28-Jul-98  #1  JJE  Submit to I-01
// 01-Sep-98       TWH  Add TCL entries, fix ORACLE_HOME
// 08-Sep-98 I-01-18 JJE   #2   Group Submission
// 01-Sep-99 I-02-08 TWH   #3   Add Environment setting PROI_HOME
// 31-Mar-00 I4.0.0.28 APB #4   Changes for Oracle 816
// 14-Apr-00  $$1  TWH  Renamed from "w_orareg_c.tpl"
// 14-Apr-00  $$2  TWH  more changes for Oracle 816 release
// 12-Nov-01       TWH  Set to ORACLE817
// 19-Nov-01  $$3  TWH  Fix hex values to 817 also
// 28-May-02  $$4  TWH  drop NLS_LANG
//
//===============================================================
// Substitution list:
// __ORALP_ASCII__ - Oracle loadpoint in ascii chars
// __ORALP_HEX__   - Oracle loadpoint in comma separated hex format
// __PROI_HOME__   - Proi_HOME loadpoint
// __NLS_LANG__    - Oracle NLS Lang value
//===============================================================
REGEDIT4

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment]
"ORACLE_HOME"="__ORALP_ASCII__"
"PROI_HOME"="__PROI_HOME__"
"ORACLE_SID"="ilnk"
"LANGUAGE"="america"
"NLS_LANG"="__NLS_LANG__"
"TCL_LIBRARY"="__PROI_HOME__\\tools\\lib\\tcl"
"TK_LIBRARY"="__PROI_HOME__\\tools\\lib\\tk"
"TCL_AUTO_PATH"="__PROI_HOME__\\tools\\tcl_api\\sysutils"

[HKEY_LOCAL_MACHINE\SOFTWARE\ORACLE\PTC\INTRALINK\Ora817]
"ORACLE_HOME"=hex(2):__ORALP_HEX__00
"API"=hex(2):__ORALP_HEX__5c,44,42,53,00
"ORACLE_GROUP_NAME"=hex(2):4f,72,61,63,6c,65,20,2d,20,4f,72,61,38,31,37,00
"SQLPATH"=hex(2):__ORALP_HEX__5c,44,42,53,00
"ORACLE_HOME_KEY"=hex(2):53,6f,66,74,77,61,72,65,5c,4f,52,41,43,4c,45,5c,50,54,43,5c,49,4e,54,52,41,4c,49,4e,4b,5c,4f,72,61,38,31,37,00
"MSHELP_TOOLS"=hex(2):__ORALP_HEX__5c,4d,53,48,45,4c,50,00
"RDBMS_CONTROL"=hex(2):__ORALP_HEX__5c,44,41,54,41,42,41,53,45,00
"RDBMS_ARCHIVE"=hex(2):__ORALP_HEX__5c,44,41,54,41,42,41,53,45,5c,41,52,43,48,49,56,45,00
"ORACLE_SID"="ilnk"
