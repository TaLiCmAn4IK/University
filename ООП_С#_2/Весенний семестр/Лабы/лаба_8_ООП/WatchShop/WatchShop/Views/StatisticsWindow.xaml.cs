using System.Threading.Tasks;
using System.Windows;
using WatchShop.Services;

namespace WatchShop.Views
{
    /// <summary>
    /// Окно статистики магазина.
    /// Демонстрирует:
    ///   - вызов хранимых процедур sp_GetStatistics, sp_GetTopSelling
    ///   - АСИНХРОННЫЕ запросы (ExecuteReaderAsync) — UI не блокируется
    /// </summary>
    public partial class StatisticsWindow : Window
    {
        private readonly WatchRepository repo = new();

        public StatisticsWindow()
        {
            InitializeComponent();
            Loaded += async (_, _) => await LoadAsync();
        }

        private async Task LoadAsync()
        {
            try
            {
                // Хранимая процедура sp_GetStatistics
                var stats = await repo.GetStatisticsAsync();
                if (stats.Rows.Count > 0)
                {
                    var r = stats.Rows[0];
                    WatchCountText.Text = r["WatchCount"].ToString();
                    InStockText.Text    = r["InStockCount"].ToString();
                    UserCountText.Text  = r["UserCount"].ToString();
                    OrderCountText.Text = r["OrderCount"].ToString();
                    RevenueText.Text    = "$" + ((decimal)r["TotalRevenue"]).ToString("F2");
                    SoldText.Text       = r["TotalSoldItems"].ToString();
                }

                // Хранимая процедура sp_GetTopSelling
                var top = await repo.GetTopSellingAsync(5);
                TopGrid.ItemsSource = top;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Не удалось загрузить статистику: " + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
            => await LoadAsync();

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
