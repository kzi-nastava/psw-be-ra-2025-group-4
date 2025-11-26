INSERT INTO blog."BlogPosts" ("Id", "Title", "Description", "UserId", "Images", "CreatedAt")
VALUES
    (-1, 'Test Blog 1', 'Opis bloga 1', 1, ARRAY['slika.jpg'], NOW()),
    (-2, 'Test Blog 2', 'Opis bloga 2', 1, ARRAY['slika.jpg'], NOW()),
    (-3, 'Test Blog 3', 'Opis bloga 3', 2, ARRAY['slika.jpg'], NOW());
