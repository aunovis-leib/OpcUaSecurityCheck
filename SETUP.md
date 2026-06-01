# OPC UA SecurityCheck - Vollständiges Test-Setup

Ein komplettes C# .NET 10 Projekt zum Testen der OPC UA Security-Parameter mit konfigurierbarem Server und analytischem Client.

## 📁 Projektstruktur

```
SecurityCheck/
├── SecurityCheck.csproj           # Client-Projekt
├── SecurityCheckServer.csproj     # Server-Projekt
├── Program.cs                     # Client-Einstiegspunkt
├── ServerProgram.cs               # Server-Einstiegspunkt
├── Server.cs                      # OPC UA Server Implementation
├── ServerConfig.cs                # Konfigurationslogik
├── README.md                      # Diese Datei
├── SERVER.md                      # Server-Dokumentation
├── test-scenarios.bat             # Windows Batch Test-Runner
└── test-scenarios.ps1             # PowerShell Test-Runner
```

## 🎯 Features

### Client (Program.cs)
- ✓ Verbindung zu OPC UA Server
- ✓ Prüfung von MessageSecurityMode
- ✓ BadCertificateUriInvalid Exception Handling
- ✓ AutoAcceptUntrustedCertificates Status
- ✓ Server-Zertifikat Validierung
- ✓ Ausführliche Sicherheits-Analyse

### Server (Server.cs + ServerConfig.cs)
- ✓ Konfigurierbare Security Modes (None, Sign, SignAndEncrypt)
- ✓ Multiple Security Policies
- ✓ Auto-Accept Zertifikate
- ✓ Kommandozeilen-Konfiguration
- ✓ Test-Knoten für Datenübertragung

## 🚀 Quick Start

### 1. Client ausführen (Standard Server)
```bash
dotnet run --project SecurityCheck.csproj
```

### 2. Server ausführen (Standard)
```bash
dotnet run --project SecurityCheckServer.csproj
```

### 3. Mit benutzerdefinierten Parametern

**Server:**
```bash
dotnet run --project SecurityCheckServer.csproj -- --port 4840 --security-mode SignAndEncrypt
```

**Client:**
```bash
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

## 🧪 Automatisierte Test-Szenarien

### Windows Batch
```bash
test-scenarios.bat
```

### PowerShell
```powershell
.\test-scenarios.ps1
```

Diese Skripte bieten ein interaktives Menü für verschiedene Test-Szenarien.

## 📋 Test-Szenarien

### Szenario 1: Ungesicherter Server
```bash
# Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --security-mode None

# Terminal 2: Client
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

**Erwartete Ausgabe:**
```
⚠️  WARNUNG: Keine Nachrichtenverschlüsselung!
```

### Szenario 2: Nur Signatur (Sign)
```bash
# Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --security-mode Sign

# Terminal 2: Client
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

**Erwartete Ausgabe:**
```
✓ Nachrichten sind signiert (Integritätsprüfung)
```

### Szenario 3: Vollständige Verschlüsselung
```bash
# Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --security-mode SignAndEncrypt

# Terminal 2: Client
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

**Erwartete Ausgabe:**
```
✓ Nachrichten sind signiert und verschlüsselt
```

### Szenario 4: Auto-Accept Zertifikate
```bash
# Terminal 1: Server
dotnet run --project SecurityCheckServer.csproj -- --auto-accept

# Terminal 2: Client
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

**Erwartete Ausgabe:**
```
⚠️  WARNUNG: Nicht vertrauenswürdige Zertifikate werden automatisch akzeptiert!
```

### Szenario 5: Verschiedene Security Policies
```bash
# Terminal 1: Server mit Basic256
dotnet run --project SecurityCheckServer.csproj -- --security-policy basic256

# Terminal 2: Client
dotnet run --project SecurityCheck.csproj "opc.tcp://localhost:4840"
```

## 📊 Ausgabe des Clients

Der Client gibt detaillierte Informationen aus:

```
=== OPC UA Server Security Check ===

Verbinde zu OPC UA Server: opc.tcp://localhost:4840

Verbindung erfolgreich hergestellt!

=== Sicherheitseinstellungen ===

1. Message Security Mode:
   - Modus: SignAndEncrypt
   ✓ Nachrichten sind signiert und verschlüsselt

2. Security Policy (URI):
   - http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256
   ✓ Sichere Security Policy

3. Auto Accept Untrusted Certificates:
   - Aktiviert: False
   ✓ Nur vertrauenswürdige Zertifikate werden akzeptiert

4. Server-Zertifikat:
   - Subject: CN=OPC UA Test Server
   - Issuer: CN=OPC UA Test Server
   - Valid From: 13.05.2026 12:00:00
   - Valid Until: 13.05.2027 12:00:00
   ✓ Zertifikat ist gültig

5. Sitzungsinformationen:
   - Session ID: ns=1;i=123456
   - Session Timeout: 60000 ms
   - Connected: True

=== Verbindung erfolgreich beendet ===
```

## 🔧 Server Kommandozeilen-Optionen

```
--port <Nummer>
  Port für den Server (Standard: 4840)
  Beispiel: --port 5000

--security-mode <Modus>
  Message Security Mode
  Optionen: None | Sign | SignAndEncrypt
  Standard: SignAndEncrypt
  Beispiel: --security-mode None

--security-policy <Policy>
  OPC UA Security Policy
  Optionen: none | basic128rsa15 | basic256 | basic256sha256 | 
            aes128sha256rsaoaep | aes256sha256rsapss
  Standard: basic256sha256
  Beispiel: --security-policy basic256

--auto-accept
  Akzeptiert nicht vertrauenswürdige Zertifikate automatisch
  Beispiel: --auto-accept

--help
  Zeigt diese Hilfe an
```

## 🔐 Sicherheitsaspekte

- **Für Entwicklung/Testing:** Verwende `--security-mode None` oder `--auto-accept`
- **Für Produktion:** Nutze `SignAndEncrypt` mit `basic256sha256` oder besser
- **Zertifikate:** Stelle sicher, dass Zertifikate korrekt konfiguriert sind

## ⚠️ Häufige Probleme

### Port bereits in Verwendung
```bash
# Verwende alternativen Port
dotnet run --project SecurityCheckServer.csproj -- --port 5000
```

### Zertifikat-Fehler
```bash
# Verwende Auto-Accept für Tests
dotnet run --project SecurityCheckServer.csproj -- --auto-accept
```

### Server lädt nicht
- Überprüfe Firewall-Einstellungen
- Stelle sicher, dass .NET 10 installiert ist
- Prüfe ob alle NuGet-Pakete installiert sind

## 📦 Abhängigkeiten

- .NET 10 SDK
- OpcUa.Client (NuGet)
- OpcUa.Server (NuGet)

## 📚 Weiterführende Ressourcen

- [OPC Foundation](https://opcfoundation.org/)
- [OPC UA Security](https://reference.opcfoundation.org/)
- [Microsoft .NET 10 Docs](https://learn.microsoft.com/de-de/dotnet/)

## 📝 Lizenz

Dieses Projekt dient zu Bildungs- und Testzwecken.

---

**Erstellt:** Mai 2026  
**Framework:** .NET 10  
**Sprache:** C#
