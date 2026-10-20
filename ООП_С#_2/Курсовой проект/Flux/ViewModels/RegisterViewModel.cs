using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flux.Infrastructure;
using Flux.Localization;
using Flux.Services;

namespace Flux.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly IAuthService _auth;
    private readonly ISessionService _session;
    private readonly ISessionPersistence _persistence;
    private readonly INavigationService _nav;
    private readonly IFriendsService _friendsService;

    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _passwordConfirm = string.Empty;

    // Per-field validation messages
    [ObservableProperty] private string? _nicknameError;
    [ObservableProperty] private string? _emailError;
    [ObservableProperty] private string? _passwordError;
    [ObservableProperty] private string? _confirmError;
    [ObservableProperty] private string? _generalError;
    [ObservableProperty] private bool _isBusy;

    public ICommand RegisterCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand ToggleLanguageCommand { get; }

    public RegisterViewModel(IAuthService auth, ISessionService session, ISessionPersistence persistence,
        INavigationService nav, IFriendsService friendsService)
    {
        _auth = auth; _session = session; _persistence = persistence; _nav = nav;
        _friendsService = friendsService;
        RegisterCommand = new AsyncRelayCommand(RegisterAsync);
        BackCommand = new RelayCommand(() => _nav.NavigateRoot<LoginViewModel>());
        ToggleLanguageCommand = new RelayCommand(() => LanguageService.Instance.ToggleLanguage());
    }

    // Live validation: re-run the relevant rule whenever a field changes
    partial void OnNicknameChanged(string value)
        => NicknameError = value.Length == 0 ? null : MessageForKey(_auth.ValidateNickname(value));

    partial void OnEmailChanged(string value)
        => EmailError = value.Length == 0 ? null : MessageForKey(_auth.ValidateEmail(value));

    partial void OnPasswordChanged(string value)
    {
        PasswordError = value.Length == 0 ? null : MessageForKey(_auth.ValidatePassword(value));
        // Re-check confirm as well when password changes
        if (!string.IsNullOrEmpty(PasswordConfirm))
            ConfirmError = PasswordConfirm == value ? null : LanguageService.Instance["PasswordsMismatch"];
    }

    partial void OnPasswordConfirmChanged(string value)
        => ConfirmError = (value.Length == 0 || value == Password) ? null : LanguageService.Instance["PasswordsMismatch"];

    private static string? MessageForKey(ValidationErrorKey key) => key switch
    {
        ValidationErrorKey.None               => null,
        ValidationErrorKey.NicknameRequired   => LanguageService.Instance["ValNicknameRequired"],
        ValidationErrorKey.NicknameLength     => LanguageService.Instance["ValNicknameLength"],
        ValidationErrorKey.NicknameChars      => LanguageService.Instance["ValNicknameChars"],
        ValidationErrorKey.NicknameTaken      => LanguageService.Instance["ValNicknameTaken"],
        ValidationErrorKey.EmailInvalid       => LanguageService.Instance["ValEmailInvalid"],
        ValidationErrorKey.EmailTaken         => LanguageService.Instance["ValEmailTaken"],
        ValidationErrorKey.PasswordLength     => LanguageService.Instance["ValPasswordLength"],
        ValidationErrorKey.PasswordComplexity => LanguageService.Instance["ValPasswordComplexity"],
        ValidationErrorKey.PasswordsMismatch  => LanguageService.Instance["PasswordsMismatch"],
        _ => LanguageService.Instance["RegistrationError"]
    };

    private async Task RegisterAsync()
    {
        GeneralError = null;
        // Run full validation to populate each field
        NicknameError = MessageForKey(_auth.ValidateNickname(Nickname));
        EmailError    = MessageForKey(_auth.ValidateEmail(Email));
        PasswordError = MessageForKey(_auth.ValidatePassword(Password));
        ConfirmError  = Password == PasswordConfirm ? null : LanguageService.Instance["PasswordsMismatch"];
        if (NicknameError != null || EmailError != null || PasswordError != null || ConfirmError != null)
            return;

        IsBusy = true;
        try
        {
            var result = await _auth.RegisterAsync(Nickname.Trim(), Email.Trim(), Password, PasswordConfirm);
            if (!result.Success || result.User == null)
            {
                // Map server-side error to the right field
                switch (result.Error)
                {
                    case ValidationErrorKey.NicknameTaken:
                        NicknameError = MessageForKey(result.Error); break;
                    case ValidationErrorKey.EmailTaken:
                        EmailError = MessageForKey(result.Error); break;
                    default:
                        GeneralError = MessageForKey(result.Error); break;
                }
                return;
            }

            // Auto-login
            _session.CurrentUser = result.User;
            _persistence.Save(result.User.Nickname);
            _friendsService.Online(result.User.Id);
            _nav.NavigateRoot<MainViewModel>();
        }
        finally { IsBusy = false; }
    }
}
