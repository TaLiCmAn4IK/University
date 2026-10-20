using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Flux.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _set;

    public Repository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync() => await _set.ToListAsync();

    public virtual async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _set.Where(predicate).ToListAsync();

    public IQueryable<T> Query() => _set.AsQueryable();

    public virtual async Task AddAsync(T entity) => await _set.AddAsync(entity);

    public virtual Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}

public class UserRepository : Repository<Models.User>
{
    public UserRepository(AppDbContext db) : base(db) { }
}

public class MatchRepository : Repository<Models.Match>
{
    public MatchRepository(AppDbContext db) : base(db) { }
}

public class MatchPlayerRepository : Repository<Models.MatchPlayer>
{
    public MatchPlayerRepository(AppDbContext db) : base(db) { }
}

public class QueueEntryRepository : Repository<Models.QueueEntry>
{
    public QueueEntryRepository(AppDbContext db) : base(db) { }
}
