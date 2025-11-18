using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IQuizRepository
    {
        Quiz Create(Quiz quiz);
        Quiz Update(Quiz quiz);
        void Delete(int id);
        Quiz GetById(int id);
        List<Quiz> GetAll();
    }
}
