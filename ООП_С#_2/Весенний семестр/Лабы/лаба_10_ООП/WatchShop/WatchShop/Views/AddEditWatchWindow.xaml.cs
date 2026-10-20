using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using WatchShop.Models;
using WatchShop.Services;

namespace WatchShop.Views
{
    public partial class AddEditWatchWindow : Window
    {
        public Watch? Watch { get; private set; }
        private readonly Watch? originalWatch;
        private readonly bool isEditMode;

        // для debounce (задержки логирования)
        private System.Windows.Threading.DispatcherTimer? logTimer;
        private double lastBubbleOld = 0;
        private double lastBubbleNew = 0;
        private bool hasBubble = false;

        private double lastTunnelOld = 0;
        private double lastTunnelNew = 0;
        private bool hasTunnel = false;

        private bool hasDirect = false;
        private double lastDirectValue = 0;

        // Лаб 8: байты загруженной картинки (для записи в Watches.Image BLOB)
        private byte[]? selectedImageBytes;

        public AddEditWatchWindow() : this(null) { }

        public AddEditWatchWindow(Watch? watchToEdit)
        {
            InitializeComponent();

            originalWatch = watchToEdit;
            isEditMode = watchToEdit != null;

            Title = LocalizationService.GetString(isEditMode ? "EditTitle" : "AddTitle");
            TitleTextBlock.Text = Title;

            // подписки
            DiscountControl.PreviewDiscountChanged += OnPreviewDiscountChanged;
            DiscountControl.DiscountChanged += OnDiscountChanged;
            DiscountControl.DiscountApplied += OnDiscountApplied;

            if (isEditMode)
                LoadWatchData(watchToEdit!);
            else
                SetDefaults();
        }

        // ========== логирование ==========

        private void FlushLogs()
        {
            if (hasDirect)
            {
                AddLogToBox($"[DIRECT] скидка активирована (с 0 на {lastDirectValue:F0}%)");
                hasDirect = false;
            }

            if (hasTunnel)
            {
                AddLogToBox($"[TUNNEL] {lastTunnelOld:F0}% → {lastTunnelNew:F0}% (сверху вниз)");
                hasTunnel = false;
            }

            if (hasBubble)
            {
                AddLogToBox($"[BUBBLE] {lastBubbleOld:F0}% → {lastBubbleNew:F0}% (снизу вверх)");
                hasBubble = false;
            }
        }

        private void ScheduleLog()
        {
            if (logTimer != null)
            {
                logTimer.Stop();
                logTimer = null;
            }

            logTimer = new System.Windows.Threading.DispatcherTimer();
            logTimer.Interval = TimeSpan.FromMilliseconds(150);
            logTimer.Tick += (s, e) =>
            {
                logTimer?.Stop();
                logTimer = null;
                FlushLogs();
            };
            logTimer.Start();
        }

        private void AddLogToBox(string message)
        {
            EventLogListBox.Items.Insert(0, $"{DateTime.Now:HH:mm:ss} - {message}");
            if (EventLogListBox.Items.Count > 15)
                EventLogListBox.Items.RemoveAt(15);
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            EventLogListBox.Items.Clear();
            AddLogToBox("лог очищен");
            hasTunnel = false;
            hasBubble = false;
            hasDirect = false;
            if (logTimer != null)
            {
                logTimer.Stop();
                logTimer = null;
            }
        }

        // ========== TUNNEL ==========
        private void OnPreviewDiscountChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // игнорируем изменения меньше 0.5%
            if (Math.Abs(e.OldValue - e.NewValue) < 0.5)
                return;

            lastTunnelOld = e.OldValue;
            lastTunnelNew = e.NewValue;
            hasTunnel = true;
            ScheduleLog();
        }

        // ========== BUBBLE ==========
        private void OnDiscountChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // игнорируем изменения меньше 0.5%
            if (Math.Abs(e.OldValue - e.NewValue) < 0.5)
                return;

