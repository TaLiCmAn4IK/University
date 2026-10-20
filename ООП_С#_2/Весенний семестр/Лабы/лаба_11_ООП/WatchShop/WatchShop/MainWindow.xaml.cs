using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WatchShop.ViewModels;

namespace WatchShop
{
    /// <summary>
    /// Главное окно. Логика полностью в MainViewModel — здесь только
    /// загрузка кастомного курсора и применение фильтров при старте.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LoadCustomCursor();

            // Применяем стартовые фильтры (показать все товары отсортированными)
            Loaded += (_, _) =>
            {
                if (DataContext is MainViewModel vm)
                    vm.ApplyFiltersCommand.Execute(null);
            };
        }

        /// <summary>
        /// Загружает свой курсор из встроенных ресурсов (pack URI).
        /// </summary>
        private void LoadCustomCursor()
        {
            try
            {
                var streamInfo = Application.GetResourceStream(
                    new Uri("/WatchShop;component/Resources/watch_cursor.cur", UriKind.Relative));

                if (streamInfo != null)
                    Cursor = new Cursor(streamInfo.Stream);
                else
                    Cursor = Cursors.Arrow;
            }
            catch
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void ShowAbout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // Команда всегда доступна
            e.CanExecute = true;
            e.Handled = true;
        }

        private void ShowAbout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Делегируем во ViewModel — там уже есть готовый метод ShowAbout
            if (DataContext is MainViewModel vm && vm.ShowAboutCommand.CanExecute(null))
                vm.ShowAboutCommand.Execute(null);
            e.Handled = true;
        }
    }
}
