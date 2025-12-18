using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class UserDbRepository : IUserRepository
{
    private readonly StakeholdersContext _dbContext;
    private readonly DbSet<User> _dbSet;

    public UserDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<User>();
    }

    public bool Exists(string username)
    {
        return _dbContext.Users.Any(user => user.Username == username);
    }

    public User? GetActiveByName(string username)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Username == username && user.IsActive);
    }

    public User? Get(long userId)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Id == userId);
    }

    public User Create(User user)
    {
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
        return user;
    }

    public long GetPersonId(long userId)
    {
        var person = _dbContext.People.FirstOrDefault(i => i.UserId == userId);
        if (person == null) throw new KeyNotFoundException("Not found.");
        return person.Id;
    }

    public User Update(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be found.");

        try
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            return user;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("Failed to update user due to concurrency conflict.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Failed to update user due to database error.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Unexpected error while updating user.", ex);
        }
    }

    public PagedResult<User> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public User? GetById(long userId)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Id == userId);
    }

    public Person? GetPersonByUserId(long userId)
    {
        return _dbContext.People.FirstOrDefault(p => p.UserId == userId);
    }
    public IEnumerable<User> GetAllActiveTourists()
    {
        return _dbSet
            .Where(u => u.IsActive && u.Role == UserRole.Tourist)
            .ToList();
    }

}