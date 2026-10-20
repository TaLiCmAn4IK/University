using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flux.Data;
using Flux.Infrastructure;
using Flux.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.ViewModels;

public partial class AdminUserRow : ObservableObject
{
    public int Id { get; set; }
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private string _login = string.Empty;
    [ObservableProperty] private int _eloCS2;
    [ObservableProperty] private int _eloChess;
    [ObservableProperty] private UserRole _role;
    [ObservableProperty] private bool _isBanned;

    public string RoleLabel => Role == UserRole.Admin
        ? Localization.LanguageService.Instance["RoleAdmin"]
        : Localization.LanguageService.Instance["RolePlayer"];

    public string StatusLabel => IsBanned
        ? Localization.LanguageService.Instance["Banned"]
        : Localization.LanguageService.Instance["Active"];
}

public partial class AdminMatchRow : ObservableObject
{
    public int Id { get; set; }
    [ObservableProperty] private DateTime _date;
    [ObservableProperty] private GameType _game;
    [ObservableProperty] private string _summary = string.Empty;

    public string GameLabel => Game == GameType.CS2
        ? Localization.LanguageService.Instance["CS2Stats"]
        : Localization.LanguageService.Instance["ChessStats"];
}

public partial class AdminViewModel : ViewModelBase
{
    private readonly IServiceProvider _sp;
    private readonly INavigationService _nav;

    public ObservableCollection<AdminUserRow> Users { get; } = new();
    public ObservableCollection<AdminMatchRow> Matches { get; } = new();

    public ICollectionView UsersView { get; }

    [ObservableProperty] private string _userFilter = string.Empty;

    partial void OnUserFilterChanged(string value) => UsersView.Refresh();

    public ICommand ToggleBanCommand { get; }
    public ICommand OpenMatchCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SetFilterCommand { get; }
    public ICommand ClearFilterCommand { get; }

    public AdminViewModel(IServiceProvider sp, INavigationService nav)
    {
        _sp = sp; _nav = nav;

        UsersView = CollectionViewSource.GetDefaultView(Users);
        UsersView.Filter = obj =>
        {
            if (string.IsNullOrWhiteSpace(UserFilter)) return true;
            if (obj is not AdminUserRow r) return true;
            var f = UserFilter.Trim();
            return r.Nickname.Contains(f, StringComparison.OrdinalIgnoreCase)
                || r.EloCS2.ToString().Equals(f, StringComparison.OrdinalIgnoreCase)
                || r.EloChess.ToString().Equals(f, StringComparison.OrdinalIgnoreCase)
                || r.RoleLabel.Contains(f, StringComparison.OrdinalIgnoreCase)
                || r.StatusLabel.Contains(f, StringComparison.OrdinalIgnoreCase);
        };

        ToggleBanCommand = new AsyncRelayCommand<AdminUserRow>(ToggleBanAsync!);
        OpenMatchCommand = new AsyncRelayCommand<AdminMatchRow>(async row =>
        {
            if (row == null) return;
            var vm = _sp.GetRequiredService<MatchDetailsViewModel>();
            await vm.LoadAsync(row.Id);
            _nav.NavigateTo(vm);
        });
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        SetFilterCommand   = new RelayCommand<string?>(v => UserFilter = v ?? "");
        ClearFilterCommand = new RelayCommand(() => UserFilter = "");

        Localization.LanguageService.Instance.LanguageChanged += async (_, _) => await LoadAsync();
    }

    public async Task LoadAsync()
    {
        Users.Clear();
        Matches.Clear();

        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        foreach (var u in await db.Users.AsNoTracking().OrderBy(u => u.Nickname).ToListAsync())
        {
            Users.Add(new AdminUserRow
            {
                Id = u.Id,
                Nickname = u.Nickname,
                Login = u.Login,
                EloCS2 = u.EloCS2,
                EloChess = u.EloChess,
                Role = u.Role,
                IsBanned = u.IsBanned
            });
        }

        var matches = await db.Matches.AsNoTracking()
            .OrderByDescending(m => m.MatchDate)
            .Take(100)
            .ToListAsync();
        foreach (var m in matches)
        {
            Matches.Add(new AdminMatchRow
            {
                Id = m.Id,
                Date = m.MatchDate.ToLocalTime(),
                Game = m.GameType,
                Summary = m.GameType == GameType.CS2
                    ? Localization.LanguageService.Instance["CS2Match"]
                    : Localization.LanguageService.Instance["ChessGame"]
            });
        }
    }

    private async Task ToggleBanAsync(AdminUserRow row)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var u = await db.Users.FirstOrDefaultAsync(x => x.Id == row.Id);
        if (u == null) return;
        u.IsBanned = !u.IsBanned;
        await db.SaveChangesAsync();
        row.IsBanned = u.IsBanned;
    }
}
