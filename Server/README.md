# OPC UA Test Server

## 🚀 Schnellstart

```bash
# Mit Konfigurationsdatei (empfohlen)
dotnet run

# Oder mit Kommandozeilen-Optionen
dotnet run -- --security-mode None
dotnet run -- --port 5000 --security-mode SignAndEncrypt
```

## ⚙️ Konfiguration: `config.json`

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

### Konfigurierbare Parameter

| Parameter | Werte | Standard |
|-----------|-------|----------|
| `port` | 1-65535 | 4840 |
| `securityMode` | None, Sign, SignAndEncrypt | SignAndEncrypt |
| `securityPolicy` | none, basic128rsa15, basic256, basic256sha256, aes128sha256rsaoaep, aes256sha256rsapss | basic256sha256 |
| `autoAcceptUntrustedCertificates` | true, false | false |

## 🔧 Kommandozeilen-Optionen

Überschreiben die config.json:

```bash
dotnet run -- --security-mode None
dotnet run -- --port 5000
dotnet run -- --security-policy basic256
dotnet run -- --auto-accept
dotnet run -- --help
```

## 📋 Test-Szenarien

### Entwicklung - Keine Sicherheit
```bash
dotnet run -- --security-mode None
```

### Entwicklung - Mit Signatur
```bash
dotnet run -- --security-mode Sign
```

### Produktion - Vollständig verschlüsselt
```bash
dotnet run -- --security-mode SignAndEncrypt --security-policy basic256sha256
```

### Mit Auto-Accept Zertifikate
```bash
dotnet run -- --auto-accept
```

## 📁 Projektdateien

- `SecurityCheckServer.csproj` - Projekt-Konfiguration
- `ServerProgram.cs` - Einstiegspunkt
- `Server.cs` - OPC UA Server Implementation (echte OPC UA Stack API)
- `ServerConfig.cs` - Konfigurationslogik
- `config.json` - Server-Konfiguration

---

**Alle Komponenten verwenden die echte OPC UA Stack API - keine Mock-Implementierungen mehr.**
