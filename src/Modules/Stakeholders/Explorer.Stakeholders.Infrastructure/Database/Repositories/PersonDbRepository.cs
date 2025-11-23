using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class PersonDbRepository : IPersonRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<Person> _dbSet; 

    public PersonDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Person>();
    }

    public Person? Get(long id)
    {
        return DbContext.People.FirstOrDefault(p => p.Id == id);
    }

    public Person Create(Person entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public PagedResult<Person> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }
}