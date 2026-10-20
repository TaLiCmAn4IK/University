using Microsoft.EntityFrameworkCore.Design;

namespace WatchShop.Data
{
    /// <summary>
    /// Фабрика DbContext для design-time операций EF Core
    /// (Add-Migration, Update-Database, Scaffold-DbContext).
    ///
    /// Когда выполняешь "Add-Migration" в Package Manager Console,
    /// инструменты EF не запускают WPF-приложение, поэтому им нужен
    /// способ создать DbContext отдельно. EF ищет класс, реализующий
    /// IDesignTimeDbContextFactory, и вызывает CreateDbContext().
    ///
    /// Наш WatchShopDbContext сам конфигурирует подключение в
    /// OnConfiguring (читает строку из App.config), поэтому здесь
    /// достаточно просто вернуть новый экземпляр.
    /// </summary>
    public class WatchShopDbContextFactory : IDesignTimeDbContextFactory<WatchShopDbContext>
    {
        public WatchShopDbContext CreateDbContext(string[] args)
            => new WatchShopDbContext();
    }
}
