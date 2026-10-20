using System;
using System.IO;
using System.Text.Json;

namespace Flux.Infrastructure;

public interface ISessionPersistence
{
    RememberedUser? Load();
    void Save(string nickname);
    void Clear();
}

public class RememberedUser
{
    public string Nickname { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
}

public class JsonSessionPersistence : ISessionPersistence
{
    private readonly string _path;

    public JsonSessionPersistence()
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Flux");
        Directory.CreateDirectory(dir);
        _path = Path.Combine(dir, "session.json");
    }

    public RememberedUser? Load()
    {
        try
        {
            if (!File.Exists(_path)) return null;
            return JsonSerializer.Deserialize<RememberedUser>(File.ReadAllText(_path));
        }
        catch { return null; }
    }

    public void Save(string nickname)
    {
        try
        {
            File.WriteAllText(_path, JsonSerializer.Serialize(new RememberedUser
            {
                Nickname = nickname,
                SavedAt = DateTime.UtcNow
            }));
        }
        catch { /* non-fatal */ }
    }

    public void Clear()
    {
        try { if (File.Exists(_path)) File.Delete(_path); } catch { }
    }
}
