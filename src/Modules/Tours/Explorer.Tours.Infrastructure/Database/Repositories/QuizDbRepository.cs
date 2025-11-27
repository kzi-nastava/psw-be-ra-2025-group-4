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
            var existing = _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefault(q => q.Id == quiz.Id);
            if (existing == null)
            {
                throw new NotFoundException("Quiz not found: " + quiz.Id);
            }

            existing.Title = quiz.Title;
            existing.AuthorId = quiz.AuthorId;

            if (quiz.Questions != null)
            {
                var questionIds = quiz.Questions.Where(q => q.Id > 0).Select(q => q.Id).ToList();
                
                var questionsToRemove = existing.Questions.Where(q => !questionIds.Contains(q.Id)).ToList();
                foreach (var question in questionsToRemove)
                {
                    var optionsToRemove = _context.Options.Where(o => o.QuestionId == question.Id).ToList();
                    _context.Options.RemoveRange(optionsToRemove);
                    _context.Questions.Remove(question);
                }

                foreach (var questionDto in quiz.Questions)
                {
                    if (questionDto.Id > 0)
                    {
                        var existingQuestion = existing.Questions.FirstOrDefault(q => q.Id == questionDto.Id);
                        if (existingQuestion != null)
                        {
                            existingQuestion.Text = questionDto.Text;

                            if (questionDto.Options != null)
                            {
                                var optionIds = questionDto.Options.Where(o => o.Id > 0).Select(o => o.Id).ToList();
                                
                                var optionsToRemove = existingQuestion.Options.Where(o => !optionIds.Contains(o.Id)).ToList();
                                _context.Options.RemoveRange(optionsToRemove);

                                foreach (var optionDto in questionDto.Options)
                                {
                                    if (optionDto.Id > 0)
                                    {
                                        var existingOption = existingQuestion.Options.FirstOrDefault(o => o.Id == optionDto.Id);
                                        if (existingOption != null)
                                        {
                                            existingOption.Text = optionDto.Text;
                                            existingOption.IsCorrect = optionDto.IsCorrect;
                                            existingOption.Feedback = optionDto.Feedback;
                                        }
                                    }
                                    else
                                    {
                                        var newOption = new Option
                                        {
                                            Text = optionDto.Text,
                                            IsCorrect = optionDto.IsCorrect,
                                            Feedback = optionDto.Feedback,
                                            QuestionId = existingQuestion.Id
                                        };
                                        existingQuestion.Options.Add(newOption);
                                        _context.Options.Add(newOption);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var newQuestion = new Question
                        {
                            Text = questionDto.Text,
                            QuizId = existing.Id,
                            Options = new List<Option>()
                        };
                        
                        existing.Questions.Add(newQuestion);
                        _context.Questions.Add(newQuestion);
                        _context.SaveChanges();
                        
                        if (questionDto.Options != null)
                        {
                            foreach (var optionDto in questionDto.Options)
                            {
                                var newOption = new Option
                                {
                                    Text = optionDto.Text,
                                    IsCorrect = optionDto.IsCorrect,
                                    Feedback = optionDto.Feedback,
                                    QuestionId = newQuestion.Id
                                };
                                newQuestion.Options.Add(newOption);
                                _context.Options.Add(newOption);
                            }
                        }
                    }
                }
            }

            _context.SaveChanges();
            
            _context.Entry(existing).State = EntityState.Detached;
            return GetById(existing.Id);
        }

        public void Delete(long id)
        {
            var quiz = GetById(id);
            if (quiz == null)
                throw new NotFoundException($"Quiz not found: {id}");

            _context.Quizzes.Remove(quiz);
            _context.SaveChanges();
        }


        public Quiz GetById(long id)
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