            lastBubbleOld = e.OldValue;
            lastBubbleNew = e.NewValue;
            hasBubble = true;
            ScheduleLog();
        }

        // ========== DIRECT ==========
        private void OnDiscountApplied(object sender, RoutedEventArgs e)
        {
            lastDirectValue = DiscountControl.Discount;
            hasDirect = true;
            ScheduleLog();
        }

        // ========== КОМАНДА ПРИМЕНИТЬ СКИДКУ 15% ==========
        private void ApplyRecommendedDiscount_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Math.Abs(DiscountControl.Discount - 15) > 0.01;
            e.Handled = true;
        }

        private void ApplyRecommendedDiscount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DiscountControl.Discount = 15;
            AddLogToBox($"[CMD] установлена скидка 15%");
            e.Handled = true;
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
            RatingControl.Value = 5;
            DiscountControl.Discount = 0;
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
            DiscountControl.Discount = w.Discount;
            InStockCheckBox.IsChecked = w.InStock;

            SelectComboBoxByCanonical(CategoryComboBox, w.Category, FindCategoryKey);
            SelectComboBoxByCanonical(MovementTypeComboBox, w.MovementType, FindMovementKey);
            SelectComboBoxByCanonical(MaterialComboBox, w.Material, FindMaterialKey);
            SelectComboBoxByCanonical(CountryComboBox, w.CountryOfOrigin, FindCountryKey);
            SelectComboBoxByCanonical(ColorComboBox, w.Color, FindColorKey);
            SelectComboBoxByCanonical(GenderComboBox, w.Gender, FindGenderKey);
            SelectComboBoxByCanonical(WaterResistanceComboBox, w.WaterResistance, FindWaterKey);

            RatingControl.Value = w.Rating;

            // Лаб 8: подтягиваем BLOB-фото из БД в превью
            if (w.ImageData != null && w.ImageData.Length > 0)
            {
                selectedImageBytes = w.ImageData;
                PreviewImage.Source = BytesToImage(w.ImageData);
            }
        }

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

            w.Category = GetCanonicalFromTag(CategoryComboBox, WatchTranslator.CanonicalCategory);
            w.MovementType = GetCanonicalFromTag(MovementTypeComboBox, WatchTranslator.CanonicalMovement);
            w.Material = GetCanonicalFromTag(MaterialComboBox, WatchTranslator.CanonicalMaterial);
            w.CountryOfOrigin = GetCanonicalFromTag(CountryComboBox, WatchTranslator.CanonicalCountry);
            w.Color = GetCanonicalFromTag(ColorComboBox, WatchTranslator.CanonicalColor);
            w.Gender = GetCanonicalFromTag(GenderComboBox, WatchTranslator.CanonicalGender);
            w.WaterResistance = GetCanonicalFromTag(WaterResistanceComboBox, WatchTranslator.CanonicalWater);

            w.Rating = RatingControl.Value;
            w.Discount = DiscountControl.Discount;

            bool checkedByUser = InStockCheckBox.IsChecked == true;
            w.InStock = qty > 0 && checkedByUser;

            // Лаб 8: BLOB-фото (записывается в столбец Watches.Image)
            w.ImageData = selectedImageBytes;

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

        // ========== Лаб 8: BLOB-фото ==========

        /// <summary>
        /// Открывает диалог выбора файла, читает картинку в byte[] и
        /// показывает в превью. Эти байты сохранятся в столбец Watches.Image.
        /// </summary>
        private void PickImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Выберите фото товара"
            };
            if (dlg.ShowDialog() != true) return;

            // Защита от слишком больших файлов
            var info = new FileInfo(dlg.FileName);
            if (info.Length > 4 * 1024 * 1024)
            {
                MessageBox.Show("Файл слишком большой (максимум 4 МБ).",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selectedImageBytes = File.ReadAllBytes(dlg.FileName);
            PreviewImage.Source = BytesToImage(selectedImageBytes);
        }

        private void ClearImage_Click(object sender, RoutedEventArgs e)
        {
            selectedImageBytes = null;
            PreviewImage.Source = null;
        }

        /// <summary>
        /// Конвертирует byte[] в BitmapImage для показа в Image-элементе.
        /// MemoryStream + CacheOption=OnLoad + Freeze() — стандартный
        /// рецепт WPF для безопасной загрузки картинки из памяти.
        /// </summary>
        private static System.Windows.Media.Imaging.BitmapImage? BytesToImage(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            try
            {
                var img = new System.Windows.Media.Imaging.BitmapImage();
                using var ms = new MemoryStream(bytes);
                img.BeginInit();
                img.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                img.StreamSource = ms;
                img.EndInit();
                img.Freeze();
                return img;
            }
            catch
            {
                return null;
            }
        }
    }
}