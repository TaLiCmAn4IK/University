using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WatchShop.Data;
using WatchShop.Entities;

namespace WatchShop.Services
{
    /// <summary>
    /// Репозиторий товаров через Entity Framework Core (лаб 9).
    /// Работает с той же БД WatchShopDB, что и ADO.NET-репозиторий из лаб 8,
    /// но через ORM: LINQ to Entities вместо ручного SQL.
    ///
    /// Демонстрирует:
    ///   - CRUD (Create/Read/Update/Delete) через DbContext
    ///   - LINQ to Entities (Where, OrderBy, Select)
    ///   - фильтрацию и поиск по нескольким полям
    ///   - асинхронные методы (ToListAsync, FirstOrDefaultAsync, SaveChangesAsync)
    ///   - транзакции (Database.BeginTransactionAsync)
    ///   - загрузку связанных данных (Include — Eager Loading)
    /// </summary>
    public class EfWatchRepository
    {
        // ---------- READ: все товары (Eager Loading связанных данных) ----------

        /// <summary>
        /// Асинхронно читает все товары вместе с производителем и категорией.
        /// Include(...) — Eager Loading: EF подтянет связанные сущности
        /// одним запросом с JOIN, без отдельных обращений к БД.
        /// AsNoTracking() — read-only выборка, EF не отслеживает изменения
        /// (быстрее и меньше памяти, т.к. для показа правки не нужны).
        /// </summary>
        public async Task<List<WatchEntity>> GetAllAsync()
        {
            using var ctx = new WatchShopDbContext();
            return await ctx.Watches
                .Include(w => w.Manufacturer)
                .Include(w => w.Category)
                .AsNoTracking()
                .OrderBy(w => w.ShortName)
                .ToListAsync();
        }

        // ---------- READ: один товар по Id ----------

