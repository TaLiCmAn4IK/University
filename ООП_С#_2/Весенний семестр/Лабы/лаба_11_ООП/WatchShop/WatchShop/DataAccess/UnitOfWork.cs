using System;
using System.Threading.Tasks;
using WatchShop.Data;
using WatchShop.Entities;

namespace WatchShop.DataAccess
{
    /// <summary>
    /// Реализация Unit of Work. Создаёт ОДИН WatchShopDbContext и
    /// "ленивые" репозитории поверх него. Все репозитории делят этот
    /// контекст, поэтому их изменения фиксируются вместе.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WatchShopDbContext _context;

        // Репозитории создаются по требованию (lazy) и кэшируются.
        private IRepository<WatchEntity>? _watches;
        private IRepository<ManufacturerEntity>? _manufacturers;
        private IRepository<CategoryEntity>? _categories;
        private IRepository<UserEntity>? _users;
        private IRepository<OrderEntity>? _orders;
        private IRepository<OrderItemEntity>? _orderItems;

        public UnitOfWork()
        {
            // Единственный контекст на всю единицу работы.
            _context = new WatchShopDbContext();
        }

        // Каждое свойство отдаёт репозиторий, привязанный к ОБЩЕМУ _context.
        public IRepository<WatchEntity> Watches
            => _watches ??= new Repository<WatchEntity>(_context);

        public IRepository<ManufacturerEntity> Manufacturers
            => _manufacturers ??= new Repository<ManufacturerEntity>(_context);

        public IRepository<CategoryEntity> Categories
            => _categories ??= new Repository<CategoryEntity>(_context);

        public IRepository<UserEntity> Users
            => _users ??= new Repository<UserEntity>(_context);

        public IRepository<OrderEntity> Orders
            => _orders ??= new Repository<OrderEntity>(_context);

        public IRepository<OrderItemEntity> OrderItems
            => _orderItems ??= new Repository<OrderItemEntity>(_context);

        /// <summary>
        /// Один SaveChanges на весь контекст — атомарно применяет
        /// все накопленные изменения всех репозиториев.
        /// EF Core сам оборачивает SaveChanges в транзакцию.
        /// </summary>
        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        /// <summary>
        /// Явная транзакция для составных операций (несколько SaveChanges
        /// подряд, которые должны быть атомарны вместе).
        /// </summary>
        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                await action();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// UoW владеет контекстом — освобождает его. Вызывается через
        /// using в бизнес-слое.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
