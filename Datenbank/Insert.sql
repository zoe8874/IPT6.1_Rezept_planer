PRAGMA foreign_keys = ON;

-- Zutat-Kategorien
INSERT INTO ZutatKategorie (Name) VALUES
    ('Gemüse'),
    ('Obst'),
    ('Fleisch'),
    ('Fisch'),
    ('Milchprodukte'),
    ('Getreide'),
    ('Gewürze'),
    ('Öle & Fette'),
    ('Getränke'),
    ('Sonstiges');

-- Rezept-Kategorien
INSERT INTO RezeptKategorie (Name) VALUES
    ('Vegetarisch'),
    ('Hauptgericht'),
    ('Beilage'),
    ('Dessert'),
    ('Frühstück'),
    ('Suppe'),
    ('Salat'),
    ('Vegan');

-- Einheiten
INSERT INTO Einheit (Name) VALUES
    ('g'),
    ('kg'),
    ('ml'),
    ('l'),
    ('Stück'),
    ('EL'),
    ('TL'),
    ('Becher'),
    ('Packung'),
    ('Dose');

-- Zutaten
INSERT INTO Zutat (Name, KategorieID, StandardEinheitID) VALUES
    -- Gemüse
    ('Tomaten', 1, 1),
    ('Zwiebeln', 1, 1),
    ('Paprika', 1, 1),
    ('Zucchini', 1, 1),
    ('Brokkoli', 1, 1),
    ('Spinat', 1, 1),
    ('Möhren', 1, 1),
    ('Knoblauch', 1, 1),
    
    -- Obst
    ('Äpfel', 2, 5),
    ('Bananen', 2, 5),
    ('Erdbeeren', 2, 1),
    ('Orangen', 2, 5),
    
    -- Fleisch
    ('Hähnchen', 3, 1),
    ('Rindfleisch', 3, 1),
    ('Schweinefleisch', 3, 1),
    
    -- Fisch
    ('Lachs', 4, 1),
    ('Forelle', 4, 1),
    
    -- Milchprodukte
    ('Milch', 5, 3),
    ('Käse', 5, 1),
    ('Eier', 5, 5),
    ('Joghurt', 5, 8),
    ('Butter', 5, 1),
    
    -- Getreide
    ('Mehl', 6, 1),
    ('Reis', 6, 1),
    ('Nudeln', 6, 1),
    ('Brot', 6, 5),
    
    -- Gewürze
    ('Salz', 7, 1),
    ('Pfeffer', 7, 1),
    ('Paprikapulver', 7, 1),
    
    -- Öle & Fette
    ('Olivenöl', 8, 3),
    ('Butter Premium', 8, 1),
    
    -- Sonstiges
    ('Tomatensose', 10, 3),
    ('Zucker', 10, 1),
    ('Basilikim', 10, 1),
    ('Oregano', 10, 1);

INSERT INTO Tag (Name) VALUES
    ('Vegetarisch'),
    ('Vegan'),
    ('Glutenfrei'),
    ('Laktosefrei'),
    ('Nussallergie'),
    ('Bio'),
    ('Regional'),
    ('Saisonal');

INSERT INTO ZutatTag (ZutatID, TagID) VALUES
    (1, 6),      -- Tomaten: Bio
    (2, 6),      -- Zwiebeln: Bio
    (5, 1),      -- Brokkoli: Vegetarisch
    (6, 1),      -- Spinat: Vegetarisch
    (9, 7),      -- Äpfel: Regional
    (10, 8),     -- Bananen: Saisonal
    (19, 1),     -- Eier: Vegetarisch
    (22, 1),     -- Joghurt: Vegetarisch
    (23, 1),     -- Butter: Vegetarisch
    (24, 3),     -- Mehl: Glutenfrei
    (28, 2);     -- Basilikum: Vegan

