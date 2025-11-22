INSERT INTO blog."BlogPosts" ("Id", "Title", "Description", "UserId", "Images", "CreatedAt")
VALUES
    (-1, 'Test Blog 1', 'Opis bloga 1', 1, '{"slika1.jpg"}', NOW()),
    (-2, 'Test Blog 2', 'Opis bloga 2', 1, '{"slika2.jpg"}', NOW()),
    (-3, 'Test Blog 3', 'Opis bloga 3', 2, '{"slika3.jpg"}', NOW());
