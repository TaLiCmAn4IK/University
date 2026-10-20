using System.Linq;
using System.Windows;
using WatchShop.Models;
using WatchShop.Services;

namespace WatchShop.Views
{
    /// <summary>
    /// Окно регистрации нового пользователя. Создаёт нового User с ролью Client
    /// и сохраняет в users.json.
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly DataService dataService = new();

        public User? CreatedUser { get; private set; }

        public RegisterWindow()
        {
            InitializeComponent();
            UsernameBox.Focus();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password;
            string passwordConfirm = PasswordConfirmBox.Password;

            // ---- валидация ----
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                ShowError("Register_Error_Username");
                return;
            }
            if (password.Length < 4)
            {
                ShowError("Register_Error_PasswordShort");
                return;
            }
            if (password != passwordConfirm)
            {
                ShowError("Register_Error_PasswordMismatch");
                return;
            }

            var users = dataService.LoadUsers();
            if (users.Any(u => u.Username.Equals(username, System.StringComparison.OrdinalIgnoreCase)))
            {
                ShowError("Register_Error_Exists");
                return;
            }

            // ---- создание ----
            var user = new User
            {
                Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1,
                Username = username,
                Password = password,
                Email = email,
                Role = "Client",
                IsActive = true
            };
            users.Add(user);
            dataService.SaveUsers(users);

            CreatedUser = user;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string key)
        {
            ErrorText.Text = LocalizationService.GetString(key);
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
