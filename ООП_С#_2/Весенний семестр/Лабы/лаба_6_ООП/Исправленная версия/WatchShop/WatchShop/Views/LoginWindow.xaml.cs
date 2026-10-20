using System.Linq;
using System.Windows;
using WatchShop.Models;
using WatchShop.Services;

namespace WatchShop.Views
{
    /// <summary>
    /// Окно входа. Простая аутентификация по username/password из users.json.
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly DataService dataService = new();

        public User? AuthenticatedUser { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            UsernameTextBox.Focus();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var user = dataService.LoadUsers()
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                AuthenticatedUser = user;
                DialogResult = true;
                Close();
            }
            else
            {
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Открывает окно регистрации; если пользователь успешно зарегистрировался,
        /// логин-окно сразу логинит его и закрывается.
        /// </summary>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var reg = new RegisterWindow { Owner = this };
            if (reg.ShowDialog() == true && reg.CreatedUser != null)
            {
                AuthenticatedUser = reg.CreatedUser;
                DialogResult = true;
                Close();
            }
        }
    }
}
