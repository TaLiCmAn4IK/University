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
        /// Загружает свой курсор из ресурсов приложения.
        /// </summary>
        private void LoadCustomCursor()
        {
            try
            {
                string cursorPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Resources", "watch_cursor.cur");

                Cursor = File.Exists(cursorPath)
                    ? new Cursor(cursorPath)
                    : Cursors.Arrow;
            }
            catch
            {
                Cursor = Cursors.Arrow;
            }
        }
    }
}
