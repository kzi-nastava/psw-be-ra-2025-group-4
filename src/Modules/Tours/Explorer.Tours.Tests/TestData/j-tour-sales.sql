-- Active sale #1: 20% discount on tours -2 and -4 (published tours)
INSERT INTO tours."Sales" ("Id", "AuthorId", "TourIds", "StartDate", "EndDate", "DiscountPercent", "IsActive")
VALUES (100, -11, ARRAY[-2, -4], NOW() - INTERVAL '1 day', NOW() + INTERVAL '7 days', 20, true);

-- Active sale #2: 30% discount on tour -2 (multiple sales for same tour - test uses highest discount)
INSERT INTO tours."Sales" ("Id", "AuthorId", "TourIds", "StartDate", "EndDate", "DiscountPercent", "IsActive")
VALUES (101, -11, ARRAY[-2], NOW() - INTERVAL '1 day', NOW() + INTERVAL '5 days', 30, true);

-- Active sale #3: 15% discount on tour -4
INSERT INTO tours."Sales" ("Id", "AuthorId", "TourIds", "StartDate", "EndDate", "DiscountPercent", "IsActive")
VALUES (102, -11, ARRAY[-4], NOW() - INTERVAL '1 day', NOW() + INTERVAL '10 days', 15, true);

-- Expired sale: should NOT show in active sales filter
INSERT INTO tours."Sales" ("Id", "AuthorId", "TourIds", "StartDate", "EndDate", "DiscountPercent", "IsActive")
VALUES (103, -11, ARRAY[-99], NOW() - INTERVAL '20 days', NOW() - INTERVAL '10 days', 25, true);