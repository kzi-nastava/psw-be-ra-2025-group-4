INSERT INTO payments."TouristReferralInvites"
("Id", "Code", "ReferrerTouristId", "IsUsed", "ReferredTouristId", "CreatedAtUtc", "UsedAtUtc")
VALUES
(-9001, 'REFLINK31',     -21, false, NULL, CURRENT_TIMESTAMP - INTERVAL '10 days', NULL),
(-9002, 'REFCONSUME32',  -21, false, NULL, CURRENT_TIMESTAMP - INTERVAL '2 days',  NULL),
(-9003, 'REFUSED31',     -21, true, -22,  CURRENT_TIMESTAMP - INTERVAL '20 days', CURRENT_TIMESTAMP - INTERVAL '19 days');

INSERT INTO payments."Wallets" ("Id", "TouristId", "Balance")
VALUES
(-2101, -21, 0.00),
(-2201, -22, 0.00),
(-2301, -23, 0.00);