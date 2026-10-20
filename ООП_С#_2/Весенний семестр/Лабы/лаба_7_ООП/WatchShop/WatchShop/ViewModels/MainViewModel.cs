using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WatchShop.Commands;
using WatchShop.Models;
using WatchShop.Services;
using WatchShop.Views;

namespace WatchShop.ViewModels
{
    /// <summary>
    /// Главная модель представления. Все действия (фильтры, заказ, добавление,
    /// редактирование, удаление, логин, смена языка, режим отображения)
    /// реализованы как команды (паттерн Command).
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly DataService dataService = new();

        private ObservableCollection<Watch> allWatches = new();
        private ObservableCollection<Watch> filteredWatches = new();
        private Watch? selectedWatch;
        private User? currentUser;

        // фильтры
        private string searchText = "";
        private string? selectedCategoryKey; // ключ ResourceDictionary (Cat_Luxury и т.п.) или null = все
        private string minPriceText = "";
        private string maxPriceText = "";
        private bool showOnlyInStock;
        private int selectedSortIndex;

        // режим отображения
        private bool isListView = true;
        private string statusText = "";

        // лаб 6: сервис Undo/Redo
        public UndoRedoService UndoRedo { get; } = new();

        public MainViewModel()
        {
            allWatches = dataService.LoadWatches();
            filteredWatches = new ObservableCollection<Watch>(allWatches);

            // ---- команды ----
            ApplyFiltersCommand = new RelayCommand(_ => ApplyFilters());
            ResetFiltersCommand = new RelayCommand(_ => ResetFilters());
            AddWatchCommand = new RelayCommand(_ => AddWatch(), _ => IsAdmin);
            EditWatchCommand = new RelayCommand(p => EditWatch(p as Watch), p => IsAdmin && p is Watch);
            DeleteWatchCommand = new RelayCommand(p => DeleteWatch(p as Watch), p => IsAdmin && p is Watch);
            ShowDetailsCommand = new RelayCommand(p => ShowDetails(p as Watch), p => p is Watch);
            OrderCommand = new RelayCommand(p => OrderWatch(p as Watch), p => p is Watch);
            LoginCommand = new RelayCommand(_ => Login(), _ => CurrentUser == null);
            LogoutCommand = new RelayCommand(_ => Logout(), _ => CurrentUser != null);
            SwitchToListCommand = new RelayCommand(_ => IsListView = true);
            SwitchToTableCommand = new RelayCommand(_ => IsListView = false);
            ChangeLanguageCommand = new RelayCommand(p => ChangeLanguage(p as string));
            ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());
            ShowAboutCommand = new RelayCommand(_ => ShowAbout());

            // лаб 6
            UndoCommand = new RelayCommand(_ => DoUndo(), _ => UndoRedo.CanUndo);
            RedoCommand = new RelayCommand(_ => DoRedo(), _ => UndoRedo.CanRedo);
            ChangeThemeCommand = new RelayCommand(p => ChangeTheme(p as string));
            OpenProfileCommand = new RelayCommand(_ => OpenProfile(), _ => CurrentUser != null);

            UndoRedo.StateChanged += (_, _) =>
            {
                CommandManager.InvalidateRequerySuggested();
            };

