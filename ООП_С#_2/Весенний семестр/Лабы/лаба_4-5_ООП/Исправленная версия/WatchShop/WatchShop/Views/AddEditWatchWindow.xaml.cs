using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WatchShop.Models;
using WatchShop.Services;

namespace WatchShop.Views
{
    /// <summary>
    /// Окно добавления и редактирования товара.
    /// Если в конструктор передан Watch — режим редактирования (поля заполняются),
    /// если передан null или пустой конструктор — режим добавления (создаётся новый Watch).
    /// </summary>
    public partial class AddEditWatchWindow : Window
    {
        /// <summary>Создаваемый или редактируемый товар. Null до нажатия Save.</summary>
        public Watch? Watch { get; private set; }

        private readonly Watch? originalWatch;
        private readonly bool isEditMode;

        public AddEditWatchWindow() : this(null) { }

        public AddEditWatchWindow(Watch? watchToEdit)
        {
            InitializeComponent();

            originalWatch = watchToEdit;
            isEditMode = watchToEdit != null;

            // переключаем заголовок окна
            Title = LocalizationService.GetString(isEditMode ? "EditTitle" : "AddTitle");
            TitleTextBlock.Text = Title;

            if (isEditMode)
                LoadWatchData(watchToEdit!);
            else
                SetDefaults();
        }

        // ========== Загрузка данных ==========

        private void SetDefaults()
        {
            CategoryComboBox.SelectedIndex = 0;
            MovementTypeComboBox.SelectedIndex = 0;
            MaterialComboBox.SelectedIndex = 0;
            CountryComboBox.SelectedIndex = 0;
            ColorComboBox.SelectedIndex = 0;
            GenderComboBox.SelectedIndex = 0;
            WaterResistanceComboBox.SelectedIndex = 0;
            RatingComboBox.SelectedIndex = 0;
            DiscountComboBox.Text = "0";
        }

        private void LoadWatchData(Watch w)
        {
            ShortNameTextBox.Text = w.ShortName;
            FullNameTextBox.Text = w.FullName;
            DescriptionTextBox.Text = w.Description;
            PriceTextBox.Text = w.Price.ToString("F2");
            QuantityTextBox.Text = w.Quantity.ToString();
            ManufacturerComboBox.Text = w.Manufacturer;
            SizeComboBox.Text = w.Size;
            DiscountComboBox.Text = w.Discount.ToString();
            InStockCheckBox.IsChecked = w.InStock;

            // Поля с Tag — выбираем по канонически-русскому значению
            SelectComboBoxByCanonical(CategoryComboBox, w.Category, FindCategoryKey);
            SelectComboBoxByCanonical(MovementTypeComboBox, w.MovementType, FindMovementKey);
            SelectComboBoxByCanonical(MaterialComboBox, w.Material, FindMaterialKey);
            SelectComboBoxByCanonical(CountryComboBox, w.CountryOfOrigin, FindCountryKey);
            SelectComboBoxByCanonical(ColorComboBox, w.Color, FindColorKey);
            SelectComboBoxByCanonical(GenderComboBox, w.Gender, FindGenderKey);
            SelectComboBoxByCanonical(WaterResistanceComboBox, w.WaterResistance, FindWaterKey);

            // Рейтинг — сравниваем по строке
            foreach (var item in RatingComboBox.Items.OfType<ComboBoxItem>())
            {
                if (item.Content?.ToString() == w.Rating.ToString("F1"))
                {
                    RatingComboBox.SelectedItem = item;
                    break;
                }
            }

            // Картинки
            foreach (var img in w.Images)
                ImagesListBox.Items.Add(img);
        }

        /// <summary>
        /// Выбирает в ComboBox элемент, у которого Tag совпадает с ключом,
        /// найденным по каноническому русскому значению.
        /// </summary>
        private static void SelectComboBoxByCanonical(
            ComboBox cb, string? canonicalValue, Func<string, string?> finder)
        {
            if (string.IsNullOrEmpty(canonicalValue)) return;
            string? key = finder(canonicalValue);
            if (key == null) return;

            foreach (var item in cb.Items.OfType<ComboBoxItem>())
            {
                if (item.Tag is string tag && tag == key)
                {
                    cb.SelectedItem = item;
                    return;
                }
            }
        }

