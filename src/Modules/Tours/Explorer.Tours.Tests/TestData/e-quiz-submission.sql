INSERT INTO tours."Quizzes" ("Id", "Title", "AuthorId")
VALUES (-100, 'Quiz for Submission Tests', '-1');

INSERT INTO tours."Questions" ("Id", "Text", "QuizId")
VALUES (-1000, 'What is 2 + 2?', -100);

INSERT INTO tours."Questions" ("Id", "Text", "QuizId")
VALUES (-1001, 'What is the capital of France?', -100);

INSERT INTO tours."Questions" ("Id", "Text", "QuizId")
VALUES (-1002, 'Which of these are colors?', -100);

INSERT INTO tours."Options" ("Id", "Text", "IsCorrect", "Feedback", "QuestionId")
VALUES (-10000, '3', false, 'Incorrect. The answer is 4.', -1000);

INSERT INTO tours."Options" ("Id", "Text", "IsCorrect", "Feedback", "QuestionId")
VALUES (-10001, '4', true, 'Correct!', -1000);

INSERT INTO tours."Options" ("Id", "Text", "IsCorrect", "Feedback", "QuestionId")
VALUES (-10002, '5', false, 'Incorrect. The answer is 4.', -1000);

INSERT INTO tours."Options" ("Id", "Text", "IsCorrect", "Feedback", "QuestionId")
VALUES (-10010, 'London', false, 'Incorrect. The capital of France is Paris.', -1001);

INSERT INTO tours."Options" ("Id", "Text", "IsCorrect", "Feedback", "QuestionId")
VALUES (-10011, 'Paris', true, 'Correct!', -1001);

INSERT INTO tours."Options" ("Id", "Text", "IsCorrect", "Feedback", "QuestionId")
VALUES (-10012, 'Berlin', false, 'Incorrect. The capital of France is Paris.', -1001);

INSERT INTO tours."Options" ("Id", "Text", "IsCorrect", "Feedback", "QuestionId")
VALUES (-10020, 'Red', true, 'Correct! Red is a color.', -1002);

INSERT INTO tours."Options" ("Id", "Text", "IsCorrect", "Feedback", "QuestionId")
VALUES (-10021, 'Blue', true, 'Correct! Blue is a color.', -1002);

INSERT INTO tours."Options" ("Id", "Text", "IsCorrect", "Feedback", "QuestionId")
VALUES (-10022, 'Car', false, 'Incorrect. Car is not a color.', -1002);

INSERT INTO tours."Options" ("Id", "Text", "IsCorrect", "Feedback", "QuestionId")
VALUES (-10023, 'Green', true, 'Correct! Green is a color.', -1002);