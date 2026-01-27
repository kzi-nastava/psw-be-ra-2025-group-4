using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos
{
    public class QuizQuestionDto
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public List<QuizAnswerDto> Answers { get; set; } = new List<QuizAnswerDto>();
    }
}
