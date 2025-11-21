INSERT INTO tours."TourPreferences"(
    "Id", "TouristId", "PreferredDifficulty",
    "WalkRating", "BikeRating", "CarRating", "BoatRating", "Tags")
VALUES
(
    -1,
    2,               
    0,
    3,
    1,
    0,
    0,
    ARRAY['priroda', 'šetnja']
);

INSERT INTO tours."TourPreferences"(
    "Id", "TouristId", "PreferredDifficulty",
    "WalkRating", "BikeRating", "CarRating", "BoatRating", "Tags")
VALUES
(
    -2,
    3,               
    1,
    1,
    3,
    1,
    0,
    ARRAY['bicikl', 'avantura']
);

INSERT INTO tours."TourPreferences"(
    "Id", "TouristId", "PreferredDifficulty",
    "WalkRating", "BikeRating", "CarRating", "BoatRating", "Tags")
VALUES
(
    -3,
    4,              
    2,
    0,
    0,
    3,
    2,
    ARRAY[]::text[]
);
