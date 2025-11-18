using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases.Author
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _repo;
        private readonly IMapper _mapper;

        public QuizService(IQuizRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public QuizDto Create(QuizDto dto)
        {
            var entity = _mapper.Map<Quiz>(dto);
            var result = _repo.Create(entity);
            return _mapper.Map<QuizDto>(result);
        }

        public QuizDto Update(QuizDto dto)
        {
            var entity = _mapper.Map<Quiz>(dto);
            var result = _repo.Update(entity);
            return _mapper.Map<QuizDto>(result);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public QuizDto GetById(int id)
        {
            var result = _repo.GetById(id);
            return _mapper.Map<QuizDto>(result);
        }

        public List<QuizDto> GetAll()
        {
            return _repo.GetAll()
                        .Select(_mapper.Map<QuizDto>)
                        .ToList();
        }

        public QuizDto SubmitAnswers(int quizId, QuizDto submitted)
        {
            var quiz = _repo.GetById(quizId);
            return _mapper.Map<QuizDto>(quiz);
        }
    }
}
