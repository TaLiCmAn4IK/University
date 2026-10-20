using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flux.Data;
using Flux.Infrastructure;
using Flux.Localization;
using Flux.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.ViewModels;

public partial class PlayerSearchResult : ObservableObject
{
    public int UserId { get; set; }
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private int _eloCS2;
    [ObservableProperty] private int _eloChess;
    [ObservableProperty] private bool _isBanned;
    [ObservableProperty] private bool _canAddFriend = true;
}

public partial class PlayerSearchViewModel : ViewModelBase
{
    private readonly IServiceProvider _sp;
    private readonly INavigationService _nav;
    private readonly IFriendsService _friendsService;
    private readonly ISessionService _session;
    private readonly IDialogService _dialog;

    [ObservableProperty] private string _query = string.Empty;
    [ObservableProperty] private bool _hasResults;
    [ObservableProperty] private bool _noResults;

    public ObservableCollection<PlayerSearchResult> Results { get; } = new();

    public ICommand SearchCommand { get; }
    public ICommand OpenProfileCommand { get; }
    public ICommand AddFriendCommand { get; }

    public PlayerSearchViewModel(IServiceProvider sp, INavigationService nav,
        IFriendsService friendsService, ISessionService session, IDialogService dialog)
    {
        _sp = sp; _nav = nav; _friendsService = friendsService; _session = session; _dialog = dialog;
        SearchCommand = new AsyncRelayCommand(SearchAsync);
        OpenProfileCommand = new AsyncRelayCommand<PlayerSearchResult>(async r =>
        {
            if (r == null) return;
            var vm = _sp.GetRequiredService<ProfileViewModel>();
            await vm.LoadAsync(r.UserId);
            _nav.NavigateTo(vm);
        });
        AddFriendCommand = new AsyncRelayCommand<PlayerSearchResult>(AddFriendAsync!);
    }

    private async Task SearchAsync()
    {
        Results.Clear();
        if (string.IsNullOrWhiteSpace(Query)) { HasResults = false; NoResults = false; return; }

        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var q = Query.Trim();
        var meId = _session.CurrentUser?.Id ?? -1;
        var list = await db.Users.AsNoTracking()
            .Where(u => u.Nickname.Contains(q) && u.Id != meId)
            .OrderBy(u => u.Nickname)
            .Take(50)
            .ToListAsync();
        foreach (var u in list)
        {
            Results.Add(new PlayerSearchResult
            {
                UserId = u.Id,
                Nickname = u.Nickname,
                EloCS2 = u.EloCS2,
                EloChess = u.EloChess,
                IsBanned = u.IsBanned
            });
        }
        HasResults = Results.Count > 0;
        NoResults = !HasResults;
    }

    private async Task AddFriendAsync(PlayerSearchResult r)
    {
        if (r == null || _session.CurrentUser == null) return;
        var result = await _friendsService.SendFriendRequestAsync(_session.CurrentUser.Id, r.UserId);
        string key = result switch
        {
            FriendRequestResult.Sent           => "FriendRequestSent",
            FriendRequestResult.AlreadyFriends => "AlreadyFriends",
            FriendRequestResult.AlreadyPending => "RequestAlreadyPending",
            FriendRequestResult.UserNotFound   => "UserNotFound",
            _ => "Error"
        };
        _dialog.ShowInfo(LanguageService.Instance[key]);
        if (result == FriendRequestResult.Sent) r.CanAddFriend = false;
    }
}
