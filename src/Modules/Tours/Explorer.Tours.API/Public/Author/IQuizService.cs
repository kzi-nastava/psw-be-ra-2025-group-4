using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Author
{
    public interface IQuizService
    {
        QuizDto Create(QuizDto dto);
        QuizDto Update(QuizDto dto);
        void Delete(long id);

        QuizDto GetById(long id);
        List<QuizDto> GetAll();
    }
}
