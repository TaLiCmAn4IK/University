using System.Text.Json;
using ASPA008_1.Config;
using Microsoft.Extensions.Options;

namespace ASPA008_1.Services
{
    /// <summary>
    /// Сервис для работы со справочником стран (ISO 3166-1 alpha-2).
    /// Загружает JSON-файл, путь к которому задаётся в Celebrities.config.json.
    /// </summary>
    public class CountryCodesService : ICountryCodesService
    {
        private readonly List<CountryCode> _items;
        private readonly Dictionary<string, string> _byCode;

        public CountryCodesService(IOptions<CelebritiesConfig> options, IWebHostEnvironment env)
        {
            var rel = options.Value.CountryCodesFile;
            if (string.IsNullOrWhiteSpace(rel))
                rel = Path.Combine("ContryCodes", "iso3166-1-alpha2-country-codes.json");

            var path = Path.IsPathRooted(rel)
                ? rel
                : Path.Combine(env.ContentRootPath, rel);

            if (!File.Exists(path))
            {
                _items = new List<CountryCode>();
                _byCode = new Dictionary<string, string>();
                return;
            }

            var json = File.ReadAllText(path);
            _items = JsonSerializer.Deserialize<List<CountryCode>>(json,
                         new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                     ?? new List<CountryCode>();

            _items = _items.OrderBy(x => x.countryLabel).ToList();
            _byCode = _items.ToDictionary(x => x.code, x => x.countryLabel,
                                          StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyList<CountryCode> GetAll() => _items;

        public string? GetLabel(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            return _byCode.TryGetValue(code, out var label) ? label : null;
        }
    }
}
