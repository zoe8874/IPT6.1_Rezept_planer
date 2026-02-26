PRAGMA foreign_keys = ON;

-- Kategorien
CREATE TABLE IF NOT EXISTS ZutatKategorie (
    KategorieID   INTEGER PRIMARY KEY AUTOINCREMENT,
    Name          TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS RezeptKategorie (
    KategorieID   INTEGER PRIMARY KEY AUTOINCREMENT,
    Name          TEXT NOT NULL UNIQUE
);

-- Einheiten
CREATE TABLE IF NOT EXISTS Einheit (
    EinheitID     INTEGER PRIMARY KEY AUTOINCREMENT,
    Name          TEXT NOT NULL UNIQUE
);

-- Zutaten
CREATE TABLE IF NOT EXISTS Zutat (
    ZutatID           INTEGER PRIMARY KEY AUTOINCREMENT,
    Name              TEXT NOT NULL UNIQUE,
    KategorieID       INTEGER, -- optional
    StandardEinheitID INTEGER,
    FOREIGN KEY (KategorieID)   REFERENCES ZutatKategorie(KategorieID) ON DELETE SET NULL,
    FOREIGN KEY (StandardEinheitID) REFERENCES Einheit(EinheitID) ON DELETE SET NULL
);

-- Kühlschrankeinträge
CREATE TABLE IF NOT EXISTS KuehlschrankEintrag (
    EintragID     INTEGER PRIMARY KEY AUTOINCREMENT,
    ZutatID       INTEGER NOT NULL,
    Menge         DECIMAL NOT NULL,
    EinheitID     INTEGER, -- optional
    Ablaufdatum   DATE,
    FOREIGN KEY (ZutatID)   REFERENCES Zutat(ZutatID) ON DELETE CASCADE,
    FOREIGN KEY (EinheitID) REFERENCES Einheit(EinheitID) ON DELETE SET NULL
);

-- Rezepte
CREATE TABLE IF NOT EXISTS Rezept (
    RezeptID        INTEGER PRIMARY KEY AUTOINCREMENT,
    Name            TEXT NOT NULL,
    Beschreibung    TEXT,
    Anleitung       TEXT,
    KategorieID     INTEGER,
    Favorit         BOOLEAN DEFAULT 0,
    FOREIGN KEY (KategorieID) REFERENCES RezeptKategorie(KategorieID) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS RezeptZutat (
    RezeptID      INTEGER NOT NULL,
    ZutatID       INTEGER NOT NULL,
    Menge         DECIMAL,
    EinheitID     INTEGER,
    PRIMARY KEY (RezeptID, ZutatID),
    FOREIGN KEY (RezeptID)  REFERENCES Rezept(RezeptID) ON DELETE CASCADE,
    FOREIGN KEY (ZutatID)   REFERENCES Zutat(ZutatID) ON DELETE CASCADE,
    FOREIGN KEY (EinheitID) REFERENCES Einheit(EinheitID) ON DELETE SET NULL
);

-- Zutaten-Tags-Verknüpfung (z.B. Allergien, Diätpräferenzen)
CREATE TABLE IF NOT EXISTS Tag (
    TagID       INTEGER PRIMARY KEY AUTOINCREMENT,
    Name        TEXT NOT NULL UNIQUE
);


CREATE TABLE IF NOT EXISTS ZutatTag (
    ZutatID     INTEGER NOT NULL,
    TagID       INTEGER NOT NULL,
    PRIMARY KEY (ZutatID, TagID),
    FOREIGN KEY (ZutatID) REFERENCES Zutat(ZutatID) ON DELETE CASCADE,
    FOREIGN KEY (TagID)   REFERENCES Tag(TagID) ON DELETE CASCADE
);

-- Sopping
CREATE TABLE IF NOT EXISTS EinkaufsListe (
    EinkaufsListeID INTEGER PRIMARY KEY AUTOINCREMENT,
    ErstellDatum    DATE DEFAULT (DATE('now'))
);

CREATE TABLE IF NOT EXISTS EinkaufsListeEintrag (
    EintragID        INTEGER PRIMARY KEY AUTOINCREMENT,
    EinkaufsListeID  INTEGER NOT NULL,
    ZutatID          INTEGER NOT NULL,
    Menge            DECIMAL,
    EinheitID        INTEGER,
    FOREIGN KEY (EinkaufsListeID) REFERENCES EinkaufsListe(EinkaufsListeID) ON DELETE CASCADE,
    FOREIGN KEY (ZutatID)          REFERENCES Zutat(ZutatID) ON DELETE CASCADE,
    FOREIGN KEY (EinheitID)        REFERENCES Einheit(EinheitID) ON DELETE SET NULL
);