using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WatchShop.DataAccess;
using WatchShop.Entities;

namespace WatchShop.Business
{
    /// <summary>
    /// Business Layer (уровень бизнес-логики) для товаров.
    ///
    /// Этот класс НЕ знает про EF Core, DbContext, SQL. Он работает
    /// только через абстракцию IUnitOfWork → IRepository. Здесь живут
    /// бизнес-правила (InStock = Quantity > 0, upsert производителя,
    /// атомарность создания товара), а как именно данные сохраняются —
    /// забота Data Access Layer.
    ///
    /// Это многоуровневая архитектура:
    ///   UI (EfDemoWindow)
    ///      ↓ вызывает
    ///   Business Layer (WatchService)   ← бизнес-правила
    ///      ↓ через IUnitOfWork / IRepository
    ///   Data Access Layer (UnitOfWork, Repository)  ← доступ к данным
    ///      ↓ EF Core
    ///   База данных WatchShopDB
    ///
    /// Слои связаны через интерфейсы (слабая связанность): можно
    /// заменить реализацию DAL, не трогая бизнес-логику и UI.
    /// </summary>
    public class WatchService
    {
        // ---------- READ ----------

        public async Task<List<WatchEntity>> GetAllAsync()
        {
            using IUnitOfWork uow = new UnitOfWork();
            return await uow.Watches.Query()
                .Include(w => w.Manufacturer)
                .Include(w => w.Category)
                .AsNoTracking()
                .OrderBy(w => w.ShortName)
                .ToListAsync();
        }

