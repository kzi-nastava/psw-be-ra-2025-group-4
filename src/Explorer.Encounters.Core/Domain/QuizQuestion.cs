using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class QuizQuestion : Entity
    {
        public string Text { get; private set; }
        public List<QuizAnswer> Answers { get; private set; }

        public long QuizEncounterId { get; private set; }
        public QuizEncounter? QuizEncounter { get; private set; }

        private QuizQuestion() { }

        public QuizQuestion(string text, List<QuizAnswer> answers)
        {
            Text = text;
            Answers = answers;

            Validate();
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Text))
                throw new ArgumentException("Question text cannot be empty.");

            if (Answers == null || Answers.Count < 2)
                throw new ArgumentException("A question must have at least two answers.");

            if (!Answers.Any(a => a.IsCorrect))
                throw new ArgumentException("At least one answer must be marked as correct.");
        }
    }
}
