using System.Windows;

namespace WatchShop.Views
{
    /// <summary>
    /// Простое модальное окно для ввода количества товара.
    /// Заменяет VisualBasic InputBox и не требует внешних зависимостей.
    /// </summary>
    public partial class QuantityInputWindow : Window
    {
        public int EnteredQuantity { get; private set; }

        public QuantityInputWindow(string message)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
            QuantityTextBox.Focus();
            QuantityTextBox.SelectAll();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(QuantityTextBox.Text, out int q) && q > 0)
            {
                EnteredQuantity = q;
                DialogResult = true;
                Close();
            }
            else
            {
                QuantityTextBox.SelectAll();
                QuantityTextBox.Focus();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
