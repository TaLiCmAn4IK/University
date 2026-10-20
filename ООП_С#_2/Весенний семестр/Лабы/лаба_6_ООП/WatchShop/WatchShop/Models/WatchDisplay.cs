using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WatchShop.Models
{
    // обертка для watch с поддержкой локализации и форматирования
    public class WatchDisplay : INotifyPropertyChanged
    {
        private Watch _watch;

        public WatchDisplay(Watch watch)
        {
            _watch = watch;
        }

        public Watch Watch => _watch;

        // основные свойства (не переводятся)
        public string ShortName => _watch.ShortName ?? "";
        public string FullName => _watch.FullName ?? "";
        public decimal Price => _watch.Price;
        public double Rating => _watch.Rating;
        public string Manufacturer => _watch.Manufacturer ?? "";
        public string ImagePath => _watch.ImagePath ?? "";
        public string StockWarning => Quantity > 0 && Quantity < 5 ? "осталось мало" : "";

        // количество и наличие
        public int Quantity
        {
            get => _watch.Quantity;
            set
            {
                _watch.Quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(InStock));
                OnPropertyChanged(nameof(InStockDisplay));
                OnPropertyChanged(nameof(AvailableText));
            }
        }

        public bool InStock
        {
            get => _watch.InStock;
            set
            {
                _watch.InStock = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(InStockDisplay));
            }
        }

        // локализованные тексты для карточек
        public string PriceLabel => Services.LocalizationService.GetString("Price_Label");
        public string InStockLabel => Services.LocalizationService.GetString("InStock");
        public string DoubleClickHint => Services.LocalizationService.GetString("DoubleClickHint");
        public string OutOfStockLabel => Services.LocalizationService.GetString("OutOfStock");
        public string OrderButtonText => Services.LocalizationService.GetString("Order");
        public string EditText => Services.LocalizationService.GetString("Edit");
        public string DeleteText => Services.LocalizationService.GetString("Delete");
        public string AvailableText => string.Format(Services.LocalizationService.GetString("Available"), Quantity);

        // отображение наличия (да/нет или yes/no)
        public string InStockDisplay => InStock ?
            (Services.LocalizationService.CurrentLanguage == "ru-RU" ? "Да" : "Yes") :
            (Services.LocalizationService.CurrentLanguage == "ru-RU" ? "Нет" : "No");

        // категория (переводится)
        public string Category
        {
            get
            {
                string category = _watch.Category ?? "";
                if (Services.LocalizationService.CurrentLanguage == "en-US")
                {
                    return category switch
                    {
                        "Люксовые" => "Luxury",
                        "Спортивные" => "Sport",
                        "Смарт-часы" => "Smart watches",
                        "Классические" => "Classic",
                        "Дайверские" => "Diver",
                        "Авиационные" => "Aviation",
                        "Модные" => "Fashion",
                        _ => category
                    };
                }
                return category;
            }
        }

        // тип механизма (переводится)
        public string MovementType
        {
            get
            {
                string movement = _watch.MovementType ?? "";
                if (Services.LocalizationService.CurrentLanguage == "en-US")
                {
                    return movement switch
                    {
                        "Механические" => "Mechanical",
                        "Кварцевые" => "Quartz",
                        "Электронные" => "Electronic",
                        "Автоматические" => "Automatic",
                        "Солнечные" => "Solar",
                        _ => movement
                    };
                }
                return movement;
            }
        }

        // материал (переводится)
        public string Material
        {
            get
            {
                string material = _watch.Material ?? "";
                if (Services.LocalizationService.CurrentLanguage == "en-US")
                {
                    return material switch
                    {
                        "Нержавеющая сталь" => "Stainless steel",
                        "Титан" => "Titanium",
                        "Керамика" => "Ceramic",
                        "Пластик" => "Plastic",
                        "Карбон" => "Carbon",
                        "Золото" => "Gold",
                        "Серебро" => "Silver",
                        _ => material
                    };
                }
                return material;
            }
        }

        // цвет (переводится)
        public string Color
        {
            get
            {
                string color = _watch.Color ?? "";
                if (Services.LocalizationService.CurrentLanguage == "en-US")
                {
                    return color switch
                    {
                        "Черный" => "Black",
                        "Серебристый" => "Silver",
                        "Золотистый" => "Gold",
                        "Синий" => "Blue",
                        "Зеленый" => "Green",
                        "Красный" => "Red",
                        "Белый" => "White",
                        "Розовый" => "Pink",
                        "Коричневый" => "Brown",
                        _ => color
                    };
                }
                return color;
            }
        }

        // страна производства (переводится)
        public string CountryOfOrigin
        {
            get
            {
                string country = _watch.CountryOfOrigin ?? "";
                if (Services.LocalizationService.CurrentLanguage == "en-US")
                {
                    return country switch
                    {
                        "Швейцария" => "Switzerland",
                        "Япония" => "Japan",
                        "Германия" => "Germany",
                        "Китай" => "China",
                        "Россия" => "Russia",
                        "США" => "USA",
                        "Франция" => "France",
                        "Италия" => "Italy",
                        _ => country
                    };
                }
                return country;
            }
        }

        // пол (переводится)
        public string Gender
        {
            get
            {
                string gender = _watch.Gender ?? "";
                if (Services.LocalizationService.CurrentLanguage == "en-US")
                {
                    return gender switch
                    {
                        "Мужские" => "Men",
                        "Женские" => "Women",
                        "Унисекс" => "Unisex",
                        _ => gender
                    };
                }
                return gender;
            }
        }

        // водонепроницаемость (переводится)
        public string WaterResistance
        {
            get
            {
                string wr = _watch.WaterResistance ?? "";
                if (Services.LocalizationService.CurrentLanguage == "en-US")
                {
                    return wr switch
                    {
                        "30m" => "30m (splash)",
                        "50m" => "50m (shower)",
                        "100m" => "100m (swimming)",
                        "200m" => "200m (diving)",
                        "300m" => "300m (professional diving)",
                        "500m+" => "500m+ (deep water)",
                        "Нет" => "None",
                        _ => wr
                    };
                }
                return wr;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        // обновление локализации при смене языка
        public void UpdateLocalization()
        {
            OnPropertyChanged(nameof(PriceLabel));
            OnPropertyChanged(nameof(InStockLabel));
            OnPropertyChanged(nameof(DoubleClickHint));
            OnPropertyChanged(nameof(OutOfStockLabel));
            OnPropertyChanged(nameof(OrderButtonText));
            OnPropertyChanged(nameof(EditText));
            OnPropertyChanged(nameof(DeleteText));
            OnPropertyChanged(nameof(AvailableText));
            OnPropertyChanged(nameof(InStockDisplay));
            OnPropertyChanged(nameof(Category));
            OnPropertyChanged(nameof(MovementType));
            OnPropertyChanged(nameof(Material));
            OnPropertyChanged(nameof(Color));
            OnPropertyChanged(nameof(CountryOfOrigin));
            OnPropertyChanged(nameof(Gender));
            OnPropertyChanged(nameof(WaterResistance));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}