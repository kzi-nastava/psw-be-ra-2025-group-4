using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class QuizAnswerDbRepository : IQuizAnswerRepository
    {
        private readonly ToursContext _context;

        public QuizAnswerDbRepository(ToursContext context)
        {
            _context = context;
        }

        public List<QuizAnswer> Create(List<QuizAnswer> answers)
        {
            _context.QuizAnswers.AddRange(answers);
            _context.SaveChanges();
            return answers;
        }
    }
}
