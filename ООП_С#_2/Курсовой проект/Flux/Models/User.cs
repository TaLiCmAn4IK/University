using System;
using System.Collections.Generic;

namespace Flux.Models;

public enum UserRole
{
    Player = 0,
    Admin = 1
}

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public int EloCS2 { get; set; } = 1000;
    public int EloChess { get; set; } = 1000;
    public UserRole Role { get; set; } = UserRole.Player;
    public bool IsBanned { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Абсолютный или относительный путь к файлу аватара (legacy, не используется).
    /// Новые аватары хранятся в <see cref="AvatarBytes"/>.</summary>
    public string? AvatarPath { get; set; }

    /// <summary>Картинка аватара пользователя как BLOB в базе данных.
    /// PNG/JPEG bytes; null — нет аватара (рендерится initial-плашка).</summary>
    public byte[]? AvatarBytes { get; set; }

    public ICollection<MatchPlayer> MatchPlayers { get; set; } = new List<MatchPlayer>();
    public ICollection<QueueEntry> QueueEntries { get; set; } = new List<QueueEntry>();
}
