using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Flux.ViewModels;

namespace Flux.Views;

public partial class AdminView : UserControl
{
    public AdminView() { InitializeComponent(); }

    /// <summary>
    /// Клик по ячейке в гриде пользователей: подставляем её значение в фильтр.
    /// Кнопки (Ban) и шаблонные колонки (Status) игнорируем.
    /// </summary>
    private void UsersGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not AdminViewModel vm) return;

        var src = e.OriginalSource as DependencyObject;
        DataGridCell? cell = null;
        while (src != null)
        {
            if (src is Button) return;
            if (src is DataGridCell c) { cell = c; break; }
            src = System.Windows.Media.VisualTreeHelper.GetParent(src);
        }
        if (cell == null) return;

        // Шаблонные колонки (Status, Action) пропускаем.
        if (cell.Column is not DataGridTextColumn tc) return;
        if (cell.DataContext is not AdminUserRow row) return;

        var path = (tc.Binding as System.Windows.Data.Binding)?.Path?.Path;
        string? value = path switch
        {
            "Nickname"   => row.Nickname,
            "EloCS2"     => row.EloCS2.ToString(),
            "EloChess"   => row.EloChess.ToString(),
            "RoleLabel"  => row.RoleLabel,
            _ => null
        };
        if (!string.IsNullOrEmpty(value))
            vm.UserFilter = value;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

    }
}