INSERT INTO KuehlschrankEintrag (ZutatID, Menge, EinheitID, Ablaufdatum) VALUES
    (1, 500, 1, DATE('now', '+7 days')),      -- 500g Tomaten (in 7 Tagen)
    (2, 3, 5, DATE('now', '+10 days')),       -- 3 Zwiebeln (in 10 Tagen)
    (3, 2, 5, DATE('now', '+5 days')),        -- 2 Paprika (in 5 Tagen)
    (5, 800, 1, DATE('now', '+3 days')),      -- 800g Brokkoli (in 3 Tagen - BALD!)
    (6, 200, 1, DATE('now', '+2 days')),      -- 200g Spinat (in 2 Tagen - SEHR BALD!)
    (7, 600, 1, DATE('now', '+8 days')),      -- 600g Möhren
    (8, 4, 5, DATE('now', '+14 days')),       -- 4 Knoblauchzehen
    (9, 5, 5, DATE('now', '+4 days')),        -- 5 Äpfel
    (10, 3, 5, DATE('now', '+6 days')),       -- 3 Bananen
    (13, 800, 1, DATE('now', '+5 days')),     -- 800g Hähnchen
    (18, 1000, 3, DATE('now', '+9 days')),    -- 1000ml Milch
    (19, 300, 1, DATE('now', '+12 days')),    -- 300g Käse
    (20, 8, 5, DATE('now', '+10 days')),      -- 8 Eier
    (22, 500, 8, DATE('now', '+6 days')),     -- 500g Joghurt
    (23, 200, 1, DATE('now', '+20 days')),    -- 200g Butter
    (24, 1000, 1, DATE('now', '+30 days')),   -- 1000g Mehl
    (25, 500, 1, DATE('now', '+180 days')),   -- 500g Reis
    (26, 500, 1, DATE('now', '+60 days')),    -- 500g Nudeln
    (27, 1, 5, DATE('now', '+3 days')),       -- 1 Brot (in 3 Tagen)
    (30, 500, 3, DATE('now', '+90 days')),    -- 500ml Olivenöl
    (32, 400, 3, DATE('now', '+15 days')),    -- 400ml Tomatensose
    (34, 10, 1, DATE('now', '+365 days'));    -- 10g Basilikum

-- Rezepte
INSERT INTO Rezept (Name, Beschreibung, Anleitung, KategorieID, Favorit) VALUES
    -- Rezept 1: Pasta Tomato
    ('Pasta Tomato', 
     'Klassische Pasta mit Tomatensose und Basilikum',
     '1. Wasser zum Kochen bringen und salzen\n2. Nudeln nach Packungsanleitung kochen\n3. Zwiebeln und Knoblauch in Olivenöl anbraten\n4. Tomatensose hinzufügen, würzen und köcheln lassen\n5. Basilikum hinzufügen\n6. Nudeln abtropfen lassen und mit der Sose mischen\n7. Mit Basilikum und Parmesan servieren',
     1, 1),
     
    -- Rezept 2: Spinat-Auflauf
    ('Spinat-Auflauf',
     'Cremiger Auflauf mit Spinat und Käse',
     '1. Ofen auf 180°C vorheizen\n2. Spinat kurz blanchieren und ausquetschen\n3. Butter und Mehl zu einer Mehlschwitze verarbeiten\n4. Milch langsam einrühren, Sauce köcheln lassen\n5. Spinat und Käse in die Sauce rühren\n6. In Auflaufform füllen und 25 Minuten backen',
     1, 0),
     
    -- Rezept 3: Brokkoli-Auflauf
    ('Brokkoli-Auflauf',
     'Gesundes Gemüse mit überbackenem Käse',
     '1. Brokkoli in Salzwasser bissfest garen\n2. Butter schmelzen und Mehl hinzufügen\n3. Milch einrühren und Sauce köcheln lassen\n4. Brokkoli in Auflaufform anordnen\n5. Sauce darüber giessen und mit Käse bestreuen\n6. Bei 180°C 20 Minuten backen',
     2, 0),
     
    -- Rezept 4: Hähnchen-Pfanne
    ('Hähnchen mit Paprika und Zucchini',
     'Schnelle und einfache Hähnchen-Pfanne',
     '1. Hähnchen in Würfel schneiden\n2. Knoblauch und Zwiebeln anbraten\n3. Hähnchen hinzufügen und anbraten\n4. Paprika und Zucchini schneiden und hinzufügen\n5. 10 Minuten köcheln lassen\n6. Mit Salz und Pfeffer würzen\n7. Mit Reis oder Nudeln servieren',
     2, 1),
     
    -- Rezept 5: Gemüsesuppe
    ('Gemüsesuppe',
     'Leichte und nahrhafte Suppe mit saisonalem Gemüse',
     '1. Zwiebel und Knoblauch in Öl anbraten\n2. Möhren, Tomaten und Brokkoli hinzufügen\n3. Mit Wasser oder Brühe aufgiessen\n4. 15 Minuten köcheln lassen\n5. Mit Salz und Pfeffer würzen\n6. Mit Basilikum garnieren',
     6, 0),
     
    -- Rezept 6: Spinatrollen
    ('Spinatrollen mit Käse',
     'Grüne Teigröllchen mit cremiger Spinat-Käse-Füllung',
     '1. Teig nach Rezept zubereiten\n2. Spinat-Käse-Mischung herstellen\n3. Teig ausrollen und mit Mischung bestreichen\n4. Rollen schneiden\n5. Bei 200°C 20 Minuten backen',
     5, 0);

-- Pasta Tomato
INSERT INTO RezeptZutat (RezeptID, ZutatID, Menge, EinheitID) VALUES
    (1, 26, 400, 1),      -- 400g Nudeln
    (1, 32, 400, 3),      -- 400ml Tomatensose
    (1, 2, 1, 5),         -- 1 Zwiebel
    (1, 8, 2, 5),         -- 2 Knoblauchzehen
    (1, 30, 2, 6),        -- 2 EL Olivenöl
    (1, 34, 1, 6),        -- 1 EL frisches Basilikum
    (1, 29, 1, 1),        -- 1g Salz
    (1, 31, 1, 1);        -- 1g Pfeffer

