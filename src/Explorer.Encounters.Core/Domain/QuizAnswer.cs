using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class QuizAnswer : Entity
    {
        public string Text { get; private set; }
        public bool IsCorrect { get; private set; }

        public long QuizQuestionId { get; private set; }
        public QuizQuestion? QuizQuestion { get; private set; }

        private QuizAnswer() { }

        public QuizAnswer(string text, bool isCorrect)
        {
            Text = text;
            IsCorrect = isCorrect;

            Validate();
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Text))
                throw new ArgumentException("Answer text cannot be empty.");
        }
    }
}
