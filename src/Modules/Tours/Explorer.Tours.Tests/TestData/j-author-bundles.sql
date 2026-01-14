INSERT INTO tours."Bundles" (
    "Id", "Name", "Price", "AuthorId", "Status"
)
VALUES
(
    -1,
    'Draft Bundle 1',
    50.00,
    -11,
    0
);

INSERT INTO tours."BundleTour" ("BundleId", "TourId")
VALUES
(-1, -1),
(-1, -2);

INSERT INTO tours."Bundles" (
    "Id", "Name", "Price", "AuthorId", "Status"
)
VALUES
(
    -2,
    'Draft Bundle 2',
    45.00,
    -11,
    0
);

INSERT INTO tours."BundleTour" ("BundleId", "TourId")
VALUES
(-2, -1),
(-2, -2);

INSERT INTO tours."Bundles" (
    "Id", "Name", "Price", "AuthorId", "Status"
)
VALUES
(
    -3,
    'Published Bundle 1',
    60.00,
    -11,
    1
);

INSERT INTO tours."BundleTour" ("BundleId", "TourId")
VALUES
(-3, -2),
(-3, -4);

INSERT INTO tours."Bundles" (
    "Id", "Name", "Price", "AuthorId", "Status"
)
VALUES
(
    -4,
    'Published Bundle 2',
    70.00,
    -11,
    1
);

INSERT INTO tours."BundleTour" ("BundleId", "TourId")
VALUES
(-4, -2),
(-4, -4);
