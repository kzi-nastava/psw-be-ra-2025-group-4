using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos
{
    public class QuizEncounterDto : EncounterDto
    {
        public List<QuizQuestionDto> Questions { get; set; } = new List<QuizQuestionDto>();
        public int TimeLimit { get; set; }
    }
}
