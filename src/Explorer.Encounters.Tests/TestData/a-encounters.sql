INSERT INTO encounters."Encounters"
("Id", "Name", "Description", "Location", "ExperiencePoints", "Status", "Type", "IsRequiredForPointCompletion", "ApprovalStatus", "TourPointId")
VALUES
    (-1, 'Test Encounter One', 'First test encounter', '{{"latitude":45.2671,"longitude":19.8335}}', 100, 0, 0, false, 0, -1),
    (-2, 'Test Encounter Two', 'Second test encounter', '{{"latitude":44.7866,"longitude":20.4489}}', 200, 0, 1, false, 0, -1),
    (-3, 'Test Encounter Three', 'Third test encounter', '{{"latitude":45.8150,"longitude":15.9819}}', 300, 1, 0, false, 0, -2),
    (-4, 'Test Misc Encounter', 'Misc encounter for CompleteEncounter tests', '{{"latitude":45.3000,"longitude":19.8000}}', 150, 0, 2, false, 0, -1),
    (-5, 'Not Misc Encounter for Test', 'Used only for NotMisc test', '{{"latitude":45.3100,"longitude":19.8100}}', 50, 0, 0, false, 1, -1),
    (-6, 'Pending Encounter', 'This is pending', '{{"latitude":45.0,"longitude":19.0}}', 100, 0, 0, false, 0, -1);
INSERT INTO encounters."SocialEncounters"
("Id", "MinimumParticipants", "ActivationRadiusMeters")
VALUES
(-1, 2, 100.0);

INSERT INTO encounters."HiddenLocationEncounters"
("Id", "ImageUrl", "PhotoPoint", "ActivationRadiusMeters")
VALUES
(-2, 'http://example.com/image.jpg', '{{"latitude":44.7866,"longitude":20.4489}}', 50.0);

INSERT INTO encounters."EncounterParticipants"
("Id", "UserId", "Level", "ExperiencePoints", "LastCompletedEncounterAtUtc")
VALUES
(-1, -21, 12, 1500, '2024-01-15T10:00:00Z'),
(-2, -22, 6, 743, '2024-01-16T11:30:00Z'),
(-3, -23, 10, 1500, '2024-01-17T12:00:00Z');