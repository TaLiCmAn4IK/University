using System.Configuration;
using Microsoft.EntityFrameworkCore;
using WatchShop.Entities;

namespace WatchShop.Data
{
    /// <summary>
    /// DbContext — главный класс EF Core. Представляет сессию работы с БД:
    ///   - содержит DbSet&lt;T&gt; для каждой сущности (это "таблицы" в коде)
    ///   - отслеживает изменения объектов (Change Tracker)
    ///   - транслирует LINQ-запросы в SQL
    ///   - применяет изменения через SaveChanges()
    ///
    /// Подход Code First: схему БД описывают C#-классы (Entities) +
    /// конфигурация здесь. Аннотации заданы прямо в сущностях,
    /// а связи и тонкая настройка — через Fluent API в OnModelCreating.
    /// </summary>
    public class WatchShopDbContext : DbContext
    {
        // DbSet — точка доступа к таблице. context.Watches это IQueryable,
        // по которому строятся LINQ to Entities запросы.
        public DbSet<WatchEntity> Watches => Set<WatchEntity>();
        public DbSet<ManufacturerEntity> Manufacturers => Set<ManufacturerEntity>();
        public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<OrderEntity> Orders => Set<OrderEntity>();
        public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();

        /// <summary>
        /// Конфигурация подключения. Строка берётся из App.config
        /// (требование ТЗ: параметры в конфигурационном файле).
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string cs = ConfigurationManager
                    .ConnectionStrings["EfConnection"].ConnectionString;
                optionsBuilder.UseSqlServer(cs);
            }
        }

        /// <summary>
        /// Fluent API — программная настройка модели. Используется для вещей,
        /// которые неудобно или нельзя задать аннотациями: составные правила,
        /// поведение при удалении (DeleteBehavior), точные имена связей,
        /// уникальные индексы.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---- Manufacturer (1) → (∞) Watch ----
            modelBuilder.Entity<WatchEntity>()
                .HasOne(w => w.Manufacturer)
                .WithMany(m => m.Watches)
                .HasForeignKey(w => w.ManufacturerId)
                .OnDelete(DeleteBehavior.SetNull); // удалили производителя → у часов FK = NULL

            // ---- Category (1) → (∞) Watch ----
            modelBuilder.Entity<WatchEntity>()
                .HasOne(w => w.Category)
                .WithMany(c => c.Watches)
                .HasForeignKey(w => w.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // ---- User (1) → (∞) Order ----
            modelBuilder.Entity<OrderEntity>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); // нельзя удалить юзера с заказами

            // ---- Order (1) → (∞) OrderItem ----
            modelBuilder.Entity<OrderItemEntity>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // удалили заказ → удалились позиции

            // ---- Watch (1) → (∞) OrderItem ----
            modelBuilder.Entity<OrderItemEntity>()
                .HasOne(oi => oi.Watch)
                .WithMany(w => w.OrderItems)
                .HasForeignKey(oi => oi.WatchId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- Уникальные индексы через Fluent API ----
            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<ManufacturerEntity>()
                .HasIndex(m => m.Name)
                .IsUnique();

            modelBuilder.Entity<CategoryEntity>()
                .HasIndex(c => c.Name)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
