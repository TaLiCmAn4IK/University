using System;
using System.Collections.Generic;

namespace WatchShop.Services
{
    // сервис для управления локализацией и переводами
    public static class LocalizationService
    {
        private static Dictionary<string, Dictionary<string, string>> translations;
        public static string CurrentLanguage { get; private set; } = "ru-RU";
        public static event EventHandler? LanguageChanged;

        static LocalizationService()
        {
            translations = new Dictionary<string, Dictionary<string, string>>();

            // русский язык
            var ru = new Dictionary<string, string>();

            ru["File"] = "Файл";
            ru["Login"] = "Войти";
            ru["Logout"] = "Выйти";
            ru["Exit"] = "Выход";
            ru["View"] = "Вид";
            ru["ListView"] = "Список товаров:";
            ru["TableView"] = "Таблица товаров";
            ru["Language"] = "Язык";
            ru["Russian"] = "Русский";
            ru["English"] = "English";
            ru["Help"] = "Помощь";
            ru["About"] = "О программе";

            ru["Filters"] = "Фильтры";
            ru["Search"] = "Поиск:";
            ru["Category"] = "Категория:";
            ru["AllCategories"] = "Все категории";
            ru["Price"] = "Цена:";
            ru["InStockOnly"] = "Только в наличии";
            ru["Sort"] = "Сортировка:";
            ru["ByName"] = "По названию";
            ru["ByPriceAsc"] = "По цене (возрастание)";
            ru["ByPriceDesc"] = "По цене (убывание)";
            ru["ByRating"] = "По рейтингу";
            ru["ApplyFilters"] = "Применить фильтры";
            ru["Management"] = "Управление";
            ru["AddProduct"] = "Добавить товар";

            ru["Luxury"] = "Люксовые";
            ru["Sport"] = "Спортивные";
            ru["Smart"] = "Смарт-часы";
            ru["Classic"] = "Классические";

            ru["ViewMode"] = "Вид:";
            ru["List"] = "Список";
            ru["Table"] = "Таблица";
            ru["DoubleClickHint"] = "Двойной клик для просмотра";
            ru["InStock"] = "В наличии:";
            ru["Price_Label"] = "Цена:";

            ru["Ready"] = "Готов к работе";
            ru["NotAuthorized"] = "Не авторизован";
            ru["Welcome"] = "Добро пожаловать";
            ru["ProductsFound"] = "Найдено товаров:";

            ru["LoginTitle"] = "Вход в систему";
            ru["Username"] = "Имя пользователя:";
            ru["Password"] = "Пароль:";
            ru["LoginButton"] = "Войти";
            ru["CancelButton"] = "Отмена";
            ru["InvalidCredentials"] = "Неверное имя пользователя или пароль";

            ru["AddTitle"] = "Добавление товара";
            ru["EditTitle"] = "Редактирование товара";
            ru["ShortName"] = "Краткое название:*";
            ru["FullName"] = "Полное название:";
            ru["Category_field"] = "Категория:";
            ru["Price_field"] = "Цена ($):";
            ru["Quantity_field"] = "Количество:";
            ru["Manufacturer_field"] = "Производитель:";
            ru["MovementType"] = "Тип механизма:";
            ru["Size_field"] = "Размер:";
            ru["Material_field"] = "Материал:";
            ru["Country_field"] = "Страна производства:";
            ru["Color_field"] = "Цвет:";
            ru["Rating_field"] = "Рейтинг (0-5):";
            ru["Discount_field"] = "Скидка (%):";
            ru["WaterResistance_field"] = "Водонепроницаемость:";
            ru["Gender_field"] = "Пол:";
            ru["InStock_field"] = "В наличии";
            ru["Description_field"] = "Описание товара";
            ru["Images_field"] = "Изображения товара";
            ru["AddImage"] = "Добавить изображение";
            ru["RemoveImage"] = "Удалить";
            ru["DragDropHint"] = "Перетащите изображения сюда или нажмите кнопку для выбора";
            ru["Save"] = "Сохранить";
            ru["Cancel"] = "Отмена";
            ru["Required"] = "* - обязательное поле";

            ru["MovementType_Mechanical"] = "Механические";
            ru["MovementType_Quartz"] = "Кварцевые";
            ru["MovementType_Electronic"] = "Электронные";
            ru["MovementType_Automatic"] = "Автоматические";
            ru["MovementType_Solar"] = "Солнечные";

            ru["Material_Steel"] = "Нержавеющая сталь";
            ru["Material_Titanium"] = "Титан";
            ru["Material_Ceramic"] = "Керамика";
            ru["Material_Plastic"] = "Пластик";
            ru["Material_Carbon"] = "Карбон";
            ru["Material_Gold"] = "Золото";
            ru["Material_Silver"] = "Серебро";

            ru["Gender_Men"] = "Мужские";
            ru["Gender_Women"] = "Женские";
            ru["Gender_Unisex"] = "Унисекс";

            ru["Country_Switzerland"] = "Швейцария";
            ru["Country_Japan"] = "Япония";
            ru["Country_Germany"] = "Германия";
            ru["Country_China"] = "Китай";
            ru["Country_Russia"] = "Россия";
            ru["Country_USA"] = "США";
            ru["Country_France"] = "Франция";
            ru["Country_Italy"] = "Италия";

            ru["Color_Black"] = "Черный";
            ru["Color_Silver"] = "Серебристый";
            ru["Color_Gold"] = "Золотистый";
            ru["Color_Blue"] = "Синий";
            ru["Color_Green"] = "Зеленый";
            ru["Color_Red"] = "Красный";
            ru["Color_White"] = "Белый";
            ru["Color_Pink"] = "Розовый";
            ru["Color_Brown"] = "Коричневый";

            ru["WaterResistance_30m"] = "30m";
            ru["WaterResistance_50m"] = "50m";
            ru["WaterResistance_100m"] = "100m";
            ru["WaterResistance_200m"] = "200m";
            ru["WaterResistance_300m"] = "300m";
            ru["WaterResistance_500m"] = "500m+";
            ru["WaterResistance_None"] = "Нет";

            ru["Details"] = "Характеристики";
            ru["FullName_Label"] = "ПОЛНОЕ НАЗВАНИЕ";
            ru["Category_Label"] = "КАТЕГОРИЯ";
            ru["Manufacturer_Label"] = "ПРОИЗВОДИТЕЛЬ";
            ru["Country_Label"] = "СТРАНА";
            ru["Price_Label2"] = "ЦЕНА";
            ru["Discount_Label"] = "Скидка";
            ru["DiscountNotApplied"] = "Скидка не применяется";
            ru["Characteristics"] = "ХАРАКТЕРИСТИКИ";
            ru["Rating_Label"] = "Рейтинг";
            ru["MovementType_Label"] = "Тип механизма";
            ru["Size_Label"] = "Размер корпуса";
            ru["Material_Label"] = "Материал";
            ru["Color_Label"] = "Цвет";
            ru["WaterResistance_Label"] = "Водонепроницаемость";
            ru["Gender_Label"] = "Пол";
            ru["Stock_Label"] = "СКЛАД";
            ru["InStock_Label"] = "В наличии";
            ru["SoldCount_Label"] = "Продано всего";
            ru["Description_Label"] = "ОПИСАНИЕ";

            ru["Order"] = "Заказать";
            ru["Order_Success"] = "Заказ оформлен!";
            ru["Order_Details"] = "Вы успешно заказали товар:\n\nНазвание: {0}\nЦена: ${1}\nКоличество: {2} шт.\nСумма: ${3}\n\nСпасибо за покупку!";
            ru["Order_OutOfStock"] = "Извините, товар '{0}' отсутствует в наличии.";
            ru["Order_Quantity"] = "Введите количество для заказа (доступно: {0} шт.):";
            ru["Edit"] = "Редактировать";
            ru["Delete"] = "Удалить";
            ru["Available"] = "Доступно: {0} шт.";
            ru["AvailableHeader"] = "Доступно:";
            ru["AdminOnly_Delete"] = "Только администратор может удалять товары";
            ru["AdminOnly_Edit"] = "Только администратор может редактировать товары";
            ru["AdminOnly_Add"] = "Только администратор может добавлять товары";
            ru["Warning"] = "Предупреждение";
            ru["Confirm_Delete"] = "Вы уверены, что хотите удалить товар '{0}'?";
            ru["Confirm_Title"] = "Подтверждение удаления";
            ru["Product_Deleted"] = "Товар успешно удален";
            ru["Product_Updated"] = "Товар успешно обновлен";
            ru["Product_Added"] = "Товар успешно добавлен";
            ru["Rating"] = "Рейтинг:";
            ru["Action"] = "Действие";
            ru["Edit"] = "Редактировать";
            ru["Delete"] = "Удалить";
            ru["profile_title"] = "личный кабинет";
            ru["username"] = "имя пользователя:";
            ru["email"] = "email:";
            ru["new_password"] = "новый пароль:";
            ru["password_hint"] = "оставьте пустым, чтобы не менять";
            ru["language"] = "язык интерфейса:";
            ru["russian"] = "русский";
            ru["english"] = "english";
            ru["save"] = "сохранить";
            ru["cancel"] = "отмена";
            ru["info"] = "информация";
            ru["profile_saved"] = "данные сохранены";
            ru["login_required"] = "необходимо войти в систему";
            ru["warning"] = "предупреждение";
            ru["PasswordHint"] = "(можно оставить пустым)";
            ru["EnterPassword"] = "введите пароль";
            ru["EnterUsername"] = "введите имя пользователя";

            translations["ru-RU"] = ru;

            // английский язык
            var en = new Dictionary<string, string>();

            en["File"] = "File";
            en["Login"] = "Login";
            en["Logout"] = "Logout";
            en["Exit"] = "Exit";
            en["View"] = "View";
            en["ListView"] = "List View:";
            en["TableView"] = "Table View";
            en["Language"] = "Language";
            en["Russian"] = "Русский";
            en["English"] = "English";
            en["Help"] = "Help";
            en["About"] = "About";

            en["Filters"] = "Filters";
            en["Search"] = "Search:";
            en["Category"] = "Category:";
            en["AllCategories"] = "All categories";
            en["Price"] = "Price:";
            en["InStockOnly"] = "In stock only";
            en["Sort"] = "Sort by:";
            en["ByName"] = "By name";
            en["ByPriceAsc"] = "Price (low to high)";
            en["ByPriceDesc"] = "Price (high to low)";
            en["ByRating"] = "By rating";
            en["ApplyFilters"] = "Apply filters";
            en["Management"] = "Management";
            en["AddProduct"] = "Add product";

            en["Luxury"] = "Luxury";
            en["Sport"] = "Sport";
            en["Smart"] = "Smart watches";
            en["Classic"] = "Classic";

            en["ViewMode"] = "View:";
            en["List"] = "List";
            en["Table"] = "Table";
            en["DoubleClickHint"] = "Double click to view";
            en["InStock"] = "In stock:";
            en["Price_Label"] = "Price:";

            en["Ready"] = "Ready";
            en["NotAuthorized"] = "Not authorized";
            en["Welcome"] = "Welcome";
            en["ProductsFound"] = "Products found:";

            en["LoginTitle"] = "Login";
            en["Username"] = "Username:";
            en["Password"] = "Password:";
            en["LoginButton"] = "Login";
            en["CancelButton"] = "Cancel";
            en["InvalidCredentials"] = "Invalid username or password";

            en["AddTitle"] = "Add Product";
            en["EditTitle"] = "Edit Product";
            en["ShortName"] = "Short name:*";
            en["FullName"] = "Full name:";
            en["Category_field"] = "Category:";
            en["Price_field"] = "Price ($):";
            en["Quantity_field"] = "Quantity:";
            en["Manufacturer_field"] = "Manufacturer:";
            en["MovementType"] = "Movement type:";
            en["Size_field"] = "Size:";
            en["Material_field"] = "Material:";
            en["Country_field"] = "Country:";
            en["Color_field"] = "Color:";
            en["Rating_field"] = "Rating (0-5):";
            en["Discount_field"] = "Discount (%):";
            en["WaterResistance_field"] = "Water resistance:";
            en["Gender_field"] = "Gender:";
            en["InStock_field"] = "In stock";
            en["Description_field"] = "Description";
            en["Images_field"] = "Product Images";
            en["AddImage"] = "Add Image";
            en["RemoveImage"] = "Remove";
            en["DragDropHint"] = "Drag images here or click the button to select";
            en["Save"] = "Save";
            en["Cancel"] = "Cancel";
            en["Required"] = "* - required field";

            en["MovementType_Mechanical"] = "Mechanical";
            en["MovementType_Quartz"] = "Quartz";
            en["MovementType_Electronic"] = "Electronic";
            en["MovementType_Automatic"] = "Automatic";
            en["MovementType_Solar"] = "Solar";

            en["Material_Steel"] = "Stainless steel";
            en["Material_Titanium"] = "Titanium";
            en["Material_Ceramic"] = "Ceramic";
            en["Material_Plastic"] = "Plastic";
            en["Material_Carbon"] = "Carbon";
            en["Material_Gold"] = "Gold";
            en["Material_Silver"] = "Silver";

            en["Gender_Men"] = "Men";
            en["Gender_Women"] = "Women";
            en["Gender_Unisex"] = "Unisex";

            en["Country_Switzerland"] = "Switzerland";
            en["Country_Japan"] = "Japan";
            en["Country_Germany"] = "Germany";
            en["Country_China"] = "China";
            en["Country_Russia"] = "Russia";
            en["Country_USA"] = "USA";
            en["Country_France"] = "France";
            en["Country_Italy"] = "Italy";

            en["Color_Black"] = "Black";
            en["Color_Silver"] = "Silver";
            en["Color_Gold"] = "Gold";
            en["Color_Blue"] = "Blue";
            en["Color_Green"] = "Green";
            en["Color_Red"] = "Red";
            en["Color_White"] = "White";
            en["Color_Pink"] = "Pink";
            en["Color_Brown"] = "Brown";

            en["WaterResistance_30m"] = "30m (splash)";
            en["WaterResistance_50m"] = "50m (shower)";
            en["WaterResistance_100m"] = "100m (swimming)";
            en["WaterResistance_200m"] = "200m (diving)";
            en["WaterResistance_300m"] = "300m (professional diving)";
            en["WaterResistance_500m"] = "500m+ (deep water)";
            en["WaterResistance_None"] = "None";

            en["Details"] = "Details";
            en["FullName_Label"] = "FULL NAME";
            en["Category_Label"] = "CATEGORY";
            en["Manufacturer_Label"] = "MANUFACTURER";
            en["Country_Label"] = "COUNTRY";
            en["Price_Label2"] = "PRICE";
            en["Discount_Label"] = "Discount";
            en["DiscountNotApplied"] = "No discount applied";
            en["Characteristics"] = "CHARACTERISTICS";
            en["Rating_Label"] = "Rating";
            en["MovementType_Label"] = "Movement type";
            en["Size_Label"] = "Case size";
            en["Material_Label"] = "Material";
            en["Color_Label"] = "Color";
            en["WaterResistance_Label"] = "Water resistance";
            en["Gender_Label"] = "Gender";
            en["Stock_Label"] = "STOCK";
            en["InStock_Label"] = "In stock";
            en["SoldCount_Label"] = "Sold total";
            en["Description_Label"] = "DESCRIPTION";

            en["Order"] = "Order";
            en["Order_Success"] = "Order placed!";
            en["Order_Details"] = "You have successfully ordered:\n\nName: {0}\nPrice: ${1}\nQuantity: {2} pcs.\nTotal: ${3}\n\nThank you for your purchase!";
            en["Order_OutOfStock"] = "Sorry, product '{0}' is out of stock.";
            en["Order_Quantity"] = "Enter quantity to order (available: {0} pcs.):";
            en["Edit"] = "Edit";
            en["Delete"] = "Delete";
            en["Available"] = "Available: {0} pcs.";
            en["AvailableHeader"] = "Available:";
            en["AdminOnly_Delete"] = "Only administrator can delete products";
            en["AdminOnly_Edit"] = "Only administrator can edit products";
            en["AdminOnly_Add"] = "Only administrator can add products";
            en["Warning"] = "Warning";
            en["Confirm_Delete"] = "Are you sure you want to delete product '{0}'?";
            en["Confirm_Title"] = "Confirm Delete";
            en["Product_Deleted"] = "Product successfully deleted";
            en["Product_Updated"] = "Product successfully updated";
            en["Product_Added"] = "Product successfully added";
            en["Rating"] = "Rating:";
            en["Action"] = "Action";
            en["Edit"] = "Edit";
            en["Delete"] = "Delete";
            en["profile_title"] = "profile";
            en["username"] = "username:";
            en["email"] = "email:";
            en["new_password"] = "new password:";
            en["password_hint"] = "leave empty to keep current password";
            en["language"] = "language:";
            en["russian"] = "русский";
            en["english"] = "english";
            en["save"] = "save";
            en["cancel"] = "cancel";
            en["info"] = "information";
            en["profile_saved"] = "profile saved";
            en["login_required"] = "login required";
            en["warning"] = "warning";
            ru["PasswordHint"] = "(можно оставить пустым)";
            ru["EnterUsername"] = "введите имя пользователя";
            en["EnterPassword"] = "enter password";

            translations["en-US"] = en;
        }

        public static string GetString(string key)
        {
            if (translations.ContainsKey(CurrentLanguage) && translations[CurrentLanguage].ContainsKey(key))
                return translations[CurrentLanguage][key];
            return key;
        }

        public static void ChangeLanguage(string language)
        {
            if (!translations.ContainsKey(language))
                language = "ru-RU";
            CurrentLanguage = language;
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}