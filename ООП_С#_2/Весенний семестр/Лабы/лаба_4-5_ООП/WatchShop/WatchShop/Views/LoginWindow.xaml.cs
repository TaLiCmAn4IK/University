using System.Linq;
using System.Windows;
using WatchShop.Models;
using WatchShop.Services;

namespace WatchShop.Views
{
    // окно для входа в систему
    public partial class LoginWindow : Window
    {
        private DataService dataService;
        public User AuthenticatedUser { get; private set; } = new();

        public LoginWindow()
        {
            InitializeComponent();
            dataService = new DataService();

            // подписка на событие смены языка
            LocalizationService.LanguageChanged += OnLanguageChanged;

            UpdateLocalization();
        }

        private void OnLanguageChanged(object? sender, System.EventArgs e)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            Title = LocalizationService.GetString("LoginTitle");
            TitleTextBlock.Text = LocalizationService.GetString("LoginTitle");
            UsernameTextBlock.Text = LocalizationService.GetString("Username");
            PasswordTextBlock.Text = LocalizationService.GetString("Password");
            LoginButton.Content = LocalizationService.GetString("LoginButton");
            CancelButton.Content = LocalizationService.GetString("CancelButton");
            ErrorMessageTextBlock.Text = LocalizationService.GetString("InvalidCredentials");
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var users = dataService.LoadUsers();
            var user = users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);

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

        protected override void OnClosed(System.EventArgs e)
        {
            LocalizationService.LanguageChanged -= OnLanguageChanged;
            base.OnClosed(e);
        }
    }
}