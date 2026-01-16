INSERT INTO tours."Sales" ("Id", "AuthorId", "TourIds", "StartDate", "EndDate", "DiscountPercent", "IsActive")
VALUES (100, -11, ARRAY[-2, -4], NOW() - INTERVAL '1 day', NOW() + INTERVAL '7 days', 20, true);

INSERT INTO tours."Sales" ("Id", "AuthorId", "TourIds", "StartDate", "EndDate", "DiscountPercent", "IsActive")
VALUES (101, -11, ARRAY[-2], NOW() - INTERVAL '1 day', NOW() + INTERVAL '5 days', 30, true);

INSERT INTO tours."Sales" ("Id", "AuthorId", "TourIds", "StartDate", "EndDate", "DiscountPercent", "IsActive")
VALUES (102, -11, ARRAY[-4], NOW() - INTERVAL '1 day', NOW() + INTERVAL '10 days', 15, true);

INSERT INTO tours."Sales" ("Id", "AuthorId", "TourIds", "StartDate", "EndDate", "DiscountPercent", "IsActive")
VALUES (103, -11, ARRAY[-99], NOW() - INTERVAL '20 days', NOW() - INTERVAL '10 days', 25, true);