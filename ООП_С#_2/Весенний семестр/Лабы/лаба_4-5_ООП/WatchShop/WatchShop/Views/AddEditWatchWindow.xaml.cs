using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using WatchShop.Models;
using WatchShop.Services;

namespace WatchShop.Views
{
    // окно для добавления и редактирования товара
    public partial class AddEditWatchWindow : Window
    {
        public Watch Watch { get; private set; }

        public AddEditWatchWindow(Watch? watch = null)
        {
            InitializeComponent();
            LocalizationService.LanguageChanged += OnLanguageChanged;

            if (watch != null)
            {
                Watch = watch;
                LoadWatchData();
                UpdateLocalization(true);
            }
            else
            {
                Watch = new Watch
                {
                    Images = new ObservableCollection<string>(),
                    RelatedWatchIds = new ObservableCollection<int>(),
                    Id = new Random().Next(1000, 9999)
                };
                UpdateLocalization(false);
            }
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateLocalization(Watch.Id != 0);
        }

        private void UpdateLocalization(bool isEdit)
        {
            // заголовок окна
            if (isEdit)
            {
                Title = LocalizationService.GetString("EditTitle");
                TitleTextBlock.Text = LocalizationService.GetString("EditTitle");
            }
            else
            {
                Title = LocalizationService.GetString("AddTitle");
                TitleTextBlock.Text = LocalizationService.GetString("AddTitle");
            }

            // левые подписи полей
            ShortNameTextBlock.Text = LocalizationService.GetString("ShortName");
            CategoryTextBlock.Text = LocalizationService.GetString("Category_field");
            PriceTextBlock.Text = LocalizationService.GetString("Price_field");
            QuantityTextBlock.Text = LocalizationService.GetString("Quantity_field");
            ManufacturerTextBlock.Text = LocalizationService.GetString("Manufacturer_field");
            MovementTypeTextBlock.Text = LocalizationService.GetString("MovementType");
            SizeTextBlock.Text = LocalizationService.GetString("Size_field");
            MaterialTextBlock.Text = LocalizationService.GetString("Material_field");

            // правые подписи полей
            FullNameTextBlock.Text = LocalizationService.GetString("FullName");
            CountryTextBlock.Text = LocalizationService.GetString("Country_field");
            ColorTextBlock.Text = LocalizationService.GetString("Color_field");
            RatingTextBlock.Text = LocalizationService.GetString("Rating_field");
            DiscountTextBlock.Text = LocalizationService.GetString("Discount_field");
            WaterResistanceTextBlock.Text = LocalizationService.GetString("WaterResistance_field");
            GenderTextBlock.Text = LocalizationService.GetString("Gender_field");
            InStockTextBlock.Text = LocalizationService.GetString("InStock_field");

            // чекбокс в наличии
            InStockCheckBox.Content = LocalizationService.GetString("InStock_field");

            // описание и изображения
            DescriptionTextBlock.Text = LocalizationService.GetString("Description_field");
            ImagesTextBlock.Text = LocalizationService.GetString("Images_field");
            DragDropHintTextBlock.Text = LocalizationService.GetString("DragDropHint");

            // кнопки
            SaveButton.Content = LocalizationService.GetString("Save");
            CancelButton.Content = LocalizationService.GetString("Cancel");
            AddImageButton.Content = LocalizationService.GetString("AddImage");
            RemoveImageButton.Content = LocalizationService.GetString("RemoveImage");

            // подсказка
            RequiredTextBlock.Text = LocalizationService.GetString("Required");

            // обновляем выпадающие списки
            UpdateComboBoxItems(MovementTypeComboBox,
                new[] { "Механические", "Кварцевые", "Электронные", "Автоматические", "Солнечные" },
                new[] { "MovementType_Mechanical", "MovementType_Quartz", "MovementType_Electronic", "MovementType_Automatic", "MovementType_Solar" });

            UpdateComboBoxItems(MaterialComboBox,
                new[] { "Нержавеющая сталь", "Титан", "Керамика", "Пластик", "Карбон", "Золото", "Серебро" },
                new[] { "Material_Steel", "Material_Titanium", "Material_Ceramic", "Material_Plastic", "Material_Carbon", "Material_Gold", "Material_Silver" });

            UpdateComboBoxItems(GenderComboBox,
                new[] { "Мужские", "Женские", "Унисекс" },
                new[] { "Gender_Men", "Gender_Women", "Gender_Unisex" });

            UpdateComboBoxItems(CountryComboBox,
                new[] { "Швейцария", "Япония", "Германия", "Китай", "Россия", "США", "Франция", "Италия" },
                new[] { "Country_Switzerland", "Country_Japan", "Country_Germany", "Country_China", "Country_Russia", "Country_USA", "Country_France", "Country_Italy" });

            UpdateComboBoxItems(ColorComboBox,
                new[] { "Черный", "Серебристый", "Золотистый", "Синий", "Зеленый", "Красный", "Белый", "Розовый", "Коричневый" },
                new[] { "Color_Black", "Color_Silver", "Color_Gold", "Color_Blue", "Color_Green", "Color_Red", "Color_White", "Color_Pink", "Color_Brown" });

            UpdateComboBoxItems(WaterResistanceComboBox,
                new[] { "30m", "50m", "100m", "200m", "300m", "500m+", "Нет" },
                new[] { "WaterResistance_30m", "WaterResistance_50m", "WaterResistance_100m", "WaterResistance_200m", "WaterResistance_300m", "WaterResistance_500m", "WaterResistance_None" });
        }

