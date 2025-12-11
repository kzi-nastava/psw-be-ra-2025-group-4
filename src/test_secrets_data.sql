-- PROVERA: Da li autor postoji?
SELECT "Id", "Username" FROM stakeholders."People" WHERE "Id" = 1;

-- PROVERA: Da li turista postoji?
SELECT "Id", "Username" FROM stakeholders."People" WHERE "Id" = 1;

-- PROVERA: Da li tura već postoji?
SELECT "Id", "Name" FROM tours."Tours";

-- Ako nema ture, KREIRAJ:
INSERT INTO tours."Tours" ("Name", "Description", "Difficulty", "Tags", "Status", "AuthorId", "Price", "TransportDuration", "PublishedAt", "ArchivedAt")
VALUES ('Tajna šetnja', 'Otkrijte tajne', 0, ARRAY['city'], 1, 1, 0.00, '[]'::jsonb, NOW(), NULL)
RETURNING "Id";

-- PROVERA: Koji ID je dobila tura?
SELECT "Id" as tour_id FROM tours."Tours" WHERE "Name" = 'Tajna šetnja';

-- SADA KREIRAJ TAČKE (zameni 1 sa tour_id iz prethodnog upita)
INSERT INTO tours."TourPoints" ("TourId", "Name", "Description", "Latitude", "Longitude", "Order", "ImageFileName", "Secret")
VALUES 
    ((SELECT "Id" FROM tours."Tours" WHERE "Name" = 'Tajna šetnja' LIMIT 1), 'Tačka 1', 'Opis 1', 44.8233, 20.4500, 1, NULL, 'Tajna 1: Prva tajna'),
    ((SELECT "Id" FROM tours."Tours" WHERE "Name" = 'Tajna šetnja' LIMIT 1), 'Tačka 2', 'Opis 2', 44.8186, 20.4600, 2, NULL, 'Tajna 2: Druga tajna'),
    ((SELECT "Id" FROM tours."Tours" WHERE "Name" = 'Tajna šetnja' LIMIT 1), 'Tačka 3', 'Opis 3', 44.8170, 20.4600, 3, NULL, NULL);

-- KREIRAJ TOKEN
INSERT INTO tours."TourPurchaseTokens" ("TouristId", "TourId", "PurchasedAt")
VALUES (1, (SELECT "Id" FROM tours."Tours" WHERE "Name" = 'Tajna šetnja' LIMIT 1), NOW());

-- KREIRAJ EXECUTION
INSERT INTO tours."TourExecutions" ("TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (1, (SELECT "Id" FROM tours."Tours" WHERE "Name" = 'Tajna šetnja' LIMIT 1), NOW(), 44.8200, 20.4500, 0, NULL, NOW());

-- KOMPLETIRAJ PRVE DVE TAČKE
INSERT INTO tours."CompletedTourPoint" ("TourPointId", "CompletedAt", "TourExecutionId")
SELECT tp."Id", NOW(), te."Id"
FROM tours."TourPoints" tp
CROSS JOIN tours."TourExecutions" te
WHERE tp."TourId" = (SELECT "Id" FROM tours."Tours" WHERE "Name" = 'Tajna šetnja' LIMIT 1)
  AND tp."Order" IN (1, 2)
  AND te."TouristId" = 1
  AND te."TourId" = (SELECT "Id" FROM tours."Tours" WHERE "Name" = 'Tajna šetnja' LIMIT 1);
