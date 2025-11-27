-- Autor1 messages Autor2
INSERT INTO stakeholders."DirectMessages"(
    "SenderId", "RecipientId", "Content", "SentAt", "EditedAt")
VALUES (-11, -12, 'Hey, have you started working on the collaborative article?', NOW(), NULL);

-- Autor2 replies to Autor1
INSERT INTO stakeholders."DirectMessages"(
    "SenderId", "RecipientId", "Content", "SentAt", "EditedAt")
VALUES (-12, -11, 'Yes, I''ve drafted the first section. Let''s sync up tomorrow!', NOW(), NULL);
