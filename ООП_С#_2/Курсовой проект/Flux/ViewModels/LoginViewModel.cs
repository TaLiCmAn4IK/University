using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flux.Infrastructure;
using Flux.Localization;
using Flux.Services;

namespace Flux.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _auth;
    private readonly ISessionService _session;
    private readonly ISessionPersistence _persistence;
    private readonly INavigationService _nav;
    private readonly IFriendsService _friendsService;

    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isBusy;

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }
    public ICommand ToggleLanguageCommand { get; }

    public LoginViewModel(IAuthService auth, ISessionService session, ISessionPersistence persistence,
        INavigationService nav, IFriendsService friendsService)
    {
        _auth = auth; _session = session; _persistence = persistence; _nav = nav;
        _friendsService = friendsService;

        LoginCommand = new AsyncRelayCommand(LoginAsync);
        GoToRegisterCommand = new RelayCommand(() => _nav.NavigateTo<RegisterViewModel>());
        ToggleLanguageCommand = new RelayCommand(() => LanguageService.Instance.ToggleLanguage());
    }

    private async Task LoginAsync()
    {
        ErrorMessage = null;
        if (string.IsNullOrWhiteSpace(Nickname) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = LanguageService.Instance["FieldsRequired"];
            return;
        }
        IsBusy = true;
        try
        {
            var user = await _auth.LoginAsync(Nickname.Trim(), Password);
            if (user == null)
            {
                ErrorMessage = LanguageService.Instance["LoginError"];
                return;
            }
            // Заблокированный аккаунт не пускаем в приложение вовсе.
            if (user.IsBanned)
            {
                ErrorMessage = LanguageService.Instance["UserBanned"];
                return;
            }
            _session.CurrentUser = user;
            _persistence.Save(user.Nickname);
            _friendsService.Online(user.Id);
            _nav.NavigateRoot<MainViewModel>();
        }
        finally { IsBusy = false; }
    }
}
