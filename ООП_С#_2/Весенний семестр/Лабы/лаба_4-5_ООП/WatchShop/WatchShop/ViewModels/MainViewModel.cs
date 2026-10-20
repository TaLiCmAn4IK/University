using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WatchShop.Commands;
using WatchShop.Models;
using WatchShop.Services;

namespace WatchShop.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private DataService dataService;
        private ObservableCollection<Watch> watches;
        private ObservableCollection<Watch> filteredWatches;
        private User? currentUser;
        private string? searchText;
        private string? selectedCategory;
        private decimal minPrice;
        private decimal maxPrice;
        private bool showOnlyInStock;
        private int selectedSortIndex;
        private Watch? selectedWatch;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel()
        {
            dataService = new DataService();
            watches = dataService.LoadWatches();
            filteredWatches = new ObservableCollection<Watch>(watches);

            // исправлено: добавлены проверки на null для параметров
            ApplyFiltersCommand = new RelayCommand(_ => ApplyFilters());
            AddWatchCommand = new RelayCommand(_ => AddWatch(), _ => IsAdmin);
            EditWatchCommand = new RelayCommand(p => EditWatch(p), _ => IsAdmin && SelectedWatch != null);
            DeleteWatchCommand = new RelayCommand(p => DeleteWatch(p), _ => IsAdmin && SelectedWatch != null);
            LoginCommand = new RelayCommand(_ => Login());
            LogoutCommand = new RelayCommand(_ => Logout());
        }

        public ObservableCollection<Watch> Watches
        {
            get => watches;
            set { watches = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Watch> FilteredWatches
        {
            get => filteredWatches;
            set { filteredWatches = value; OnPropertyChanged(); }
        }

        public Watch? SelectedWatch
        {
            get => selectedWatch;
            set { selectedWatch = value; OnPropertyChanged(); }
        }

        public string? SearchText
        {
            get => searchText;
            set { searchText = value; OnPropertyChanged(); }
        }

        public string? SelectedCategory
        {
            get => selectedCategory;
            set { selectedCategory = value; OnPropertyChanged(); }
        }

        public decimal MinPrice
        {
            get => minPrice;
            set { minPrice = value; OnPropertyChanged(); }
        }

        public decimal MaxPrice
        {
            get => maxPrice;
            set { maxPrice = value; OnPropertyChanged(); }
        }

        public bool ShowOnlyInStock
        {
            get => showOnlyInStock;
            set { showOnlyInStock = value; OnPropertyChanged(); }
        }

        public int SelectedSortIndex
        {
            get => selectedSortIndex;
            set { selectedSortIndex = value; OnPropertyChanged(); }
        }

        public User? CurrentUser
        {
            get => currentUser;
            set
            {
                currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAdmin));
                OnPropertyChanged(nameof(UserStatusText));
            }
        }

        public bool IsAdmin => CurrentUser?.Role == "Admin";
        public string UserStatusText => CurrentUser != null ? $"{CurrentUser.Username} ({CurrentUser.Role})" : "Не авторизован";

        public ICommand ApplyFiltersCommand { get; }
        public ICommand AddWatchCommand { get; }
        public ICommand EditWatchCommand { get; }
        public ICommand DeleteWatchCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }

        private void ApplyFilters() { }
        private void AddWatch() { }
        private void EditWatch(object? parameter) { }
        private void DeleteWatch(object? parameter) { }
        private void Login() { }
        private void Logout() { CurrentUser = null; }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}