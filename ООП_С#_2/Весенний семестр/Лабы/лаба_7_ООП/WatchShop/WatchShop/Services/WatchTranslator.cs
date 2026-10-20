using System.Collections.Generic;

namespace WatchShop.Services
{
    /// <summary>
    /// Переводит значения свойств товара (категория, материал, цвет и т.д.)
    /// на текущий язык. Канонические значения хранятся на русском в JSON,
    /// перевод выполняется через ResourceDictionary по ключам типа "Cat_Luxury".
    /// </summary>
    public static class WatchTranslator
    {
        // Канонический русский -> ключ в Strings.*.xaml
        private static readonly Dictionary<string, string> Categories = new()
        {
            ["Люксовые"] = "Cat_Luxury",
            ["Спортивные"] = "Cat_Sport",
            ["Смарт-часы"] = "Cat_Smart",
            ["Классические"] = "Cat_Classic",
            ["Дайверские"] = "Cat_Diver",
            ["Авиационные"] = "Cat_Aviation",
            ["Модные"] = "Cat_Fashion",
        };

        private static readonly Dictionary<string, string> Movements = new()
        {
            ["Механические"] = "Mov_Mechanical",
            ["Кварцевые"] = "Mov_Quartz",
            ["Электронные"] = "Mov_Electronic",
            ["Автоматические"] = "Mov_Automatic",
            ["Солнечные"] = "Mov_Solar",
        };

        private static readonly Dictionary<string, string> Materials = new()
        {
            ["Нержавеющая сталь"] = "Mat_Steel",
            ["Сталь"] = "Mat_Steel",
            ["Титан"] = "Mat_Titanium",
            ["Керамика"] = "Mat_Ceramic",
            ["Пластик"] = "Mat_Plastic",
            ["Карбон"] = "Mat_Carbon",
            ["Золото"] = "Mat_Gold",
            ["Серебро"] = "Mat_Silver",
        };

        private static readonly Dictionary<string, string> Genders = new()
        {
            ["Мужские"] = "Gen_Men",
            ["Женские"] = "Gen_Women",
            ["Унисекс"] = "Gen_Unisex",
        };

        private static readonly Dictionary<string, string> Countries = new()
        {
            ["Швейцария"] = "Cnt_Switzerland",
            ["Япония"] = "Cnt_Japan",
            ["Германия"] = "Cnt_Germany",
            ["Китай"] = "Cnt_China",
            ["Россия"] = "Cnt_Russia",
            ["США"] = "Cnt_USA",
            ["Франция"] = "Cnt_France",
            ["Италия"] = "Cnt_Italy",
        };

        private static readonly Dictionary<string, string> Colors = new()
        {
            ["Черный"] = "Clr_Black",
            ["Серебристый"] = "Clr_Silver",
            ["Золотистый"] = "Clr_Gold",
            ["Синий"] = "Clr_Blue",
            ["Зеленый"] = "Clr_Green",
            ["Красный"] = "Clr_Red",
            ["Белый"] = "Clr_White",
            ["Розовый"] = "Clr_Pink",
            ["Коричневый"] = "Clr_Brown",
        };

        private static readonly Dictionary<string, string> Waters = new()
        {
            ["30m"] = "Wr_30m",
            ["50m"] = "Wr_50m",
            ["100m"] = "Wr_100m",
            ["200m"] = "Wr_200m",
            ["300m"] = "Wr_300m",
            ["500m+"] = "Wr_500m",
            ["Нет"] = "Wr_None",
        };

        private static string Translate(string? value, Dictionary<string, string> map)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return map.TryGetValue(value, out var key)
                ? LocalizationService.GetString(key)
                : value;
        }

        public static string Category(string? v) => Translate(v, Categories);
        public static string Movement(string? v) => Translate(v, Movements);
        public static string Material(string? v) => Translate(v, Materials);
        public static string Gender(string? v) => Translate(v, Genders);
        public static string Country(string? v) => Translate(v, Countries);
        public static string Color(string? v) => Translate(v, Colors);
        public static string WaterResistance(string? v) => Translate(v, Waters);

        // Для выпадающих списков добавления/редактирования —
        // отдаём список локализованных ключей.
        public static IEnumerable<string> CategoryKeys => Categories.Values;
        public static IEnumerable<string> MovementKeys => Movements.Values;
        public static IEnumerable<string> MaterialKeys
        {
            get
            {
                // убираем дубликат "Сталь"
                var seen = new HashSet<string>();
                foreach (var k in Materials.Values)
                    if (seen.Add(k)) yield return k;
            }
        }
        public static IEnumerable<string> GenderKeys => Genders.Values;
        public static IEnumerable<string> CountryKeys => Countries.Values;
        public static IEnumerable<string> ColorKeys => Colors.Values;
        public static IEnumerable<string> WaterKeys => Waters.Values;

        // Канонический русский ключ из ключа ресурса (для сохранения в JSON)
        public static string CanonicalCategory(string resourceKey) =>
            FindCanonical(Categories, resourceKey);
        public static string CanonicalMovement(string resourceKey) =>
            FindCanonical(Movements, resourceKey);
        public static string CanonicalMaterial(string resourceKey) =>
            FindCanonical(Materials, resourceKey);
        public static string CanonicalGender(string resourceKey) =>
            FindCanonical(Genders, resourceKey);
        public static string CanonicalCountry(string resourceKey) =>
            FindCanonical(Countries, resourceKey);
        public static string CanonicalColor(string resourceKey) =>
            FindCanonical(Colors, resourceKey);
        public static string CanonicalWater(string resourceKey) =>
            FindCanonical(Waters, resourceKey);

        private static string FindCanonical(Dictionary<string, string> map, string resourceKey)
        {
            foreach (var pair in map)
                if (pair.Value == resourceKey)
                    return pair.Key;
            return resourceKey;
        }
    }
}
