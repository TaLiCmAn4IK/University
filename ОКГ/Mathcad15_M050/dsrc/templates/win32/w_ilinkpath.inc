// 22-Apr-99 $$1  JJE  Created.
// 14-Jul-99 $$2  MAZ  Always set the path -> SPR 766368
// 29-Jul-05 K-03-30 $$3 TWH Drop win95

if NOT exist "%INTRALINK_DIR%\%MC%\illib\proilws.dll" goto ptcdefpathset
  set path=%INTRALINK_DIR%\%MC%\illib;%path%
  goto ptcilibpathset
:ptcdefpathset
  set path=%path%;%PRO_DIRECTORY%\%MC%\deflib
:ptcilibpathset
