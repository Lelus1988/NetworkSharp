# NetworkSharp Updates

## Aktuelle Version: 2.1

NetworkSharp ist ein kompaktes Windows-Werkzeug, mit dem du deine Netzwerkverbindung prüfen, beobachten und besser verstehen kannst. Die wichtigsten Informationen stehen direkt in der App, ohne dass du mehrere Systemtools öffnen musst.

> [!NOTE]
> Die Versionsnummer lautet `2.1.0`. Für den normalen Download ist der fertige Windows-Installer gedacht. Der Quellcode bleibt auf GitHub sichtbar, muss aber nicht heruntergeladen oder selbst gebaut werden.

## Die wichtigsten Funktionen

### Was NetworkSharp ausmacht

- Netzwerk-Monitor für Download, Upload, Spitzenwerte und Datenverbrauch
- Ping-Monitor für Antwortzeit, Durchschnitt, Minimum, Maximum und Paketverlust
- LAN-Geräte-Erkennung für Geräte im eigenen Netzwerk
- Port-Scanner für erreichbare Dienste
- DNS-Tester für die Prüfung der Namensauflösung
- Speed-Test für Download- und Upload-Geschwindigkeit
- IP- und Standortinformationen für die aktuelle öffentliche Verbindung
- WLAN-Profil-Ansicht für vorhandene Netzwerkprofile

### Was in 2.1 neu ist

Die Oberfläche wurde zu einer übersichtlichen Network Console überarbeitet. Die Anwendung soll technisch wirken, aber trotzdem schnell verständlich bleiben.

- dunkles Tech-Design mit klaren Statusfarben
- überarbeitete Navigation mit sichtbarem aktivem Bereich
- `Network#` und `NetworkSharp` als Hashtag-Logo im Kopfbereich
- Statusanzeige für den lokalen Netzwerk-Knoten
- ruhigere Karten, Abstände und Beschriftungen für längere Nutzung
- abgerundete Textfelder mit dunklem Eingabefeld und blauem Fokus-Rand
- moderne blaue Buttons mit weichen Ecken und Hover-/Klick-Zuständen
- einheitliche ComboBoxen für Auswahlfelder wie Ping-Intervall und Testserver
- weniger Standard-WPF-Optik und ein konsistenteres Erscheinungsbild über alle Ansichten

### Netzwerk-Monitor

Der Netzwerk-Monitor zeigt aktuelle und historische Daten in einer einheitlichen Kurve.

- Download und Upload werden getrennt und farblich klar erkennbar dargestellt
- Unterstützte Zeiträume sind Live, 24 Stunden, 7 Tage und 30 Tage
- historische Werte werden gleichmäßig auf der Kurve verteilt
- der Live-Modus schiebt neue Messwerte sichtbar von rechts hinein
- der tägliche Datenverbrauch wird separat dargestellt

### Ping-Monitor

Der Ping-Bereich zeigt nicht nur einen einzelnen Wert, sondern macht die Stabilität der Verbindung sichtbar.

- Verlauf über ungefähr zwei Minuten mit 121 stabilen Punkten
- Aktuelle Antwortzeit, Durchschnitt, Minimum und Maximum auf einen Blick
- eine gestrichelte Durchschnittslinie macht Schwankungen leichter sichtbar
- verlorene Pakete lassen die Kurve nicht unnötig springen
- Ein Paketstreifen zeigt die letzten 60 Pakete einzeln an
- Erfolgreiche Pakete und Paketverluste werden farblich unterschieden
- Paketverlust wird laufend in Prozent berechnet
- das Ping-Intervall kann auf 1, 2, 5 oder 10 Sekunden gestellt werden
- Statistik und Diagramm werden bei einer neuen Messung zurückgesetzt

### Stabilität

- WPF-Bindings wurden gegen schreibgeschützte Eigenschaften abgesichert
- UI-Updates aus Hintergrundprozessen laufen über den Dispatcher
- Ping-Ereignisse werden beim Verlassen der Ansicht sauber abgemeldet
- Live-Timer werden beim Verlassen der Netzwerkansicht beendet
- Diagrammwerte werden laufend aktualisiert, ohne die komplette Serie auszutauschen

> [!IMPORTANT]
> NetworkSharp 2.1 enthält bewusst **keine VPN-Funktion**. Die Anwendung misst und analysiert deine Verbindung, baut aber keinen VPN-Tunnel auf und verändert deine öffentliche IP nicht.

## Download und Installation

Die neueste Version bekommst du immer hier:

- [Neueste Version herunterladen](https://github.com/Lelus1988/NetworkSharp/releases/latest)
- [NetworkSharp auf GitHub](https://github.com/Lelus1988/NetworkSharp)
- [GitHub Actions und Build-Status](https://github.com/Lelus1988/NetworkSharp/actions)

> [!TIP]
> Lade auf der Release-Seite unter **Assets** die Datei `NetworkSharp-Setup.exe` herunter. Der Installer installiert NetworkSharp nur für deinen Benutzer unter `%LOCALAPPDATA%\Programs\NetworkSharp` und benötigt dafür normalerweise keine Administratorrechte.

1. Öffne die [neueste GitHub-Version](https://github.com/Lelus1988/NetworkSharp/releases/latest).
2. Lade `NetworkSharp-Setup.exe` unter **Assets** herunter.
3. Starte die Datei und wähle bei Bedarf eine Desktop-Verknüpfung.
4. NetworkSharp wird in deinem Benutzerprofil installiert und kann direkt gestartet werden.

> [!WARNING]
> Einige Netzwerkfunktionen hängen von Windows-Berechtigungen und der jeweiligen Netzwerkkonfiguration ab. Der Port-Scanner, Ping und die Anzeige lokaler Netzwerkdaten sollten normalerweise ohne Administratorrechte funktionieren. Das Auslesen gespeicherter WLAN-Passwörter kann dagegen Administratorrechte oder zusätzliche Windows-Berechtigungen benötigen.

## Ältere Versionen

Ältere Versionen werden hier nicht mehr beschrieben, bleiben aber zum Download verfügbar:

- [Alle GitHub-Releases](https://github.com/Lelus1988/NetworkSharp/releases)
- [Alle Versionen und Tags](https://github.com/Lelus1988/NetworkSharp/tags)
- [Release v1.0](https://github.com/Lelus1988/NetworkSharp/releases/tag/v1.0)
- [Release v2.0](https://github.com/Lelus1988/NetworkSharp/releases/tag/v2.0)
- [Release v2.1](https://github.com/Lelus1988/NetworkSharp/releases/tag/v2.1)

## Hinweis zu Updates

Bei einem Update startest du einfach den Installer der neuesten Version. Die Installation bleibt in deinem Benutzerprofil und benötigt normalerweise keine Administratorrechte. Wenn du ganz vorsichtig aktualisieren möchtest, kannst du die ältere Version zunächst behalten und die neue Version testen.

Lokale Daten und Einstellungen sollten nicht ungeprüft gelöscht werden. Wenn du einen Fehler bemerkst, notiere dir am besten zuerst die verwendete Version und die Windows-Version.

Wenn ein Fehler auftritt, kannst du ihn im [GitHub-Issue-Bereich](https://github.com/Lelus1988/NetworkSharp/issues) melden. Am hilfreichsten sind die verwendete Version, Windows-Version und eine kurze Beschreibung dessen, was passiert ist.
