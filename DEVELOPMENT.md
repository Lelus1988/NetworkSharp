# NetworkSharp Entwicklung

Diese Datei erklärt kurz, wo die wichtigsten Teile von NetworkSharp liegen und wie sie zusammenspielen. Sie ist als Orientierung für die Weiterentwicklung gedacht, nicht als vollständige API-Dokumentation.

## Grundidee

NetworkSharp folgt grundsätzlich dem MVVM-Muster:

```text
View (XAML)
   |
   v
ViewModel (Zustand, Bindings, Commands)
   |
   v
Service (Netzwerkzugriff und Messungen)
   |
   v
Windows / Netzwerk / Datenbank
```

- **Views** zeigen die Oberfläche und nehmen einfache UI-Ereignisse entgegen.
- **ViewModels** halten den sichtbaren Zustand, aktualisieren Bindings und starten Commands.
- **Services** führen Netzwerkoperationen aus und liefern Ergebnisse oder Events.
- **Data** kapselt lokale historische Daten und die SQLite-Anbindung.
- **Tests** prüfen besonders Hilfsfunktionen und wichtige Netzwerklogik.

## Projektstruktur

```text
NetworkSharp/
├── README.md                         # Einstieg, Download und wichtigste Funktionen
├── DEVELOPMENT.md                    # Diese Entwicklerübersicht
├── CONTRIBUTING.md                   # Regeln für Issues, Pull Requests und Beiträge
├── LICENSE                           # MIT-Lizenz
├── update.md                         # Aktuelle Änderungen und Release-Hinweise
├── App.xaml                          # Globale Farben, Styles, Templates und Converter
├── App.xaml.cs                       # Start, Dependency Injection und Datenbankinitialisierung
├── MainWindow.xaml                   # Hauptfenster, Sidebar und Inhaltsbereich
├── MainWindow.xaml.cs                # Navigation zwischen den Views
├── NetworkSharp.csproj               # Target Framework, WPF und NuGet-Pakete
│
├── Common/
│   └── RelayCommand.cs               # Commands für Buttons und andere UI-Aktionen
│
├── Converters/
│   └── ValueConverters.cs            # WPF-Konverter für Sichtbarkeit, Farben und Status
│
├── Data/
│   ├── NetworkSharpDbContext.cs      # EF-Core-Kontext für lokale SQLite-Daten
│   ├── Models/                       # Datenmodelle für gespeicherte Netzwerkdaten
│   │   ├── DnsTestResult.cs
│   │   ├── LanDevice.cs
│   │   ├── NetworkStatistics.cs
│   │   ├── PingHistory.cs
│   │   └── UserSetting.cs
│   └── Repositories/                 # Zugriff auf gespeicherte Daten
│       ├── INetworkDataRepository.cs
│       └── NetworkDataRepository.cs
│
├── Services/                         # Netzwerklogik und externe Systemzugriffe
│   ├── IDnsTesterService.cs          # Vertrag für DNS-Tests
│   ├── DnsTesterService.cs           # DNS-Auflösung und Testergebnisse
│   ├── INetworkMonitorService.cs     # Vertrag für Netzwerkstatistiken
│   ├── NetworkMonitorService.cs      # Download, Upload und Verlauf
│   ├── IPingService.cs               # Vertrag für kontinuierliche Pings
│   ├── PingService.cs                # Ping-Ausführung und Statistik
│   ├── ILanDeviceService.cs          # Vertrag für LAN-Erkennung
│   ├── LanDeviceService.cs           # Geräte im lokalen Netz
│   ├── IPortScannerService.cs        # Vertrag für Port-Scans
│   ├── PortScannerService.cs         # TCP-Port-Scan
│   ├── ISpeedTestService.cs          # Vertrag für Speed-Tests
│   ├── SpeedTestService.cs           # Download-/Upload-Messung
│   ├── IIpGeoIpService.cs            # Vertrag für IP- und Standortdaten
│   ├── IpGeoIpService.cs             # Öffentliche und lokale IP-Informationen
│   ├── IWlanProfileService.cs        # Vertrag für WLAN-Profile
│   ├── WlanProfileService.cs         # Windows-WLAN-Profilzugriff
│   ├── ITracerouteService.cs         # Vertrag für Routenverfolgung
│   ├── TracerouteService.cs          # Traceroute-Funktion
│   ├── IWhoisService.cs              # Vertrag für WHOIS-Abfragen
│   ├── WhoisService.cs               # WHOIS-Abfragen
│   └── NetworkAddressHelper.cs       # IP- und Subnetz-Hilfsfunktionen
│
├── ViewModels/                       # Bindable Zustände der Ansichten
│   ├── NetworkMonitorViewModel.cs    # Kennzahlen und Monitoring-Lebenszyklus
│   ├── NetworkMonitorViewModel.Chart.cs # Live-Kurven, Zeiträume und Resampling
│   ├── PingViewModel.cs              # Ping-Ziel, Statistik und Lebenszyklus
│   ├── PingViewModel.Chart.cs        # Ping-Kurve, Durchschnitt und Paketstreifen
│   ├── LanDevicesViewModel.cs        # LAN-Geräteliste und Scanstatus
│   ├── PortScannerViewModel.cs       # Ziel, Portbereich und Scanergebnisse
│   ├── DnsTesterViewModel.cs         # DNS-Eingaben und Testergebnisse
│   ├── SpeedTestViewModel.cs         # Speed-Test-Zustand und Diagramm
│   ├── IpGeoIpViewModel.cs           # IP- und Standortanzeige
│   └── WlanProfilesViewModel.cs      # WLAN-Profile und Auswahlzustand
│
├── Views/                            # WPF-Oberflächen und einfache UI-Events
│   ├── NetworkMonitorView.xaml       # Netzwerk-Karten und Diagramme
│   ├── NetworkMonitorView.xaml.cs    # Start/Stop des Monitorings
│   ├── PingView.xaml                 # Ping-Eingabe, Kurve und Paketstreifen
│   ├── PingView.xaml.cs              # Start/Stop und View-Lebenszyklus
│   ├── LanDevicesView.xaml           # LAN-Geräteliste
│   ├── PortScannerView.xaml          # Port-Scanner-Oberfläche
│   ├── DnsTesterView.xaml            # DNS-Tester-Oberfläche
│   ├── SpeedTestView.xaml            # Speed-Test-Oberfläche
│   ├── IpGeoIpView.xaml              # IP- und Standort-Oberfläche
│   └── WlanProfilesView.xaml          # WLAN-Profil-Oberfläche
│
├── NetworkSharp.Tests/               # xUnit-Tests
│   ├── NetworkAddressHelperTests.cs  # Tests für IP-/Subnetzberechnung
│   └── NetworkSharp.Tests.csproj
│
├── installer/
│   └── NetworkSharp.iss               # Per-user Installer ohne Adminpflicht
│
└── .github/
    ├── ISSUE_TEMPLATE/               # Vorlagen für Fehler und Feature-Ideen
    │   ├── bug_report.md
    │   └── feature_request.md
    └── workflows/
        └── release.yml                # Baut Installer bei einem v*-Tag
```

