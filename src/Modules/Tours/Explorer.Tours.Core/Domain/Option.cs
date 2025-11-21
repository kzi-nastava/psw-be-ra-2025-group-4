using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class Option : Entity
    {
        public string Text { get; set; }

        public bool IsCorrect { get; set; }
        public string Feedback { get; set; }

        public long QuestionId { get; set; }
        public Question Question { get; set; }

        public Option()
        {
        }

        public Option(string text, bool isCorrect, string feedback, long questionId)
        {
            Text = text;
            IsCorrect = isCorrect;
            Feedback = feedback;
            QuestionId = questionId;
        }
    }
}
