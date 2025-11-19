INSERT INTO tours."Quizzes" ("Id", "Title", "AuthorId")
VALUES
(-1, 'Existing Negative Quiz 1', '-1'),
(-2, 'Existing Negative Quiz 2', '-1'),
(-3, 'Existing Negative Quiz 3', '-1');
INSERT INTO tours."Questions" ("Id", "Text", "QuizId")
VALUES
(-10, 'Question for quiz -1', -1),
(-11, 'Another question for quiz -1', -1),
(-20, 'Question for quiz -2', -2),
(-30, 'Question for quiz -3', -3);
INSERT INTO tours."Options" ("Id", "Text", "IsCorrect", "Feedback", "QuestionId")
VALUES
(-100, 'Option A', true, 'Correct', -10),
(-101, 'Option B', false, 'Incorrect', -10),
(-110, 'Option C', true, 'Correct', -11),
(-200, 'Option X', false, 'Incorrect', -20),
(-201, 'Option Y', true, 'Correct', -20),
(-300, 'Option M', true, 'Correct', -30);