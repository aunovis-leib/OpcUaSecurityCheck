# SecurityCheckGui - OPC UA Server Security Check (Grafische Benutzeroberfläche)

Eine WPF-Anwendung zur grafischen Überprüfung der Sicherheitseinstellungen eines OPC UA Servers.

## Features

- **Grafische Benutzeroberfläche**: Benutzerfreundliches WPF-UI statt Konsole
- **Server URL Eingabe**: Eingabefeld für die OPC UA Server-Adresse
- **Konfigurierbare Einstellungen**:
  - Auto Accept Untrusted Certificates (Checkbox)
  - Session Timeout (Eingabefeld)
- **Umfassende Sicherheitsprüfungen**:
  1. Message Security Mode
  2. Security Policy
  3. Auto Accept Untrusted Certificates
  4. Server-Zertifikat (Subject, Issuer, Gültigkeitsdauer)
  5. Sitzungsinformationen
- **Farbige Statusanzeige**:
  - Grün: Sichere/gültige Einstellungen
  - Orange: Warnungen
  - Rot: Kritische Fehler
- **Detailliertes Fehlerlog**: Vollständiges Fehlerlog zur Diagnose

## Verwendung

### Starten der Anwendung

```bash
dotnet run
```

oder über Visual Studio / Visual Studio Code starten.

### Bedienung

1. **Server URL eingeben** (z.B. `opc.tcp://localhost:4840`)
2. **Optional: Checkboxen und Timeouts anpassen**
3. **"Verbindung Prüfen" Button klicken**
4. **Ergebnisse ansehen** in den Ergebnis-Abschnitten

## Konfiguration

Die Datei `appsettings.json` enthält Standard-Einstellungen:

```json
{
  "client": {
    "serverUrl": "opc.tcp://localhost:4840",
    "autoAcceptUntrustedCertificates": false,
    "sessionTimeout": 60000
  }
}
```

## Abhängigkeiten

- **Opc.Ua.Client**: OPC UA Client-Bibliothek
- **.NET 10.0 (Windows)**: WPF-Support erforderlich

## Systemanforderungen

- Windows OS
- .NET 10.0 oder höher
- OPC UA Server zum Testen (lokal oder remote)

## Unterschiede zur Console-Version

| Feature | SecurityCheck (Console) | SecurityCheckGui (WPF) |
|---------|-------------------------|------------------------|
| Ausgabe | Konsole | Grafisches UI |
| Eingabe | Kommandozeilenargumente | UI-Eingabefelder |
| Fehlerbehandlung | Text-Output | Fehlerlog-Textbox |
| Interaktivität | Einmalig | Wiederholbar |
| Benutzerfreundlichkeit | Für Entwickler | Für alle |

## Lizenz

Siehe Haupt-README
