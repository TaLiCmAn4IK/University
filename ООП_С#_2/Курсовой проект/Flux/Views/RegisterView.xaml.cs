using System.Windows;
using System.Windows.Controls;
using Flux.ViewModels;

namespace Flux.Views;

public partial class RegisterView : UserControl
{
    private bool _syncingPwd, _syncingConfirm;
    private bool _pwdVisible, _confirmVisible;

    public RegisterView() { InitializeComponent(); }

    // Поле пароля ------------------------------------------------------

    private void Pwd_Changed(object sender, RoutedEventArgs e)
    {
        if (_syncingPwd) return;
        if (DataContext is RegisterViewModel vm && sender is PasswordBox pb)
        {
            vm.Password = pb.Password;
            _syncingPwd = true;
            if (PasswordPlain.Text != pb.Password) PasswordPlain.Text = pb.Password;
            _syncingPwd = false;
        }
    }

    private void PasswordPlain_Changed(object sender, TextChangedEventArgs e)
    {
        if (_syncingPwd) return;
        if (DataContext is RegisterViewModel vm)
        {
            vm.Password = PasswordPlain.Text;
            _syncingPwd = true;
            if (PasswordBox.Password != PasswordPlain.Text) PasswordBox.Password = PasswordPlain.Text;
            _syncingPwd = false;
        }
    }

    private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
    {
        _pwdVisible = !_pwdVisible;
        PasswordPlain.Visibility = _pwdVisible ? Visibility.Visible : Visibility.Collapsed;
        PasswordBox.Visibility = _pwdVisible ? Visibility.Collapsed : Visibility.Visible;
        TogglePasswordButton.Content = _pwdVisible ? "🙈" : "👁";
    }

    // Подтверждение пароля ----------------------------------------------------

    private void PwdConfirm_Changed(object sender, RoutedEventArgs e)
    {
        if (_syncingConfirm) return;
        if (DataContext is RegisterViewModel vm && sender is PasswordBox pb)
        {
            vm.PasswordConfirm = pb.Password;
            _syncingConfirm = true;
            if (PasswordConfirmPlain.Text != pb.Password) PasswordConfirmPlain.Text = pb.Password;
            _syncingConfirm = false;
        }
    }

    private void PasswordConfirmPlain_Changed(object sender, TextChangedEventArgs e)
    {
        if (_syncingConfirm) return;
        if (DataContext is RegisterViewModel vm)
        {
            vm.PasswordConfirm = PasswordConfirmPlain.Text;
            _syncingConfirm = true;
            if (PasswordBoxConfirm.Password != PasswordConfirmPlain.Text)
                PasswordBoxConfirm.Password = PasswordConfirmPlain.Text;
            _syncingConfirm = false;
        }
    }

    private void ToggleConfirmPasswordVisibility(object sender, RoutedEventArgs e)
    {
        _confirmVisible = !_confirmVisible;
        PasswordConfirmPlain.Visibility = _confirmVisible ? Visibility.Visible : Visibility.Collapsed;
        PasswordBoxConfirm.Visibility = _confirmVisible ? Visibility.Collapsed : Visibility.Visible;
        ToggleConfirmPasswordButton.Content = _confirmVisible ? "🙈" : "👁";
    }
}
