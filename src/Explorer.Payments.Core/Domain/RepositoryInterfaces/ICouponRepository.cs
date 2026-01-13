using Explorer.Payments.Core.Domain;
using System.Collections.Generic;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ICouponRepository
    {
        Coupon GetById(int id);
        Coupon GetByCode(string code);
        Coupon Create(Coupon coupon);
        Coupon Update(Coupon coupon);
        void Delete(int id);
        IEnumerable<Coupon> GetByAuthor(int authorId);
        bool CodeExists(string code);
    }
}