# Zu NetworkSharp beitragen

Danke, dass du NetworkSharp verbessern möchtest. Fehlerberichte, Ideen, Tests und Pull Requests sind willkommen.

## Vor dem Start

1. Prüfe, ob das Problem bereits in den [Issues](https://github.com/Lelus1988/NetworkSharp/issues) bekannt ist.
2. Für einen Fehler kannst du die [Bug-Vorlage](https://github.com/Lelus1988/NetworkSharp/issues/new?template=bug_report.md) verwenden.
3. Für eine Idee kannst du die [Feature-Vorlage](https://github.com/Lelus1988/NetworkSharp/issues/new?template=feature_request.md) verwenden.

## Lokale Voraussetzungen

- Windows
- .NET 10 SDK
- Visual Studio oder VS Code
- Git

Repository klonen und Projekt bauen:

```powershell
git clone https://github.com/Lelus1988/NetworkSharp.git
cd NetworkSharp
dotnet restore
dotnet build .\NetworkSharp.csproj
```

Tests ausführen:

```powershell
dotnet test .\NetworkSharp.Tests\NetworkSharp.Tests.csproj
```

## Änderungen entwickeln

- Halte Änderungen möglichst klein und auf ein Thema konzentriert.
- Verwende die bestehenden MVVM-, Service- und WPF-Muster.
- Verändere keine fremden oder nicht zum Thema gehörenden Dateien.
- Ergänze Tests, wenn sich Verhalten oder Logik ändern.
- Prüfe WPF-Bindings besonders auf schreibgeschützte Eigenschaften.
- Prüfe bei UI-Änderungen auch kleine Fensterbreiten.
- Schreibe verständliche Commit-Nachrichten, die den konkreten Änderungsumfang nennen.

## Pull Requests

Ein guter Pull Request enthält:

- eine kurze Beschreibung des Problems
- eine Erklärung der Lösung
- die betroffenen Bereiche oder Dateien
- Hinweise zu manuellen Tests
- das Ergebnis von `dotnet build` und `dotnet test`
- Screenshots bei sichtbaren UI-Änderungen

Bitte starte keinen Pull Request mit generierten Ordnern wie `bin`, `obj` oder `publish`.

## Commit-Nachrichten

Commit-Nachrichten sollten kurz und eindeutig sein, zum Beispiel:

```text
fix(ping): prevent duplicate result updates
feat(ui): add rounded themed input controls
docs(readme): describe portable release download
```

## Lizenz

Mit einem Beitrag erklärst du dich damit einverstanden, dass dein Beitrag unter der [MIT-Lizenz](LICENSE) des Projekts veröffentlicht wird.
