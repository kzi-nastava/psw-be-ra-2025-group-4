INSERT INTO encounters."Encounters"
("Id", "Name", "Description", "Location", "ExperiencePoints", "Status", "Type", "IsRequiredForPointCompletion", "ApprovalStatus", "TourPointId")
VALUES
    (-1, 'Test Encounter One', 'First test encounter', '{{"latitude":45.2671,"longitude":19.8335}}', 100, 0, 0, false, 0, -1),
    (-2, 'Test Encounter Two', 'Second test encounter', '{{"latitude":44.7866,"longitude":20.4489}}', 200, 0, 1, false, 0, -1),
    (-3, 'Test Encounter Three', 'Third test encounter', '{{"latitude":45.8150,"longitude":15.9819}}', 300, 1, 0, false, 0, -2),
    (-4, 'Test Misc Encounter', 'Misc encounter for CompleteEncounter tests', '{{"latitude":45.3000,"longitude":19.8000}}', 150, 0, 2, false, 0, -1),
    (-5, 'Not Misc Encounter for Test', 'Used only for NotMisc test', '{{"latitude":45.3100,"longitude":19.8100}}', 50, 0, 0, false, 1, -1),
    (-6, 'Pending Encounter', 'This is pending', '{{"latitude":45.0,"longitude":19.0}}', 100, 0, 3, false, 0, -1),
    (-7, 'Test Encounter 3', 'This is pending', '{{"latitude":45.0,"longitude":19.0}}', 100, 1, 3, false, 0, -1),
    (-8, 'Test Encounter 4', 'awdaw', '{{"latitude":45.0,"longitude":19.0}}', 100, 1, 3, false, 0, -101),
    (-9, 'Test Encounter 5', 'awdaw', '{{"latitude":45.0,"longitude":19.0}}', 100, 1, 3, false, 0, -101),
    (-10, 'Test Encounter 6', 'awdaw', '{{"latitude":45.0,"longitude":19.0}}', 100, 1, 1, false, 0, -1),
    (-11, 'Test Encounter 6', 'awdaw', '{{"latitude":45.0,"longitude":19.0}}', 100, 1, 1, false, 0, -1),
    (-12, 'Pending Encounter', 'This is pending', '{{"latitude":45.0,"longitude":19.0}}', 100, 0, 3, false, 1, -1);


INSERT INTO encounters."SocialEncounters"
("Id", "MinimumParticipants", "ActivationRadiusMeters")
VALUES
(-1, 2, 100.0);

INSERT INTO encounters."HiddenLocationEncounters"
("Id", "ImageUrl", "PhotoPoint", "ActivationRadiusMeters")
VALUES
(-2, 'http://example.com/image.jpg', '{{"latitude":44.7866,"longitude":20.4489}}', 200.0),
(-10, 'http://example.com/image.jpg', '{{"latitude":44.7866,"longitude":20.4489}}', 200.0),
(-11, 'http://example.com/image.jpg', '{{"latitude":44.7866,"longitude":20.4489}}', 200.0);


INSERT INTO encounters."QuizEncounters"
("Id", "TimeLimit")
VALUES
(-6, 60);

INSERT INTO encounters."QuizQuestions"
("Id", "Text", "QuizEncounterId")
VALUES
(-70, 'Seed question?', -6);

INSERT INTO encounters."QuizAnswers"
("Id", "Text", "IsCorrect", "QuizQuestionId")
VALUES
(-700, 'Yes', true, -70),
(-701, 'No', false, -70);


INSERT INTO encounters."EncounterParticipants"
("Id", "UserId", "Level", "ExperiencePoints", "LastCompletedEncounterAtUtc")
VALUES
(-1, -21, 12, 1500, '2024-01-15T10:00:00Z'),
(-2, -22, 6, 743, '2024-01-16T11:30:00Z'),
(-3, -23, 10, 1500, '2024-01-17T12:00:00Z');