using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WatchShop.Business;
using WatchShop.Entities;
using WatchShop.Services;

namespace WatchShop.Views
{
    /// <summary>
    /// Окно демонстрации EF Core + многоуровневой архитектуры (лаб 9-10).
    ///
    /// UI-слой. НЕ обращается к EF/DbContext напрямую — работает только
    /// через Business Layer (WatchService). Бизнес-слой, в свою очередь,
    /// использует Data Access Layer (UnitOfWork + Repository).
    /// Это демонстрирует паттерны Repository и Unit of Work и слабую
    /// связанность между слоями.
    /// </summary>
    public partial class EfDemoWindow : Window
    {
        // UI знает только про бизнес-сервис, не про репозитории и не про EF
        private readonly WatchService service = new();

        public EfDemoWindow()
        {
            InitializeComponent();
            Loaded += async (_, _) => await LoadAllAsync();
        }

        private async Task LoadAllAsync()
        {
            try
            {
                StatusText.Text = LocalizationService.GetString("Ef_Loading");
                var list = await service.GetAllAsync();
                WatchesGrid.ItemsSource = list;
                StatusText.Text = string.Format(
                    LocalizationService.GetString("Ef_LoadedCount"), list.Count);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // ---------- Поиск / фильтр / сортировка (LINQ to Entities) ----------
        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal? min = TryDecimal(MinPriceBox.Text);
                decimal? max = TryDecimal(MaxPriceBox.Text);
                bool inStock = InStockCheck.IsChecked == true;

                string sortBy = "Name";
                if (SortBox.SelectedItem is ComboBoxItem item && item.Tag is string tag)
                    sortBy = tag;

                StatusText.Text = LocalizationService.GetString("Ef_Searching");
                var result = await service.SearchAsync(
                    SearchBox.Text, min, max, inStock, sortBy);

                WatchesGrid.ItemsSource = result;
                StatusText.Text = string.Format(
                    LocalizationService.GetString("Ef_FoundCount"), result.Count);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // ---------- CREATE ----------
        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new EfEditWindow { Owner = this };
            if (dlg.ShowDialog() != true) return;

            try
            {
                int newId = await service.AddAsync(
                    dlg.Entity, dlg.ManufacturerName, dlg.CategoryName);
                StatusText.Text = string.Format(
                    LocalizationService.GetString("Ef_Added"), newId);
                await LoadAllAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // ---------- UPDATE ----------
        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (WatchesGrid.SelectedItem is not WatchEntity selected)
            {
                MessageBox.Show(
                    LocalizationService.GetString("Ef_SelectRow"),
                    LocalizationService.GetString("Warning"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // подгружаем свежую копию с навигациями
            var full = await service.GetByIdAsync(selected.Id);
            if (full == null) return;

            var dlg = new EfEditWindow(full) { Owner = this };
            if (dlg.ShowDialog() != true) return;

            try
            {
                await service.UpdateAsync(
                    dlg.Entity, dlg.ManufacturerName, dlg.CategoryName);
                StatusText.Text = LocalizationService.GetString("Ef_Updated");
                await LoadAllAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // ---------- DELETE ----------
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (WatchesGrid.SelectedItem is not WatchEntity selected)
            {
                MessageBox.Show(
                    LocalizationService.GetString("Ef_SelectRow"),
                    LocalizationService.GetString("Warning"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                string.Format(LocalizationService.GetString("Ef_ConfirmDelete"), selected.ShortName),
                LocalizationService.GetString("Confirm_Title"),
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                await service.DeleteAsync(selected.Id);
                StatusText.Text = LocalizationService.GetString("Ef_Deleted");
                await LoadAllAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
            => await LoadAllAsync();

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        // ---------- helpers ----------
        private static decimal? TryDecimal(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            return decimal.TryParse(s.Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out var v)
                ? v : (decimal?)null;
        }

        private void ShowError(Exception ex)
        {
            StatusText.Text = LocalizationService.GetString("Ef_Error") + " " + ex.Message;
            MessageBox.Show(ex.Message,
                LocalizationService.GetString("Ef_Error"),
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