            // подписка на смену языка — обновить локализованные свойства товаров и статусную строку
            LocalizationService.LanguageChanged += OnLanguageChanged;
            UpdateReadyStatus();
        }

        // ---------- наблюдаемые свойства ----------

        public ObservableCollection<Watch> FilteredWatches
        {
            get => filteredWatches;
            private set => SetProperty(ref filteredWatches, value);
        }

        public Watch? SelectedWatch
        {
            get => selectedWatch;
            set => SetProperty(ref selectedWatch, value);
        }

        public User? CurrentUser
        {
            get => currentUser;
            set
            {
                if (SetProperty(ref currentUser, value))
                {
                    OnPropertyChanged(nameof(IsAdmin));
                    OnPropertyChanged(nameof(IsAuthenticated));
                    OnPropertyChanged(nameof(UserStatusText));
                }
            }
        }

        public bool IsAdmin => CurrentUser?.IsAdmin ?? false;
        public bool IsAuthenticated => CurrentUser != null;

        public string UserStatusText =>
            CurrentUser != null
                ? $"{CurrentUser.Username} ({CurrentUser.Role})"
                : LocalizationService.GetString("NotAuthorized");

        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        /// <summary>
        /// Ключ выбранной категории (Cat_Luxury, Cat_Sport...) или null = все.
        /// </summary>
        public string? SelectedCategoryKey
        {
            get => selectedCategoryKey;
            set => SetProperty(ref selectedCategoryKey, value);
        }

        public string MinPriceText
        {
            get => minPriceText;
            set => SetProperty(ref minPriceText, value);
        }

        public string MaxPriceText
        {
            get => maxPriceText;
            set => SetProperty(ref maxPriceText, value);
        }

        public bool ShowOnlyInStock
        {
            get => showOnlyInStock;
            set => SetProperty(ref showOnlyInStock, value);
        }

        public int SelectedSortIndex
        {
            get => selectedSortIndex;
            set => SetProperty(ref selectedSortIndex, value);
        }

        public bool IsListView
        {
            get => isListView;
            set
            {
                if (SetProperty(ref isListView, value))
                    OnPropertyChanged(nameof(IsTableView));
            }
        }

        public bool IsTableView => !isListView;

        public string StatusText
        {
            get => statusText;
            set => SetProperty(ref statusText, value);
        }

        // ---------- команды ----------

        public ICommand ApplyFiltersCommand { get; }
        public ICommand ResetFiltersCommand { get; }
        public ICommand AddWatchCommand { get; }
        public ICommand EditWatchCommand { get; }
        public ICommand DeleteWatchCommand { get; }
        public ICommand ShowDetailsCommand { get; }
        public ICommand OrderCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand SwitchToListCommand { get; }
        public ICommand SwitchToTableCommand { get; }
        public ICommand ChangeLanguageCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ShowAboutCommand { get; }

        // лаб 6: новые команды
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand ChangeThemeCommand { get; }
        public ICommand OpenProfileCommand { get; }

        // ---------- реализации ----------

        private void ApplyFilters()
        {
            IEnumerable<Watch> q = allWatches;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.ToLowerInvariant();
                q = q.Where(w =>
                    (w.ShortName?.ToLowerInvariant().Contains(s) ?? false) ||
                    (w.FullName?.ToLowerInvariant().Contains(s) ?? false));
            }

            if (!string.IsNullOrEmpty(SelectedCategoryKey))
            {
                var canonical = WatchTranslator.CanonicalCategory(SelectedCategoryKey);
                q = q.Where(w => w.Category == canonical);
            }

            if (decimal.TryParse(MinPriceText, out var min))
                q = q.Where(w => w.Price >= min);
            if (decimal.TryParse(MaxPriceText, out var max))
                q = q.Where(w => w.Price <= max);

            if (ShowOnlyInStock)
                q = q.Where(w => w.InStock && w.Quantity > 0);

            q = SelectedSortIndex switch
            {
                1 => q.OrderBy(w => w.Price),
                2 => q.OrderByDescending(w => w.Price),
                3 => q.OrderByDescending(w => w.Rating),
                _ => q.OrderBy(w => w.ShortName),
            };

            FilteredWatches = new ObservableCollection<Watch>(q);
            StatusText = $"{LocalizationService.GetString("ProductsFound")} {FilteredWatches.Count}";
        }

        private void ResetFilters()
        {
            SearchText = "";
            SelectedCategoryKey = null;
            MinPriceText = "";
            MaxPriceText = "";
            ShowOnlyInStock = false;
            SelectedSortIndex = 0;
            ApplyFilters();
        }

        private void AddWatch()
        {
            if (!IsAdmin)
            {
                MessageBox.Show(LocalizationService.GetString("AdminOnly_Add"),
                    LocalizationService.GetString("Warning"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new AddEditWatchWindow { Owner = Application.Current.MainWindow };
            if (window.ShowDialog() == true && window.Watch != null)
            {
                var w = window.Watch;
                allWatches.Add(w);
                dataService.SaveWatches(allWatches);
                ApplyFilters();

                // регистрируем в UndoRedo
                UndoRedo.Push(new UndoableAction(
                    LocalizationService.Format("Action_Add", w.ShortName),
                    doAction: () =>
                    {
                        if (!allWatches.Contains(w)) allWatches.Add(w);
                        dataService.SaveWatches(allWatches);
                        ApplyFilters();
                    },
                    undoAction: () =>
                    {
                        if (allWatches.Contains(w)) allWatches.Remove(w);
                        dataService.SaveWatches(allWatches);
                        ApplyFilters();
                    }));

                StatusText = LocalizationService.GetString("Product_Added");
            }
        }

        private void EditWatch(Watch? watch)
        {
            if (watch == null || !IsAdmin) return;

            // снимок ДО редактирования — для отмены
            var before = WatchSnapshot.From(watch);

            var window = new AddEditWatchWindow(watch) { Owner = Application.Current.MainWindow };
            if (window.ShowDialog() == true)
            {
                var after = WatchSnapshot.From(watch);
                dataService.SaveWatches(allWatches);
                ApplyFilters();

                UndoRedo.Push(new UndoableAction(
                    LocalizationService.Format("Action_Edit", watch.ShortName),
                    doAction: () =>
                    {
                        after.ApplyTo(watch);
                        dataService.SaveWatches(allWatches);
                        ApplyFilters();
                    },
                    undoAction: () =>
                    {
                        before.ApplyTo(watch);
                        dataService.SaveWatches(allWatches);
                        ApplyFilters();
                    }));

                StatusText = LocalizationService.GetString("Product_Updated");
            }
        }

        private void DeleteWatch(Watch? watch)
        {
            if (watch == null || !IsAdmin) return;

            var result = MessageBox.Show(
                LocalizationService.Format("Confirm_Delete", watch.ShortName),
                LocalizationService.GetString("Confirm_Title"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            int oldIndex = allWatches.IndexOf(watch);
            allWatches.Remove(watch);
            dataService.SaveWatches(allWatches);
            ApplyFilters();

            UndoRedo.Push(new UndoableAction(
                LocalizationService.Format("Action_Delete", watch.ShortName),
                doAction: () =>
                {
                    if (allWatches.Contains(watch)) allWatches.Remove(watch);
                    dataService.SaveWatches(allWatches);
                    ApplyFilters();
                },
                undoAction: () =>
                {
                    if (!allWatches.Contains(watch))
                    {
                        if (oldIndex >= 0 && oldIndex <= allWatches.Count)
                            allWatches.Insert(oldIndex, watch);
                        else
                            allWatches.Add(watch);
                    }
                    dataService.SaveWatches(allWatches);
                    ApplyFilters();
                }));

            StatusText = LocalizationService.GetString("Product_Deleted");
        }

        private void DoUndo()
        {
            UndoRedo.Undo();
            StatusText = LocalizationService.GetString("Undo");
        }

        private void DoRedo()
        {
            UndoRedo.Redo();
            StatusText = LocalizationService.GetString("Redo");
        }

        private void ChangeTheme(string? themeName)
        {
            if (string.IsNullOrEmpty(themeName)) return;
            ThemeService.ChangeTheme(themeName);
        }

        private void OpenProfile()
        {
            if (CurrentUser == null) return;
            var window = new UserProfileWindow(CurrentUser)
            {
                Owner = Application.Current.MainWindow
            };
            if (window.ShowDialog() == true)
            {
                OnPropertyChanged(nameof(UserStatusText));
                StatusText = LocalizationService.GetString("Profile");
            }
        }

        private void ShowDetails(Watch? watch)
        {
            if (watch == null) return;
            var window = new WatchDetailsWindow(watch) { Owner = Application.Current.MainWindow };
            window.ShowDialog();
        }

        private void OrderWatch(Watch? watch)
        {
            if (watch == null) return;

            if (CurrentUser == null)
            {
                MessageBox.Show(
                    LocalizationService.GetString("LoginRequired"),
                    LocalizationService.GetString("Warning"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Login();
                return;
            }

            if (!watch.InStock || watch.Quantity <= 0)
            {
                MessageBox.Show(
                    LocalizationService.Format("Order_OutOfStock", watch.ShortName),
                    LocalizationService.GetString("Warning"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var quantityWindow = new QuantityInputWindow(
                LocalizationService.Format("Order_Quantity", watch.Quantity))
            {
                Owner = Application.Current.MainWindow,
                Title = LocalizationService.GetString("Order")
            };

            if (quantityWindow.ShowDialog() != true) return;
            int qty = quantityWindow.EnteredQuantity;

            if (qty > watch.Quantity)
            {
                MessageBox.Show(
                    LocalizationService.Format("Order_TooMany", watch.Quantity),
                    LocalizationService.GetString("Warning"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal total = watch.DiscountedPrice * qty;
            MessageBox.Show(
                LocalizationService.Format("Order_Details",
                    watch.ShortName, watch.DiscountedPrice.ToString("F2"), qty, total.ToString("F2")),
                LocalizationService.GetString("Order_Success"),
                MessageBoxButton.OK, MessageBoxImage.Information);

            watch.Quantity -= qty;
            watch.SoldCount += qty;
            if (watch.Quantity == 0) watch.InStock = false;

            dataService.SaveWatches(allWatches);
            StatusText = LocalizationService.GetString("Order_Placed");
        }

        private void Login()
        {
            var loginWindow = new LoginWindow { Owner = Application.Current.MainWindow };
            if (loginWindow.ShowDialog() == true)
            {
                CurrentUser = loginWindow.AuthenticatedUser;
                StatusText = $"{LocalizationService.GetString("Welcome")}, {CurrentUser?.Username}!";
            }
        }

        private void Logout()
        {
            CurrentUser = null;
            UpdateReadyStatus();
        }

        private void ChangeLanguage(string? lang)
        {
            if (string.IsNullOrEmpty(lang)) return;
            LocalizationService.ChangeLanguage(lang);
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                LocalizationService.GetString("AboutText"),
                LocalizationService.GetString("AboutTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            // обновляем локализованные свойства в каждом товаре
            foreach (var w in allWatches) w.RefreshLocalization();

            // и UI-свойства самой VM, зависящие от LocalizationService.GetString
            OnPropertyChanged(nameof(UserStatusText));
            UpdateReadyStatus();
        }

        private void UpdateReadyStatus()
        {
            if (CurrentUser == null)
                StatusText = LocalizationService.GetString("Ready");
        }
    }
}
