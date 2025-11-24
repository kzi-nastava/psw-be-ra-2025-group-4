INSERT INTO tours."Tours"(
    "Id", "Name", "Description", "Difficulty", "AuthorId", "Price", "Status", "Tags")
VALUES (-1, 'Test Tura 1', 'Opis 1', 0, 2, 10.00, 0, ARRAY['test']);

INSERT INTO tours."Tours"(
    "Id", "Name", "Description", "Difficulty", "AuthorId", "Price", "Status", "Tags")
VALUES (-2, 'Test Tura 2', 'Opis 2', 1, 2, 20.00, 1, ARRAY['planina','avantura']);

INSERT INTO tours."Tours"(
    "Id", "Name", "Description", "Difficulty", "AuthorId", "Price", "Status", "Tags")
VALUES (-3, 'Test Tura 3', 'Opis 3', 2, 2, 30.00, 2, ARRAY['test']);