## Wichtige Einstiegspunkte

### Anwendung starten

[App.xaml.cs](App.xaml.cs) registriert die Services über Dependency Injection, initialisiert die lokale Datenbank und öffnet das Hauptfenster.

### Navigation ändern

[MainWindow.xaml](MainWindow.xaml) enthält Sidebar und Inhaltsbereich. [MainWindow.xaml.cs](MainWindow.xaml.cs) entscheidet anhand des View-Namens, welches ViewModel und welcher View geladen werden.

### Netzwerkfunktion erweitern

Für eine neue Netzwerkfunktion empfiehlt sich diese Reihenfolge:

1. Service-Interface mit einem kleinen, klaren Vertrag anlegen.
2. Service-Implementierung ohne UI-Abhängigkeit erstellen.
3. Service in [App.xaml.cs](App.xaml.cs) registrieren.
4. ViewModel mit bindbaren Properties und Commands ergänzen.
5. View und XAML-Bindings hinzufügen.
6. Tests für die reine Logik ergänzen.

### Diagramme ändern

Die Diagrammlogik liegt bewusst in eigenen Partial-Dateien:

- [NetworkMonitorViewModel.Chart.cs](ViewModels/NetworkMonitorViewModel.Chart.cs)
- [PingViewModel.Chart.cs](ViewModels/PingViewModel.Chart.cs)

Dadurch bleiben Messwerte, Lebenszyklus und Diagrammaufbereitung getrennt. Bei LiveCharts sollten bestehende Werte aktualisiert werden, statt die komplette Serie bei jedem Messpunkt auszutauschen.

## Sicherheits- und Wartungshinweise

- Keine Passwörter, Tokens, privaten IP-Listen oder API-Schlüssel in den Quellcode oder in Dokumentation schreiben.
- Netzwerkzugriffe gehören in `Services/`, nicht in XAML-Code-behind oder Views.
- Benutzerangaben und externe Antworten validieren, bevor sie in Prozesse, Dateien oder Netzwerkaufrufe gelangen.
- Adminrechte nicht voraussetzen, wenn eine Funktion ohne sie auskommt.
- Sensible WLAN-Daten nur dort verarbeiten, wo Windows es ausdrücklich erlaubt.
- Generierte Ordner wie `bin/`, `obj/`, `publish/` und `installer/output/` nicht committen.
- Kommentare sollen erklären, **warum** ein ungewöhnlicher Ablauf nötig ist, nicht bloß den Code wiederholen.

Die sichtbare Projektstruktur ist kein Sicherheitsmechanismus. Sicherheit entsteht durch Eingabevalidierung, minimale Berechtigungen, sichere Prozessaufrufe und den sorgfältigen Umgang mit sensiblen Daten.

## Lokale Prüfungen

```powershell
dotnet restore
dotnet build .\NetworkSharp.csproj
dotnet test .\NetworkSharp.Tests\NetworkSharp.Tests.csproj
git diff --check
```

Für eine Release-Veröffentlichung wird ein Tag wie `v2.1.1` zum GitHub-Repository gepusht. Der Workflow unter `.github/workflows/release.yml` baut daraus den per-user Installer.
