using Explorer.Encounters.Core.Domain;
using System;
using System.Collections.Generic;
using Xunit;

namespace Explorer.Encounters.Tests.Unit
{
    [Collection("Sequential")]
    public class QuizEncounterUnitTests
    {
        private static Location ValidLocation()
            => new Location(45.0, 19.0);

        private static QuizAnswer CorrectAnswer()
            => new QuizAnswer("Correct", true);

        private static QuizAnswer WrongAnswer()
            => new QuizAnswer("Wrong", false);

        private static QuizQuestion ValidQuestion()
            => new QuizQuestion(
                "What is 2 + 2?",
                new List<QuizAnswer>
                {
                    CorrectAnswer(),
                    WrongAnswer()
                });

        private static QuizEncounter CreateValidQuizEncounter(
            EncounterApprovalStatus approvalStatus = EncounterApprovalStatus.PENDING)
        {
            return new QuizEncounter(
                name: "Quiz encounter",
                description: "Some description",
                location: ValidLocation(),
                experiencePoints: 100,
                approvalStatus: approvalStatus,
                questions: new List<QuizQuestion> { ValidQuestion() },
                timeLimit: 60
            );
        }

        [Fact]
        public void Constructor_Should_Create_Draft_Encounter()
        {
            var quiz = CreateValidQuizEncounter();

            Assert.Equal(EncounterStatus.Draft, quiz.Status);
            Assert.Equal(EncounterType.Quiz, quiz.Type);
        }

        [Fact]
        public void Constructor_Should_Throw_When_Name_Is_Empty()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuizEncounter(
                    "",
                    "desc",
                    ValidLocation(),
                    10,
                    EncounterApprovalStatus.PENDING,
                    new List<QuizQuestion> { ValidQuestion() },
                    30));
        }

        [Fact]
        public void Constructor_Should_Throw_When_Description_Is_Empty()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuizEncounter(
                    "Name",
                    "",
                    ValidLocation(),
                    10,
                    EncounterApprovalStatus.PENDING,
                    new List<QuizQuestion> { ValidQuestion() },
                    30));
        }

        [Fact]
        public void Constructor_Should_Throw_When_Experience_Is_Negative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new QuizEncounter(
                    "Name",
                    "Desc",
                    ValidLocation(),
                    -1,
                    EncounterApprovalStatus.PENDING,
                    new List<QuizQuestion> { ValidQuestion() },
                    30));
        }

        [Fact]
        public void Approve_Should_Set_Status_To_Approved()
        {
            var quiz = CreateValidQuizEncounter();

            quiz.Approve();

            Assert.Equal(EncounterApprovalStatus.APPROVED, quiz.ApprovalStatus);
        }

        [Fact]
        public void Approve_Should_Throw_When_Already_Approved()
        {
            var quiz = CreateValidQuizEncounter(EncounterApprovalStatus.APPROVED);

            Assert.Throws<InvalidOperationException>(() => quiz.Approve());
        }

        [Fact]
        public void Decline_Should_Set_Status_To_Declined()
        {
            var quiz = CreateValidQuizEncounter();

            quiz.Decline();

            Assert.Equal(EncounterApprovalStatus.DECLINED, quiz.ApprovalStatus);
        }

        [Fact]
        public void Decline_Should_Throw_When_Already_Declined()
        {
            var quiz = CreateValidQuizEncounter(EncounterApprovalStatus.DECLINED);

            Assert.Throws<InvalidOperationException>(() => quiz.Decline());
        }

        [Fact]
        public void Activate_Should_Throw_When_Not_Approved()
        {
            var quiz = CreateValidQuizEncounter(EncounterApprovalStatus.PENDING);

            Assert.Throws<InvalidOperationException>(() => quiz.Activate());
        }

        [Fact]
        public void Activate_Should_Set_Status_To_Active_When_Approved()
        {
            var quiz = CreateValidQuizEncounter(EncounterApprovalStatus.APPROVED);

            quiz.Activate();

            Assert.Equal(EncounterStatus.Active, quiz.Status);
        }

        [Fact]
        public void Activate_Should_Throw_When_Already_Active()
        {
            var quiz = CreateValidQuizEncounter(EncounterApprovalStatus.APPROVED);
            quiz.Activate();

            Assert.Throws<InvalidOperationException>(() => quiz.Activate());
        }

        [Fact]
        public void Archive_Should_Set_Status_To_Archived()
        {
            var quiz = CreateValidQuizEncounter();

            quiz.Archive();

            Assert.Equal(EncounterStatus.Archived, quiz.Status);
        }

        [Fact]
        public void Archive_Should_Throw_When_Already_Archived()
        {
            var quiz = CreateValidQuizEncounter();
            quiz.Archive();

            Assert.Throws<InvalidOperationException>(() => quiz.Archive());
        }

        [Fact]
        public void QuizEncounter_Should_Throw_When_No_Questions()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuizEncounter(
                    "Quiz",
                    "Desc",
                    ValidLocation(),
                    10,
                    EncounterApprovalStatus.PENDING,
                    new List<QuizQuestion>(),
                    30));
        }

        [Fact]
        public void QuizEncounter_Should_Throw_When_TimeLimit_Is_NonPositive()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuizEncounter(
                    "Quiz",
                    "Desc",
                    ValidLocation(),
                    10,
                    EncounterApprovalStatus.PENDING,
                    new List<QuizQuestion> { ValidQuestion() },
                    0));
        }

        [Fact]
        public void QuizQuestion_Should_Throw_When_Text_Is_Empty()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuizQuestion(
                    "",
                    new List<QuizAnswer> { CorrectAnswer(), WrongAnswer() }));
        }

        [Fact]
        public void QuizQuestion_Should_Throw_When_Less_Than_Two_Answers()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuizQuestion(
                    "Question",
                    new List<QuizAnswer> { CorrectAnswer() }));
        }

        [Fact]
        public void QuizQuestion_Should_Throw_When_No_Correct_Answer()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuizQuestion(
                    "Question",
                    new List<QuizAnswer>
                    {
                        new QuizAnswer("A", false),
                        new QuizAnswer("B", false)
                    }));
        }

        [Fact]
        public void QuizAnswer_Should_Throw_When_Text_Is_Empty()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuizAnswer("", true));
        }
    }
}
