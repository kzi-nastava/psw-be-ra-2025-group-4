INSERT INTO payments."AffiliateCodes"
("Id", "Code", "AuthorId", "TourId", "CreatedAt", "Active")
VALUES
(-1, 'AFFTOUR1', -11, -2, CURRENT_TIMESTAMP - INTERVAL '10 days', true),
(-2, 'AFFTOUR2', -11, -2, CURRENT_TIMESTAMP - INTERVAL '5 days', true),
(-3, 'AFFGLOBAL1', -11, NULL, CURRENT_TIMESTAMP - INTERVAL '3 days', true),
(-4, 'AFFGLOBAL2', -11, NULL, CURRENT_TIMESTAMP - INTERVAL '1 days', false),
(-5, 'AFFOTHER1', -12, -3, CURRENT_TIMESTAMP - INTERVAL '2 days', true);