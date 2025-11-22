using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public List<OptionDto> Options { get; set; }
    }
}
