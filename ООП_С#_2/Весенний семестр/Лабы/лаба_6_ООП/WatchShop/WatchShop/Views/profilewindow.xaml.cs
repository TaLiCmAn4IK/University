using System;
using System.Windows;
using WatchShop.Models;
using WatchShop.Services;

namespace WatchShop.Views
{
    public partial class ProfileWindow : Window
    {
        private User currentUser;
        private DataService dataService;
        private Action onProfileUpdated;

        public ProfileWindow(User user, Action onUpdated)
        {
            InitializeComponent();

            currentUser = user;
            dataService = new DataService();
            onProfileUpdated = onUpdated;

            LocalizationService.LanguageChanged += OnLanguageChanged;

            LoadUserData();
            UpdateLocalization();
            SetLanguageRadio();
        }

        private void LoadUserData()
        {
            UsernameTextBox.Text = currentUser.Username;
            EmailTextBox.Text = currentUser.Email;
        }

        private void SetLanguageRadio()
        {
            if (LocalizationService.CurrentLanguage == "ru-RU")
                RussianRadio.IsChecked = true;
            else
                EnglishRadio.IsChecked = true;
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            Title = LocalizationService.GetString("profile_title");
            TitleTextBlock.Text = LocalizationService.GetString("profile_title");
            UsernameLabel.Text = LocalizationService.GetString("username");
            EmailLabel.Text = LocalizationService.GetString("email");
            PasswordLabel.Text = LocalizationService.GetString("new_password");
            PasswordHint.Text = LocalizationService.GetString("password_hint");
            LanguageLabel.Text = LocalizationService.GetString("language");
            RussianRadio.Content = LocalizationService.GetString("russian");
            EnglishRadio.Content = LocalizationService.GetString("english");
            SaveButton.Content = LocalizationService.GetString("save");
            CancelButton.Content = LocalizationService.GetString("cancel");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // сохраняем email
            currentUser.Email = EmailTextBox.Text;

            // сохраняем пароль если введён
            if (!string.IsNullOrEmpty(NewPasswordBox.Password))
            {
                currentUser.Password = NewPasswordBox.Password;
            }

            // сохраняем язык
            string newLanguage = RussianRadio.IsChecked == true ? "ru-RU" : "en-US";
            if (LocalizationService.CurrentLanguage != newLanguage)
            {
                LocalizationService.ChangeLanguage(newLanguage);
            }

            // сохраняем пользователя
            var users = dataService.LoadUsers();
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Id == currentUser.Id)
                {
                    users[i] = currentUser;
                    break;
                }
            }
            dataService.SaveUsers(users);

            // обновляем текущего пользователя в главном окне
            onProfileUpdated?.Invoke();

            MessageBox.Show(LocalizationService.GetString("profile_saved"),
                           LocalizationService.GetString("info"),
                           MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            LocalizationService.LanguageChanged -= OnLanguageChanged;
            base.OnClosed(e);
        }
    }
}