using System.Windows;
using WatchShop.Models;

namespace WatchShop.Views
{
    /// <summary>
    /// Окно деталей товара. DataContext — сам Watch,
    /// все поля привязаны через {Binding} и {DynamicResource}.
    /// </summary>
    public partial class WatchDetailsWindow : Window
    {
        public WatchDetailsWindow(Watch watch)
        {
            InitializeComponent();
            DataContext = watch;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
