// 19-Dec-03  K-01-21  $$1 MAZ  SPR 1067044: Create w_proe_borrow 
#include:head.inc

%start_cmd% "%PRODIR%\bin\proe.exe" -borrow %*
exit /B %ERRORLEVEL%
