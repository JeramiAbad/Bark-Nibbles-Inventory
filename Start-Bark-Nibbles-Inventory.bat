@echo off
setlocal
set "APP_DIR=%~dp0"
if exist "%APP_DIR%BarkNibblesInventory.exe" goto :run

for /r "%APP_DIR%" %%F in (BarkNibblesInventory.exe) do (
    if exist "%%~fF" (
        set "APP_DIR=%%~dpF"
        goto :run
    )
)

echo ERROR: BarkNibblesInventory.exe was not found.
echo Extract all files from the ZIP, then run this batch file from the extracted folder.
pause
exit /b 1

:run
if "%~1"=="" (set "APP_PORT=5128") else (set "APP_PORT=%~1")
cd /d "%APP_DIR%"
if errorlevel 1 (
    echo ERROR: Cannot open the application folder: "%APP_DIR%"
    pause
    exit /b 1
)

echo Open http://localhost:%APP_PORT% on this PC.
echo Other devices on the same Wi-Fi can use http://^<host-PC-IPv4^>:%APP_PORT%.
"%APP_DIR%BarkNibblesInventory.exe" --urls "http://0.0.0.0:%APP_PORT%"
set "APP_EXIT_CODE=%ERRORLEVEL%"
if not "%APP_EXIT_CODE%"=="0" echo The app exited with code %APP_EXIT_CODE%.
pause
exit /b %APP_EXIT_CODE%
