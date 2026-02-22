# Smart Koch App – Konzept

## Überblick
Die App dient zur Verwaltung von Lebensmitteln im Kühlschrank sowie zur Planung von Mahlzeiten.  
Sie unterstützt den Benutzer dabei, Lebensmittel rechtzeitig zu verbrauchen, Rezepte zu finden und automatisch Einkaufslisten zu erstellen.

## Ziel der App
- Kochplanung vereinfachen
- Lebensmittelverschwendung reduzieren
- Inspiration und schnelle Rezeptideen liefern

### Kühlschrankverwaltung
In der App werden gespeichert:
- Inhalt des Kühlschranks des Benutzers
- Ablaufdatum der einzelnen Lebensmittel
- Mengenangaben

### Rezeptverwaltung
Die App enthält:
- Eine Liste von Rezepten
- Schritt-für-Schritt Anleitungen
- Benötigte Zutaten pro Rezept
- Kategorien (z. B. vegetarisch, Dessert)

### Automatische Einkaufslisten
Die App kann automatisch Einkaufslisten generieren:
- Basierend auf ausgewählten Rezepten
- Basierend auf fehlenden Zutaten

### Rezeptvorschläge
Die App schlägt Rezepte vor basierend auf:
- Lebensmitteln, die bereits vorhanden sind
- Lebensmitteln, die bald ablaufen
- Optional: Benutzerpräferenzen

## Technische Umsetzung

### Programmiersprache & Framework
- **WPF (Windows Presentation Foundation)**
- **C#**

### Datenbank
- **SQLite** (lokale Datenbank)