        // обратные поиски: каноническое русское значение -> ключ ResourceDictionary
        private static string? FindCategoryKey(string ru) =>
            FindKeyFor(ru, WatchTranslator.CategoryKeys, WatchTranslator.CanonicalCategory);
        private static string? FindMovementKey(string ru) =>
            FindKeyFor(ru, WatchTranslator.MovementKeys, WatchTranslator.CanonicalMovement);
        private static string? FindMaterialKey(string ru) =>
            FindKeyFor(ru, WatchTranslator.MaterialKeys, WatchTranslator.CanonicalMaterial);
        private static string? FindCountryKey(string ru) =>
            FindKeyFor(ru, WatchTranslator.CountryKeys, WatchTranslator.CanonicalCountry);
        private static string? FindColorKey(string ru) =>
            FindKeyFor(ru, WatchTranslator.ColorKeys, WatchTranslator.CanonicalColor);
        private static string? FindGenderKey(string ru) =>
            FindKeyFor(ru, WatchTranslator.GenderKeys, WatchTranslator.CanonicalGender);
        private static string? FindWaterKey(string ru) =>
            FindKeyFor(ru, WatchTranslator.WaterKeys, WatchTranslator.CanonicalWater);

        private static string? FindKeyFor(
            string canonical,
            System.Collections.Generic.IEnumerable<string> keys,
            Func<string, string> reverse)
        {
            foreach (var key in keys)
                if (reverse(key) == canonical)
                    return key;
            return null;
        }

        // ========== Картинки ==========

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Все файлы|*.*",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var path in dialog.FileNames)
                    ImagesListBox.Items.Add(path);
            }
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImagesListBox.SelectedItem != null)
                ImagesListBox.Items.Remove(ImagesListBox.SelectedItem);
        }

        private void ImageBorder_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private void ImageBorder_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string[] allowed = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

            foreach (var f in files)
            {
                if (allowed.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    ImagesListBox.Items.Add(f);
            }
        }

        // ========== Сохранение ==========

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ShortNameTextBox.Text))
            {
                MessageBox.Show(
                    LocalizationService.GetString("Validation_ShortNameRequired"),
                    LocalizationService.GetString("Warning"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ShortNameTextBox.Focus();
                return;
            }

            // Если редактируем — обновляем оригинал, чтобы биндинги в основном окне сохранились
            var w = isEditMode ? originalWatch! : new Watch { Id = GenerateId() };

            w.ShortName = ShortNameTextBox.Text.Trim();
            w.FullName = FullNameTextBox.Text.Trim();
            w.Description = DescriptionTextBox.Text.Trim();

            decimal.TryParse(PriceTextBox.Text, out var price);
            w.Price = price;

            int.TryParse(QuantityTextBox.Text, out var qty);
            w.Quantity = qty;

            w.Manufacturer = ManufacturerComboBox.Text;
            w.Size = SizeComboBox.Text;

            // Tag-комбобоксы: SelectedValue = ключ ResourceDictionary, превращаем в канонический русский
            w.Category = GetCanonicalFromTag(CategoryComboBox, WatchTranslator.CanonicalCategory);
            w.MovementType = GetCanonicalFromTag(MovementTypeComboBox, WatchTranslator.CanonicalMovement);
            w.Material = GetCanonicalFromTag(MaterialComboBox, WatchTranslator.CanonicalMaterial);
            w.CountryOfOrigin = GetCanonicalFromTag(CountryComboBox, WatchTranslator.CanonicalCountry);
            w.Color = GetCanonicalFromTag(ColorComboBox, WatchTranslator.CanonicalColor);
            w.Gender = GetCanonicalFromTag(GenderComboBox, WatchTranslator.CanonicalGender);
            w.WaterResistance = GetCanonicalFromTag(WaterResistanceComboBox, WatchTranslator.CanonicalWater);

            if (RatingComboBox.SelectedItem is ComboBoxItem rItem &&
                double.TryParse(rItem.Content?.ToString(), out var rating))
            {
                w.Rating = rating;
            }

            double.TryParse(DiscountComboBox.Text, out var disc);
            w.Discount = disc;

            // InStock синхронизируем с Quantity:
            // если количество > 0 — товар точно в наличии,
            // если 0 — точно нет, не зависимо от чекбокса.
            // Чекбокс используется только когда Quantity > 0 (можно вручную скрыть товар).
            bool checkedByUser = InStockCheckBox.IsChecked == true;
            w.InStock = qty > 0 && checkedByUser;

            // картинки
            w.Images.Clear();
            foreach (var item in ImagesListBox.Items)
                if (item is string s) w.Images.Add(s);

            if (string.IsNullOrEmpty(w.ImagePath) && w.Images.Count > 0)
                w.ImagePath = w.Images[0];

            Watch = w;
            DialogResult = true;
            Close();
        }

        private static string GetCanonicalFromTag(ComboBox cb, Func<string, string> reverse)
        {
            if (cb.SelectedValue is string key && !string.IsNullOrEmpty(key))
                return reverse(key);
            return "";
        }

        private static int GenerateId() => (int)(DateTime.Now.Ticks % int.MaxValue);

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
