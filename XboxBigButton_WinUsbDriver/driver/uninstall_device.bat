@echo off

REM Print the path of the working directory to confirm that we're indeed inside the INSTDIR\Driver folder
cd

goto check_Permissions

:check_Permissions
    echo Administrative permissions required. Detecting permissions...

    net session >nul 2>&1
    if %errorLevel% == 0 (
        echo Success: Administrative permissions confirmed.
        goto uninstall
    ) else (
        echo Failure: Current permissions inadequate.
        goto fail
    )
 
:uninstall

REM START BY FINDING THE OEM INF FILE
setlocal EnableDelayedExpansion
SET OEM_FILE=
set oemdata="devcon.exe dp_enum"
FOR /F "eol=. tokens=*" %%a IN ( '%oemdata%' ) DO (
    set line=%%a
    set ourline=!line:Sverrir Sigmundarson=!
    if not !line!==!ourline! (
        SET OEM_FILE=!prev_line!
    )
    SET prev_line=%%a
)
echo Installed OEM file found as: !OEM_FILE!
setlocal DisableDelayedExpansion

REM REMOVE THE DEVICE
devcon.exe remove =media "*VID_045e&PID_02a0*"
if NOT errorlevel == 0 (
    echo Can not remove USB device, error %errorlevel%
    exit /b %errorlevel%
)
echo USB device successfully uninstalled from the system

:uninstall_delete_inf
IF [%OEM_FILE%] == [] (
    echo Could not locate OEM file installed. No INF to remove.
    goto success
)

pause >nul
devcon.exe dp_delete %OEM_FILE%
if NOT errorlevel == 0 (
    echo Can not delete the inf file named %OEM_FILE% from DriverStore, error %errorlevel%
    exit /b %errorlevel%
)
echo OEM file %OEM_FILE% successfully deleted from the DriverStore

:success
echo Success
exit /b 0

:fail
echo Failure
exit /b 100