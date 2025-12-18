INSERT INTO stakeholders."Clubs"("Id", "Name", "Description", "OwnerId", "ImageUrls", "Status", "Members", "InvitedTourist", "RequestedTourists")
VALUES 
    (-1, 'Test Club 1', 'Opis testnog kluba 1', -21, ARRAY['test'], 0, ARRAY[]::bigint[], ARRAY[]::bigint[], ARRAY[]::bigint[]),
    (-2, 'Test Club 2', 'Opis testnog kluba 2', -21, ARRAY['test'], 1, ARRAY[]::bigint[], ARRAY[]::bigint[], ARRAY[]::bigint[]);