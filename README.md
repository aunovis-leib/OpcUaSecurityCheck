# OPC UA Server Security Check

Ein C# .NET 10 Programm zur Überprüfung der Sicherheitseinstellungen beim Verbinden zu einem OPC UA Server.

## Funktionalität

Das Programm prüft und gibt folgende Sicherheitsparameter aus:

1. **Message Security Mode**
   - None: Keine Verschlüsselung
   - Sign: Nachrichten sind signiert
   - SignAndEncrypt: Vollständige Verschlüsselung

2. **Security Policy URI**
   - Prüft auf gültige Sicherheitsrichtlinien

3. **BadCertificateUriInvalid**
   - Erkennt ungültige Zertifikat-URIs

4. **AutoAcceptUntrustedCertificates**
   - Zeigt an, ob nicht vertrauenswürdige Zertifikate automatisch akzeptiert werden

5. **Server-Zertifikat**
   - Subject, Issuer, Gültigkeitsdauer
   - Warnung bei abgelaufenem oder bald ablaufendem Zertifikat

6. **Sitzungsinformationen**
   - Session ID und Status

## Anforderungen

- .NET 10 SDK
- OPC UA Server für Tests (z.B. Prosys OPC UA Simulation Server)

## Verwendung

```bash
# Mit Standard-Server (localhost:4840)
dotnet run

# Mit benutzerdefinierten Server
dotnet run "opc.tcp://server-address:port"
```

Beispiel:
```bash
dotnet run "opc.tcp://192.168.1.100:4840"
```

## NuGet Packages

- `OpcUa.Client` - OPC UA Client Bibliothek (OPC Foundation)

## Ausgabe

Das Programm gibt die Sicherheitseinstellungen in einem strukturierten Format aus und kennzeichnet potenzielle Sicherheitsprobleme mit:
- ✓ Grüner Häkchen für sichere Einstellungen
- ⚠️ Warnsymbole für problematische Einstellungen
- ℹ️ Informationssymbole für weitere Details

## Hinweise

- Die AutoAcceptUntrustedCertificates ist im Code auf `true` gesetzt (nur für Testzwecke!)
- Für Produktionsumgebungen sollte dies auf `false` gesetzt werden
- Die Zertifikatsspeicher müssen korrekt konfiguriert sein
