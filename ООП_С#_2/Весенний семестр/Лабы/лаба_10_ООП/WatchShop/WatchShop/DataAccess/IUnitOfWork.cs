using System;
using System.Threading.Tasks;
using WatchShop.Entities;

namespace WatchShop.DataAccess
{
    /// <summary>
    /// Паттерн Unit of Work ("единица работы").
    ///это паттерн проектирования, который объединяет
    ///несколько репозиториев в одну транзакцию и фиксирует все изменения одним вызовом.
    ///
    /// Unit of Work держит ОДИН общий DbContext и раздаёт его всем
    /// в рамках ОДНОЙ транзакции — либо всё, либо ничего.
    /// Даёт доступ ко всем репозиториям через одно место
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Репозитории для каждой сущности. Все они работают с одним
        // и тем же контекстом внутри этого UoW.
        IRepository<WatchEntity> Watches { get; }
        IRepository<ManufacturerEntity> Manufacturers { get; }
        IRepository<CategoryEntity> Categories { get; }
        IRepository<UserEntity> Users { get; }
        IRepository<OrderEntity> Orders { get; }
        IRepository<OrderItemEntity> OrderItems { get; }

        /// <summary>
        /// Зафиксировать все накопленные изменения как одну транзакцию.
        /// Возвращает количество затронутых строк.
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Выполнить действие в явной транзакции. Если внутри
        /// бросится исключение — всё откатывается.
        /// </summary>
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
