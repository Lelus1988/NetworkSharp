# NetworkSharp

NetworkSharp ist ein modernes Windows-Werkzeug, mit dem du deine Netzwerkverbindung prüfen, beobachten und besser verstehen kannst.

[![Latest Release](https://img.shields.io/github/v/release/Lelus1988/NetworkSharp?display_name=tag&sort=semver&label=latest%20release)](https://github.com/Lelus1988/NetworkSharp/releases/latest)
[![Build](https://img.shields.io/github/actions/workflow/status/Lelus1988/NetworkSharp/release.yml?label=build)](https://github.com/Lelus1988/NetworkSharp/actions)
[![License](https://img.shields.io/github/license/Lelus1988/NetworkSharp)](LICENSE)

## Download

Die aktuelle Windows-Version gibt es auf der [Release-Seite](https://github.com/Lelus1988/NetworkSharp/releases/latest).

1. Öffne den neuesten Release.
2. Lade unter **Assets** `NetworkSharp-Setup.exe` herunter.
3. Starte den Installer.
4. NetworkSharp wird nur für deinen Benutzer unter `%LOCALAPPDATA%\Programs\NetworkSharp` installiert.

Die Installation benötigt normalerweise keine Administratorrechte.

> [!IMPORTANT]
> GitHub zeigt zusätzlich automatisch Quellcode-Archive als `Source code (zip)` und `Source code (tar.gz)` an. Das eigentliche fertige Download-Programm ist `NetworkSharp-Setup.exe`.

## Was NetworkSharp kann

- Netzwerk-Monitor für Download, Upload, Spitzenwerte und Datenverbrauch
- Ping-Monitor für Antwortzeit, Durchschnitt und Paketverlust
- LAN-Geräte-Erkennung im eigenen Netzwerk
- Port-Scanner für erreichbare Dienste
- DNS-Tester für Namensauflösung
- Speed-Test für Download und Upload
- IP- und Standortinformationen
- WLAN-Profil-Ansicht
- SQLite-Grundlage für historische Netzwerkdaten

## Aktuelle Oberfläche

NetworkSharp 2.1.1 verwendet ein dunkles Network-Console-Design mit:

- runden blauen Buttons und Eingabefeldern
- dunklen ComboBoxen ohne weiße Standard-WPF-Flächen
- Cyan-, Grün- und Warnfarben für Statusinformationen
- animierten Seitenwechseln und Live-Statusanzeigen
- Hashtag-Branding mit `Network#` und `NetworkSharp`
- Live-Diagrammen für Netzwerkverkehr und Ping

## Dokumentation

- [Aktuelle Änderungen](update.md)
- [Neueste Releases](https://github.com/Lelus1988/NetworkSharp/releases/latest)
- [Alle Releases](https://github.com/Lelus1988/NetworkSharp/releases)
- [GitHub Actions](https://github.com/Lelus1988/NetworkSharp/actions)
- [Mitmachen](CONTRIBUTING.md)
- [Lizenz](LICENSE)

## Aus dem Quellcode bauen

Voraussetzungen:

- Windows
- .NET 10 SDK
- Visual Studio oder VS Code mit WPF-Unterstützung

```powershell
dotnet restore
dotnet build .\NetworkSharp.csproj
dotnet test .\NetworkSharp.Tests\NetworkSharp.Tests.csproj
```

Für eine portable Veröffentlichung:

```powershell
dotnet publish .\NetworkSharp.csproj -c Release -r win-x64 --self-contained true -o .\publish
```

## Sicherheit und Berechtigungen

Die normalen Netzwerkfunktionen sind für die Nutzung ohne Administratorrechte gedacht. Einige Windows-Funktionen, insbesondere das Auslesen gespeicherter WLAN-Passwörter, können abhängig von Windows und den lokalen Berechtigungen zusätzliche Rechte benötigen.

NetworkSharp enthält keine VPN-Funktion und baut keinen VPN-Tunnel auf.

## Mitmachen

Fehler, Verbesserungsvorschläge und neue Ideen sind willkommen. Lies vor einem Pull Request bitte die [Contributing-Anleitung](CONTRIBUTING.md).

- [Fehler melden](https://github.com/Lelus1988/NetworkSharp/issues/new?template=bug_report.md)
- [Feature vorschlagen](https://github.com/Lelus1988/NetworkSharp/issues/new?template=feature_request.md)
- [Alle Issues](https://github.com/Lelus1988/NetworkSharp/issues)

## Lizenz

NetworkSharp steht unter der [MIT-Lizenz](LICENSE).
