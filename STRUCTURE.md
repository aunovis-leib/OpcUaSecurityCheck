# OPC UA SecurityCheck - Neue Projektstruktur

## 📁 Verzeichnisstruktur

```
SecurityCheck/
├── SecurityCheck.sln                    # Solution-Datei
├── README.md                            # Basis-Dokumentation
├── SETUP.md                             # Vollständiges Setup-Guide
├── EXAMPLES.md                          # Test-Beispiele
│
├── Client/                              # OPC UA Client Projekt
│   ├── SecurityCheck.csproj             # Client Projektdatei
│   ├── Program.cs                       # Client-Einstiegspunkt
│   ├── OpcUaMocks.cs                    # OPC UA Mock-Implementierung
│   ├── appsettings.json                 # Client-Konfiguration
│   └── bin/                             # Build-Ausgabe
│
├── Server/                              # OPC UA Server Projekt
│   ├── SecurityCheckServer.csproj       # Server Projektdatei
│   ├── ServerProgram.cs                 # Server-Einstiegspunkt
│   ├── Server.cs                        # OPC UA Server Implementation
│   ├── ServerConfig.cs                  # Server Konfigurationslogik
│   ├── OpcUaMocks.cs                    # OPC UA Mock-Implementierung
│   ├── config.json                      # Server-Konfiguration
│   └── bin/                             # Build-Ausgabe
│
└── (alte Dateien im Root - können gelöscht werden)
```

## ⚙️ Konfigurationsdateien

### Client: `Client/appsettings.json`

```json
{
  "client": {
    "serverUrl": "opc.tcp://localhost:4840",
    "autoAcceptUntrustedCertificates": false,
    "sessionTimeout": 60000
  }
}
```

**Optionen:**
- `serverUrl`: OPC UA Server-Adresse
- `autoAcceptUntrustedCertificates`: Auto-Akzept untrusted Zertifikate (true/false)
- `sessionTimeout`: Sitzungs-Timeout in Millisekunden

### Server: `Server/config.json`

```json
{
  "server": {
    "port": 4840,
    "securityMode": "SignAndEncrypt",
    "securityPolicy": "basic256sha256",
    "autoAcceptUntrustedCertificates": false
  }
}
```

**Optionen:**
- `port`: Port für Server (Standard: 4840)
- `securityMode`: None | Sign | SignAndEncrypt (Standard: SignAndEncrypt)
- `securityPolicy`: 
  - `none`
  - `basic128rsa15` (veraltet)
  - `basic256`
  - `basic256sha256` (empfohlen)
  - `aes128sha256rsaoaep`
  - `aes256sha256rsapss`
- `autoAcceptUntrustedCertificates`: Auto-Akzept untrusted Zertifikate

## 🚀 Verwendung

### Server starten

Mit Konfigurationsdatei (empfohlen):
```bash
cd Server
dotnet run
```

Mit Kommandozeilen-Optionen (überschreibt config.json):
```bash
cd Server
dotnet run -- --security-mode None
dotnet run -- --port 5000 --security-mode Sign
```

### Client ausführen

Mit Konfigurationsdatei:
```bash
cd Client
dotnet run
```

Mit spezifischem Server:
```bash
cd Client
dotnet run "opc.tcp://192.168.1.100:4840"
```

## 📋 Test-Szenarien

### Szenario 1: Ungesicherter Server (schneller Test)

**Server-Konfiguration (`Server/config.json`):**
```json
{
  "server": {
    "port": 4840,
    "securityMode": "None",
    "securityPolicy": "basic256sha256",
    "autoAcceptUntrustedCertificates": false
  }
}
```

**Starten:**
```bash
# Terminal 1
cd Server && dotnet run

# Terminal 2
cd Client && dotnet run
```

### Szenario 2: Nur Signatur

**Server-Konfiguration:**
```json
{
  "server": {
    "securityMode": "Sign"
  }
}
```

### Szenario 3: Vollständig verschlüsselt (Produktion)

**Server-Konfiguration:**
```json
{
  "server": {
    "securityMode": "SignAndEncrypt",
    "securityPolicy": "basic256sha256",
    "autoAcceptUntrustedCertificates": false
  }
}
```

