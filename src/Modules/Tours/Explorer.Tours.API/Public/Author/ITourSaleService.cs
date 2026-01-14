using Explorer.Tours.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Tours.API.Public.Author
{
    public interface ITourSaleService
    {
        List<SaleDto> GetByAuthor(int authorId);
        SaleDto GetById(int id, int authorId);
        SaleDto Create(SaleCreateDto dto, int authorId);
        SaleDto Update(int id, SaleUpdateDto dto, int authorId);
        void Delete(int id, int authorId);
    }
}

