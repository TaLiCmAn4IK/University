using Flux.Models;

namespace Flux.Infrastructure;

public interface ISessionService
{
    User? CurrentUser { get; set; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    void Clear();
}

public class SessionService : ISessionService
{
    public User? CurrentUser { get; set; }
    public bool IsAuthenticated => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
    public void Clear() => CurrentUser = null;
}
