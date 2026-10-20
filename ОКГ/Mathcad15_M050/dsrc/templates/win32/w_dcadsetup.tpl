// 17-Feb-00       JJE  Created
// 15-Mar-00  $$1  JJE  Fixed
// 24-Mar-00  $$2  JJE  Add temp path
// 11-Apr-00       JJE  Changes
// 20-Apr-00       JJE  Add dcad flag
// 02-May-00  $$3  JJE  Use timeout of 500
// 26-Jun-00  $$4  prf  Used -env to check compatibility
// 21-Aug-00       JJE  Fix formatting
// 01-Nov-00  $$5  JJE  Sub changes to J-01
// 06-Feb-01  $$6  prf  Add cadds5atb jobman to PTCNMS_SERVICE_LIST
// 13-Mar-01  $$7  prf  Add DISABLE_PRELOAD_PRO_CADDS5 setting
// 19-Feb-02  ALG  $$8  SPR 925595: Fix spelling of "succeeded"
// 05-Mar-02  ALG  $$9  Add service dbatchs
// 24-Sep-02 J-03-35  ALG  $$10  SPR 977187: Add "-status" argument
// 28-Apr-03 K-01-06 $$11 TWH Add exit
#include:head.inc
#includeenv:DCAD_TEMP:dcadtemp.env
#includeenv:PROE_START:proe_start.env

set DISABLE_PRELOAD_PRO_CADDS5=yes

set PRO_COMM_MSG_EXE=%prodir%\%mc%\obj\pro_comm_msg.exe

set nmsdir=%PRO_DIRECTORY%\%MC%\nms

if NOT "%1" == "-status" goto nmsd_notstatus

	"%nmsdir%\nmsq.exe" -quiet -ping

	if NOT ERRORLEVEL 1 goto nmsd_status

		echo Could not connect to Name Service Daemon.
		goto ptc_end

	:nmsd_status

		"%nmsdir%\nmsq.exe" -quiet -hostinfo

		echo The following remote services are enabled:
		"%nmsdir%\nmsq.exe" -quiet -servinfo

		echo Environment settings related to dbatchs:
		"%nmsdir%\nmsq.exe" -env PTCNMS_SERVICE_DIR PROE_START PROE_STARTUP_EXE PROE_STARTUP_PSF DCAD_TEMP

		goto ptc_end

:nmsd_notstatus

if "%PTCNMS_SERVICE_LIST%" == "" set PTCNMS_SERVICE_LIST=dsserver cadds5atb jobman dbatchs
if "%NMSD_TIMEOUT%" == "" set NMSD_TIMEOUT=500

"%nmsdir%\nmsq.exe" -quiet -env PROE_START=%PROE_START%

if NOT ERRORLEVEL 1 goto nmsd_on

	"%nmsdir%\nmsq.exe" -quiet -ping

	if NOT ERRORLEVEL 1 goto nmsd_frc

		goto nmsd_sta

	:nmsd_frc

		if "%1" == "-force" goto nmsd_sto

			echo DCAD setup failed. Incompatible Name Service Daemon is running.
			echo 1. Exit all PTC applications.
			echo 2. Run %0 -force
			goto ptc_end

		:nmsd_sto

			echo Shutting down Name Service Daemon ...
			"%nmsdir%\nmsq.exe" -quiet -shutdown
			goto nmsd_sta

:nmsd_on

	"%nmsdir%\nmsq.exe" -quiet -on %PTCNMS_SERVICE_LIST%
	goto nmsd_ok

:nmsd_sta

	echo Starting Name Service Daemon ...
	%start_cmd% /min "%nmsdir%\nmsd.exe" -noservice -timeout %NMSD_TIMEOUT% -dcad

:nmsd_ok

echo DCAD setup succeeded. The following remote services are enabled:
"%nmsdir%\nmsq.exe" -quiet -servinfo
:ptc_end
exit /B %ERRORLEVEL%

