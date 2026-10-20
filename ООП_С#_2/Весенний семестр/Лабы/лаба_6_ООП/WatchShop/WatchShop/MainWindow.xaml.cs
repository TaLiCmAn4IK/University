using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WatchShop.Models;
using WatchShop.Services;
using WatchShop.Views;
using System.Linq;
namespace WatchShop
{
    public partial class MainWindow : Window
    {
        private DataService? dataService;
        private ObservableCollection<Watch>? watches;
        private ObservableCollection<WatchDisplay>? watchDisplays;
        private User? currentUser;
        private ContextMenu? adminContextMenu;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            InitializeApp();
            LocalizationService.LanguageChanged += OnLanguageChanged;
        }

        public bool IsAdmin => currentUser?.Role == "Admin";

        private void InitializeApp()
        {
            try
            {
                dataService = new DataService();
                watches = dataService.LoadWatches();

                if (watches != null)
                {
                    watchDisplays = new ObservableCollection<WatchDisplay>();
                    foreach (var watch in watches)
                    {
                        watchDisplays.Add(new WatchDisplay(watch));
                    }
                }

                LoadCustomCursor();
                SetupCommands();

                if (watchDisplays != null)
                {
                    WatchesListView.ItemsSource = watchDisplays;
                    WatchesDataGrid.ItemsSource = watchDisplays;
                }

                SubscribeToEvents();
                UpdateUserStatus();
                UpdateAllTexts();

                WatchesListView.Visibility = Visibility.Visible;
                WatchesDataGrid.Visibility = Visibility.Collapsed;

                CreateAdminContextMenu();

                // таймер для демонстрации смены динамического ресурса (задание 1)
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(5);
                timer.Tick += (s, e) =>
                {
                    ChangeDynamicAccentColor();
                    timer.Stop();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка при инициализации: {ex.Message}");
            }
        }

        private void CreateAdminContextMenu()
        {
            adminContextMenu = new ContextMenu();

            var editItem = new MenuItem();
            editItem.Header = LocalizationService.GetString("Edit");
            editItem.Click += EditMenuItem_Click;
            adminContextMenu.Items.Add(editItem);

            var deleteItem = new MenuItem();
            deleteItem.Header = LocalizationService.GetString("Delete");
            deleteItem.Foreground = System.Windows.Media.Brushes.Red;
            deleteItem.Click += DeleteMenuItem_Click;
            adminContextMenu.Items.Add(deleteItem);
        }

        private void UpdateContextMenuForListView()
        {
            if (WatchesListView == null) return;

            if (WatchesListView.Items.Count > 0 &&
                WatchesListView.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                WatchesListView.ItemContainerGenerator.StatusChanged += OnListViewItemContainerStatusChanged;
                return;
            }

            ApplyListViewContextMenu();
        }

        private void OnListViewItemContainerStatusChanged(object? sender, EventArgs e)
        {
            if (WatchesListView?.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                WatchesListView.ItemContainerGenerator.StatusChanged -= OnListViewItemContainerStatusChanged;
                ApplyListViewContextMenu();
            }
        }

        private void ApplyListViewContextMenu()
        {
            if (WatchesListView == null) return;

            for (int i = 0; i < WatchesListView.Items.Count; i++)
            {
                var item = WatchesListView.Items[i];
                var container = WatchesListView.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
                if (container != null)
                {
                    var border = FindVisualChild<Border>(container);
                    if (border != null)
                    {
                        border.ContextMenu = null;
                    }
                }
            }

            if (IsAdmin && adminContextMenu != null)
            {
                for (int i = 0; i < WatchesListView.Items.Count; i++)
                {
                    var item = WatchesListView.Items[i];
                    var container = WatchesListView.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
                    if (container != null)
                    {
                        var border = FindVisualChild<Border>(container);
                        if (border != null)
                        {
                            border.ContextMenu = adminContextMenu;
                            border.ContextMenu.Tag = item;
                            foreach (MenuItem mi in border.ContextMenu.Items)
                            {
                                mi.Tag = item;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateContextMenuForDataGrid()
        {
            if (WatchesDataGrid == null) return;

            if (IsAdmin && adminContextMenu != null)
            {
                var gridMenu = new ContextMenu();
                var editItem = new MenuItem();
                editItem.Header = LocalizationService.GetString("Edit");
                editItem.Click += DataGridEditMenuItem_Click;
                gridMenu.Items.Add(editItem);

                var deleteItem = new MenuItem();
                deleteItem.Header = LocalizationService.GetString("Delete");
                deleteItem.Foreground = System.Windows.Media.Brushes.Red;
                deleteItem.Click += DataGridDeleteMenuItem_Click;
                gridMenu.Items.Add(deleteItem);

                WatchesDataGrid.ContextMenu = gridMenu;
            }
            else
            {
                WatchesDataGrid.ContextMenu = null;
            }
        }

        private void DataGridEditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = WatchesDataGrid?.SelectedItem as WatchDisplay;
            if (selectedItem != null)
            {
                var editWindow = new AddEditWatchWindow(selectedItem.Watch);
                if (editWindow.ShowDialog() == true)
                {
                    selectedItem.UpdateLocalization();
                    WatchesListView?.Items.Refresh();
                    WatchesDataGrid?.Items.Refresh();
                    if (dataService != null && watches != null) dataService.SaveWatches(watches);
                    if (StatusTextBlock != null) StatusTextBlock.Text = LocalizationService.GetString("Product_Updated");
                    LoggerService.LogProductChange("редактирование", selectedItem.ShortName);
                }
            }
        }

        private void DataGridDeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = WatchesDataGrid?.SelectedItem as WatchDisplay;
            if (selectedItem != null)
            {
                var result = MessageBox.Show(
                    string.Format(LocalizationService.GetString("Confirm_Delete"), selectedItem.ShortName),
                    LocalizationService.GetString("Confirm_Title"),
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    watches?.Remove(selectedItem.Watch);
                    watchDisplays?.Remove(selectedItem);
                    if (dataService != null && watches != null) dataService.SaveWatches(watches);
                    if (StatusTextBlock != null) StatusTextBlock.Text = LocalizationService.GetString("Product_Deleted");
                    LoggerService.LogProductChange("удаление", selectedItem.ShortName);
                    ApplyFilters();
                }
            }
        }

        private T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T t)
                    return t;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        private void LoadCustomCursor()
        {
            try
            {
                string cursorPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "watch_cursor.cur");
                if (System.IO.File.Exists(cursorPath))
                {
                    var cursor = new Cursor(cursorPath);
                    this.Cursor = cursor;
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                }
            }
            catch (Exception)
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void SetupCommands()
        {
            if (ExitMenuItem != null) ExitMenuItem.Click += (s, e) => Close();
            if (AboutMenuItem != null) AboutMenuItem.Click += (s, e) => ShowAboutDialog();
            if (LoginMenuItem != null) LoginMenuItem.Click += (s, e) => Login();
            if (LogoutMenuItem != null) LogoutMenuItem.Click += (s, e) => Logout();

            if (ListViewRadio != null && TableViewRadio != null)
            {
                ListViewRadio.Checked += (s, e) => SwitchToListView();
                TableViewRadio.Checked += (s, e) => SwitchToTableView();
            }
            if (ListViewMenuItem != null) ListViewMenuItem.Click += (s, e) => ListViewRadio.IsChecked = true;
            if (TableViewMenuItem != null) TableViewMenuItem.Click += (s, e) => TableViewRadio.IsChecked = true;

            if (ApplyFiltersButton != null) ApplyFiltersButton.Click += (s, e) => ApplyFilters();
            if (AddWatchButton != null) AddWatchButton.Click += (s, e) => OpenAddWatchWindow();
        }

        private void SubscribeToEvents()
        {
            if (RussianLanguageMenuItem != null) RussianLanguageMenuItem.Click += (s, e) => ChangeLanguage("ru-RU");
            if (EnglishLanguageMenuItem != null) EnglishLanguageMenuItem.Click += (s, e) => ChangeLanguage("en-US");

            if (LightThemeMenuItem != null) LightThemeMenuItem.Click += (s, e) => ChangeTheme("light");
            if (PinkThemeMenuItem != null) PinkThemeMenuItem.Click += (s, e) => ChangeTheme("pink");
        }

        private void ChangeLanguage(string language)
        {
            try { LocalizationService.ChangeLanguage(language); }
            catch (Exception ex) { MessageBox.Show($"ошибка при смене языка: {ex.Message}"); }
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            try
            {
                UpdateAllTexts();

                if (watchDisplays != null)
                {
                    foreach (var display in watchDisplays)
                    {
                        display.UpdateLocalization();
                    }
                }

                if (adminContextMenu != null && adminContextMenu.Items.Count >= 2)
                {
                    (adminContextMenu.Items[0] as MenuItem).Header = LocalizationService.GetString("Edit");
                    (adminContextMenu.Items[1] as MenuItem).Header = LocalizationService.GetString("Delete");
                }

                UpdateContextMenuForListView();
                UpdateContextMenuForDataGrid();

                WatchesListView.Items.Refresh();
                WatchesDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ошибка при смене языка: {ex.Message}");
            }
        }

        private void UpdateAllTexts()
        {
            try
            {
                if (FileMenu != null) FileMenu.Header = LocalizationService.GetString("File");
                if (LoginMenuItem != null) LoginMenuItem.Header = LocalizationService.GetString("Login");
                if (LogoutMenuItem != null) LogoutMenuItem.Header = LocalizationService.GetString("Logout");
                if (ExitMenuItem != null) ExitMenuItem.Header = LocalizationService.GetString("Exit");
                if (ViewMenu != null) ViewMenu.Header = LocalizationService.GetString("View");
                if (ListViewMenuItem != null) ListViewMenuItem.Header = LocalizationService.GetString("ListView");
                if (TableViewMenuItem != null) TableViewMenuItem.Header = LocalizationService.GetString("TableView");
                if (LanguageMenu != null) LanguageMenu.Header = LocalizationService.GetString("Language");
                if (RussianLanguageMenuItem != null) RussianLanguageMenuItem.Header = LocalizationService.GetString("Russian");
                if (EnglishLanguageMenuItem != null) EnglishLanguageMenuItem.Header = LocalizationService.GetString("English");
                if (HelpMenu != null) HelpMenu.Header = LocalizationService.GetString("Help");
                if (AboutMenuItem != null) AboutMenuItem.Header = LocalizationService.GetString("About");

                if (FiltersTitleText != null) FiltersTitleText.Text = LocalizationService.GetString("Filters");
                if (SearchText != null) SearchText.Text = LocalizationService.GetString("Search");
                if (CategoryText != null) CategoryText.Text = LocalizationService.GetString("Category");
                if (PriceText != null) PriceText.Text = LocalizationService.GetString("Price");
                if (InStockCheckBox != null) InStockCheckBox.Content = LocalizationService.GetString("InStockOnly");
                if (SortText != null) SortText.Text = LocalizationService.GetString("Sort");
                if (ApplyFiltersButton != null) ApplyFiltersButton.Content = LocalizationService.GetString("ApplyFilters");
                if (ManagementText != null) ManagementText.Text = LocalizationService.GetString("Management");
                if (AddWatchButton != null) AddWatchButton.Content = LocalizationService.GetString("AddProduct");

                if (AllCategoriesItem != null) AllCategoriesItem.Content = LocalizationService.GetString("AllCategories");
                if (LuxuryCategory != null) LuxuryCategory.Content = LocalizationService.GetString("Luxury");
                if (SportCategory != null) SportCategory.Content = LocalizationService.GetString("Sport");
                if (SmartCategory != null) SmartCategory.Content = LocalizationService.GetString("Smart");
                if (ClassicCategory != null) ClassicCategory.Content = LocalizationService.GetString("Classic");

                if (SortByNameItem != null) SortByNameItem.Content = LocalizationService.GetString("ByName");
                if (SortByPriceAscItem != null) SortByPriceAscItem.Content = LocalizationService.GetString("ByPriceAsc");
                if (SortByPriceDescItem != null) SortByPriceDescItem.Content = LocalizationService.GetString("ByPriceDesc");
                if (SortByRatingItem != null) SortByRatingItem.Content = LocalizationService.GetString("ByRating");

                if (ViewModeText != null) ViewModeText.Text = LocalizationService.GetString("ViewMode");
                if (ListViewRadio != null) ListViewRadio.Content = LocalizationService.GetString("List");
                if (TableViewRadio != null) TableViewRadio.Content = LocalizationService.GetString("Table");

                if (StatusTextBlock != null && currentUser == null) StatusTextBlock.Text = LocalizationService.GetString("Ready");
                if (UserStatusTextBlock != null && currentUser == null) UserStatusTextBlock.Text = LocalizationService.GetString("NotAuthorized");

                Title = LocalizationService.CurrentLanguage == "ru-RU" ? "Магазин часов" : "Watch Shop";

                if (WatchesDataGrid != null && WatchesDataGrid.Columns.Count >= 7)
                {
                    WatchesDataGrid.Columns[0].Header = LocalizationService.GetString("ListView");
                    WatchesDataGrid.Columns[1].Header = LocalizationService.GetString("Category");
                    WatchesDataGrid.Columns[2].Header = LocalizationService.GetString("Price_Label");
                    WatchesDataGrid.Columns[3].Header = LocalizationService.GetString("Rating");
                    WatchesDataGrid.Columns[4].Header = LocalizationService.GetString("InStock");
                    WatchesDataGrid.Columns[5].Header = LocalizationService.GetString("Manufacturer_field");
                    WatchesDataGrid.Columns[6].Header = LocalizationService.GetString("AvailableHeader");
                }

                WatchesListView.Items.Refresh();
                WatchesDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ошибка обновления текстов: {ex.Message}");
            }
        }

        private void SwitchToListView()
        {
            try
            {
                if (WatchesListView != null && WatchesDataGrid != null)
                {
                    WatchesListView.Visibility = Visibility.Visible;
                    WatchesDataGrid.Visibility = Visibility.Collapsed;

                    Dispatcher.BeginInvoke(new Action(() => {
                        UpdateContextMenuForListView();
                    }), DispatcherPriority.Background);

                    WatchesListView.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка: {ex.Message}");
            }
        }

        private void SwitchToTableView()
        {
            try
            {
                if (WatchesListView != null && WatchesDataGrid != null)
                {
                    if (WatchesDataGrid.ItemsSource == null && watchDisplays != null)
                    {
                        WatchesDataGrid.ItemsSource = watchDisplays;
                    }

                    WatchesListView.Visibility = Visibility.Collapsed;
                    WatchesDataGrid.Visibility = Visibility.Visible;
                    UpdateContextMenuForDataGrid();
                    WatchesDataGrid.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка при переключении на таблицу: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            try
            {
                if (watchDisplays == null || watches == null) return;

                var filtered = watches.AsEnumerable();

                if (SearchTextBox != null && !string.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    string searchText = SearchTextBox.Text.ToLower();
                    filtered = filtered.Where(w =>
                        (w.ShortName != null && w.ShortName.ToLower().Contains(searchText)) ||
                        (w.FullName != null && w.FullName.ToLower().Contains(searchText)));
                }

                if (CategoriesListBox != null && CategoriesListBox.SelectedItem != null)
                {
                    var selectedItem = CategoriesListBox.SelectedItem as ListBoxItem;
                    if (selectedItem != null)
                    {
                        string selectedCategory = selectedItem.Content.ToString() ?? "";

                        string filterCategory = selectedCategory;
                        if (LocalizationService.CurrentLanguage == "en-US")
                        {
                            filterCategory = selectedCategory switch
                            {
                                "Luxury" => "Люксовые",
                                "Sport" => "Спортивные",
                                "Smart watches" => "Смарт-часы",
                                "Classic" => "Классические",
                                _ => selectedCategory
                            };
                        }

                        if (filterCategory != LocalizationService.GetString("AllCategories") &&
                            filterCategory != "All categories")
                        {
                            filtered = filtered.Where(w => w.Category == filterCategory);
                        }
                    }
                }

                if (MinPriceTextBox != null && decimal.TryParse(MinPriceTextBox.Text, out decimal minPrice))
                    filtered = filtered.Where(w => w.Price >= minPrice);
                if (MaxPriceTextBox != null && decimal.TryParse(MaxPriceTextBox.Text, out decimal maxPrice))
                    filtered = filtered.Where(w => w.Price <= maxPrice);

                if (InStockCheckBox != null && InStockCheckBox.IsChecked == true)
                    filtered = filtered.Where(w => w.InStock && w.Quantity > 0);

                if (SortComboBox != null)
                {
                    switch (SortComboBox.SelectedIndex)
                    {
                        case 0: filtered = filtered.OrderBy(w => w.ShortName); break;
                        case 1: filtered = filtered.OrderBy(w => w.Price); break;
                        case 2: filtered = filtered.OrderByDescending(w => w.Price); break;
                        case 3: filtered = filtered.OrderByDescending(w => w.Rating); break;
                    }
                }

                var filteredList = filtered.ToList();
                if (watchDisplays != null)
                {
                    watchDisplays.Clear();
                    foreach (var watch in filteredList)
                    {
                        watchDisplays.Add(new WatchDisplay(watch));
                    }
                }

                if (StatusTextBlock != null)
                    StatusTextBlock.Text = $"{LocalizationService.GetString("ProductsFound")} {watchDisplays?.Count ?? 0}";

                Dispatcher.BeginInvoke(new Action(() => {
                    UpdateContextMenuForListView();
                    UpdateContextMenuForDataGrid();
                }), DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка при фильтрации: {ex.Message}");
            }
        }

        private void WatchesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedDisplay = (sender as ListView)?.SelectedItem as WatchDisplay;
            if (selectedDisplay != null) ShowWatchDetails(selectedDisplay.Watch);
        }

        private void WatchesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedDisplay = (sender as DataGrid)?.SelectedItem as WatchDisplay;
            if (selectedDisplay != null) ShowWatchDetails(selectedDisplay.Watch);
        }

        private void ShowWatchDetails(Watch watch)
        {
            string localizedCategory = watch.Category ?? "";
            if (LocalizationService.CurrentLanguage == "en-US")
            {
                localizedCategory = watch.Category switch
                {
                    "Люксовые" => "Luxury",
                    "Спортивные" => "Sport",
                    "Смарт-часы" => "Smart watches",
                    "Классические" => "Classic",
                    "Дайверские" => "Diver",
                    "Авиационные" => "Aviation",
                    "Модные" => "Fashion",
                    _ => watch.Category ?? ""
                };
            }

            string localizedMovement = watch.MovementType ?? "";
            if (LocalizationService.CurrentLanguage == "en-US")
            {
                localizedMovement = watch.MovementType switch
                {
                    "Механические" => "Mechanical",
                    "Кварцевые" => "Quartz",
                    "Электронные" => "Electronic",
                    "Автоматические" => "Automatic",
                    "Солнечные" => "Solar",
                    _ => watch.MovementType ?? ""
                };
            }

            string localizedMaterial = watch.Material ?? "";
            if (LocalizationService.CurrentLanguage == "en-US")
            {
                localizedMaterial = watch.Material switch
                {
                    "Нержавеющая сталь" => "Stainless steel",
                    "Титан" => "Titanium",
                    "Керамика" => "Ceramic",
                    "Пластик" => "Plastic",
                    "Карбон" => "Carbon",
                    "Золото" => "Gold",
                    "Серебро" => "Silver",
                    _ => watch.Material ?? ""
                };
            }

            string localizedColor = watch.Color ?? "";
            if (LocalizationService.CurrentLanguage == "en-US")
            {
                localizedColor = watch.Color switch
                {
                    "Черный" => "Black",
                    "Серебристый" => "Silver",
                    "Золотистый" => "Gold",
                    "Синий" => "Blue",
                    "Зеленый" => "Green",
                    "Красный" => "Red",
                    "Белый" => "White",
                    "Розовый" => "Pink",
                    "Коричневый" => "Brown",
                    _ => watch.Color ?? ""
                };
            }

            string localizedCountry = watch.CountryOfOrigin ?? "";
            if (LocalizationService.CurrentLanguage == "en-US")
            {
                localizedCountry = watch.CountryOfOrigin switch
                {
                    "Швейцария" => "Switzerland",
                    "Япония" => "Japan",
                    "Германия" => "Germany",
                    "Китай" => "China",
                    "Россия" => "Russia",
                    "США" => "USA",
                    "Франция" => "France",
                    "Италия" => "Italy",
                    _ => watch.CountryOfOrigin ?? ""
                };
            }

            string localizedGender = watch.Gender ?? "";
            if (LocalizationService.CurrentLanguage == "en-US")
            {
                localizedGender = watch.Gender switch
                {
                    "Мужские" => "Men",
                    "Женские" => "Women",
                    "Унисекс" => "Unisex",
                    _ => watch.Gender ?? ""
                };
            }

            string localizedWaterResistance = watch.WaterResistance ?? "";
            if (LocalizationService.CurrentLanguage == "en-US")
            {
                localizedWaterResistance = watch.WaterResistance switch
                {
                    "30m" => "30m (splash)",
                    "50m" => "50m (shower)",
                    "100m" => "100m (swimming)",
                    "200m" => "200m (diving)",
                    "300m" => "300m (professional diving)",
                    "500m+" => "500m+ (deep water)",
                    "Нет" => "None",
                    _ => watch.WaterResistance ?? ""
                };
            }

            string stockStatus = watch.InStock
                ? $"{LocalizationService.GetString("InStock_Label")} {watch.Quantity} шт."
                : LocalizationService.GetString("OutOfStock");

            string discountInfo = watch.Discount > 0
                ? $"{LocalizationService.GetString("Discount_Label")}: {watch.Discount}% (цена со скидкой: ${watch.DiscountedPrice:F2})"
                : LocalizationService.GetString("DiscountNotApplied");

            string details = $"=== {watch.ShortName} ===\n\n" +
                            $"{LocalizationService.GetString("FullName_Label")}:\n{watch.FullName}\n\n" +
                            $"{LocalizationService.GetString("Category_Label")}: {localizedCategory}\n" +
                            $"{LocalizationService.GetString("Manufacturer_Label")}: {watch.Manufacturer}\n" +
                            $"{LocalizationService.GetString("Country_Label")}: {localizedCountry}\n\n" +
                            $"{LocalizationService.GetString("Price_Label2")}: ${watch.Price:F2}\n" +
                            $"{discountInfo}\n\n" +
                            $"{LocalizationService.GetString("Characteristics")}:\n" +
                            $"• {LocalizationService.GetString("Rating_Label")}: {watch.Rating} ★\n" +
                            $"• {LocalizationService.GetString("MovementType_Label")}: {localizedMovement}\n" +
                            $"• {LocalizationService.GetString("Size_Label")}: {watch.Size}\n" +
                            $"• {LocalizationService.GetString("Material_Label")}: {localizedMaterial}\n" +
                            $"• {LocalizationService.GetString("Color_Label")}: {localizedColor}\n" +
                            $"• {LocalizationService.GetString("WaterResistance_Label")}: {localizedWaterResistance}\n" +
                            $"• {LocalizationService.GetString("Gender_Label")}: {localizedGender}\n\n" +
                            $"{LocalizationService.GetString("Stock_Label")}:\n" +
                            $"• {stockStatus}\n" +
                            $"• {LocalizationService.GetString("SoldCount_Label")}: {watch.SoldCount} шт.\n\n" +
                            $"{LocalizationService.GetString("Description_Label")}:\n{watch.Description}";

            MessageBox.Show(details, $"{LocalizationService.GetString("Details")}: {watch.ShortName}",
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var display = button?.Tag as WatchDisplay;

                if (display == null) return;

                if (!display.InStock || display.Quantity <= 0)
                {
                    MessageBox.Show(
                        string.Format(LocalizationService.GetString("Order_OutOfStock"), display.ShortName),
                        LocalizationService.GetString("Warning"),
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                string quantityInput = Microsoft.VisualBasic.Interaction.InputBox(
                    string.Format(LocalizationService.GetString("Order_Quantity"), display.Quantity),
                    LocalizationService.GetString("Order"),
                    "1");

                if (!int.TryParse(quantityInput, out int orderQuantity) || orderQuantity <= 0)
                {
                    return;
                }

                if (orderQuantity > display.Quantity)
                {
                    MessageBox.Show(
                        string.Format("доступно только {0} шт.", display.Quantity),
                        LocalizationService.GetString("Warning"),
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                decimal totalPrice = display.Price * orderQuantity;

                string message = string.Format(
                    LocalizationService.GetString("Order_Details"),
                    display.ShortName,
                    display.Price,
                    orderQuantity,
                    totalPrice);

                MessageBox.Show(
                    message,
                    LocalizationService.GetString("Order_Success"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                LoggerService.LogOrder(display.ShortName, orderQuantity, totalPrice);

                display.Quantity -= orderQuantity;

                if (display.Quantity == 0)
                {
                    display.InStock = false;
                    StatusTextBlock.Text = $"товар '{display.ShortName}' полностью распродан!";
                    LoggerService.Log($"товар '{display.ShortName}' полностью распродан");
                }
                else if (display.Quantity < 5)
                {
                    StatusTextBlock.Text = $"внимание: товара '{display.ShortName}' осталось мало ({display.Quantity} шт.)";
                    LoggerService.LogLowStock(display.ShortName, display.Quantity);
                }

                if (dataService != null && watches != null)
                {
                    dataService.SaveWatches(watches);
                }

                WatchesListView.Items.Refresh();
                WatchesDataGrid.Items.Refresh();

                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = "заказ оформлен! спасибо за покупку!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка при оформлении заказа: {ex.Message}");
            }
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var display = menuItem?.Tag as WatchDisplay;

            if (display != null)
            {
                var editWindow = new AddEditWatchWindow(display.Watch);
                if (editWindow.ShowDialog() == true)
                {
                    display.UpdateLocalization();
                    WatchesListView?.Items.Refresh();
                    WatchesDataGrid?.Items.Refresh();
                    if (dataService != null && watches != null) dataService.SaveWatches(watches);
                    if (StatusTextBlock != null) StatusTextBlock.Text = LocalizationService.GetString("Product_Updated");
                    LoggerService.LogProductChange("редактирование", display.ShortName);
                }
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var display = menuItem?.Tag as WatchDisplay;

            if (display != null)
            {
                var result = MessageBox.Show(
                    string.Format(LocalizationService.GetString("Confirm_Delete"), display.ShortName),
                    LocalizationService.GetString("Confirm_Title"),
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    watches?.Remove(display.Watch);
                    watchDisplays?.Remove(display);
                    if (dataService != null && watches != null) dataService.SaveWatches(watches);
                    if (StatusTextBlock != null) StatusTextBlock.Text = LocalizationService.GetString("Product_Deleted");
                    LoggerService.LogProductChange("удаление", display.ShortName);
                    ApplyFilters();
                }
            }
        }

        private void Login()
        {
            try
            {
                var loginWindow = new LoginWindow();
                if (loginWindow.ShowDialog() == true)
                {
                    currentUser = loginWindow.AuthenticatedUser;
                    UpdateUserStatus();

                    if (currentUser?.Role == "Admin")
                    {
                        if (AdminPanel != null) AdminPanel.Visibility = Visibility.Visible;
                        Dispatcher.BeginInvoke(new Action(() => {
                            UpdateContextMenuForListView();
                            UpdateContextMenuForDataGrid();
                        }), DispatcherPriority.Background);
                    }
                    else
                    {
                        if (AdminPanel != null) AdminPanel.Visibility = Visibility.Collapsed;
                    }

                    if (LoginMenuItem != null) LoginMenuItem.IsEnabled = false;
                    if (LogoutMenuItem != null) LogoutMenuItem.IsEnabled = true;

                    // активация пункта профиль после входа
                    if (ProfileMenuItem != null) ProfileMenuItem.IsEnabled = true;

                    if (StatusTextBlock != null) StatusTextBlock.Text = $"{LocalizationService.GetString("Welcome")}, {currentUser?.Username}!";

                    LoggerService.LogLogin(currentUser?.Username ?? "unknown");

                    if (WatchesListView != null) WatchesListView.Items.Refresh();
                    if (WatchesDataGrid != null) WatchesDataGrid.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка при входе: {ex.Message}");
            }
        }

        private void Logout()
        {
            if (currentUser != null)
            {
                LoggerService.LogLogout(currentUser.Username);
            }

            currentUser = null;
            UpdateUserStatus();

            if (AdminPanel != null) AdminPanel.Visibility = Visibility.Collapsed;
            if (LoginMenuItem != null) LoginMenuItem.IsEnabled = true;
            if (LogoutMenuItem != null) LogoutMenuItem.IsEnabled = false;
            if (StatusTextBlock != null) StatusTextBlock.Text = LocalizationService.GetString("Ready");

            UpdateContextMenuForListView();
            UpdateContextMenuForDataGrid();

            WatchesListView?.Items.Refresh();
            WatchesDataGrid?.Items.Refresh();
        }

        private void UpdateUserStatus()
        {
            if (UserStatusTextBlock != null)
            {
                UserStatusTextBlock.Text = currentUser != null
                    ? $"{currentUser.Username} ({currentUser.Role})"
                    : LocalizationService.GetString("NotAuthorized");
            }
        }

        private void OpenAddWatchWindow()
        {
            try
            {
                var addWindow = new AddEditWatchWindow();
                if (addWindow.ShowDialog() == true && watches != null && dataService != null && watchDisplays != null)
                {
                    if (addWindow.Watch != null)
                    {
                        watches.Add(addWindow.Watch);
                        watchDisplays.Add(new WatchDisplay(addWindow.Watch));
                        dataService.SaveWatches(watches);
                        ApplyFilters();
                        if (StatusTextBlock != null) StatusTextBlock.Text = LocalizationService.GetString("Product_Added");
                        LoggerService.LogProductChange("добавление", addWindow.Watch.ShortName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка при добавлении товара: {ex.Message}");
            }
        }

        private void ViewLogButton_Click(object sender, RoutedEventArgs e)
        {
            string logContent = LoggerService.ReadLog();

            var logWindow = new Window
            {
                Title = "журнал событий",
                Width = 600,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = System.Windows.Media.Brushes.White
            };

            var textBox = new TextBox
            {
                Text = logContent,
                TextWrapping = TextWrapping.Wrap,
                IsReadOnly = true,
                Margin = new Thickness(10),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            var clearButton = new Button
            {
                Content = "очистить лог",
                Width = 100,
                Height = 30,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            clearButton.Click += (s, args) =>
            {
                LoggerService.ClearLog();
                textBox.Text = "лог очищен";
            };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(clearButton);

            logWindow.Content = stackPanel;
            logWindow.ShowDialog();
        }

        private void ShowAboutDialog()
        {
            string title = LocalizationService.CurrentLanguage == "ru-RU" ? "О программе" : "About";
            string message = LocalizationService.CurrentLanguage == "ru-RU"
                ? "магазин часов v1.0\n\nразработано для лабораторной работы\nпо дисциплине 'объектно-ориентированное программирование'\n\n© 2026 WatchShop"
                : "watch shop v1.0\n\ndeveloped for laboratory work\non 'object-oriented programming'\n\n© 2026 WatchShop";
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (dataService != null && watches != null) dataService.SaveWatches(watches);
            LocalizationService.LanguageChanged -= OnLanguageChanged;
            base.OnClosed(e);
        }
        // смена цвета динамического ресурса (для демонстрации работы dynamicresource)
        private void ChangeDynamicAccentColor()
        {
            // создаём новую кисть, а не меняем существующую
            var newBrush = new SolidColorBrush(System.Windows.Media.Colors.Green);
            Application.Current.Resources["DynamicAccentBrush"] = newBrush;

            if (StatusTextBlock != null)
                StatusTextBlock.Text = "динамический ресурс изменён на зелёный";
        }
        private void ChangeTheme(string themeName)
        {
            var newTheme = new ResourceDictionary();

            switch (themeName)
            {
                case "light":
                    newTheme.Source = new Uri("Resources/Themes/ThemeLight.xaml", UriKind.Relative);
                    break;
                case "pink":
                    newTheme.Source = new Uri("Resources/Themes/ThemePink.xaml", UriKind.Relative);
                    break;
                default:
                    return;
            }

            var oldTheme = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null &&
                       (d.Source.OriginalString.Contains("ThemeLight") ||
                        d.Source.OriginalString.Contains("ThemePink")));

            if (oldTheme != null)
                Application.Current.Resources.MergedDictionaries.Remove(oldTheme);

            Application.Current.Resources.MergedDictionaries.Add(newTheme);

            // заменяем динамический ресурс, а не меняем его цвет
            if (themeName == "pink")
                Application.Current.Resources["DynamicAccentBrush"] = new SolidColorBrush(System.Windows.Media.Colors.HotPink);
            else
                Application.Current.Resources["DynamicAccentBrush"] = new SolidColorBrush(System.Windows.Media.Colors.DodgerBlue);
        }

        private void ProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser == null)
            {
                MessageBox.Show(LocalizationService.GetString("login_required"),
                               LocalizationService.GetString("warning"),
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var profileWindow = new Views.ProfileWindow(currentUser, () =>
            {
                UpdateUserStatus();
                UpdateAllTexts();
            });

            profileWindow.ShowDialog();
        }

    }
}