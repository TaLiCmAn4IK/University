using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WatchShop.Models;

namespace WatchShop.Services
{
    /// <summary>
    /// Репозиторий для таблицы Watches и связанных таблиц.
    /// Демонстрирует основные классы ADO.NET:
    ///   SqlConnection, SqlCommand, SqlDataReader, SqlDataAdapter,
    ///   SqlParameter, SqlTransaction.
    /// Часть методов асинхронные (заканчиваются на ...Async).
    /// </summary>
    public class WatchRepository
    {
        private readonly string connectionString;

        public WatchRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["WatchShopConnection"].ConnectionString;
        }

        // =========================================================================
        // READ — через SqlDataAdapter + DataTable.
        // SqlDataAdapter удобен для отображения в DataGrid (привязка к
        // DataTable.DefaultView). Используется для админ-панели.
        // =========================================================================
        public DataTable GetAllWatchesAsTable()
        {
            using var conn = new SqlConnection(connectionString);
            using var adapter = new SqlDataAdapter("SELECT * FROM vw_WatchesFull", conn);
            var table = new DataTable("Watches");
            adapter.Fill(table);
            return table;
        }

        public DataTable GetAllUsersAsTable()
        {
            using var conn = new SqlConnection(connectionString);
            using var adapter = new SqlDataAdapter(
                "SELECT Id, Username, Role, Email, RegistrationDate, IsActive FROM Users", conn);
            var table = new DataTable("Users");
            adapter.Fill(table);
            return table;
        }

        public DataTable GetAllOrdersAsTable()
        {
            using var conn = new SqlConnection(connectionString);
            using var adapter = new SqlDataAdapter(@"
                SELECT o.Id, u.Username, o.OrderDate, o.TotalAmount, o.Status
                FROM Orders o
                INNER JOIN Users u ON o.UserId = u.Id
                ORDER BY o.OrderDate DESC", conn);
            var table = new DataTable("Orders");
            adapter.Fill(table);
            return table;
        }

        // =========================================================================
        // READ через SqlDataReader (forward-only, экономнее) — используется в VM.
        // =========================================================================
        public List<Watch> GetAllWatches()
        {
            var list = new List<Watch>();
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            const string sql = @"
                SELECT w.Id, w.ShortName, w.FullName, w.Description, w.Price, w.Quantity,
                       w.Rating, w.Discount, w.InStock, w.Color, w.Size, w.Material,
                       w.Gender, w.MovementType, w.WaterResistance, w.SoldCount, w.ImagePath, w.Image,
                       m.Name AS ManufacturerName, c.Name AS CategoryName
                FROM Watches w
                LEFT JOIN Manufacturers m ON w.ManufacturerId = m.Id
                LEFT JOIN Categories    c ON w.CategoryId     = c.Id
                ORDER BY w.ShortName";

            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(MapWatch(reader));

            return list;
        }

        /// <summary>Асинхронная версия — не блокирует UI-поток.</summary>
        public async Task<List<Watch>> GetAllWatchesAsync()
        {
            var list = new List<Watch>();
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT w.Id, w.ShortName, w.FullName, w.Description, w.Price, w.Quantity,
                       w.Rating, w.Discount, w.InStock, w.Color, w.Size, w.Material,
                       w.Gender, w.MovementType, w.WaterResistance, w.SoldCount, w.ImagePath, w.Image,
                       m.Name AS ManufacturerName, c.Name AS CategoryName
                FROM Watches w
                LEFT JOIN Manufacturers m ON w.ManufacturerId = m.Id
                LEFT JOIN Categories    c ON w.CategoryId     = c.Id
                ORDER BY w.ShortName";

            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapWatch(reader));

            return list;
        }

