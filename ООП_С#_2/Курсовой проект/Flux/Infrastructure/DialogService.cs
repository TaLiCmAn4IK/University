using System.Windows;
using Flux.Localization;

namespace Flux.Infrastructure;

public interface IDialogService
{
    void ShowInfo(string message, string? title = null);
    void ShowError(string message, string? title = null);
    bool Confirm(string message, string? title = null);
}

public class DialogService : IDialogService
{
    public void ShowInfo(string message, string? title = null)
        => MessageBox.Show(message, title ?? LanguageService.Instance["Success"],
            MessageBoxButton.OK, MessageBoxImage.Information);

    public void ShowError(string message, string? title = null)
        => MessageBox.Show(message, title ?? LanguageService.Instance["Error"],
            MessageBoxButton.OK, MessageBoxImage.Error);

    public bool Confirm(string message, string? title = null)
        => MessageBox.Show(message, title ?? LanguageService.Instance["Confirm"],
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
}
