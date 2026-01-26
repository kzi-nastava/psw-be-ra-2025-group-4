INSERT INTO payments."AffiliateCodes"
(
  "Id",
  "Code",
  "AuthorId",
  "TourId",
  "AffiliateTouristId",
  "Percent",
  "CreatedAt",
  "ExpiresAt",
  "Active",
  "UsageCount",
  "DeactivatedAt"
)
VALUES
-- Kod za konkretnu turu, aktivan
(-1, 'AFFTOUR1', -11, -2, -21, 10.00, CURRENT_TIMESTAMP - INTERVAL '10 days', NULL, true, 0, NULL),

-- Kod za konkretnu turu, aktivan
(-2, 'AFFTOUR2', -11, -2, -22, 15.00, CURRENT_TIMESTAMP - INTERVAL '5 days', NULL, true, 0, NULL),

-- Globalni kod, aktivan
(-3, 'AFFGLOBAL1', -11, NULL, -23, 12.50, CURRENT_TIMESTAMP - INTERVAL '3 days', NULL, true, 0, NULL),

-- Globalni kod, deaktiviran
(-4, 'AFFGLOBAL2', -11, NULL, -24, 20.00, CURRENT_TIMESTAMP - INTERVAL '1 day', NULL, false, 0, CURRENT_TIMESTAMP - INTERVAL '12 hours'),

-- Kod drugog autora
(-5, 'AFFOTHER1', -12, -3, -25, 8.00, CURRENT_TIMESTAMP - INTERVAL '2 days', NULL, true, 0, NULL);
