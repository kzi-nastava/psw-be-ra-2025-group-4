using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class QuestionResultDto
    {
        public long QuestionId { get; set; }
        public long SelectedOptionId { get; set; }
        public bool IsCorrect { get; set; }
        public string Feedback { get; set; }
    }
}
