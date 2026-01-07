using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class CouponDbRepository : ICouponRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<Coupon> _dbSet;

        public CouponDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Coupon>();
        }

        public Coupon GetById(int id)
        {
            var entity = _dbSet.FirstOrDefault(c => c.Id == id);
            if (entity == null)
                throw new NotFoundException($"Coupon with id {id} not found.");
            return entity;
        }

        public Coupon GetByCode(string code)
        {
            var entity = _dbSet.FirstOrDefault(c => c.Code == code.ToUpper());
            if (entity == null)
                throw new NotFoundException($"Coupon with code {code} not found.");
            return entity;
        }

        public Coupon Create(Coupon coupon)
        {
            _dbSet.Add(coupon);
            _dbContext.SaveChanges();
            return coupon;
        }

        public Coupon Update(Coupon coupon)
        {
            try
            {
                _dbContext.Update(coupon);
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return coupon;
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IEnumerable<Coupon> GetByAuthor(int authorId)
        {
            return _dbSet
                .Where(c => c.AuthorId == authorId)
                .OrderByDescending(c => c.Id)
                .ToList();
        }

        public bool CodeExists(string code)
        {
            return _dbSet.Any(c => c.Code == code.ToUpper());
        }
    }
}