-- Spinat-Auflauf
INSERT INTO RezeptZutat (RezeptID, ZutatID, Menge, EinheitID) VALUES
    (2, 6, 500, 1),       -- 500g Spinat
    (2, 23, 50, 1),       -- 50g Butter
    (2, 24, 40, 1),       -- 40g Mehl
    (2, 18, 400, 3),      -- 400ml Milch
    (2, 19, 200, 1),      -- 200g Käse
    (2, 29, 1, 1),        -- 1g Salz
    (2, 31, 1, 1);        -- 1g Pfeffer

-- Brokkoli-Auflauf
INSERT INTO RezeptZutat (RezeptID, ZutatID, Menge, EinheitID) VALUES
    (3, 5, 800, 1),       -- 800g Brokkoli
    (3, 23, 50, 1),       -- 50g Butter
    (3, 24, 40, 1),       -- 40g Mehl
    (3, 18, 400, 3),      -- 400ml Milch
    (3, 19, 200, 1),      -- 200g Käse
    (3, 29, 1, 1),        -- 1g Salz
    (3, 31, 1, 1);        -- 1g Pfeffer

-- Hähnchen mit Paprika und Zucchini
INSERT INTO RezeptZutat (RezeptID, ZutatID, Menge, EinheitID) VALUES
    (4, 13, 500, 1),      -- 500g Hähnchen
    (4, 3, 2, 5),         -- 2 Paprika
    (4, 4, 2, 5),         -- 2 Zucchini
    (4, 2, 1, 5),         -- 1 Zwiebel
    (4, 8, 2, 5),         -- 2 Knoblauchzehen
    (4, 30, 2, 6),        -- 2 EL Olivenöl
    (4, 29, 1, 1),        -- 1g Salz
    (4, 31, 1, 1);        -- 1g Pfeffer

-- Gemüsesuppe
INSERT INTO RezeptZutat (RezeptID, ZutatID, Menge, EinheitID) VALUES
    (5, 2, 2, 5),         -- 2 Zwiebeln
    (5, 8, 2, 5),         -- 2 Knoblauchzehen
    (5, 7, 3, 5),         -- 3 Möhren
    (5, 1, 400, 1),       -- 400g Tomaten
    (5, 5, 300, 1),       -- 300g Brokkoli
    (5, 30, 2, 6),        -- 2 EL Olivenöl
    (5, 34, 1, 6),        -- 1 EL Basilikum
    (5, 29, 2, 1),        -- 2g Salz
    (5, 31, 1, 1);        -- 1g Pfeffer

-- Spinatrollen mit Käse
INSERT INTO RezeptZutat (RezeptID, ZutatID, Menge, EinheitID) VALUES
    (6, 24, 300, 1),      -- 300g Mehl
    (6, 20, 3, 5),        -- 3 Eier
    (6, 18, 100, 3),      -- 100ml Milch
    (6, 6, 400, 1),       -- 400g Spinat
    (6, 19, 250, 1),      -- 250g Käse
    (6, 23, 50, 1),       -- 50g Butter
    (6, 29, 1, 1),        -- 1g Salz
    (6, 31, 1, 1);        -- 1g Pfeffer

INSERT INTO EinkaufsListe (ErstellDatum) VALUES
    (DATE('now', '-3 days')),  -- Vor 3 Tagen erstellte Liste
    (DATE('now'));              -- Heute erstellte Liste

-- Einkaufsliste 1 (alte Liste)
INSERT INTO EinkaufsListeEintrag (EinkaufsListeID, ZutatID, Menge, EinheitID) VALUES
    (1, 13, 500, 1),      -- 500g Hähnchen
    (1, 3, 2, 5),         -- 2 Paprika
    (1, 4, 2, 5),         -- 2 Zucchini
    (1, 25, 1000, 1),     -- 1kg Reis
    (1, 30, 500, 3);      -- 500ml Olivenöl

-- Einkaufsliste 2 (neue Liste)
INSERT INTO EinkaufsListeEintrag (EinkaufsListeID, ZutatID, Menge, EinheitID) VALUES
    (2, 11, 500, 1),      -- 500g Erdbeeren
    (2, 14, 500, 1),      -- 500g Rindfleisch
    (2, 7, 1000, 1),      -- 1kg Möhren
    (2, 21, 500, 8),      -- 500g Joghurt
    (2, 26, 500, 1);      -- 500g Nudeln

-- Erfolgreiche Nachricht ausgeben
SELECT 'Beispieldaten erfolgreich eingefügt!' AS Status;
