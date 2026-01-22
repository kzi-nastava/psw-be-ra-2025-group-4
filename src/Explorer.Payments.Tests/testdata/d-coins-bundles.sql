INSERT INTO payments."Wallets"(
    "Id", "TouristId", "Balance")
VALUES (-1, -1, 1000.00);

INSERT INTO payments."Wallets"(
    "Id", "TouristId", "Balance")
VALUES (-2, -2, 2400.00);

INSERT INTO payments."CoinsBundles"(
    "Id", "Name", "Description", "CoinsAmount", "BonusCoins", "Price", "ImageUrl", "DisplayOrder")
VALUES (1, 'Starter Pack', 'Perfect for trying out the platform', 500, 0, 5.00, 'bundle-1.png', 1);

INSERT INTO payments."CoinsBundles"(
    "Id", "Name", "Description", "CoinsAmount", "BonusCoins", "Price", "ImageUrl", "DisplayOrder")
VALUES (2, 'Explorer Pack', 'Great value for casual travelers', 1000, 100, 10.00, 'bundle-2.png', 2);

INSERT INTO payments."CoinsBundles"(
    "Id", "Name", "Description", "CoinsAmount", "BonusCoins", "Price", "ImageUrl", "DisplayOrder")
VALUES (3, 'Adventurer Pack', 'Most popular choice!', 2000, 400, 20.00, 'bundle-3.png', 3);

INSERT INTO payments."CoinsBundles"(
    "Id", "Name", "Description", "CoinsAmount", "BonusCoins", "Price", "ImageUrl", "DisplayOrder")
VALUES (4, 'Pro Pack', 'For serious explorers', 3500, 850, 35.00, 'bundle-4.png', 4);

INSERT INTO payments."CoinsBundles"(
    "Id", "Name", "Description", "CoinsAmount", "BonusCoins", "Price", "ImageUrl", "DisplayOrder")
VALUES (5, 'Elite Pack', 'Maximum savings!', 5000, 1500, 50.00, 'bundle-5.png', 5);

INSERT INTO payments."CoinsBundles"(
    "Id", "Name", "Description", "CoinsAmount", "BonusCoins", "Price", "ImageUrl", "DisplayOrder")
VALUES (6, 'Ultimate Treasure', 'The best deal ever!', 10000, 4000, 100.00, 'bundle-6.png', 6);

INSERT INTO payments."CoinsBundleSales"(
    "Id", "CoinsBundleId", "DiscountPercentage", "StartDate", "EndDate", "IsActive")
VALUES (-1, 2, 20, CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP + INTERVAL '7 days', true);

INSERT INTO payments."CoinsBundleSales"(
    "Id", "CoinsBundleId", "DiscountPercentage", "StartDate", "EndDate", "IsActive")
VALUES (-2, 3, 15, CURRENT_TIMESTAMP + INTERVAL '1 day', CURRENT_TIMESTAMP + INTERVAL '7 days', true);

INSERT INTO payments."CoinsBundleSales"(
    "Id", "CoinsBundleId", "DiscountPercentage", "StartDate", "EndDate", "IsActive")
VALUES (-3, 4, 30, CURRENT_TIMESTAMP - INTERVAL '10 days', CURRENT_TIMESTAMP - INTERVAL '3 days', false);

INSERT INTO payments."CoinsBundlePurchases"(
    "Id", "TouristId", "CoinsBundleId", "BundleName", "CoinsReceived", 
    "PricePaid", "OriginalPrice", "PaymentMethod", "PurchaseDate", "TransactionId")
VALUES (-1, -1, 1, 'Starter Pack', 500, 5.00, 5.00, 1, 
        CURRENT_TIMESTAMP - INTERVAL '5 days', 'CC-TEST001');

INSERT INTO payments."CoinsBundlePurchases"(
    "Id", "TouristId", "CoinsBundleId", "BundleName", "CoinsReceived", 
    "PricePaid", "OriginalPrice", "PaymentMethod", "PurchaseDate", "TransactionId")
VALUES (-2, -1, 2, 'Explorer Pack', 1100, 8.00, 10.00, 2, 
        CURRENT_TIMESTAMP - INTERVAL '2 days', 'PP-TEST002');

INSERT INTO payments."CoinsBundlePurchases"(
    "Id", "TouristId", "CoinsBundleId", "BundleName", "CoinsReceived", 
    "PricePaid", "OriginalPrice", "PaymentMethod", "PurchaseDate", "TransactionId")
VALUES (-3, -2, 3, 'Adventurer Pack', 2400, 20.00, 20.00, 3, 
        CURRENT_TIMESTAMP - INTERVAL '1 day', 'GC-TEST003');