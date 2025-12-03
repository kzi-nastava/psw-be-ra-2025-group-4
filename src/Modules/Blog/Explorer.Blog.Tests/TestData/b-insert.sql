INSERT INTO blog."BlogPosts"
("Id", "Title", "Description", "UserId", "Images", "CreatedAt", "LastUpdatedAt", "Status")
VALUES
    (-1, 'Test Blog 1', 'Opis bloga 1', 1, ARRAY['slika.jpg'], NOW(), NULL, 0),
    (-2, 'Test Blog 2', 'Opis bloga 2', 1, ARRAY['slika.jpg'], NOW(), NULL, 1),
    (-3, 'Test Blog 3', 'Opis bloga 3', 1, ARRAY['slika.jpg'], NOW(), NULL, 2),
    (-4, 'Test Blog 4', 'Opis bloga 4', 1, ARRAY['slika.jpg'], NOW(), NULL, 0),
    (-5, 'Test Blog 4', 'Opis bloga 5', 2, ARRAY['slika.jpg'], NOW(), NULL, 0);
