using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class QuizDto
    {
        public long Id { get; set; }
        public string Title { get; set; }

        public string AuthorId { get; set; }

        public List<QuestionDto> Questions { get; set; }
    }
}
