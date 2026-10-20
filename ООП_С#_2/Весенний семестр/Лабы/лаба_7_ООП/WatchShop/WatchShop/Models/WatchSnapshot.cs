namespace WatchShop.Models
{
    /// <summary>
    /// Снимок состояния товара. Используется в Undo/Redo сервисе,
    /// чтобы можно было отменить редактирование (восстановить старые значения)
    /// или повторить (применить новые).
    /// </summary>
    public class WatchSnapshot
    {
        public string ShortName { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public double Rating { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; } = "";
        public string Size { get; set; } = "";
        public string CountryOfOrigin { get; set; } = "";
        public double Discount { get; set; }
        public bool InStock { get; set; }
        public string Manufacturer { get; set; } = "";
        public string MovementType { get; set; } = "";
        public string WaterResistance { get; set; } = "";
        public string Material { get; set; } = "";
        public string Gender { get; set; } = "";
        public string ImagePath { get; set; } = "";

        public static WatchSnapshot From(Watch w) => new()
        {
            ShortName = w.ShortName,
            FullName = w.FullName,
            Description = w.Description,
            Category = w.Category,
            Rating = w.Rating,
            Price = w.Price,
            Quantity = w.Quantity,
            Color = w.Color,
            Size = w.Size,
            CountryOfOrigin = w.CountryOfOrigin,
            Discount = w.Discount,
            InStock = w.InStock,
            Manufacturer = w.Manufacturer,
            MovementType = w.MovementType,
            WaterResistance = w.WaterResistance,
            Material = w.Material,
            Gender = w.Gender,
            ImagePath = w.ImagePath
        };

        public void ApplyTo(Watch w)
        {
            w.ShortName = ShortName;
            w.FullName = FullName;
            w.Description = Description;
            w.Category = Category;
            w.Rating = Rating;
            w.Price = Price;
            w.Quantity = Quantity;
            w.Color = Color;
            w.Size = Size;
            w.CountryOfOrigin = CountryOfOrigin;
            w.Discount = Discount;
            w.InStock = InStock;
            w.Manufacturer = Manufacturer;
            w.MovementType = MovementType;
            w.WaterResistance = WaterResistance;
            w.Material = Material;
            w.Gender = Gender;
            w.ImagePath = ImagePath;
        }
    }
}
