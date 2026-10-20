using System.Linq;
using System.Windows;
using WatchShop.Models;
using WatchShop.Services;

namespace WatchShop.Views
{
    public partial class LoginWindow : Window
    {
        private DataService dataService;
        public User? AuthenticatedUser { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            dataService = new DataService();

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
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username))
            {
                ErrorMessageTextBlock.Text = LocalizationService.GetString("EnterUsername");
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ErrorMessageTextBlock.Text = LocalizationService.GetString("EnterPassword");
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            var users = dataService.LoadUsers();
            var user = users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                if (user.Password == password)
                {
                    AuthenticatedUser = user;
                    DialogResult = true;
                    Close();
                    return;
                }
                else
                {
                    ErrorMessageTextBlock.Text = LocalizationService.GetString("InvalidCredentials");
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                    return;
                }
            }
            else
            {
                int newId = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
                var newUser = new User
                {
                    Id = newId,
                    Username = username,
                    Password = password,
                    Role = "Client",
                    Email = "",
                    RegistrationDate = System.DateTime.Now,
                    IsActive = true
                };

                users.Add(newUser);
                dataService.SaveUsers(users);

                AuthenticatedUser = newUser;
                DialogResult = true;
                Close();
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