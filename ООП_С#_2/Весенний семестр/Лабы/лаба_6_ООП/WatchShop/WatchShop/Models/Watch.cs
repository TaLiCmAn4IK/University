using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WatchShop.Models
{
    // основная модель данных товара
    public class Watch : INotifyPropertyChanged
    {
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
        private bool inStock;
        private ObservableCollection<int> relatedWatchIds = new();
        private int soldCount;
        private string manufacturer = "";
        private string movementType = "";
        private string waterResistance = "";
        private string material = "";
        private string gender = "";
        private string imagePath = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id
        {
            get => id;
            set { id = value; OnPropertyChanged(); }
        }

        public string ShortName
        {
            get => shortName;
            set { shortName = value; OnPropertyChanged(); }
        }

        public string FullName
        {
            get => fullName;
            set { fullName = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Images
        {
            get => images;
            set { images = value; OnPropertyChanged(); }
        }

        public string Category
        {
            get => category;
            set { category = value; OnPropertyChanged(); }
        }

        public double Rating
        {
            get => rating;
            set { rating = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get => price;
            set { price = value; OnPropertyChanged(); }
        }

        public int Quantity
        {
            get => quantity;
            set { quantity = value; OnPropertyChanged(); }
        }

        public string Color
        {
            get => color;
            set { color = value; OnPropertyChanged(); }
        }

        public string Size
        {
            get => size;
            set { size = value; OnPropertyChanged(); }
        }

        public string CountryOfOrigin
        {
            get => countryOfOrigin;
            set { countryOfOrigin = value; OnPropertyChanged(); }
        }

        public double Discount
        {
            get => discount;
            set { discount = value; OnPropertyChanged(); }
        }

        public bool InStock
        {
            get => inStock;
            set { inStock = value; OnPropertyChanged(); }
        }

        public ObservableCollection<int> RelatedWatchIds
        {
            get => relatedWatchIds;
            set { relatedWatchIds = value; OnPropertyChanged(); }
        }

        public int SoldCount
        {
            get => soldCount;
            set { soldCount = value; OnPropertyChanged(); }
        }

        public string Manufacturer
        {
            get => manufacturer;
            set { manufacturer = value; OnPropertyChanged(); }
        }

        public string MovementType
        {
            get => movementType;
            set { movementType = value; OnPropertyChanged(); }
        }

        public string WaterResistance
        {
            get => waterResistance;
            set { waterResistance = value; OnPropertyChanged(); }
        }

        public string Material
        {
            get => material;
            set { material = value; OnPropertyChanged(); }
        }

        public string Gender
        {
            get => gender;
            set { gender = value; OnPropertyChanged(); }
        }

        public string ImagePath
        {
            get => imagePath;
            set { imagePath = value; OnPropertyChanged(); }
        }

        // цена с учетом скидки
        public decimal DiscountedPrice => Price * (1 - (decimal)Discount / 100);

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}