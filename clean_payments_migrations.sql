-- Obriši zapis iz migracija za Payments modul
DELETE FROM payments."__EFMigrationsHistory" WHERE "MigrationId" LIKE '%Init%' OR "MigrationId" LIKE '%Payments%';

-- Opciono: Obriši sve tabele iz payments schema (ako želiš potpuno čist start)
-- PAZI: Ovo će obrisati sve podatke!
-- DROP TABLE IF EXISTS payments."PaymentRecords" CASCADE;
-- DROP TABLE IF EXISTS payments."Wallets" CASCADE;
-- DROP TABLE IF EXISTS payments."TourPurchaseTokens" CASCADE;
-- DROP TABLE IF EXISTS payments."OrderItem" CASCADE;
-- DROP TABLE IF EXISTS payments."ShoppingCarts" CASCADE;

