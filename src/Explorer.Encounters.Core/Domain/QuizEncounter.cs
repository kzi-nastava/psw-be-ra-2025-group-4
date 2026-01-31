using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class QuizEncounter : Encounter
    {
        public List<QuizQuestion> Questions { get; private set; }
        public int TimeLimit { get; private set; } // in seconds

        private QuizEncounter() { }

        public QuizEncounter(string name, string description, Location location, int experiencePoints, EncounterApprovalStatus approvalStatus, List<QuizQuestion> questions, int timeLimit)
            : base(name, description, location, experiencePoints, EncounterType.Quiz, approvalStatus)
        {
            Questions = questions;
            TimeLimit = timeLimit;

            Validate();
        }

        public void UpdateQuiz(
            string name,
            string description,
            Location location,
            int experiencePoints,
            List<QuizQuestion> questions,
            int timeLimit)
        {
            base.Update(name, description, location, experiencePoints, EncounterType.Social);
            
            Questions = questions;
            TimeLimit = timeLimit;

            Validate();
        }

        protected new void Validate()
        {
            base.Validate();

            if (Questions == null || Questions.Count == 0)
                throw new ArgumentException("Quiz encounter must have at least one question.");
            if (TimeLimit <= 0)
                throw new ArgumentException("Time limit must be a positive integer.");
        }

        public bool IsAnswerCorrect(long questionId, long answerId)
            => Questions
                .FirstOrDefault(q => q.Id == questionId)?
                .Answers
                .Any(a => a.Id == answerId && a.IsCorrect) ?? false;

        public bool IsCompletedOnTime(DateTime startedAtUtc)
        {
            return DateTime.UtcNow - startedAtUtc <= TimeSpan.FromSeconds(TimeLimit);
        }

    }
}
