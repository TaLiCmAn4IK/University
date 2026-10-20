using System.Linq;
using System.Threading.Tasks;
using Flux.Models;
using Microsoft.EntityFrameworkCore;

namespace Flux.Data;

/// <summary>
/// Инициализатор локальной базы данных SQLite.
/// При несовпадении версии схемы (SchemaVersion) полностью пересоздаёт БД через EnsureDeleted + EnsureCreated.
/// миграции не используются.
/// </summary>
public static class DatabaseInitializer
{
    private const int SchemaVersion = 12;

    public static async Task InitializeAsync(AppDbContext db)
    {
        // Если БД ещё нет — создаём
        if (!await db.Database.CanConnectAsync())
        {
            await db.Database.EnsureCreatedAsync();
            await WriteSchemaVersionAsync(db);
        }
        else if (await NeedsResetAsync(db))
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
            await WriteSchemaVersionAsync(db);
        }

        // Сидинг: админ + боты
        if (!await db.Users.AnyAsync(u => u.Role == UserRole.Admin))
        {
            db.Users.Add(new User
            {
                Login = "Admin",
                Nickname = "Admin",
                Email = "admin@flux.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = UserRole.Admin,
                EloCS2 = 1500,
                EloChess = 1500
            });
        }

        if (await db.Users.CountAsync() < 15)
        {
            var names = new[] { "s1mple","ZywOo","NiKo","dev1ce","GuardiaN","olofmeister","GeT_RiGhT","coldzera","electronic","shox","Magnus","Hikaru","Fabiano","Levon","Anish" };
            foreach (var n in names)
            {
                if (!await db.Users.AnyAsync(u => u.Nickname == n))
                {
                    db.Users.Add(new User
                    {
                        Login = n.ToLower(),
                        Nickname = n,
                        Email = $"{n.ToLower()}@bot.local",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                        Role = UserRole.Player,
                        EloCS2 = 900 + System.Random.Shared.Next(300),
                        EloChess = 900 + System.Random.Shared.Next(300)
                    });
                }
            }
        }

        await db.SaveChangesAsync();
    }

    /// <summary>Проверяем, нужна ли пересборка БД (схема устарела).</summary>
    private static async Task<bool> NeedsResetAsync(AppDbContext db)
    {
        try
        {
            if (!await TableExistsAsync(db, "Users")) return true;

            // Сверяем версию
            if (!await TableExistsAsync(db, "SchemaMeta")) return true;
            var stored = await ReadSchemaVersionAsync(db);
            return stored != SchemaVersion;
        }
        catch
        {
            return true;
        }
    }

    private static async Task<bool> TableExistsAsync(AppDbContext db, string table)
    {
        var conn = db.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT 1 FROM sqlite_master WHERE type = 'table' AND name = @n";
        var p = cmd.CreateParameter(); p.ParameterName = "@n"; p.Value = table;
        cmd.Parameters.Add(p);
        var r = await cmd.ExecuteScalarAsync();
        return r != null && r != System.DBNull.Value;
    }

    private static async Task<int> ReadSchemaVersionAsync(AppDbContext db)
    {
        var conn = db.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT [Version] FROM SchemaMeta ORDER BY Id DESC LIMIT 1";
        var r = await cmd.ExecuteScalarAsync();
        return r is int i ? i : (r is long l ? (int)l : 0);
    }

    private static async Task WriteSchemaVersionAsync(AppDbContext db)
    {
        var conn = db.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();

        using (var create = conn.CreateCommand())
        {
            create.CommandText = @"
CREATE TABLE IF NOT EXISTS SchemaMeta (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    [Version] INTEGER NOT NULL
);";
            await create.ExecuteNonQueryAsync();
        }

        using (var clear = conn.CreateCommand())
        {
            clear.CommandText = "DELETE FROM SchemaMeta;";
            await clear.ExecuteNonQueryAsync();
        }

        using (var insert = conn.CreateCommand())
        {
            insert.CommandText = "INSERT INTO SchemaMeta ([Version]) VALUES (@v);";
            var p = insert.CreateParameter(); p.ParameterName = "@v"; p.Value = SchemaVersion;
            insert.Parameters.Add(p);
            await insert.ExecuteNonQueryAsync();
        }
    }
}
