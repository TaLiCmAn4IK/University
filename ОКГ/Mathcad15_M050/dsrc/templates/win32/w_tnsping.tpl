// 27-Mar-97 $$1  Pete  Created.
// 28-Jul-98 $$2  TWH   Rm dupelicate code
// 14-Aug-98 $$3  JJE  Support spaces in directories
// 02-Nov-98  $$4  JJE  Add ptc_end tag
// 04-Jan-01 $$5  TWH   Add %MC%\obj\ORACLE816\lib to path
// 12-Nov-01 $$6  TWH  Set to ORACLE817
// 28-Apr-03 K-01-06 $$7 TWH Add exit
// 09-Jul-03 K-01-10 $$8 TWH Add ORACLE_HOME
// 25-Sep-03 K-01-16 $$9 TWH Add TNS_ADMIN
// 12-May-04 K-03-02 $$10 TWH Chg to ORACLE920 dir name
// 18-Aug-04 K-03-09 $$11 TWH Chg path to Oracle920\bin
// 18-APR-05 K-03-23 $$12 MTP Use head_ilink
// 29-Jul-05 K-03-30 $$13 TWH Drop win95
// 22-Aug-07 K-03-50 $$14 TWH Chg path to ORACLE9208
// 17-Jan-08 L-01-42 $$15 TWH Chg path to ORACLE10203
#include:head_ilink.inc

set ORACLE_HOME=%PRO_DIRECTORY%\%MC%\obj\ORACLE10203
set TNS_ADMIN=%PRO_DIRECTORY%

set path=%path%;%ORACLE_HOME%\bin

"%PRO_DIRECTORY%\%MC%\obj\tnsping" %1 %2 %3 %4 %5 %6

:ptc_end