        public async Task<WatchEntity?> GetByIdAsync(int id)
        {
            using var ctx = new WatchShopDbContext();
            return await ctx.Watches
                .Include(w => w.Manufacturer)
                .Include(w => w.Category)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        // ---------- READ: фильтрация + поиск по нескольким полям + сортировка ----------

        /// <summary>
        /// Гибкая выборка: поиск по тексту (по нескольким полям сразу),
        /// фильтр по цене и наличию, сортировка. Всё это LINQ to Entities —
        /// EF переведёт выражения в один SQL-запрос.
        /// Запрос строится постепенно (IQueryable), к БД уходит только
        /// при ToListAsync().
        /// </summary>
        public async Task<List<WatchEntity>> SearchAsync(
            string? searchText,
            decimal? minPrice,
            decimal? maxPrice,
            bool inStockOnly,
            string sortBy)
        {
            using var ctx = new WatchShopDbContext();

            // IQueryable — отложенное выполнение. Пока к БД не обращаемся.
            IQueryable<WatchEntity> query = ctx.Watches
                .Include(w => w.Manufacturer)
                .Include(w => w.Category)
                .AsNoTracking();

            // Поиск по НЕСКОЛЬКИМ полям: имя, полное имя, описание, производитель
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string s = searchText.Trim();
                query = query.Where(w =>
                    w.ShortName.Contains(s) ||
                    (w.FullName != null && w.FullName.Contains(s)) ||
                    (w.Description != null && w.Description.Contains(s)) ||
                    (w.Manufacturer != null && w.Manufacturer.Name.Contains(s)));
            }

            if (minPrice.HasValue)
                query = query.Where(w => w.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(w => w.Price <= maxPrice.Value);

            if (inStockOnly)
                query = query.Where(w => w.InStock);

            // Сортировка (LINQ OrderBy / OrderByDescending)
            query = sortBy switch
            {
                "PriceAsc"  => query.OrderBy(w => w.Price),
                "PriceDesc" => query.OrderByDescending(w => w.Price),
                "Rating"    => query.OrderByDescending(w => w.Rating),
                "Sold"      => query.OrderByDescending(w => w.SoldCount),
                _           => query.OrderBy(w => w.ShortName),
            };

            // Только здесь формируется и выполняется SQL
            return await query.ToListAsync();
        }

        // ---------- CREATE ----------

        /// <summary>
        /// Добавление товара в транзакции. Производитель и категория
        /// разрешаются (находятся или создаются) в той же транзакции —
        /// если что-то упадёт, откатывается всё.
        /// </summary>
        public async Task<int> AddAsync(WatchEntity watch, string? manufacturerName, string? categoryName)
        {
            using var ctx = new WatchShopDbContext();

            // Явная транзакция EF Core
            using IDbContextTransaction tx = await ctx.Database.BeginTransactionAsync();
            try
            {
                watch.ManufacturerId = await ResolveManufacturerAsync(ctx, manufacturerName);
                watch.CategoryId = await ResolveCategoryAsync(ctx, categoryName);
                watch.CreatedAt = DateTime.Now;
                watch.UpdatedAt = DateTime.Now;
                watch.InStock = watch.Quantity > 0;

                ctx.Watches.Add(watch);          // помечаем для вставки
                await ctx.SaveChangesAsync();    // INSERT + получение Id

                await tx.CommitAsync();
                return watch.Id;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // ---------- UPDATE ----------

        public async Task UpdateAsync(WatchEntity watch, string? manufacturerName, string? categoryName)
        {
            using var ctx = new WatchShopDbContext();
            using IDbContextTransaction tx = await ctx.Database.BeginTransactionAsync();
            try
            {
                var existing = await ctx.Watches.FirstOrDefaultAsync(w => w.Id == watch.Id);
                if (existing == null)
                    throw new InvalidOperationException($"Товар Id={watch.Id} не найден");

                existing.ManufacturerId = await ResolveManufacturerAsync(ctx, manufacturerName);
                existing.CategoryId = await ResolveCategoryAsync(ctx, categoryName);

                existing.ShortName = watch.ShortName;
                existing.FullName = watch.FullName;
                existing.Description = watch.Description;
                existing.Price = watch.Price;
                existing.Quantity = watch.Quantity;
                existing.Rating = watch.Rating;
                existing.Discount = watch.Discount;
                existing.InStock = watch.Quantity > 0;
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

                // EF сам отследил изменения объекта existing (Change Tracker)
                await ctx.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // ---------- DELETE ----------

        public async Task DeleteAsync(int id)
        {
            using var ctx = new WatchShopDbContext();
            var entity = await ctx.Watches.FirstOrDefaultAsync(w => w.Id == id);
            if (entity == null) return;

            ctx.Watches.Remove(entity);   // помечаем на удаление
            await ctx.SaveChangesAsync(); // DELETE
        }

        // ---------- Агрегаты через LINQ ----------

        public async Task<(int total, int inStock, decimal avgPrice)> GetStatsAsync()
        {
            using var ctx = new WatchShopDbContext();
            int total = await ctx.Watches.CountAsync();
            int inStock = await ctx.Watches.CountAsync(w => w.InStock);
            decimal avg = total > 0
                ? await ctx.Watches.AverageAsync(w => w.Price)
                : 0m;
            return (total, inStock, avg);
        }

        public async Task<List<WatchEntity>> GetTopSellingAsync(int topN)
        {
            using var ctx = new WatchShopDbContext();
            return await ctx.Watches
                .AsNoTracking()
                .OrderByDescending(w => w.SoldCount)
                .Take(topN)
                .ToListAsync();
        }

        // ---------- Помощники: upsert производителя / категории ----------

        private static async Task<int?> ResolveManufacturerAsync(WatchShopDbContext ctx, string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            var existing = await ctx.Manufacturers
                .FirstOrDefaultAsync(m => m.Name == name);
            if (existing != null) return existing.Id;

            var created = new ManufacturerEntity { Name = name };
            ctx.Manufacturers.Add(created);
            await ctx.SaveChangesAsync();
            return created.Id;
        }

        private static async Task<int?> ResolveCategoryAsync(WatchShopDbContext ctx, string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            var existing = await ctx.Categories
                .FirstOrDefaultAsync(c => c.Name == name);
            if (existing != null) return existing.Id;

            var created = new CategoryEntity { Name = name };
            ctx.Categories.Add(created);
            await ctx.SaveChangesAsync();
            return created.Id;
        }
    }
}
