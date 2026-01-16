INSERT INTO encounters."Encounters"
("Id", "Name", "Description", "Location", "ExperiencePoints", "Status", "Type", "IsRequiredForPointCompletion", "ApprovalStatus")
VALUES
(-1, 'Test Encounter One', 'First test encounter', '{{"latitude":45.2671,"longitude":19.8335}}', 100, 0, 0, false, 0),
(-2, 'Test Encounter Two', 'Second test encounter', '{{"latitude":44.7866,"longitude":20.4489}}', 200, 0, 1, false, 0),
(-3, 'Test Encounter Three', 'Third test encounter', '{{"latitude":45.8150,"longitude":15.9819}}', 300, 1, 0, false, 0);

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
(-2, -22, 6, 743, '2024-01-16T11:30:00Z');