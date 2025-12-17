INSERT INTO stakeholders."Notifications"(
    "Id", "UserId", "Content", "IsRead", "CreatedAt", "ResourceUrl", "Type", "ActorId", "ActorUsername", "Count" )
    VALUES ( -6001, -12, 'You have 2 new messages', false, NOW(), NULL, 0, -11, 'autor1@gmail.com', 2 );

-- Autor2 replies to Autor1 (1 message)
INSERT INTO stakeholders."Notifications"(
    "Id", "UserId", "Content", "IsRead", "CreatedAt", "ResourceUrl", "Type", "ActorId", "ActorUsername", "Count" )
    VALUES ( -6002, -11, 'You have 1 new message', false, NOW(), NULL, 0, -12, 'autor2@gmail.com', 1 );