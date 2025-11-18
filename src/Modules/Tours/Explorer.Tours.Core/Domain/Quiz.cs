using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Question> Questions { get; set; }

        public int AuthorId { get; private set; }

        public Quiz()
        {
            Questions = new List<Question>();
        }

        public Quiz(string title, int authorId)
        {
            Title = title;
            Questions = new List<Question>();
            AuthorId = authorId;
        }
    }
}
