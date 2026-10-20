using System.Windows;
using System.Windows.Controls;
using Flux.ViewModels;

namespace Flux.Views;

public partial class LoginView : UserControl
{
    private bool _syncing;
    private bool _isPasswordVisible;

    public LoginView()
    {
        InitializeComponent();
    }

    private void PasswordBox_Changed(object sender, RoutedEventArgs e)
    {
        if (_syncing) return;
        if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
        {
            vm.Password = pb.Password;
            _syncing = true;
            if (PasswordPlain.Text != pb.Password) PasswordPlain.Text = pb.Password;
            _syncing = false;
        }
    }

    private void PasswordPlain_Changed(object sender, TextChangedEventArgs e)
    {
        if (_syncing) return;
        if (DataContext is LoginViewModel vm)
        {
            vm.Password = PasswordPlain.Text;
            _syncing = true;
            if (PasswordBox.Password != PasswordPlain.Text) PasswordBox.Password = PasswordPlain.Text;
            _syncing = false;
        }
    }

    private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        if (_isPasswordVisible)
        {
            PasswordPlain.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Collapsed;
            TogglePasswordButton.Content = "🙈";
        }
        else
        {
            PasswordPlain.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;
            TogglePasswordButton.Content = "👁";
        }
    }
}
