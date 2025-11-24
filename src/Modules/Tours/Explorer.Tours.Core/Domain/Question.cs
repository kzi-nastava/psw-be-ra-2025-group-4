using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class Question : Entity
    {
        public string Text { get; set; }

        public long QuizId { get; set; }
        public Quiz Quiz { get; set; }

        public List<Option> Options { get; set; }

        public Question()
        {
            Options = new List<Option>();
        }

        public Question(string text, long quizId)
        {
            Text = text;
            QuizId = quizId;
            Options = new List<Option>();
        }
    }
}