        private void UpdateComboBoxItems(ComboBox comboBox, string[] originalValues, string[] translationKeys)
        {
            if (comboBox.Items.Count != originalValues.Length) return;

            string currentSelected = comboBox.Text;

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                var item = comboBox.Items[i] as ComboBoxItem;
                if (item != null)
                {
                    item.Content = LocalizationService.GetString(translationKeys[i]);
                }
            }

            if (!string.IsNullOrEmpty(currentSelected))
            {
                comboBox.Text = currentSelected;
            }
        }

        private void LoadWatchData()
        {
            try
            {
                ShortNameTextBox.Text = Watch.ShortName ?? "";
                FullNameTextBox.Text = Watch.FullName ?? "";
                DescriptionTextBox.Text = Watch.Description ?? "";

                if (!string.IsNullOrEmpty(Watch.Category))
                {
                    foreach (ComboBoxItem item in CategoryComboBox.Items)
                    {
                        if (item.Content.ToString() == Watch.Category)
                        {
                            CategoryComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                PriceTextBox.Text = Watch.Price.ToString();
                QuantityTextBox.Text = Watch.Quantity.ToString();

                if (!string.IsNullOrEmpty(Watch.Manufacturer))
                {
                    ManufacturerComboBox.Text = Watch.Manufacturer;
                }

                if (!string.IsNullOrEmpty(Watch.MovementType))
                {
                    foreach (ComboBoxItem item in MovementTypeComboBox.Items)
                    {
                        if (item.Content.ToString() == Watch.MovementType)
                        {
                            MovementTypeComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Watch.Size))
                {
                    SizeComboBox.Text = Watch.Size;
                }

                if (!string.IsNullOrEmpty(Watch.Material))
                {
                    foreach (ComboBoxItem item in MaterialComboBox.Items)
                    {
                        if (item.Content.ToString() == Watch.Material)
                        {
                            MaterialComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Watch.CountryOfOrigin))
                {
                    foreach (ComboBoxItem item in CountryComboBox.Items)
                    {
                        if (item.Content.ToString() == Watch.CountryOfOrigin)
                        {
                            CountryComboBox.SelectedItem = item;
                            break;
                        }
                    }
                    if (CountryComboBox.SelectedItem == null)
                    {
                        CountryComboBox.Text = Watch.CountryOfOrigin;
                    }
                }

                if (!string.IsNullOrEmpty(Watch.Color))
                {
                    foreach (ComboBoxItem item in ColorComboBox.Items)
                    {
                        if (item.Content.ToString() == Watch.Color)
                        {
                            ColorComboBox.SelectedItem = item;
                            break;
                        }
                    }
                    if (ColorComboBox.SelectedItem == null)
                    {
                        ColorComboBox.Text = Watch.Color;
                    }
                }

                if (Watch.Rating > 0)
                {
                    foreach (ComboBoxItem item in RatingComboBox.Items)
                    {
                        if (item.Content.ToString() == Watch.Rating.ToString("F1"))
                        {
                            RatingComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                DiscountComboBox.Text = Watch.Discount.ToString();

                if (!string.IsNullOrEmpty(Watch.WaterResistance))
                {
                    foreach (ComboBoxItem item in WaterResistanceComboBox.Items)
                    {
                        if (item.Content.ToString() == Watch.WaterResistance)
                        {
                            WaterResistanceComboBox.SelectedItem = item;
                            break;
                        }
                    }
                    if (WaterResistanceComboBox.SelectedItem == null)
                    {
                        WaterResistanceComboBox.Text = Watch.WaterResistance;
                    }
                }

                if (!string.IsNullOrEmpty(Watch.Gender))
                {
                    foreach (ComboBoxItem item in GenderComboBox.Items)
                    {
                        if (item.Content.ToString() == Watch.Gender)
                        {
                            GenderComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                InStockCheckBox.IsChecked = Watch.InStock;

                // загрузка изображений с проверкой на null
                if (Watch.Images != null)
                {
                    foreach (var image in Watch.Images)
                    {
                        if (!string.IsNullOrEmpty(image))
                        {
                            ImagesListBox.Items.Add(image);
                        }
                    }
                }

                // установка основного изображения с проверкой
                if (!string.IsNullOrEmpty(Watch.ImagePath))
                {
                    bool imageExists = false;
                    foreach (var item in ImagesListBox.Items)
                    {
                        if (item?.ToString() == Watch.ImagePath)
                        {
                            imageExists = true;
                            break;
                        }
                    }
                    if (!imageExists)
                    {
                        ImagesListBox.Items.Add(Watch.ImagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void ImageBorder_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void ImageBorder_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    foreach (string file in files)
                    {
                        string extension = Path.GetExtension(file).ToLower();
                        if (extension == ".jpg" || extension == ".jpeg" ||
                            extension == ".png" || extension == ".gif" ||
                            extension == ".bmp")
                        {
                            ImagesListBox.Items.Add(file);
                            if (Watch.Images == null) Watch.Images = new ObservableCollection<string>();
                            Watch.Images.Add(file);
                            if (string.IsNullOrEmpty(Watch.ImagePath)) Watch.ImagePath = file;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка при загрузке изображения: {ex.Message}");
            }
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    ImagesListBox.Items.Add(file);
                    if (Watch.Images == null) Watch.Images = new ObservableCollection<string>();
                    Watch.Images.Add(file);
                    if (string.IsNullOrEmpty(Watch.ImagePath)) Watch.ImagePath = file;
                }
            }
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImagesListBox.SelectedItem != null)
            {
                string? selectedImage = ImagesListBox.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(selectedImage))
                {
                    ImagesListBox.Items.Remove(selectedImage);
                    if (Watch.Images != null && Watch.Images.Contains(selectedImage))
                    {
                        Watch.Images.Remove(selectedImage);
                    }
                    if (Watch.ImagePath == selectedImage)
                    {
                        Watch.ImagePath = ImagesListBox.Items.Count > 0 ? ImagesListBox.Items[0]?.ToString() ?? "" : "";
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ShortNameTextBox.Text))
                {
                    MessageBox.Show(LocalizationService.GetString("ShortName") + " - " +
                                   (LocalizationService.CurrentLanguage == "ru-RU" ? "обязательное поле" : "required field"));
                    ShortNameTextBox.Focus();
                    return;
                }

                Watch.ShortName = ShortNameTextBox.Text;
                Watch.FullName = FullNameTextBox.Text ?? "";
                Watch.Description = DescriptionTextBox.Text ?? "";

                if (CategoryComboBox.SelectedItem != null)
                {
                    Watch.Category = ((ComboBoxItem)CategoryComboBox.SelectedItem).Content.ToString() ?? "";
                }

                if (decimal.TryParse(PriceTextBox.Text, out decimal price))
                {
                    Watch.Price = price;
                }

                if (int.TryParse(QuantityTextBox.Text, out int quantity))
                {
                    Watch.Quantity = quantity;
                }

                Watch.Manufacturer = ManufacturerComboBox.Text ?? "";

                if (MovementTypeComboBox.SelectedItem != null)
                {
                    Watch.MovementType = ((ComboBoxItem)MovementTypeComboBox.SelectedItem).Content.ToString() ?? "";
                }

                Watch.Size = SizeComboBox.Text ?? "";

                if (MaterialComboBox.SelectedItem != null)
                {
                    Watch.Material = ((ComboBoxItem)MaterialComboBox.SelectedItem).Content.ToString() ?? "";
                }
                else if (!string.IsNullOrEmpty(MaterialComboBox.Text))
                {
                    Watch.Material = MaterialComboBox.Text;
                }

                if (CountryComboBox.SelectedItem != null)
                {
                    Watch.CountryOfOrigin = ((ComboBoxItem)CountryComboBox.SelectedItem).Content.ToString() ?? "";
                }
                else if (!string.IsNullOrEmpty(CountryComboBox.Text))
                {
                    Watch.CountryOfOrigin = CountryComboBox.Text;
                }

                if (ColorComboBox.SelectedItem != null)
                {
                    Watch.Color = ((ComboBoxItem)ColorComboBox.SelectedItem).Content.ToString() ?? "";
                }
                else if (!string.IsNullOrEmpty(ColorComboBox.Text))
                {
                    Watch.Color = ColorComboBox.Text;
                }

                if (RatingComboBox.SelectedItem != null)
                {
                    if (double.TryParse(((ComboBoxItem)RatingComboBox.SelectedItem).Content.ToString(), out double rating))
                    {
                        Watch.Rating = rating;
                    }
                }

                if (double.TryParse(DiscountComboBox.Text, out double discount))
                {
                    Watch.Discount = discount;
                }

                if (WaterResistanceComboBox.SelectedItem != null)
                {
                    Watch.WaterResistance = ((ComboBoxItem)WaterResistanceComboBox.SelectedItem).Content.ToString() ?? "";
                }
                else if (!string.IsNullOrEmpty(WaterResistanceComboBox.Text))
                {
                    Watch.WaterResistance = WaterResistanceComboBox.Text;
                }

                if (GenderComboBox.SelectedItem != null)
                {
                    Watch.Gender = ((ComboBoxItem)GenderComboBox.SelectedItem).Content.ToString() ?? "";
                }

                Watch.InStock = InStockCheckBox.IsChecked ?? true;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка при сохранении: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            LocalizationService.LanguageChanged -= OnLanguageChanged;
            base.OnClosed(e);
        }
    }
}