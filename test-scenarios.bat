@echo off
REM OPC UA SecurityCheck Test-Szenarien
REM Startet verschiedene Test-Kombinationen von Server und Client

echo.
echo === OPC UA SecurityCheck - Test-Szenarien ===
echo.
echo Waehle ein Test-Szenario aus:
echo.
echo 1) Ungesicherter Server (SecurityMode: None)
echo 2) Signierter Server (SecurityMode: Sign)
echo 3) Verschluesselter Server (SecurityMode: SignAndEncrypt)
echo 4) Custom: Mit auto-accept Zertifikaten
echo 5) Custom: Mit anderer Security Policy (Basic256)
echo.
set /p choice="Eingabe (1-5): "

if "%choice%"=="1" (
    call :test_scenario "None" "basic256sha256" "No encryption test"
) else if "%choice%"=="2" (
    call :test_scenario "Sign" "basic256sha256" "Sign-only test"
) else if "%choice%"=="3" (
    call :test_scenario "SignAndEncrypt" "basic256sha256" "Full encryption test"
) else if "%choice%"=="4" (
    call :test_scenario "SignAndEncrypt" "basic256sha256" "Auto-accept test" "true"
) else if "%choice%"=="5" (
    call :test_scenario "SignAndEncrypt" "basic256" "Custom policy test"
) else (
    echo Ungueltige Eingabe!
    exit /b 1
)

exit /b 0

:test_scenario
setlocal enabledelayedexpansion
set "security_mode=%~1"
set "security_policy=%~2"
set "description=%~3"
set "auto_accept=%~4"

echo.
echo === Starte Szenario: %description% ===
echo SecurityMode: %security_mode%
echo SecurityPolicy: %security_policy%
if "%auto_accept%"=="true" (
    echo AutoAccept: true
)
echo.
echo Starte Server in neuem Fenster...

if "%auto_accept%"=="true" (
    start "OPC UA Test Server" cmd /k "dotnet run --project SecurityCheckServer.csproj -- --security-mode %security_mode% --security-policy %security_policy% --auto-accept"
) else (
    start "OPC UA Test Server" cmd /k "dotnet run --project SecurityCheckServer.csproj -- --security-mode %security_mode% --security-policy %security_policy%"
)

timeout /t 3 /nobreak

echo Starte Client in neuem Fenster...
start "OPC UA Security Check Client" cmd /k "dotnet run --project SecurityCheck.csproj -- opc.tcp://localhost:4840"

echo.
echo Test-Szenario wird ausgefuehrt. Schliesse die Fenster um zu beenden.
echo.

endlocal
exit /b 0
