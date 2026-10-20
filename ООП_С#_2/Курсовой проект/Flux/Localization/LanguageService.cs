using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Data;

namespace Flux.Localization;

public class LanguageService : INotifyPropertyChanged
{
    private static readonly Lazy<LanguageService> _instance = new(() => new LanguageService());
    public static LanguageService Instance => _instance.Value;

    private readonly ResourceManager _rm;
    private CultureInfo _current;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? LanguageChanged;

    private LanguageService()
    {
        _rm = new ResourceManager("Flux.Localization.Strings", typeof(Strings).Assembly);
        _current = CultureInfo.GetCultureInfo("en");
        Thread.CurrentThread.CurrentUICulture = _current;
    }

    public CultureInfo CurrentCulture => _current;

    public string this[string key]
    {
        get => _rm.GetString(key, _current) ?? $"[{key}]";
    }

    public void SetLanguage(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        if (_current.Name == culture.Name) return;
        _current = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Binding.IndexerName));
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ToggleLanguage()
        => SetLanguage(_current.Name.StartsWith("ru") ? "en" : "ru");
}
