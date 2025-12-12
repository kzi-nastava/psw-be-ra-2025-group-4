INSERT INTO blog."BlogPosts"
("Id", "Title", "Description", "UserId", "Images", "CreatedAt", "LastUpdatedAt", "Status", "Popularity")
VALUES
    (-1, 'Test Blog 1', 'Opis bloga 1', 1, ARRAY['slika.jpg'], NOW(), NULL, 0, 0), 
    (-2, 'Test Blog 2', 'Opis bloga 2', 1, ARRAY['slika.jpg'], NOW(), NULL, 1, 1), 
    (-3, 'Test Blog 3', 'Opis bloga 3', 1, ARRAY['slika.jpg'], NOW(), NULL, 2, 2),  
    (-4, 'Test Blog 4', 'Opis bloga 4', 1, ARRAY['slika.jpg'], NOW(), NULL, 0, 0),  
    (-5, 'Test Blog 5', 'Opis bloga 5', 2, ARRAY['slika.jpg'], NOW(), NULL, 0, 0); 


    INSERT INTO blog."Comments"
("Id", "BlogId", "UserId", "Text", "CreatedAt", "LastModifiedAt")
VALUES
    (-1, -2, 1, 'Komentar na objavljen blog', NOW(), NULL),            
    (-2, -2, 2, 'Drugi user komentar', NOW(), NULL),                   
    (-3, -2, 1, 'Stari komentar', NOW() - INTERVAL '20 minutes', NULL);