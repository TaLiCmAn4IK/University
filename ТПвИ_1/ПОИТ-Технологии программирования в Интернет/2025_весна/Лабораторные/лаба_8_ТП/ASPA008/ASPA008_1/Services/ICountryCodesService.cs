namespace ASPA008_1.Services
{
    public interface ICountryCodesService
    {
        IReadOnlyList<CountryCode> GetAll();
        string? GetLabel(string code);
    }

    public class CountryCode
    {
        public string countryLabel { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
    }
}
