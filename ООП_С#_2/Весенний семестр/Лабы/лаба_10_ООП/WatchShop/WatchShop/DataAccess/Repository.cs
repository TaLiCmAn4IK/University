using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WatchShop.Data;

namespace WatchShop.DataAccess
{
    /// <summary>
    /// Обобщённая реализация паттерна Repository поверх EF Core.
    ///
    /// Важно: репозиторий НЕ создаёт свой DbContext и НЕ вызывает
    /// SaveChanges. Контекст передаётся снаружи (из Unit of Work),
    /// один на все репозитории. SaveChanges делает Unit of Work —
    /// так несколько операций по разным репозиториям фиксируются
    /// как одна атомарная транзакция.
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        // Контекст и набор приходят извне (от UnitOfWork) — это и есть
        // слабая связанность: репозиторий не управляет временем жизни ctx.
        protected readonly WatchShopDbContext Context;
        protected readonly DbSet<T> DbSet;

        public Repository(WatchShopDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
            => await DbSet.AsNoTracking().ToListAsync();

        public async Task<T?> GetByIdAsync(int id)
            => await DbSet.FindAsync(id);

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await DbSet.Where(predicate).ToListAsync();

        public IQueryable<T> Query()
            => DbSet.AsQueryable();

        public async Task AddAsync(T entity)
            => await DbSet.AddAsync(entity);

        public void Update(T entity)
            => DbSet.Update(entity);

        public void Remove(T entity)
            => DbSet.Remove(entity);

        public async Task RemoveByIdAsync(int id)
        {
            var entity = await DbSet.FindAsync(id);
            if (entity != null)
                DbSet.Remove(entity);
        }
    }
}
