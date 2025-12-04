-- Tour 1
INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty",
    "AuthorId", "Price", "Status", "Tags",
    "TransportDuration", "PublishedAt", "ArchivedAt"
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
    NULL
);

-- Tour 2
INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty",
    "AuthorId", "Price", "Status", "Tags",
    "TransportDuration", "PublishedAt", "ArchivedAt"
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
    NULL
);

-- Tour 3
INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty",
    "AuthorId", "Price", "Status", "Tags",
    "TransportDuration", "PublishedAt", "ArchivedAt"
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
    '2024-01-16 14:20:00+00'
);
