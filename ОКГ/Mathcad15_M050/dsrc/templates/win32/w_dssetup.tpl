// 23-Jul-03 K-01-11   MAZ  $$1   SPR 1025041: copy from dcadsetup
// 01-Oct-03 K-01-16   MAZ  $$2   Sub __LOADPOINT__ and __ARCH__
// 19-Jan-04 K-01-22   MAZ  $$3   SPR 1069176: env vars for ds_protoolkit_svc
// 
@echo off

rem User Input Area: Please customize the following variables
rem                  according to your setup.

rem PTCNMS_SERVICE_DIR is the path to the DS services directory.
rem    Only one directory is allowed in this variable. All service
rem    executables need to be located in this directory.

rem PTCNMS_SERVICE_LIST is a space delimited list of DS service names.
rem    Do not quote the list if it contains more than one item.

rem The following environment variables are required for the 
rem DS sample Pro/TOOLKIT service (ds_protoolkit_svc). If they
rem are not set, the sample Pro/TOOLKIT service will not be available.
rem Uncomment and modify the following two lines to use ds_protoolkit_svc
rem if "%PROE_START%" == "" set PROE_START=<PROE_LOADPOINT>\bin\<PROE_BATCH_FILE.bat>
rem if "%PRO_COMM_MSG_EXE%" == "" set PRO_COMM_MSG_EXE=<PROE_LOADPOINT>\i486_nt\obj\pro_comm_msg.exe

rem You can set these variables outside of the batch file in your command shell
if "%PTCNMS_SERVICE_DIR%" == "" set PTCNMS_SERVICE_DIR=__LOADPOINT__\ds_api\c_api\__ARCH__\obj
if "%PTCNMS_SERVICE_LIST%" == "" set PTCNMS_SERVICE_LIST=ds_simple_svc ds_protoolkit_svc


rem WARNING: please do not edit the script content below.


#include:head.inc

set nmsdir=%PRO_DIRECTORY%\%MC%\nms

if NOT "%1" == "-status" goto nmsd_notstatus

	"%nmsdir%\nmsq.exe" -quiet -ping

	if NOT ERRORLEVEL 1 goto nmsd_status

		echo Could not connect to Name Service Daemon.
		goto ptc_end

	:nmsd_status

		"%nmsdir%\nmsq.exe" -quiet -hostinfo

		echo The following remote services are registered:
		"%nmsdir%\nmsq.exe" -quiet -servinfo

		goto ptc_end

:nmsd_notstatus

"%nmsdir%\nmsq.exe" -quiet -env PTCNMS_SERVICE_LIST=%PTCNMS_SERVICE_LIST%

if NOT ERRORLEVEL 1 goto nmsd_on

	"%nmsdir%\nmsq.exe" -quiet -ping

	if NOT ERRORLEVEL 1 goto nmsd_frc

		goto nmsd_sta

	:nmsd_frc

		if "%1" == "-force" goto nmsd_sto

			echo DS setup failed. Incompatible Name Service Daemon is running.
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

if "%NMSD_TIMEOUT%" == "" set NMSD_TIMEOUT=500
   echo Starting Name Service Daemon ...
   %start_cmd% /min "%nmsdir%\nmsd.exe" -noservice -timeout %NMSD_TIMEOUT% -dcad

:nmsd_ok

echo DS setup succeeded. The following remote services are enabled:
"%nmsdir%\nmsq.exe" -quiet -servinfo
:ptc_end
exit /B %ERRORLEVEL%

