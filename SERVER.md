# OPC UA Test Server

Ein OPC UA Server zum Testen des SecurityCheck-Client-Programms mit konfigurierbaren Sicherheitseinstellungen.

## Features

- ✓ Konfigurierbare Message Security Modes (None, Sign, SignAndEncrypt)
- ✓ Verschiedene Security Policies
- ✓ Auto-Accept Untrusted Certificates Option
- ✓ Kommandozeilen-Konfiguration
- ✓ Test-Knoten für Datenübertragung

## Kommandozeilen-Optionen

```
--port <Nummer>                   Port (Standard: 4840)
--security-mode <Modus>           None | Sign | SignAndEncrypt (Standard: SignAndEncrypt)
--security-policy <Policy>        none | basic128rsa15 | basic256 | basic256sha256 | 
                                  aes128sha256rsaoaep | aes256sha256rsapss 
                                  (Standard: basic256sha256)
--auto-accept                      Auto-Accept untrusted certificates
--help                             Hilfe anzeigen
```

## Beispiele

### 1. Server mit Standard-Einstellungen starten
```bash
dotnet run --project ./SecurityCheck.csproj -- --server
```

### 2. Server mit keiner Verschlüsselung (Test/Debugging)
```bash
dotnet run --project ./SecurityCheck.csproj -- --server --security-mode None
```

### 3. Server mit nur Signatur (Sign-only)
```bash
dotnet run --project ./SecurityCheck.csproj -- --server --security-mode Sign
```

### 4. Server mit vollständiger Verschlüsselung
```bash
dotnet run --project ./SecurityCheck.csproj -- --server --security-mode SignAndEncrypt
```

### 5. Server mit anderer Security Policy
```bash
dotnet run --project ./SecurityCheck.csproj -- --server --security-policy basic256
```

### 6. Server mit Auto-Accept für untrusted Certificates
```bash
dotnet run --project ./SecurityCheck.csproj -- --server --auto-accept --security-mode None
```

### 7. Server auf anderem Port
```bash
dotnet run --project ./SecurityCheck.csproj -- --server --port 5000
```

## Testszenarien

### Szenario 1: Ungesicherter Server (für Basis-Tests)
```bash
# Server
dotnet run --project ./SecurityCheck.csproj -- --server --security-mode None

# Client (in anderem Terminal)
dotnet run --project ./SecurityCheck.csproj "opc.tcp://localhost:4840"
```

**Erwartet:** Client zeigt `⚠️ WARNUNG: Keine Nachrichtenverschlüsselung!`

### Szenario 2: Signierter Server
```bash
# Server
dotnet run --project ./SecurityCheck.csproj -- --server --security-mode Sign

# Client
dotnet run --project ./SecurityCheck.csproj "opc.tcp://localhost:4840"
```

**Erwartet:** Client zeigt `✓ Nachrichten sind signiert`

### Szenario 3: Verschlüsselter Server
```bash
# Server
dotnet run --project ./SecurityCheck.csproj -- --server --security-mode SignAndEncrypt

# Client
dotnet run --project ./SecurityCheck.csproj "opc.tcp://localhost:4840"
```

**Erwartet:** Client zeigt `✓ Nachrichten sind signiert und verschlüsselt`

### Szenario 4: Auto-Accept Zertifikate
```bash
# Server
dotnet run --project ./SecurityCheck.csproj -- --server --auto-accept

# Client
dotnet run --project ./SecurityCheck.csproj "opc.tcp://localhost:4840"
```

**Erwartet:** Client zeigt Warnung bei Auto-Accept aktiviert

## Architektur

```
ServerProgram.cs
    ↓
ServerConfig.cs (Konfigurationsparameter)
    ↓
Server.cs (OpcUaTestServer + TestNodeManager)
    ↓
OPC UA Stack (Opc.Ua.Server)
```

## Fehlerbehebung

### Port bereits in Verwendung
```bash
# Alternativen Port verwenden
dotnet run --project ./SecurityCheck.csproj -- --server --port 5000
```

### Zertifikat-Fehler
- Stelle sicher, dass der OPC Foundation Certificate Store konfiguriert ist
- Verwende `--auto-accept` für Tests

### Server startet nicht
- Überprüfe die Firewall-Einstellungen
- Stelle sicher, dass der Port nicht bereits von anderen Prozessen genutzt wird

## Weiterführende Links

- [OPC Foundation](https://opcfoundation.org/)
- [OPC UA Security Policies](https://reference.opcfoundation.org/)
