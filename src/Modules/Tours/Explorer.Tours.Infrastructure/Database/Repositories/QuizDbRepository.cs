using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class QuizDbRepository : IQuizRepository
    {
        private readonly ToursContext _context;

        public QuizDbRepository(ToursContext context)
        {
            _context = context;
        }

        public Quiz Create(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            _context.SaveChanges();
            return quiz;
        }

        public Quiz Update(Quiz quiz)
        {
            var existing = _context.Quizzes.FirstOrDefault(q => q.Id == quiz.Id);
            if (existing == null)
            {
                throw new NotFoundException("Quiz not found: " + quiz.Id);
            }

            existing.Title = quiz.Title;
            existing.AuthorId = quiz.AuthorId;

            _context.SaveChanges();
            return existing;
        }

        public void Delete(int id)
        {
            var quiz = _context.Quizzes.FirstOrDefault(q => q.Id == id);
            if (quiz == null)
                throw new NotFoundException($"Quiz not found: {id}");

            _context.Quizzes.Remove(quiz);
            _context.SaveChanges();
        }


        public Quiz GetById(int id)
        {
            return _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefault(q => q.Id == id);
        }

        public List<Quiz> GetAll()
        {
            return _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .ToList();
        }
    }
}
