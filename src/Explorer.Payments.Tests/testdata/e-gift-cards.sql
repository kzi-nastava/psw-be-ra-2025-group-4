INSERT INTO payments."GiftCards"("Id", "Code", "RecipientTouristId", "Amount", "Balance", "BuyerTouristId", "PurchasedAt")
VALUES (-1, 'GIFT1234567890', -1, 25.00, 25.00, -2, CURRENT_TIMESTAMP);

INSERT INTO payments."GiftCards"("Id", "Code", "RecipientTouristId", "Amount", "Balance", "BuyerTouristId", "PurchasedAt")
VALUES (-2, 'LOWBALANCE1234', -1, 10.00, 5.00, -2, CURRENT_TIMESTAMP);