        public async Task<WatchEntity?> GetByIdAsync(int id)
        {
            using IUnitOfWork uow = new UnitOfWork();
            return await uow.Watches.Query()
                .Include(w => w.Manufacturer)
                .Include(w => w.Category)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        /// <summary>
        /// Поиск по нескольким полям + фильтр + сортировка.
        /// Строится через IQueryable репозитория (LINQ to Entities).
        /// </summary>
        public async Task<List<WatchEntity>> SearchAsync(
            string? searchText, decimal? minPrice, decimal? maxPrice,
            bool inStockOnly, string sortBy)
        {
            using IUnitOfWork uow = new UnitOfWork();

            IQueryable<WatchEntity> query = uow.Watches.Query()
                .Include(w => w.Manufacturer)
                .Include(w => w.Category)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string s = searchText.Trim();
                query = query.Where(w =>
                    w.ShortName.Contains(s) ||
                    (w.FullName != null && w.FullName.Contains(s)) ||
                    (w.Description != null && w.Description.Contains(s)) ||
                    (w.Manufacturer != null && w.Manufacturer.Name.Contains(s)));
            }

            if (minPrice.HasValue) query = query.Where(w => w.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(w => w.Price <= maxPrice.Value);
            if (inStockOnly) query = query.Where(w => w.InStock);

            query = sortBy switch
            {
                "PriceAsc"  => query.OrderBy(w => w.Price),
                "PriceDesc" => query.OrderByDescending(w => w.Price),
                "Rating"    => query.OrderByDescending(w => w.Rating),
                "Sold"      => query.OrderByDescending(w => w.SoldCount),
                _           => query.OrderBy(w => w.ShortName),
            };

            return await query.ToListAsync();
        }

        // ---------- CREATE ----------

        /// <summary>
        /// Создание товара. Производитель/категория разрешаются и сам
        /// товар сохраняются в ОДНОЙ единице работы — один SaveChanges,
        /// одна транзакция (атомарно). Это и есть смысл Unit of Work.
        /// </summary>
        public async Task<int> AddAsync(WatchEntity watch,
            string? manufacturerName, string? categoryName)
        {
            using IUnitOfWork uow = new UnitOfWork();

            watch.ManufacturerId = await ResolveManufacturerAsync(uow, manufacturerName);
            watch.CategoryId = await ResolveCategoryAsync(uow, categoryName);
            watch.CreatedAt = DateTime.Now;
            watch.UpdatedAt = DateTime.Now;
            watch.InStock = watch.Quantity > 0;       // бизнес-правило

            await uow.Watches.AddAsync(watch);
            await uow.SaveChangesAsync();             // одна транзакция
            return watch.Id;
        }

        // ---------- UPDATE ----------

        public async Task UpdateAsync(WatchEntity watch,
            string? manufacturerName, string? categoryName)
        {
            using IUnitOfWork uow = new UnitOfWork();

            var existing = await uow.Watches.GetByIdAsync(watch.Id)
                ?? throw new InvalidOperationException($"Товар Id={watch.Id} не найден");

            existing.ManufacturerId = await ResolveManufacturerAsync(uow, manufacturerName);
            existing.CategoryId = await ResolveCategoryAsync(uow, categoryName);
            existing.ShortName = watch.ShortName;
            existing.FullName = watch.FullName;
            existing.Description = watch.Description;
            existing.Price = watch.Price;
            existing.Quantity = watch.Quantity;
            existing.Rating = watch.Rating;
            existing.Discount = watch.Discount;
            existing.InStock = watch.Quantity > 0;    // бизнес-правило
            existing.Color = watch.Color;
            existing.Size = watch.Size;
            existing.Material = watch.Material;
            existing.Gender = watch.Gender;
            existing.MovementType = watch.MovementType;
            existing.WaterResistance = watch.WaterResistance;
            existing.SoldCount = watch.SoldCount;
            existing.Image = watch.Image;
            existing.ImagePath = watch.ImagePath;
            existing.UpdatedAt = DateTime.Now;

            uow.Watches.Update(existing);
            await uow.SaveChangesAsync();
        }

        // ---------- DELETE ----------

        public async Task DeleteAsync(int id)
        {
            using IUnitOfWork uow = new UnitOfWork();
            await uow.Watches.RemoveByIdAsync(id);
            await uow.SaveChangesAsync();
        }

        // ---------- Пример составной атомарной операции ----------

        /// <summary>
        /// Оформление заказа: создаём заказ + позицию + уменьшаем остаток
        /// товара. Три изменения по ТРЁМ разным репозиториям, но один
        /// SaveChanges внутри ExecuteInTransactionAsync — либо всё, либо
        /// ничего. Это демонстрирует главное преимущество Unit of Work.
        /// </summary>
        public async Task<int> CreateOrderAsync(int userId, int watchId, int quantity)
        {
            using IUnitOfWork uow = new UnitOfWork();
            int orderId = 0;

            await uow.ExecuteInTransactionAsync(async () =>
            {
                var watch = await uow.Watches.GetByIdAsync(watchId)
                    ?? throw new InvalidOperationException("Товар не найден");

                if (watch.Quantity < quantity)
                    throw new InvalidOperationException("Недостаточно товара на складе");

                decimal unitPrice = watch.Price * (1 - (decimal)watch.Discount / 100m);

                var order = new OrderEntity
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    TotalAmount = unitPrice * quantity,
                    Status = "Новый"
                };
                await uow.Orders.AddAsync(order);
                await uow.SaveChangesAsync();        // получили order.Id

                await uow.OrderItems.AddAsync(new OrderItemEntity
                {
                    OrderId = order.Id,
                    WatchId = watchId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });

                watch.Quantity -= quantity;          // бизнес-правило
                watch.SoldCount += quantity;
                watch.InStock = watch.Quantity > 0;
                uow.Watches.Update(watch);

                await uow.SaveChangesAsync();
                orderId = order.Id;
            });

            return orderId;
        }

        // ---------- Бизнес-помощники (upsert справочников) ----------

        private static async Task<int?> ResolveManufacturerAsync(
            IUnitOfWork uow, string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            var existing = (await uow.Manufacturers
                .FindAsync(m => m.Name == name)).FirstOrDefault();
            if (existing != null) return existing.Id;

            var created = new ManufacturerEntity { Name = name };
            await uow.Manufacturers.AddAsync(created);
            await uow.SaveChangesAsync();
            return created.Id;
        }

        private static async Task<int?> ResolveCategoryAsync(
            IUnitOfWork uow, string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            var existing = (await uow.Categories
                .FindAsync(c => c.Name == name)).FirstOrDefault();
            if (existing != null) return existing.Id;

            var created = new CategoryEntity { Name = name };
            await uow.Categories.AddAsync(created);
            await uow.SaveChangesAsync();
            return created.Id;
        }
    }
}
