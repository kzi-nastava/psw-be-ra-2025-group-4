INSERT INTO payments."Coupons"(
    "Id", "Code", "DiscountPercentage", "ExpirationDate", "AuthorId", "TourId", 
    "IsUsed", "UsedByTouristId", "UsedAt")
VALUES (-1, 'TESTCODE', 20, CURRENT_TIMESTAMP + INTERVAL '30 days', -11, -2, 
        false, NULL, NULL);

INSERT INTO payments."Coupons"(
    "Id", "Code", "DiscountPercentage", "ExpirationDate", "AuthorId", "TourId", 
    "IsUsed", "UsedByTouristId", "UsedAt")
VALUES (-2, 'USED0001', 15, CURRENT_TIMESTAMP + INTERVAL '30 days', -11, -2, 
        true, -1, CURRENT_TIMESTAMP - INTERVAL '5 days');

INSERT INTO payments."Coupons"(
    "Id", "Code", "DiscountPercentage", "ExpirationDate", "AuthorId", "TourId", 
    "IsUsed", "UsedByTouristId", "UsedAt")
VALUES (-3, 'EXPIRED1', 25, CURRENT_TIMESTAMP - INTERVAL '5 days', -11, -2, 
        false, NULL, NULL);

INSERT INTO payments."Coupons"(
    "Id", "Code", "DiscountPercentage", "ExpirationDate", "AuthorId", "TourId", 
    "IsUsed", "UsedByTouristId", "UsedAt")
VALUES (-4, 'ALLAUTH1', 10, NULL, -11, NULL, 
        false, NULL, NULL);

INSERT INTO payments."Coupons"(
    "Id", "Code", "DiscountPercentage", "ExpirationDate", "AuthorId", "TourId", 
    "IsUsed", "UsedByTouristId", "UsedAt")
VALUES (-5, 'OTHER001', 30, CURRENT_TIMESTAMP + INTERVAL '60 days', -12, -3, 
        false, NULL, NULL);

INSERT INTO payments."Coupons"(
    "Id", "Code", "DiscountPercentage", "ExpirationDate", "AuthorId", "TourId", 
    "IsUsed", "UsedByTouristId", "UsedAt")
VALUES (-6, 'SUMMER25', 25, CURRENT_TIMESTAMP + INTERVAL '90 days', -11, NULL, 
        false, NULL, NULL);