-- Gift card for tourist -1, used in "Purchases_bundle_with_gift_card_successfully" test (bundle 3 = 20)
INSERT INTO payments."GiftCards"("Id", "Code", "RecipientTouristId", "Amount", "Balance", "BuyerTouristId", "PurchasedAt")
VALUES (-1, 'GIFT1234567890', -1, 25.00, 25.00, -2, CURRENT_TIMESTAMP);

-- Low balance card for -1, used in "Purchase_fails_when_insufficient_gift_card_balance" (bundle 3 = 20)
INSERT INTO payments."GiftCards"("Id", "Code", "RecipientTouristId", "Amount", "Balance", "BuyerTouristId", "PurchasedAt")
VALUES (-2, 'LOWBALANCE1234', -1, 10.00, 5.00, -2, CURRENT_TIMESTAMP);
