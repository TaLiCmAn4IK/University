using System.Data;
using System.Windows;
using WatchShop.Services;

namespace WatchShop.Views
{
    /// <summary>
    /// Админ-панель: отображение трёх таблиц БД через SqlDataAdapter +
    /// DataTable + привязка к DataGrid.ItemsSource (через DataView).
    /// Доступна только пользователям с ролью Admin.
    ///
    /// Демонстрирует:
    ///   - SqlDataAdapter / DataTable / DataView
    ///   - Привязку данных к ЭУ (DataGrid.ItemsSource)
    ///   - Сортировку (DataGrid встроенно умеет — щелчок по заголовку колонки;
    ///     внутри DataView это работает автоматически)
    ///   - Кнопки навигации между таблицами
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly WatchRepository repo = new();
        private string currentTable = "Watches";

        public AdminWindow()
        {
            InitializeComponent();
            // По умолчанию открыта таблица товаров
            Loaded += (_, _) => ShowWatches_Click(this, new RoutedEventArgs());
        }

        // ===== Навигация между таблицами =====

        private void ShowWatches_Click(object sender, RoutedEventArgs e)
        {
            currentTable = "Watches";
            UpdateNavStyles();
            LoadCurrent();
        }

        private void ShowUsers_Click(object sender, RoutedEventArgs e)
        {
            currentTable = "Users";
            UpdateNavStyles();
            LoadCurrent();
        }

        private void ShowOrders_Click(object sender, RoutedEventArgs e)
        {
            currentTable = "Orders";
            UpdateNavStyles();
            LoadCurrent();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) => LoadCurrent();

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        // ===== Загрузка данных через SqlDataAdapter =====

        private void LoadCurrent()
        {
            try
            {
                DataTable table = currentTable switch
                {
                    "Users" => repo.GetAllUsersAsTable(),
                    "Orders" => repo.GetAllOrdersAsTable(),
                    _ => repo.GetAllWatchesAsTable(),
                };

                // Привязка через DataView — DataGrid поддерживает сортировку
                // кликом по заголовку колонки автоматически.
                MainGrid.ItemsSource = table.DefaultView;
                CurrentTableLabel.Text = currentTable;
                RowsCountText.Text = $"{LocalizationService.GetString("Admin_RowsCount")}: {table.Rows.Count}";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Не удалось загрузить таблицу: " + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateNavStyles()
        {
            // Активная вкладка — стиль PrimaryButton, неактивные — Info.
            var primary = (Style)FindResource("PrimaryButtonStyle");
            var info = (Style)FindResource("InfoButtonStyle");

            WatchesNavBtn.Style = currentTable == "Watches" ? primary : info;
            UsersNavBtn.Style   = currentTable == "Users"   ? primary : info;
            OrdersNavBtn.Style  = currentTable == "Orders"  ? primary : info;
        }
    }
}
