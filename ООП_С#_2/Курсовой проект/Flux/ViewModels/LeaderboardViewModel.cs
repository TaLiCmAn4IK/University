using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flux.Data;
using Flux.Infrastructure;
using Flux.Localization;
using Flux.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Flux.ViewModels;

public partial class LeaderboardRow : ObservableObject
{
    public int Id { get; set; }
    [ObservableProperty] private int _rank;
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private int _elo;
    [ObservableProperty] private int _level;
    public bool IsTop3 => Rank <= 3;
}

public partial class LeaderboardViewModel : ViewModelBase
{
    private readonly IServiceProvider _sp;
    private readonly INavigationService _nav;

    public ObservableCollection<LeaderboardRow> Rows { get; } = new();

    [ObservableProperty] private GameType _selectedGame = GameType.CS2;
    [ObservableProperty] private bool _isLoading;

    public ICommand SelectCS2Command { get; }
    public ICommand SelectChessCommand { get; }
    public IAsyncRelayCommand<int?> OpenProfileCommand { get; }

    public LeaderboardViewModel(IServiceProvider sp, INavigationService nav)
    {
        _sp = sp; _nav = nav;
        SelectCS2Command   = new AsyncRelayCommand(() => ChangeGameAsync(GameType.CS2));
        SelectChessCommand = new AsyncRelayCommand(() => ChangeGameAsync(GameType.Chess));
        OpenProfileCommand = new AsyncRelayCommand<int?>(async uid =>
        {
            if (uid is null or 0) return;
            var vm = _sp.GetRequiredService<ProfileViewModel>();
            await vm.LoadAsync(uid.Value);
            _nav.NavigateTo(vm);
        });

        // Перезагружаем при смене языка (чтобы пустое состояние и т.п. обновлялись)
        LanguageService.Instance.LanguageChanged += async (_, _) => await LoadAsync();
    }

    partial void OnSelectedGameChanged(GameType value) => _ = LoadAsync();

    private Task ChangeGameAsync(GameType g)
    {
        if (SelectedGame != g)
            SelectedGame = g; // триггерит OnSelectedGameChanged → LoadAsync()
        return Task.CompletedTask;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var query = db.Users.AsNoTracking().Where(u => u.Role == UserRole.Player);
            // Сортируем по выбранной игре
            var ordered = SelectedGame == GameType.CS2
                ? query.OrderByDescending(u => u.EloCS2).Take(100)
                : query.OrderByDescending(u => u.EloChess).Take(100);

            var users = await ordered.ToListAsync();

            Rows.Clear();
            int rank = 1;
            foreach (var u in users)
            {
                int elo = SelectedGame == GameType.CS2 ? u.EloCS2 : u.EloChess;
                Rows.Add(new LeaderboardRow
                {
                    Id = u.Id,
                    Rank = rank++,
                    Nickname = u.Nickname,
                    Elo = elo,
                    Level = Services.EloLevels.GetInfo(elo).Level
                });
            }
        }
        finally { IsLoading = false; }
    }
}
