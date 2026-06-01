# OPC UA SecurityCheck - Vordefinierte Test-Konfigurationen

Diese Datei enthält vordefinierte Kommandos für schnelle Tests verschiedener Szenarien.

## Windows Command Prompt

### Basis-Setup: Ungesicherter Server
```batch
REM Terminal 1: Server starten
dotnet run --project SecurityCheckServer.csproj -- --security-mode None

REM Terminal 2: Client ausführen
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

### Nur Signatur
```batch
REM Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --security-mode Sign

REM Terminal 2: Client
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

### Vollständige Verschlüsselung
```batch
REM Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --security-mode SignAndEncrypt --security-policy basic256sha256

REM Terminal 2: Client
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

### Auto-Accept Zertifikate (unsicher!)
```batch
REM Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --auto-accept --security-mode None

REM Terminal 2: Client
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

### Verschiedene Security Policy (Basic256)
```batch
REM Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --security-policy basic256 --security-mode SignAndEncrypt

REM Terminal 2: Client
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

### Verschiedene Security Policy (AES128-SHA256)
```batch
REM Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --security-policy aes128sha256rsaoaep --security-mode SignAndEncrypt

REM Terminal 2: Client
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

### Verschiedene Security Policy (AES256-SHA256)
```batch
REM Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --security-policy aes256sha256rsapss --security-mode SignAndEncrypt

REM Terminal 2: Client
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

### Anderer Port
```batch
REM Terminal 1: Server auf Port 5000
dotnet run --project SecurityCheckServer.csproj -- --port 5000

REM Terminal 2: Client auf Port 5000
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:5000"
```

## PowerShell

### Basis-Setup: Ungesicherter Server
```powershell
# Terminal 1: Server starten
dotnet run --project SecurityCheckServer.csproj -- --security-mode None

# Terminal 2: Client ausführen
dotnet run --project SecurityCheck.csproj -- "opc.tcp://localhost:4840"
```

### Nur Signatur
```powershell
# Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --security-mode Sign

# Terminal 2: Client
dotnet run --project SecurityCheck.csproj -- "opc.tcp://localhost:4840"
```

### Vollständige Verschlüsselung
```powershell
# Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --security-mode SignAndEncrypt --security-policy basic256sha256

# Terminal 2: Client
dotnet run --project SecurityCheck.csproj -- "opc.tcp://localhost:4840"
```

### Auto-Accept Zertifikate
```powershell
# Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --auto-accept --security-mode None

# Terminal 2: Client
dotnet run --project SecurityCheck.csproj -- "opc.tcp://localhost:4840"
```

## Bash/Linux

### Basis-Setup: Ungesicherter Server
```bash
# Terminal 1: Server starten
dotnet run --project SecurityCheckServer.csproj -- --security-mode None

# Terminal 2: Client ausführen
dotnet run --project SecurityCheck.csproj -- opc.tcp://localhost:4840
```

### Nur Signatur
```bash
# Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --security-mode Sign

# Terminal 2: Client
dotnet run --project SecurityCheck.csproj -- opc.tcp://localhost:4840
```

### Vollständige Verschlüsselung
```bash
# Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --security-mode SignAndEncrypt --security-policy basic256sha256

# Terminal 2: Client
dotnet run --project SecurityCheck.csproj -- opc.tcp://localhost:4840
```

## Erwartete Ausgaben nach Security Mode

### Mode: None
```
⚠️  WARNUNG: Keine Nachrichtenverschlüsselung!
```

### Mode: Sign
```
✓ Nachrichten sind signiert (Integritätsprüfung)
```

### Mode: SignAndEncrypt
```
✓ Nachrichten sind signiert und verschlüsselt
```

## Troubleshooting - Kommandos

### Port prüfen
```bash
# Windows
netstat -ano | findstr :4840

# Linux/Mac
lsof -i :4840
```

### Prozess killen (falls Server hängt)
```bash
# Windows
taskkill /IM dotnet.exe /F

# Linux/Mac
pkill -f dotnet
```

### .NET Version prüfen
```bash
dotnet --version
```

### Abhängigkeiten installieren
```bash
dotnet restore
```

### Projekt bereinigen
```bash
dotnet clean
```

### Build testen
```bash
dotnet build
```

## Performance-Test (optionale erweiterte Tests)

### Mit erhöhter Verbosity für Debugging
```bash
# Server
dotnet run --project SecurityCheckServer.csproj -- --security-mode SignAndEncrypt /v:d

# Client  
dotnet run --project SecurityCheck.csproj -- opc.tcp://localhost:4840
```

---

**Tipp:** Für ein vollständiges interaktives Test-Menü, nutze:
- Windows: `test-scenarios.bat`
- PowerShell: `.\test-scenarios.ps1`
