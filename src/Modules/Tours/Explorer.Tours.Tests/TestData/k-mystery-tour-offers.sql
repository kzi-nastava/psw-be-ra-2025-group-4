INSERT INTO tours."MysteryTourOffers"(
    "Id", "TouristId", "TourId", "DiscountPercent", "CreatedAt", "ExpiresAt", "Redeemed")
VALUES (
    '00000000-0000-0000-0000-000000000090',
    -90,
    -2,
    20,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP + INTERVAL '10 minute',
    false
);
