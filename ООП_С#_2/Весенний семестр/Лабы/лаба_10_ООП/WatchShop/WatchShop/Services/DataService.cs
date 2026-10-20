using System;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using WatchShop.Models;

namespace WatchShop.Services
{
    /// <summary>
    /// Загружает и сохраняет товары и пользователей в JSON-файлы
    /// в %LocalAppData%/WatchShop/. При первом запуске создаёт демо-данные.
    /// </summary>
    public class DataService
    {
        private readonly string watchesFilePath;
        private readonly string usersFilePath;

        public DataService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WatchShop");

            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            watchesFilePath = Path.Combine(appDataPath, "watches.json");
            usersFilePath = Path.Combine(appDataPath, "users.json");
        }

        public ObservableCollection<Watch> LoadWatches()
        {
            if (File.Exists(watchesFilePath))
            {
                try
                {
                    var json = File.ReadAllText(watchesFilePath);
                    return JsonConvert.DeserializeObject<ObservableCollection<Watch>>(json)
                           ?? CreateSampleData();
                }
                catch
                {
                    return CreateSampleData();
                }
            }
            return CreateSampleData();
        }

        public void SaveWatches(ObservableCollection<Watch> watches)
        {
            var json = JsonConvert.SerializeObject(watches, Formatting.Indented);
            File.WriteAllText(watchesFilePath, json);
        }

        public ObservableCollection<User> LoadUsers()
        {
            if (File.Exists(usersFilePath))
            {
                try
                {
                    var json = File.ReadAllText(usersFilePath);
                    return JsonConvert.DeserializeObject<ObservableCollection<User>>(json)
                           ?? CreateDefaultUsers();
                }
                catch
                {
                    return CreateDefaultUsers();
                }
            }
            var users = CreateDefaultUsers();
            SaveUsers(users);
            return users;
        }

        public void SaveUsers(ObservableCollection<User> users)
        {
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(usersFilePath, json);
        }

        private static ObservableCollection<User> CreateDefaultUsers() => new()
        {
            new User { Id = 1, Username = "admin", Password = "admin123", Role = "Admin", Email = "admin@watchshop.com" },
            new User { Id = 2, Username = "client", Password = "client123", Role = "Client", Email = "client@example.com" }
        };

        private ObservableCollection<Watch> CreateSampleData()
        {
            var watches = new ObservableCollection<Watch>
            {
                new Watch
                {
                    Id = 1,
                    ShortName = "Rolex Submariner",
                    FullName = "Rolex Submariner Date 41mm",
                    Description = "Легендарные дайверские часы с механизмом автоматического завода. Водонепроницаемость до 300 метров.",
                    Category = "Люксовые",
                    Rating = 4.9,
                    Price = 8500m,
                    Quantity = 3,
                    Color = "Черный",
                    Size = "41mm",
                    CountryOfOrigin = "Швейцария",
                    Discount = 0,
                    InStock = true,
                    Manufacturer = "Rolex",
                    MovementType = "Автоматические",
                    WaterResistance = "300m",
                    Material = "Нержавеющая сталь",
                    Gender = "Мужские",
                    SoldCount = 125,
                    ImagePath = "/Images/rolex.jpg"
                },
                new Watch
                {
                    Id = 2,
                    ShortName = "Casio G-Shock",
                    FullName = "Casio G-Shock GA-2100",
                    Description = "Ударопрочные часы с аналогово-цифровым дисплеем. Стильный дизайн, множество функций.",
                    Category = "Спортивные",
                    Rating = 4.7,
                    Price = 120m,
                    Quantity = 15,
                    Color = "Черный",
                    Size = "45mm",
                    CountryOfOrigin = "Япония",
                    Discount = 10,
                    InStock = true,
                    Manufacturer = "Casio",
                    MovementType = "Кварцевые",
                    WaterResistance = "200m",
                    Material = "Пластик",
                    Gender = "Унисекс",
                    SoldCount = 450,
                    ImagePath = "/Images/casio.jpg"
                },
                new Watch
                {
                    Id = 3,
                    ShortName = "Apple Watch",
                    FullName = "Apple Watch Series 9",
                    Description = "Умные часы с расширенными функциями мониторинга здоровья и фитнеса.",
                    Category = "Смарт-часы",
                    Rating = 4.8,
                    Price = 399m,
                    Quantity = 0,
                    Color = "Серебристый",
                    Size = "45mm",
                    CountryOfOrigin = "Китай",
                    Discount = 0,
                    InStock = false,
                    Manufacturer = "Apple",
                    MovementType = "Электронные",
                    WaterResistance = "50m",
                    Material = "Титан",
                    Gender = "Унисекс",
                    SoldCount = 1200,
                    ImagePath = "/Images/apple.jpg"
                }
            };

            SaveWatches(watches);
            return watches;
        }
    }
}
