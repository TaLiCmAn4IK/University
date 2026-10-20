using System.Windows;
using WatchShop.Models;
using WatchShop.Services;

namespace WatchShop.Views
{
    /// <summary>
    /// Окно регистрации. Создаёт пользователя в БД (UserRepository.Register).
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly UserRepository userRepo = new();

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

            // Проверка через БД
            if (userRepo.UsernameExists(username))
            {
                ShowError("Register_Error_Exists");
                return;
            }

            try
            {
                int newId = userRepo.Register(username, password, email);
                CreatedUser = new User
                {
                    Id = newId,
                    Username = username,
                    Password = password,
                    Email = email,
                    Role = "Client",
                    IsActive = true
                };
                DialogResult = true;
                Close();
            }
            catch (System.Exception ex)
            {
                ErrorText.Text = "Ошибка регистрации: " + ex.Message;
                ErrorText.Visibility = Visibility.Visible;
            }
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
