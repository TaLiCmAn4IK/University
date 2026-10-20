using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using WatchShop.Services;

namespace WatchShop.Models
{
    /// <summary>
    /// Основная модель товара.
    /// Свойства (Category, Material, Color, …) хранятся в JSON на каноническом
    /// русском, а отображение идёт через *Display-свойства, которые
    /// прогоняют значение через WatchTranslator под текущий язык.
    /// </summary>
    public class Watch : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int id;
        private string shortName = "";
        private string fullName = "";
        private string description = "";
        private ObservableCollection<string> images = new();
        private string category = "";
        private double rating;
        private decimal price;
        private int quantity;
        private string color = "";
        private string size = "";
        private string countryOfOrigin = "";
        private double discount;
        private bool inStock = true;
        private ObservableCollection<int> relatedWatchIds = new();
        private int soldCount;
        private string manufacturer = "";
        private string movementType = "";
        private string waterResistance = "";
        private string material = "";
        private string gender = "";
        private string imagePath = "";
        private byte[]? imageData;

        public int Id { get => id; set { id = value; OnPropertyChanged(); } }
        public string ShortName { get => shortName; set { shortName = value; OnPropertyChanged(); } }
        public string FullName { get => fullName; set { fullName = value; OnPropertyChanged(); } }
        public string Description { get => description; set { description = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Images { get => images; set { images = value; OnPropertyChanged(); } }

        public string Category
        {
            get => category;
            set { category = value; OnPropertyChanged(); OnPropertyChanged(nameof(CategoryDisplay)); }
        }
        public double Rating { get => rating; set { rating = value; OnPropertyChanged(); } }
        public decimal Price
        {
            get => price;
            set { price = value; OnPropertyChanged(); OnPropertyChanged(nameof(DiscountedPrice)); }
        }
        public int Quantity
        {
            get => quantity;
            set
            {
                quantity = value; OnPropertyChanged();
                OnPropertyChanged(nameof(AvailableText));
            }
        }
        public string Color
        {
            get => color;
            set { color = value; OnPropertyChanged(); OnPropertyChanged(nameof(ColorDisplay)); }
        }
        public string Size { get => size; set { size = value; OnPropertyChanged(); } }
        public string CountryOfOrigin
        {
            get => countryOfOrigin;
            set { countryOfOrigin = value; OnPropertyChanged(); OnPropertyChanged(nameof(CountryDisplay)); }
        }
        public double Discount
        {
            get => discount;
            set { discount = value; OnPropertyChanged(); OnPropertyChanged(nameof(DiscountedPrice)); }
        }
        public bool InStock
        {
            get => inStock;
            set { inStock = value; OnPropertyChanged(); OnPropertyChanged(nameof(InStockDisplay)); }
        }
        public ObservableCollection<int> RelatedWatchIds
        {
            get => relatedWatchIds;
            set { relatedWatchIds = value; OnPropertyChanged(); }
        }
        public int SoldCount { get => soldCount; set { soldCount = value; OnPropertyChanged(); } }
        public string Manufacturer { get => manufacturer; set { manufacturer = value; OnPropertyChanged(); } }
        public string MovementType
        {
            get => movementType;
            set { movementType = value; OnPropertyChanged(); OnPropertyChanged(nameof(MovementTypeDisplay)); }
        }
        public string WaterResistance
        {
            get => waterResistance;
            set { waterResistance = value; OnPropertyChanged(); OnPropertyChanged(nameof(WaterResistanceDisplay)); }
        }
        public string Material
        {
            get => material;
            set { material = value; OnPropertyChanged(); OnPropertyChanged(nameof(MaterialDisplay)); }
        }
        public string Gender
        {
            get => gender;
            set { gender = value; OnPropertyChanged(); OnPropertyChanged(nameof(GenderDisplay)); }
        }
        public string ImagePath { get => imagePath; set { imagePath = value; OnPropertyChanged(); } }

        /// <summary>
        /// Содержимое поля Image (VARBINARY(MAX)) из БД — двоичные данные
        /// картинки. Заполняется в WatchRepository.MapWatch при чтении и
        /// записывается через AddWatchParameters при Insert/Update.
        /// [JsonIgnore] — в старый JSON не сериализуем (для миграции
        /// это значение не нужно, фотки на тот момент были только в ImagePath).
        /// </summary>
        [JsonIgnore]
        public byte[]? ImageData
        {
            get => imageData;
            set { imageData = value; OnPropertyChanged(); }
        }

        // ---------- Вычисляемые свойства (не сериализуем в JSON) ----------

        [JsonIgnore] public decimal DiscountedPrice => Price * (1 - (decimal)Discount / 100m);

        [JsonIgnore] public string CategoryDisplay => WatchTranslator.Category(category);
        [JsonIgnore] public string MovementTypeDisplay => WatchTranslator.Movement(movementType);
        [JsonIgnore] public string MaterialDisplay => WatchTranslator.Material(material);
        [JsonIgnore] public string GenderDisplay => WatchTranslator.Gender(gender);
        [JsonIgnore] public string CountryDisplay => WatchTranslator.Country(countryOfOrigin);
        [JsonIgnore] public string ColorDisplay => WatchTranslator.Color(color);
        [JsonIgnore] public string WaterResistanceDisplay => WatchTranslator.WaterResistance(waterResistance);

        [JsonIgnore]
        public string InStockDisplay => InStock
            ? LocalizationService.GetString("Yes")
            : LocalizationService.GetString("No");

        [JsonIgnore]
        public string AvailableText => string.Format(
            LocalizationService.GetString("Card_Available"), Quantity);

        /// <summary>
        /// Уведомить View, что все локализованные свойства поменялись
        /// (вызывается при смене языка).
        /// </summary>
        public void RefreshLocalization()
        {
            OnPropertyChanged(nameof(CategoryDisplay));
            OnPropertyChanged(nameof(MovementTypeDisplay));
            OnPropertyChanged(nameof(MaterialDisplay));
            OnPropertyChanged(nameof(GenderDisplay));
            OnPropertyChanged(nameof(CountryDisplay));
            OnPropertyChanged(nameof(ColorDisplay));
            OnPropertyChanged(nameof(WaterResistanceDisplay));
            OnPropertyChanged(nameof(InStockDisplay));
            OnPropertyChanged(nameof(AvailableText));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
