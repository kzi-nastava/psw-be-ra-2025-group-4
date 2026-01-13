-- Tour 1
INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty",
    "AuthorId", "Price", "Status", "Tags",
    "TransportDuration", "PublishedAt", "ArchivedAt", "LengthInKm"
)
VALUES
(
    -1,
    'Test Tura 1',
    'Opis 1',
    0,
    -11,
    20.00,
    0,
    ARRAY['test'],
    '[]',
    NULL,
    NULL,
    0.0
);

-- Tour 2
INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty",
    "AuthorId", "Price", "Status", "Tags",
    "TransportDuration", "PublishedAt", "ArchivedAt", "LengthInKm"
)
VALUES
(
    -2,
    'Test Tura 2',
    'Opis 2',
    1,
    -11,
    20.00,
    1,
    ARRAY['planina','avantura'],
    '[]',
    '2024-01-16 14:20:00+00',
    NULL,
    0.0
);

-- Tour 3
INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty",
    "AuthorId", "Price", "Status", "Tags",
    "TransportDuration", "PublishedAt", "ArchivedAt", "LengthInKm"
)
VALUES
(
    -3,
    'Test Tura 3',
    'Opis 3',
    2,
    -11,
    20.00,
    2,
    ARRAY['test'],
    '[]',
    '2023-11-16 14:20:00+00',
    '2024-01-16 14:20:00+00',
    0.0
);

-- Tour 4 (Published for bundle tests)
INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty",
    "AuthorId", "Price", "Status", "Tags",
    "TransportDuration", "PublishedAt", "ArchivedAt", "LengthInKm"
)
VALUES
(
    -4,
    'Test Tura 4',
    'Opis 4',
    1,
    -11,
    25.00,
    1,
    ARRAY['test', 'published'],
    '[]',
    '2024-01-16 14:20:00+00',
    NULL,
    0.0
);