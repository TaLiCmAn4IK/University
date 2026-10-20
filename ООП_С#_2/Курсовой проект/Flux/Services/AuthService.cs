using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Flux.Data;
using Flux.Localization;
using Flux.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Services;

public enum ValidationErrorKey
{
    None,
    NicknameRequired,
    NicknameLength,
    NicknameChars,
    NicknameTaken,
    EmailInvalid,
    EmailTaken,
    PasswordLength,
    PasswordComplexity,
    PasswordsMismatch
}

public record AuthResult(bool Success, ValidationErrorKey Error = ValidationErrorKey.None, User? User = null);

public interface IAuthService
{
    Task<User?> LoginAsync(string nickname, string password);

    Task<AuthResult> RegisterAsync(string nickname, string email, string password, string passwordConfirm);

    ValidationErrorKey ValidateNickname(string nickname);
    ValidationErrorKey ValidateEmail(string email);
    ValidationErrorKey ValidatePassword(string password);
}

public class AuthService : IAuthService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuthService> _logger;

    private static readonly Regex NicknameRegex = new(@"^[A-Za-z0-9_]+$", RegexOptions.Compiled);
    private static readonly Regex EmailRegex = new(
        @"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$", RegexOptions.Compiled);

    public AuthService(IServiceScopeFactory scopeFactory, ILogger<AuthService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public ValidationErrorKey ValidateNickname(string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname)) return ValidationErrorKey.NicknameRequired;
        if (nickname.Length < 3 || nickname.Length > 20) return ValidationErrorKey.NicknameLength;
        if (!NicknameRegex.IsMatch(nickname)) return ValidationErrorKey.NicknameChars;
        return ValidationErrorKey.None;
    }

    public ValidationErrorKey ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return ValidationErrorKey.EmailInvalid;
        if (!EmailRegex.IsMatch(email)) return ValidationErrorKey.EmailInvalid;
        return ValidationErrorKey.None;
    }

    public ValidationErrorKey ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 6) return ValidationErrorKey.PasswordLength;
        bool hasLetter = false, hasDigit = false;
        foreach (var c in password)
        {
            if (char.IsLetter(c)) hasLetter = true;
            else if (char.IsDigit(c)) hasDigit = true;
        }
        if (!hasLetter || !hasDigit) return ValidationErrorKey.PasswordComplexity;
        return ValidationErrorKey.None;
    }

    public async Task<User?> LoginAsync(string nickname, string password)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Nickname == nickname);
        if (user == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;
        _logger.LogInformation("User {Nickname} logged in", nickname);
        return user;
    }

    public async Task<AuthResult> RegisterAsync(string nickname, string email, string password, string passwordConfirm)
    {
        var err = ValidateNickname(nickname);
        if (err != ValidationErrorKey.None) return new AuthResult(false, err);
        err = ValidateEmail(email);
        if (err != ValidationErrorKey.None) return new AuthResult(false, err);
        err = ValidatePassword(password);
        if (err != ValidationErrorKey.None) return new AuthResult(false, err);
        if (password != passwordConfirm) return new AuthResult(false, ValidationErrorKey.PasswordsMismatch);

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await db.Users.AnyAsync(u => u.Nickname == nickname))
            return new AuthResult(false, ValidationErrorKey.NicknameTaken);
        if (await db.Users.AnyAsync(u => u.Email == email))
            return new AuthResult(false, ValidationErrorKey.EmailTaken);

        var user = new User
        {
            Login = nickname,       
            Email = email,
            Nickname = nickname,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRole.Player,
            EloCS2 = 1000,
            EloChess = 1000
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        _logger.LogInformation("New user registered: {Nickname}", nickname);
        return new AuthResult(true, ValidationErrorKey.None, user);
    }
}
