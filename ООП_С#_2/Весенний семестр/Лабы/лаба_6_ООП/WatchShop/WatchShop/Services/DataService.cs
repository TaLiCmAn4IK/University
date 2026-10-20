using System;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using WatchShop.Models;
using System.Windows;

namespace WatchShop.Services
{
    // сервис для загрузки и сохранения данных в json файлы
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

        // загрузка товаров из json
        public ObservableCollection<Watch> LoadWatches()
        {
            try
            {
                if (File.Exists(watchesFilePath))
                {
                    string json = File.ReadAllText(watchesFilePath);
                    return JsonConvert.DeserializeObject<ObservableCollection<Watch>>(json)
                           ?? new ObservableCollection<Watch>();
                }
                else
                {
                    return CreateSampleData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка загрузки данных: {ex.Message}");
                return new ObservableCollection<Watch>();
            }
        }

        // сохранение товаров в json
        public void SaveWatches(ObservableCollection<Watch> watches)
        {
            try
            {
                string json = JsonConvert.SerializeObject(watches, Formatting.Indented);
                File.WriteAllText(watchesFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка сохранения данных: {ex.Message}");
            }
        }

        // загрузка пользователей из json
        public ObservableCollection<User> LoadUsers()
        {
            try
            {
                if (File.Exists(usersFilePath))
                {
                    string json = File.ReadAllText(usersFilePath);
                    return JsonConvert.DeserializeObject<ObservableCollection<User>>(json)
                           ?? new ObservableCollection<User>();
                }
                else
                {
                    var users = new ObservableCollection<User>
                    {
                        new User { Id = 1, Username = "admin", Password = "admin123", Role = "Admin", Email = "admin@watchshop.com" },
                        new User { Id = 2, Username = "client", Password = "client123", Role = "Client", Email = "client@example.com" }
                    };
                    SaveUsers(users);
                    return users;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка загрузки пользователей: {ex.Message}");
                return new ObservableCollection<User>();
            }
        }

        // сохранение пользователей в json
        public void SaveUsers(ObservableCollection<User> users)
        {
            try
            {
                string json = JsonConvert.SerializeObject(users, Formatting.Indented);
                File.WriteAllText(usersFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ошибка сохранения пользователей: {ex.Message}");
            }
        }

        // создание тестовых данных при первом запуске
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
                    Price = 8500,
                    Quantity = 3,
                    Color = "Черный",
                    Size = "41mm",
                    CountryOfOrigin = "Швейцария",
                    Discount = 0,
                    InStock = true,
                    Manufacturer = "Rolex",
                    MovementType = "Автоматические",
                    WaterResistance = "300m",
                    Material = "Сталь",
                    Gender = "Мужские",
                    SoldCount = 125,
                    ImagePath = "/Images/rolex.jpg",
                    Images = new ObservableCollection<string> { "/Images/rolex.jpg" }
                },
                new Watch
                {
                    Id = 2,
                    ShortName = "Casio G-Shock",
                    FullName = "Casio G-Shock GA-2100",
                    Description = "Ударопрочные часы с аналогово-цифровым дисплеем. Стильный дизайн, множество функций.",
                    Category = "Спортивные",
                    Rating = 4.7,
                    Price = 120,
                    Quantity = 15,
                    Color = "Черный/Зеленый",
                    Size = "45mm",
                    CountryOfOrigin = "Япония",
                    Discount = 10,
                    InStock = true,
                    Manufacturer = "Casio",
                    MovementType = "Кварцевые",
                    WaterResistance = "200m",
                    Material = "Смола",
                    Gender = "Унисекс",
                    SoldCount = 450,
                    ImagePath = "/Images/casio.jpg",
                    Images = new ObservableCollection<string> { "/Images/casio.jpg" }
                },
                new Watch
                {
                    Id = 3,
                    ShortName = "Apple Watch",
                    FullName = "Apple Watch Series 9",
                    Description = "Умные часы с расширенными функциями мониторинга здоровья и фитнеса.",
                    Category = "Смарт-часы",
                    Rating = 4.8,
                    Price = 399,
                    Quantity = 0,
                    Color = "Серебристый",
                    Size = "45mm",
                    CountryOfOrigin = "Китай",
                    Discount = 0,
                    InStock = false,
                    Manufacturer = "Apple",
                    MovementType = "Электронные",
                    WaterResistance = "50m",
                    Material = "Алюминий",
                    Gender = "Унисекс",
                    SoldCount = 1200,
                    ImagePath = "/Images/apple.jpg",
                    Images = new ObservableCollection<string> { "/Images/apple.jpg" }
                }
            };

            SaveWatches(watches);
            return watches;
        }
    }
}