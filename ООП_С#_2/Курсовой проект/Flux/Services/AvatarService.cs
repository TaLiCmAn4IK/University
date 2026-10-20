using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Flux.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Services;

public interface IAvatarService
{
    /// <summary>Сохраняет файл как аватар пользователя в БД (BLOB). Возвращает true при успехе.</summary>
    Task<bool> SaveAvatarAsync(int userId, string sourceFilePath);

    Task DeleteAvatarAsync(int userId);
}

public class AvatarService : IAvatarService
{
    private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg" };
    private const int MaxBytes = 4 * 1024 * 1024;

    private readonly IServiceProvider _sp;
    public AvatarService(IServiceProvider sp) { _sp = sp; }

    public async Task<bool> SaveAvatarAsync(int userId, string sourceFilePath)
    {
        if (string.IsNullOrWhiteSpace(sourceFilePath) || !File.Exists(sourceFilePath))
            return false;

        var ext = Path.GetExtension(sourceFilePath).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext)) return false;

        var info = new FileInfo(sourceFilePath);
        if (info.Length > MaxBytes) return false;

        byte[] bytes = await File.ReadAllBytesAsync(sourceFilePath);

        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;
        user.AvatarBytes = bytes;
        user.AvatarPath = null;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task DeleteAvatarAsync(int userId)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return;
        user.AvatarBytes = null;
        user.AvatarPath = null;
        await db.SaveChangesAsync();
    }
}
