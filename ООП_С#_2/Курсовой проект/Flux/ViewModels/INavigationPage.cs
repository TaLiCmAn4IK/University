namespace Flux.ViewModels;

/// <summary>
/// Вкладка главного окна, которой нужны колбэки при уходе/входе
/// (таймеры, подписки и т.п.).
/// </summary>
public interface INavigationPage
{
    void OnNavigatedFrom();
    void OnNavigatedTo();
}
