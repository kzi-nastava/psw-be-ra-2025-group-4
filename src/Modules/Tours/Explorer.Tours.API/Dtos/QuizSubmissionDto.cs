using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class QuizSubmissionDto
    {
        public long QuizId { get; set; }
        public long TouristId { get; set; }
        public List<QuizAnswerDto> Answers { get; set; }
    }
}
