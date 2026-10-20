using System.Windows;
using System.Windows.Controls;

namespace Flux.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Адаптивная боковая панель:
        //   ≥ 1100 — полная (240px)
        //   900–1100 — компактная (180px)
        //   < 900 — узкая (140px)
        // Сами кнопки SideNavButton сжимаются автоматически (без явной ширины).
        if (e.NewSize.Width >= 1100)
            SidebarColumn.Width = new GridLength(240);
        else if (e.NewSize.Width >= 900)
            SidebarColumn.Width = new GridLength(180);
        else
            SidebarColumn.Width = new GridLength(140);
    }
}
