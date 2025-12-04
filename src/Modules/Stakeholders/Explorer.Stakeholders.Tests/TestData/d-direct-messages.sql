-- Autor1 messages Autor2
INSERT INTO stakeholders."DirectMessages"(
	"SenderId", "RecipientId", "Content", "SentAt", "EditedAt", "ResourceId", "ResourceType")
	VALUES (-11, -12, 'Hey, have you started working on the collaborative article?', NOW(), NULL, NULL, 0);

-- Autor2 replies to Autor1
INSERT INTO stakeholders."DirectMessages"(
	"SenderId", "RecipientId", "Content", "SentAt", "EditedAt", "ResourceId", "ResourceType")
	VALUES (-12, -11, 'Yes, I''ve drafted the first section. Let''s sync up tomorrow!', NOW(), NULL, 103, 1);

INSERT INTO stakeholders."DirectMessages"(
    "SenderId", "RecipientId", "Content", "SentAt", "EditedAt", "ResourceId", "ResourceType")
VALUES (-11, -12, 'Check my latest blog!', NOW(), NULL, 47, 2);

