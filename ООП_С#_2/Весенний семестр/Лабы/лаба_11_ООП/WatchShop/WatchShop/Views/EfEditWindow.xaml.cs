using System.Globalization;
using System.Windows;
using WatchShop.Entities;
using WatchShop.Services;

namespace WatchShop.Views
{
    /// <summary>
    /// Диалог добавления/редактирования товара для демонстрации EF (лаб 9).
    /// Работает с WatchEntity (Code First сущность), а не с Models.Watch.
    /// </summary>
    public partial class EfEditWindow : Window
    {
        public WatchEntity Entity { get; private set; }
        public string ManufacturerName => ManufacturerBox.Text.Trim();
        public string CategoryName => CategoryBox.Text.Trim();

        private readonly bool isEdit;

        public EfEditWindow(WatchEntity? toEdit = null)
        {
            InitializeComponent();

            isEdit = toEdit != null;
            Entity = toEdit ?? new WatchEntity();

            TitleText.Text = LocalizationService.GetString(
                isEdit ? "Ef_EditTitle" : "Ef_AddTitle");
            Title = TitleText.Text;

            if (isEdit && toEdit != null)
            {
                NameBox.Text = toEdit.ShortName;
                FullNameBox.Text = toEdit.FullName ?? "";
                ManufacturerBox.Text = toEdit.Manufacturer?.Name ?? "";
                CategoryBox.Text = toEdit.Category?.Name ?? "";
                PriceBox.Text = toEdit.Price.ToString(CultureInfo.InvariantCulture);
                QtyBox.Text = toEdit.Quantity.ToString();
                RatingBox.Text = toEdit.Rating.ToString(CultureInfo.InvariantCulture);
                DiscountBox.Text = toEdit.Discount.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                PriceBox.Text = "0";
                QtyBox.Text = "0";
                RatingBox.Text = "0";
                DiscountBox.Text = "0";
            }

            NameBox.Focus();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show(
                    LocalizationService.GetString("Validation_ShortNameRequired"),
                    LocalizationService.GetString("Warning"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                NameBox.Focus();
                return;
            }

            Entity.ShortName = NameBox.Text.Trim();
            Entity.FullName = string.IsNullOrWhiteSpace(FullNameBox.Text)
                ? null : FullNameBox.Text.Trim();

            decimal.TryParse(PriceBox.Text.Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out var price);
            Entity.Price = price;

            int.TryParse(QtyBox.Text, out var qty);
            Entity.Quantity = qty;

            double.TryParse(RatingBox.Text.Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out var rating);
            Entity.Rating = rating;

            double.TryParse(DiscountBox.Text.Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out var disc);
            Entity.Discount = disc;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
