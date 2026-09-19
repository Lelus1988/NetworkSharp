# NetworkSharp Releases erstellen

Diese Anleitung beschreibt den manuellen Release-Ablauf für NetworkSharp. Der aktuelle Stand verwendet Git-Tags und GitHub Actions, um automatisch einen Windows-Installer zu bauen.

## Voraussetzungen

- Git ist installiert.
- Das Repository ist geklont.
- Du befindest dich im Projektordner.
- Du hast Push-Rechte für das Repository.
- Das .NET 10 SDK ist installiert, wenn du lokal bauen und testen möchtest.

## 1. Änderungen prüfen

Vor einem Commit immer zuerst den Arbeitsstand ansehen:

```powershell
git status --short
git diff --check
git diff
```

Prüfe jede Datei einzeln. Gruppiere nur Dateien, die inhaltlich zusammengehören, zum Beispiel UI-Dateien, Diagrammdateien oder Release-Dokumentation.

Generierte Ordner gehören nicht in einen Commit:

```text
bin/
obj/
publish/
installer/output/
```

## 2. Build und Tests ausführen

```powershell
dotnet build .\NetworkSharp.csproj --nologo -v minimal
dotnet test .\NetworkSharp.Tests\NetworkSharp.Tests.csproj --nologo -v minimal
```

Wenn die App noch läuft und die EXE gesperrt ist:

```powershell
Get-Process -Name NetworkSharp -ErrorAction SilentlyContinue |
    Stop-Process -Force
```

Danach Build und Tests erneut ausführen.

## 3. Versionsnummer erhöhen

Für ein neues Release müssen diese Werte zusammenpassen:

```xml
<Version>2.3.0</Version>
<AssemblyVersion>2.3.0.0</AssemblyVersion>
<FileVersion>2.3.0.0</FileVersion>
```

Die Versionsnummer wird außerdem angepasst in:

- `NetworkSharp.csproj`
- `MainWindow.xaml`
- `installer/NetworkSharp.iss`
- `README.md`
- `update.md`

Im Installer muss die Fallback-Version ebenfalls stimmen:

```text
#define MyAppVersion "2.3.0"
```

## 4. Präzise Commits erstellen

Nur geprüfte Dateien hinzufügen:

```powershell
git add App.xaml MainWindow.xaml NetworkSharp.csproj
git diff --cached --name-status
git commit -m "feat(ui): add new network status panel"
```

Gute Commit-Nachrichten beschreiben den konkreten Inhalt:

```text
feat(ui): add rounded themed controls
fix(ping): prevent duplicate result updates
feat(update): offer confirmed per-user upgrades
docs(dev): document project structure
chore(release): prepare NetworkSharp 2.3.0
```

Keine unklaren Nachrichten verwenden:

```text
update files
changes
final
```

## 5. Release-Tag erstellen

Für Version `2.3.0` wird ein annotierter Tag mit `v` erstellt:

```powershell
git tag -a v2.3.0 -m "NetworkSharp 2.3.0"
```

Die Schreibweise ist wichtig:

```text
Projektversion: 2.3.0
Git-Tag:        v2.3.0
```

## 6. Branch und Tag pushen

```powershell
git push origin main
git push origin v2.3.0
```

Der Tag startet automatisch den Workflow unter:

```text
.github/workflows/release.yml
```

## 7. Was GitHub Actions automatisch macht

Der Workflow startet bei jedem Tag, der mit `v` beginnt.

Danach passiert automatisch:

1. Das Repository wird ausgecheckt.
2. .NET 10 wird eingerichtet.
3. NetworkSharp wird self-contained für Windows veröffentlicht.
4. Inno Setup wird installiert.
5. Der per-user Installer wird gebaut.
6. Ein GitHub Release wird erstellt.
7. Die fertige `NetworkSharp-Setup-{Version}.exe` wird als Asset hochgeladen.

Der Installer verwendet:

```text
%LOCALAPPDATA%\Programs\NetworkSharp
```

Dafür sind normalerweise keine Administratorrechte erforderlich.

## 8. Release prüfen

Den Workflow kannst du hier prüfen:

- [GitHub Actions](https://github.com/Lelus1988/NetworkSharp/actions)
- [Alle Releases](https://github.com/Lelus1988/NetworkSharp/releases)

Die Zustände bedeuten:

```text
queued       wartet auf einen Runner
in_progress  wird gerade ausgeführt
success      erfolgreich abgeschlossen
failure      fehlgeschlagen
```

Im Release sollte als eigenes Asset nur der Installer stehen, zum Beispiel:

```text
NetworkSharp-Setup-2.3.0.exe
```

GitHub zeigt zusätzlich automatisch `Source code (zip)` und `Source code (tar.gz)` an. Diese Archive werden von GitHub selbst erzeugt und können nicht durch unseren Workflow entfernt werden.

## 9. Release-Beschreibung

Eine ausführliche Beschreibung kannst du direkt auf der GitHub-Release-Seite eintragen:

```markdown
## NetworkSharp 2.3.0

### Neu

- Neue Netzwerkfunktion
- Verbesserte Diagramme
- Stabilere WPF-Bindings

### Download

- `NetworkSharp-Setup-2.3.0.exe`
- Installation ohne Administratorrechte

### Wichtig

- NetworkSharp enthält keine VPN-Funktion.
```

## 10. Final prüfen

```powershell
git status --short --branch
git log --oneline --decorate -5
git ls-remote --tags origin
```

Der Branch sollte am Ende sauber sein:

```text
## main...origin/main
```

Eine lokale Änderung wie `M App.xaml` bleibt uncommitted, wenn sie nicht zum aktuellen Release gehört oder vorher ausdrücklich geprüft werden muss.

## Update-Verhalten der App

NetworkSharp prüft beim Start, ob auf GitHub eine neuere veröffentlichte Version vorhanden ist.

- Bei einer neueren Version erscheint ein modales Popup.
- `Ja, jetzt aktualisieren` lädt den passenden Installer.
- Danach wird die alte App geschlossen und der Installer gestartet.
- `Schließen` beendet die App vollständig.
- Ohne Internetverbindung läuft NetworkSharp normal weiter.
- Beim ersten Start müssen die Nutzungsbedingungen bestätigt werden.

## Kompletter Kurzablauf

```powershell
dotnet build .\NetworkSharp.csproj
dotnet test .\NetworkSharp.Tests\NetworkSharp.Tests.csproj
git status --short
git diff --check

git add <geprüfte-dateien>
git diff --cached --name-status
git commit -m "chore(release): prepare NetworkSharp 2.3.0"

git tag -a v2.3.0 -m "NetworkSharp 2.3.0"
git push origin main
git push origin v2.3.0
```