### Szenario 4: Mit Auto-Accept Zertifikate (Entwicklung)

**Server-Konfiguration:**
```json
{
  "server": {
    "securityMode": "SignAndEncrypt",
    "autoAcceptUntrustedCertificates": true
  }
}
```

**Client-Konfiguration (`Client/appsettings.json`):**
```json
{
  "client": {
    "autoAcceptUntrustedCertificates": true
  }
}
```

## 🔧 Kommandozeilen-Argumente (Server)

Die Kommandozeilen-Argumente überschreiben die config.json:

```bash
dotnet run -- [Optionen]

Optionen:
  --port <Nummer>              Port (Standard: 4840)
  --security-mode <Modus>      None | Sign | SignAndEncrypt
  --security-policy <Policy>   none | basic128rsa15 | basic256 | basic256sha256 | 
                               aes128sha256rsaoaep | aes256sha256rsapss
  --auto-accept                Auto-Accept untrusted certificates
  --help                       Hilfe anzeigen
```

**Beispiele:**
```bash
# Alle Defaults von config.json
dotnet run

# Nur Signatur
dotnet run -- --security-mode Sign

# Mit anderen Policy und Auto-Accept
dotnet run -- --security-policy basic256 --auto-accept

# Mit anderem Port
dotnet run -- --port 5000
```

## 💡 Best Practices

### Für Entwicklung/Testing:
```json
{
  "server": {
    "securityMode": "None",
    "autoAcceptUntrustedCertificates": true
  },
  "client": {
    "autoAcceptUntrustedCertificates": true
  }
}
```

### Für Produktion:
```json
{
  "server": {
    "securityMode": "SignAndEncrypt",
    "securityPolicy": "basic256sha256",
    "autoAcceptUntrustedCertificates": false
  },
  "client": {
    "autoAcceptUntrustedCertificates": false
  }
}
```

## 📊 Ausgabe-Beispiel

**Server:**
```
=== OPC UA Test Server ===

Konfiguration geladen aus: config.json

Server-Konfiguration:
  Port: 4840
  SecurityMode: SignAndEncrypt
  SecurityPolicy: http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256
  AutoAcceptUntrustedCertificates: False

✓ OPC UA Server erfolgreich gestartet
  - Adresse: opc.tcp://localhost:4840
  - Message Security Mode: SignAndEncrypt
  - Security Policy: http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256
  - Auto Accept Untrusted Certs: False

Drücke eine beliebige Taste zum Beenden...
```

**Client:**
```
=== OPC UA Server Security Check (Client) ===

Client-Einstellungen:
  - AutoAcceptUntrustedCertificates: False
  - Session Timeout: 60000ms

Verbinde zu OPC UA Server: opc.tcp://localhost:4840

Verbindung erfolgreich hergestellt!

=== Sicherheitseinstellungen ===

1. Message Security Mode:
   - Modus: SignAndEncrypt
   ✓ Nachrichten sind signiert und verschlüsselt

...
```

## 🔄 Alte Dateien aufräumen

Die folgenden Dateien im Root-Verzeichnis können gelöscht werden, da sie jetzt in den Unterordnern sind:

```bash
rm -f Program.cs
rm -f Server.cs
rm -f ServerProgram.cs
rm -f ServerConfig.cs
rm -f SecurityCheck.csproj
rm -f SecurityCheckServer.csproj
rm -f OpcUaMocks.cs
```

## ✨ Vorteile der neuen Struktur

✅ **Klare Trennung** - Client und Server sind vollständig getrennt  
✅ **Konfigurationsdateien** - Einfache Änderung ohne Code-Änderungen  
✅ **Einfach zu testen** - Verschiedene Szenarien durch JSON-Dateien  
✅ **Produktionsreif** - Konfiguration kann einfach deployt werden  
✅ **Wartbar** - Klare Struktur für zukünftige Erweiterungen

---

**Version:** 2.0 (Mit Konfigurationsdateien)  
**Datum:** Mai 2026  
**Framework:** .NET 10
