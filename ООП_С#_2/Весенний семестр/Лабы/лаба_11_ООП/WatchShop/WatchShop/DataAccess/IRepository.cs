using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WatchShop.DataAccess
{
    /// <summary>
    /// Репозиторий — это паттерн проектирования, который
    /// создаёт прослойку между бизнес-логикой и базой данных.
    /// Паттерн Repository (обобщённый).
    ///Определяет контракт — какие методы должен
    ///иметь любой репозиторий для любой сущности.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);

        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);

        IQueryable<T> Query();

        Task AddAsync(T entity);

        void Update(T entity);

        void Remove(T entity);
        Task RemoveByIdAsync(int id);
    }
}
