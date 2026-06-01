# OPC UA SecurityCheck Test-Szenarien (PowerShell)

function Test-Scenario {
    param(
        [string]$SecurityMode,
        [string]$SecurityPolicy,
        [string]$Description,
        [switch]$AutoAccept
    )

    Write-Host ""
    Write-Host "=== Starte Szenario: $Description ===" -ForegroundColor Green
    Write-Host "SecurityMode: $SecurityMode"
    Write-Host "SecurityPolicy: $SecurityPolicy"
    if ($AutoAccept) {
        Write-Host "AutoAccept: true"
    }
    Write-Host ""

    # Server starten
    $serverArgs = @(
        "run",
        "--project", "SecurityCheckServer.csproj",
        "--",
        "--security-mode", $SecurityMode,
        "--security-policy", $SecurityPolicy
    )

    if ($AutoAccept) {
        $serverArgs += "--auto-accept"
    }

    Write-Host "Starte Server..."
    $serverProcess = Start-Process -FilePath "dotnet" `
        -ArgumentList $serverArgs `
        -WindowStyle Normal `
        -PassThru `
        -NoNewWindow:$false

    Start-Sleep -Seconds 3

    Write-Host "Starte Client..."
    $clientProcess = Start-Process -FilePath "dotnet" `
        -ArgumentList @(
        "run",
        "--project", "SecurityCheck.csproj",
        "--",
        "opc.tcp://localhost:4840"
    ) `
        -WindowStyle Normal `
        -PassThru

    Write-Host ""
    Write-Host "Beide Prozesse laufen. Drücke eine Taste um zu beenden..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

    # Stoppe Prozesse
    Stop-Process -Id $serverProcess.Id -Force -ErrorAction SilentlyContinue
    Stop-Process -Id $clientProcess.Id -Force -ErrorAction SilentlyContinue

    Write-Host "Test beendet." -ForegroundColor Green
}

function Show-Menu {
    Write-Host ""
    Write-Host "=== OPC UA SecurityCheck - Test-Szenarien ===" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "1) Ungesicherter Server (SecurityMode: None)" -ForegroundColor White
    Write-Host "2) Signierter Server (SecurityMode: Sign)" -ForegroundColor White
    Write-Host "3) Verschluesselter Server (SecurityMode: SignAndEncrypt)" -ForegroundColor White
    Write-Host "4) Custom: Mit auto-accept Zertifikaten" -ForegroundColor White
    Write-Host "5) Custom: Mit anderer Security Policy (Basic256)" -ForegroundColor White
    Write-Host "6) Beende Programm" -ForegroundColor White
    Write-Host ""
}

# Hauptschleife
$exitLoop = $false
while (-not $exitLoop) {
    Show-Menu
    $choice = Read-Host "Waehle ein Szenario (1-6)"

    switch ($choice) {
        "1" {
            Test-Scenario -SecurityMode "None" -SecurityPolicy "basic256sha256" -Description "Ungesicherter Server (SecurityMode: None)"
        }
        "2" {
            Test-Scenario -SecurityMode "Sign" -SecurityPolicy "basic256sha256" -Description "Signierter Server (SecurityMode: Sign)"
        }
        "3" {
            Test-Scenario -SecurityMode "SignAndEncrypt" -SecurityPolicy "basic256sha256" -Description "Verschluesselter Server (SecurityMode: SignAndEncrypt)"
        }
        "4" {
            Test-Scenario -SecurityMode "SignAndEncrypt" -SecurityPolicy "basic256sha256" -Description "Custom: Mit auto-accept Zertifikaten" -AutoAccept
        }
        "5" {
            Test-Scenario -SecurityMode "SignAndEncrypt" -SecurityPolicy "basic256" -Description "Custom: Mit anderer Security Policy (Basic256)"
        }
        "6" {
            $exitLoop = $true
            Write-Host "Auf Wiedersehen!" -ForegroundColor Yellow
        }
        default {
            Write-Host "Ungueltige Eingabe! Bitte versuche es erneut." -ForegroundColor Red
        }
    }
}
