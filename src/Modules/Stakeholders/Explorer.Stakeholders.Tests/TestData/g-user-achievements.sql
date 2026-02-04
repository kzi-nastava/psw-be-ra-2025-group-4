INSERT INTO stakeholders."UserAchievements"
("Id", "UserId")
VALUES 
(1001, -21);

INSERT INTO stakeholders."UserAchievement"
("Id", "Type", "EarnedAt", "UserAchievementsId")
VALUES
(2001, 0, NOW(), 1001), 
(2002, 1, NOW(), 1001), 
(2003, 2, NOW(), 1001);