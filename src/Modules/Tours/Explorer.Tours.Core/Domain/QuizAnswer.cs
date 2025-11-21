using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class QuizAnswer
    {
        public int Id { get; set; }
        public long TouristId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedOptionId { get; set; }

        public QuizAnswer()
        {
        }

        public QuizAnswer(long touristId, int questionId, int selectedOptionId)
        {
            TouristId = touristId;
            QuestionId = questionId;
            SelectedOptionId = selectedOptionId;
        }
    }
}
