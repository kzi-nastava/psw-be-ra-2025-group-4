using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class QuizResultDto
    {
        public int QuizId { get; set; }
        public List<QuestionResultDto> QuestionResults { get; set; }
    }
}