        // =========================================================================
        // Запрос с параметрами через хранимую процедуру sp_GetWatchesFiltered.
        // =========================================================================
        public async Task<DataTable> GetFilteredAsync(
            string? searchText, int? categoryId, decimal? minPrice, decimal? maxPrice,
            bool inStockOnly, string sortBy = "Name")
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_GetWatchesFiltered", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@SearchText",  SqlDbType.NVarChar, 100).Value = (object?)searchText ?? DBNull.Value;
            cmd.Parameters.Add("@CategoryId",  SqlDbType.Int).Value           = (object?)categoryId ?? DBNull.Value;
            cmd.Parameters.Add("@MinPrice",    SqlDbType.Decimal).Value       = (object?)minPrice ?? DBNull.Value;
            cmd.Parameters.Add("@MaxPrice",    SqlDbType.Decimal).Value       = (object?)maxPrice ?? DBNull.Value;
            cmd.Parameters.Add("@InStockOnly", SqlDbType.Bit).Value           = inStockOnly;
            cmd.Parameters.Add("@SortBy",      SqlDbType.NVarChar, 20).Value  = sortBy;

            var table = new DataTable("FilteredWatches");
            using var reader = await cmd.ExecuteReaderAsync();
            table.Load(reader);
            return table;
        }

        // =========================================================================
        // CREATE с транзакцией. ResolveManufacturerId/ResolveCategoryId работают
        // в одной транзакции — если что-то упадёт, никаких "обрывков" в БД.
        // =========================================================================
        public int InsertWatch(Watch w)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                int? manufacturerId = ResolveManufacturerId(conn, tx, w.Manufacturer);
                int? categoryId = ResolveCategoryId(conn, tx, w.Category);

                const string sql = @"
                    INSERT INTO Watches
                        (ShortName, FullName, Description, ManufacturerId, CategoryId,
                         Price, Quantity, Rating, Discount, InStock, Color, Size,
                         Material, Gender, MovementType, WaterResistance, SoldCount, ImagePath, Image)
                    OUTPUT INSERTED.Id
                    VALUES
                        (@ShortName, @FullName, @Description, @ManufacturerId, @CategoryId,
                         @Price, @Quantity, @Rating, @Discount, @InStock, @Color, @Size,
                         @Material, @Gender, @MovementType, @WaterResistance, @SoldCount, @ImagePath, @Image)";

                using var cmd = new SqlCommand(sql, conn, tx);
                AddWatchParameters(cmd, w, manufacturerId, categoryId);

                int newId = (int)cmd.ExecuteScalar();
                tx.Commit();
                return newId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // =========================================================================
        // UPDATE с транзакцией.
        // =========================================================================
        public void UpdateWatch(Watch w)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                int? manufacturerId = ResolveManufacturerId(conn, tx, w.Manufacturer);
                int? categoryId = ResolveCategoryId(conn, tx, w.Category);

                const string sql = @"
                    UPDATE Watches SET
                        ShortName=@ShortName, FullName=@FullName, Description=@Description,
                        ManufacturerId=@ManufacturerId, CategoryId=@CategoryId,
                        Price=@Price, Quantity=@Quantity, Rating=@Rating, Discount=@Discount,
                        InStock=@InStock, Color=@Color, Size=@Size, Material=@Material,
                        Gender=@Gender, MovementType=@MovementType,
                        WaterResistance=@WaterResistance, SoldCount=@SoldCount,
                        ImagePath=@ImagePath, Image=@Image, UpdatedAt=GETDATE()
                    WHERE Id=@Id";

                using var cmd = new SqlCommand(sql, conn, tx);
                AddWatchParameters(cmd, w, manufacturerId, categoryId);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = w.Id;
                cmd.ExecuteNonQuery();

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // =========================================================================
        // DELETE.
        // =========================================================================
        public void DeleteWatch(int watchId)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Watches WHERE Id=@Id", conn);
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = watchId;
            cmd.ExecuteNonQuery();
        }

        // =========================================================================
        // ExecuteScalar — простой запрос на одно значение.
        // =========================================================================
        public int CountWatches()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM Watches", conn);
            return (int)cmd.ExecuteScalar();
        }

        // =========================================================================
        // Хранимая процедура sp_GetTopSelling — асинхронно.
        // Возвращает DTO TopSellingItem (свойства, чтобы XAML-биндинги работали).
        // =========================================================================
        public async Task<List<TopSellingItem>> GetTopSellingAsync(int topN)
        {
            var list = new List<TopSellingItem>();
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_GetTopSelling", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@TopN", SqlDbType.Int).Value = topN;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new TopSellingItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("ShortName")),
                    SoldCount = reader.GetInt32(reader.GetOrdinal("SoldCount"))
                });
            }
            return list;
        }

        // =========================================================================
        // Хранимая процедура sp_CreateOrder с OUTPUT-параметром.
        // Используется при оформлении заказа — внутри процедуры свой
        // BEGIN TRANSACTION / COMMIT.
        // =========================================================================
        public int CreateOrder(int userId, int watchId, int quantity)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var cmd = new SqlCommand("sp_CreateOrder", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@UserId",  SqlDbType.Int).Value = userId;
            cmd.Parameters.Add("@WatchId", SqlDbType.Int).Value = watchId;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = quantity;

            var orderIdParam = cmd.Parameters.Add("@OrderId", SqlDbType.Int);
            orderIdParam.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();
            return (int)orderIdParam.Value;
        }

        // =========================================================================
        // Хранимая процедура sp_GetStatistics — асинхронно.
        // =========================================================================
        public async Task<DataTable> GetStatisticsAsync()
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("sp_GetStatistics", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            var table = new DataTable("Statistics");
            using var reader = await cmd.ExecuteReaderAsync();
            table.Load(reader);
            return table;
        }

        // =========================================================================
        // Внутренние помощники
        // =========================================================================

        private static Watch MapWatch(SqlDataReader r) => new()
        {
            Id = r.GetInt32(r.GetOrdinal("Id")),
            ShortName = r.GetString(r.GetOrdinal("ShortName")),
            FullName = r.IsDBNull(r.GetOrdinal("FullName")) ? "" : r.GetString(r.GetOrdinal("FullName")),
            Description = r.IsDBNull(r.GetOrdinal("Description")) ? "" : r.GetString(r.GetOrdinal("Description")),
            Price = r.GetDecimal(r.GetOrdinal("Price")),
            Quantity = r.GetInt32(r.GetOrdinal("Quantity")),
            Rating = r.GetDouble(r.GetOrdinal("Rating")),
            Discount = r.GetDouble(r.GetOrdinal("Discount")),
            InStock = r.GetBoolean(r.GetOrdinal("InStock")),
            Color = r.IsDBNull(r.GetOrdinal("Color")) ? "" : r.GetString(r.GetOrdinal("Color")),
            Size = r.IsDBNull(r.GetOrdinal("Size")) ? "" : r.GetString(r.GetOrdinal("Size")),
            Material = r.IsDBNull(r.GetOrdinal("Material")) ? "" : r.GetString(r.GetOrdinal("Material")),
            Gender = r.IsDBNull(r.GetOrdinal("Gender")) ? "" : r.GetString(r.GetOrdinal("Gender")),
            MovementType = r.IsDBNull(r.GetOrdinal("MovementType")) ? "" : r.GetString(r.GetOrdinal("MovementType")),
            WaterResistance = r.IsDBNull(r.GetOrdinal("WaterResistance")) ? "" : r.GetString(r.GetOrdinal("WaterResistance")),
            SoldCount = r.GetInt32(r.GetOrdinal("SoldCount")),
            ImagePath = r.IsDBNull(r.GetOrdinal("ImagePath")) ? "" : r.GetString(r.GetOrdinal("ImagePath")),
            // Двоичная картинка из VARBINARY(MAX). Если в БД NULL — оставляем null.
            ImageData = r.IsDBNull(r.GetOrdinal("Image")) ? null : (byte[])r["Image"],
            Manufacturer = r.IsDBNull(r.GetOrdinal("ManufacturerName")) ? "" : r.GetString(r.GetOrdinal("ManufacturerName")),
            Category = r.IsDBNull(r.GetOrdinal("CategoryName")) ? "" : r.GetString(r.GetOrdinal("CategoryName")),
            CountryOfOrigin = ""
        };

        private static void AddWatchParameters(SqlCommand cmd, Watch w,
            int? manufacturerId, int? categoryId)
        {
            cmd.Parameters.Add("@ShortName",       SqlDbType.NVarChar, 100).Value = w.ShortName ?? "";
            cmd.Parameters.Add("@FullName",        SqlDbType.NVarChar, 200).Value = (object?)w.FullName ?? DBNull.Value;
            cmd.Parameters.Add("@Description",     SqlDbType.NVarChar, -1).Value  = (object?)w.Description ?? DBNull.Value;
            cmd.Parameters.Add("@ManufacturerId",  SqlDbType.Int).Value           = (object?)manufacturerId ?? DBNull.Value;
            cmd.Parameters.Add("@CategoryId",      SqlDbType.Int).Value           = (object?)categoryId ?? DBNull.Value;
            cmd.Parameters.Add("@Price",           SqlDbType.Decimal).Value       = w.Price;
            cmd.Parameters.Add("@Quantity",        SqlDbType.Int).Value           = w.Quantity;
            cmd.Parameters.Add("@Rating",          SqlDbType.Float).Value         = w.Rating;
            cmd.Parameters.Add("@Discount",        SqlDbType.Float).Value         = w.Discount;
            cmd.Parameters.Add("@InStock",         SqlDbType.Bit).Value           = w.InStock;
            cmd.Parameters.Add("@Color",           SqlDbType.NVarChar, 50).Value  = (object?)w.Color ?? DBNull.Value;
            cmd.Parameters.Add("@Size",            SqlDbType.NVarChar, 20).Value  = (object?)w.Size ?? DBNull.Value;
            cmd.Parameters.Add("@Material",        SqlDbType.NVarChar, 50).Value  = (object?)w.Material ?? DBNull.Value;
            cmd.Parameters.Add("@Gender",          SqlDbType.NVarChar, 20).Value  = (object?)w.Gender ?? DBNull.Value;
            cmd.Parameters.Add("@MovementType",    SqlDbType.NVarChar, 50).Value  = (object?)w.MovementType ?? DBNull.Value;
            cmd.Parameters.Add("@WaterResistance", SqlDbType.NVarChar, 20).Value  = (object?)w.WaterResistance ?? DBNull.Value;
            cmd.Parameters.Add("@SoldCount",       SqlDbType.Int).Value           = w.SoldCount;
            cmd.Parameters.Add("@ImagePath",       SqlDbType.NVarChar, 500).Value = (object?)w.ImagePath ?? DBNull.Value;
            cmd.Parameters.Add("@Image",           SqlDbType.VarBinary, -1).Value = (object?)w.ImageData ?? DBNull.Value;
        }

        /// <summary>Получить или создать запись производителя; вернуть Id или null.</summary>
        private static int? ResolveManufacturerId(SqlConnection conn, SqlTransaction tx, string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            using (var cmd = new SqlCommand("SELECT Id FROM Manufacturers WHERE Name=@n", conn, tx))
            {
                cmd.Parameters.Add("@n", SqlDbType.NVarChar, 100).Value = name;
                var id = cmd.ExecuteScalar();
                if (id != null && id != DBNull.Value) return (int)id;
            }
            using (var cmd = new SqlCommand(
                "INSERT INTO Manufacturers (Name) OUTPUT INSERTED.Id VALUES (@n)", conn, tx))
            {
                cmd.Parameters.Add("@n", SqlDbType.NVarChar, 100).Value = name;
                return (int)cmd.ExecuteScalar();
            }
        }

        private static int? ResolveCategoryId(SqlConnection conn, SqlTransaction tx, string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            using (var cmd = new SqlCommand("SELECT Id FROM Categories WHERE Name=@n", conn, tx))
            {
                cmd.Parameters.Add("@n", SqlDbType.NVarChar, 50).Value = name;
                var id = cmd.ExecuteScalar();
                if (id != null && id != DBNull.Value) return (int)id;
            }
            using (var cmd = new SqlCommand(
                "INSERT INTO Categories (Name) OUTPUT INSERTED.Id VALUES (@n)", conn, tx))
            {
                cmd.Parameters.Add("@n", SqlDbType.NVarChar, 50).Value = name;
                return (int)cmd.ExecuteScalar();
            }
        }
    }

    /// <summary>
    /// DTO для результата хранимой процедуры sp_GetTopSelling.
    /// Используем класс со свойствами (не tuple), чтобы работали XAML-биндинги.
    /// </summary>
    public class TopSellingItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int SoldCount { get; set; }
    }
}
