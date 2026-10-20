using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flux.Infrastructure;
using Flux.Services;

namespace Flux.ViewModels;

public partial class FriendListItem : ObservableObject
{
    public int UserId { get; set; }
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private int _eloCS2;
    [ObservableProperty] private int _eloChess;
    [ObservableProperty] private bool _isOnline;
}

public partial class IncomingRequestItem : ObservableObject
{
    public int RequestId { get; set; }
    public int FromUserId { get; set; }
    [ObservableProperty] private string _fromNickname = string.Empty;
    [ObservableProperty] private DateTime _receivedAt;
}

public partial class FriendsViewModel : ViewModelBase
{
    private readonly IFriendsService _friends;
    private readonly ISessionService _session;
    private readonly IServiceProvider _sp;
    private readonly INavigationService _nav;

    public ObservableCollection<FriendListItem> FriendsList { get; } = new();
    public ObservableCollection<IncomingRequestItem> Requests { get; } = new();

    [ObservableProperty] private bool _hasRequests;
    [ObservableProperty] private bool _hasFriends;

    public ICommand AcceptRequestCommand { get; }
    public ICommand DeclineRequestCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand OpenProfileCommand { get; }
    public ICommand RemoveFriendCommand { get; }

    public FriendsViewModel(IFriendsService friends, ISessionService session, IServiceProvider sp, INavigationService nav)
    {
        _friends = friends; _session = session; _sp = sp; _nav = nav;
        AcceptRequestCommand = new AsyncRelayCommand<IncomingRequestItem>(AcceptAsync!);
        DeclineRequestCommand = new AsyncRelayCommand<IncomingRequestItem>(DeclineAsync!);
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        OpenProfileCommand = new AsyncRelayCommand<int?>(async uid =>
        {
            if (uid is null or 0) return;
            var vm = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
                .GetRequiredService<ProfileViewModel>(_sp);
            await vm.LoadAsync(uid.Value);
            _nav.NavigateTo(vm);
        });
        RemoveFriendCommand = new AsyncRelayCommand<FriendListItem?>(async item =>
        {
            if (item == null || _session.CurrentUser == null) return;
            await _friends.RemoveFriendAsync(_session.CurrentUser.Id, item.UserId);
            await LoadAsync();
        });
    }

    public async Task LoadAsync()
    {
        var me = _session.CurrentUser!;

        FriendsList.Clear();
        var list = await _friends.GetFriendsAsync(me.Id);
        foreach (var u in list)
        {
            FriendsList.Add(new FriendListItem
            {
                UserId = u.Id,
                Nickname = u.Nickname,
                EloCS2 = u.EloCS2,
                EloChess = u.EloChess,
                IsOnline = _friends.IsOnline(u.Id)
            });
        }
        HasFriends = FriendsList.Count > 0;

        Requests.Clear();
        var reqs = await _friends.GetIncomingRequestsAsync(me.Id);
        foreach (var r in reqs)
        {
            Requests.Add(new IncomingRequestItem
            {
                RequestId = r.Id,
                FromUserId = r.FromUserId,
                FromNickname = r.FromUser.Nickname,
                ReceivedAt = r.CreatedAt.ToLocalTime()
            });
        }
        HasRequests = Requests.Count > 0;
    }

    private async Task AcceptAsync(IncomingRequestItem req)
    {
        await _friends.AcceptRequestAsync(req.RequestId);
        await LoadAsync();
    }

    private async Task DeclineAsync(IncomingRequestItem req)
    {
        await _friends.DeclineRequestAsync(req.RequestId);
        await LoadAsync();
    }
}
