# OPC UA Client - SecurityCheck

## 🚀 Schnellstart

```bash
# Standard
dotnet run

# Mit spezifischem Server
dotnet run "opc.tcp://192.168.1.100:4840"
```

## ⚙️ Konfiguration: `appsettings.json`

```json
{
  "client": {
    "serverUrl": "opc.tcp://localhost:4840",
    "autoAcceptUntrustedCertificates": false,
    "sessionTimeout": 60000
  }
}
```

## 📋 Funktionen

- ✓ Verbindung zu OPC UA Server
- ✓ Prüfung von MessageSecurityMode
- ✓ BadCertificateUriInvalid Exception Handling
- ✓ AutoAcceptUntrustedCertificates Status
- ✓ Server-Zertifikat Validierung
- ✓ Ausführliche Sicherheits-Analyse

## 📊 Ausgabe

Detaillierte Sicherheitsanalyse mit:
- Message Security Mode
- Security Policy
- Zertifikat-Status
- Auto-Accept Status
- Session-Informationen

## 📁 Projektdateien

- `SecurityCheck.csproj` - Projekt-Konfiguration
- `Program.cs` - Einstiegspunkt und Konfiguration
- `appsettings.json` - Client-Konfiguration
