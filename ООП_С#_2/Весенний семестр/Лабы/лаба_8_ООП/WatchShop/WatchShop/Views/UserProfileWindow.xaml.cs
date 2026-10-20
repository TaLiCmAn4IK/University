using System.Linq;
using System.Windows;
using WatchShop.Models;
using WatchShop.Services;

namespace WatchShop.Views
{
    /// <summary>
    /// Личный кабинет — позволяет пользователю отредактировать свои данные,
    /// поменять язык интерфейса и тему оформления.
    /// </summary>
    public partial class UserProfileWindow : Window
    {
        private readonly UserRepository userRepo = new();
        private readonly User user;

        public bool DataSaved { get; private set; }

        public UserProfileWindow(User user)
        {
            InitializeComponent();
            this.user = user;

            // личные данные
            UsernameBox.Text = user.Username;
            EmailBox.Text = user.Email;
            PasswordBox.Text = user.Password;
            UserRoleText.Text = user.Role;

            // язык
            if (LocalizationService.CurrentLanguage == LocalizationService.LanguageEn)
                LangEnRadio.IsChecked = true;
            else
                LangRuRadio.IsChecked = true;

            // тема
            switch (ThemeService.CurrentTheme)
            {
                case ThemeService.ThemeDark: ThemeDarkRadio.IsChecked = true; break;
                case ThemeService.ThemePink: ThemePinkRadio.IsChecked = true; break;
                default: ThemeLightRadio.IsChecked = true; break;
            }

            // показ "спасибо!" при выставлении оценки
            FavRating.MouseUp += (_, _) =>
            {
                if (FavRating.Value > 0)
                    RatingThanksText.Visibility = System.Windows.Visibility.Visible;
            };
        }

        private void LangRuRadio_Checked(object sender, RoutedEventArgs e)
            => LocalizationService.ChangeLanguage(LocalizationService.LanguageRu);

        private void LangEnRadio_Checked(object sender, RoutedEventArgs e)
            => LocalizationService.ChangeLanguage(LocalizationService.LanguageEn);

        private void ThemeLightRadio_Checked(object sender, RoutedEventArgs e)
            => ThemeService.ChangeTheme(ThemeService.ThemeLight);

        private void ThemeDarkRadio_Checked(object sender, RoutedEventArgs e)
            => ThemeService.ChangeTheme(ThemeService.ThemeDark);

        private void ThemePinkRadio_Checked(object sender, RoutedEventArgs e)
            => ThemeService.ChangeTheme(ThemeService.ThemePink);

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            user.Email = EmailBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(PasswordBox.Text))
                user.Password = PasswordBox.Text;

            // UPDATE в БД через ADO.NET
            try
            {
                userRepo.UpdateProfile(user.Id, user.Email, user.Password);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Не удалось сохранить профиль: " + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataSaved = true;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
