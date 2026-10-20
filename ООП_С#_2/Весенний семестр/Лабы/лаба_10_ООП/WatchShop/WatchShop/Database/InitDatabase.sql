-- =============================================================================
--  Скрипт инициализации базы данных WatchShopDB.
--  Запускается приложением при первом старте, если БД ещё не существует.
--  Создаёт 6 таблиц со связями, представление, 2 триггера и 4 хранимые
--  процедуры.
-- =============================================================================

-- ====================== ТАБЛИЦА Users ======================
-- Хранит пользователей системы.
-- Username UNIQUE — БД сама не даст создать двух пользователей с одним именем.
CREATE TABLE Users (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Username    NVARCHAR(50)  NOT NULL UNIQUE,
    Password    NVARCHAR(100) NOT NULL,
    Role        NVARCHAR(20)  NOT NULL DEFAULT 'Client',
    Email       NVARCHAR(100) NULL,
    RegistrationDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive    BIT NOT NULL DEFAULT 1
);

-- ====================== ТАБЛИЦА Manufacturers ======================
-- Производители часов вынесены в отдельный справочник: один производитель
-- выпускает много моделей. Если завтра нужно поменять имя ("Rolex" → "ROLEX SA")
-- — правится одна запись, а не каждый товар.
CREATE TABLE Manufacturers (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100) NOT NULL UNIQUE,
    Country     NVARCHAR(50)  NULL,
    FoundedYear INT NULL
);

-- ====================== ТАБЛИЦА Categories ======================
-- Категории часов (Люксовые, Спортивные, Смарт-часы и т.д.).
-- NameKey — ключ для поиска перевода в ResourceDictionary (Cat_Luxury и т.п.).
CREATE TABLE Categories (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(50) NOT NULL UNIQUE,
    NameKey     NVARCHAR(50) NULL
);

-- ====================== ТАБЛИЦА Watches ======================
-- Главная таблица товаров. Связана FK с Manufacturers и Categories.
-- Поле Image (VARBINARY(MAX)) — графическое поле по требованию ТЗ;
-- может хранить превью товара прямо в БД (BLOB).
-- ImagePath — альтернативно путь к файлу-картинке в ресурсах.
CREATE TABLE Watches (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ShortName       NVARCHAR(100) NOT NULL,
    FullName        NVARCHAR(200) NULL,
    Description     NVARCHAR(MAX) NULL,
    ManufacturerId  INT NULL,
    CategoryId      INT NULL,
    Price           DECIMAL(18,2) NOT NULL DEFAULT 0,
    Quantity        INT NOT NULL DEFAULT 0,
    Rating          FLOAT NOT NULL DEFAULT 0,
    Discount        FLOAT NOT NULL DEFAULT 0,
    InStock         BIT NOT NULL DEFAULT 1,
    Color           NVARCHAR(50) NULL,
    Size            NVARCHAR(20) NULL,
    Material        NVARCHAR(50) NULL,
    Gender          NVARCHAR(20) NULL,
    MovementType    NVARCHAR(50) NULL,
    WaterResistance NVARCHAR(20) NULL,
    SoldCount       INT NOT NULL DEFAULT 0,
    Image           VARBINARY(MAX) NULL,
    ImagePath       NVARCHAR(500) NULL,
    CreatedAt       DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt       DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Watches_Manufacturer FOREIGN KEY (ManufacturerId) REFERENCES Manufacturers(Id),
    CONSTRAINT FK_Watches_Category     FOREIGN KEY (CategoryId)     REFERENCES Categories(Id)
);

-- ====================== ТАБЛИЦА Orders ======================
-- Заголовок заказа: кто заказывал, когда, на какую сумму, статус.
CREATE TABLE Orders (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    UserId      INT NOT NULL,
    OrderDate   DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Status      NVARCHAR(30) NOT NULL DEFAULT N'Новый',
    CONSTRAINT FK_Orders_User FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- ====================== ТАБЛИЦА OrderItems ======================
-- Позиции заказа: какие именно товары и в каком количестве.
-- ON DELETE CASCADE — при удалении заказа автоматически удаляются его позиции.
CREATE TABLE OrderItems (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    OrderId     INT NOT NULL,
    WatchId     INT NOT NULL,
    Quantity    INT NOT NULL DEFAULT 1,
    UnitPrice   DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Order FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Watch FOREIGN KEY (WatchId) REFERENCES Watches(Id)
);
GO

-- =============================================================================
--  ПРЕДСТАВЛЕНИЕ vw_WatchesFull
--  Товары с подтянутыми именами производителя и категории — удобно
--  для отображения в DataGrid одной выборкой без JOIN'ов в коде.
-- =============================================================================
CREATE VIEW vw_WatchesFull AS
SELECT
    w.Id, w.ShortName, w.FullName, w.Description,
    w.Price, w.Quantity, w.Rating, w.Discount, w.InStock,
    w.Color, w.Size, w.Material, w.Gender,
    w.MovementType, w.WaterResistance, w.SoldCount,
    w.ImagePath,
    m.Name  AS ManufacturerName,
    c.Name  AS CategoryName,
    c.NameKey AS CategoryKey,
    w.ManufacturerId,
    w.CategoryId
FROM Watches w
LEFT JOIN Manufacturers m ON w.ManufacturerId = m.Id
LEFT JOIN Categories    c ON w.CategoryId     = c.Id;
GO

-- =============================================================================
--  ТРИГГЕР 1: trg_Watches_UpdateInStock
--  При UPDATE поля Quantity автоматически синхронизирует InStock:
--  Quantity = 0 → InStock = 0, иначе → 1. Также обновляет UpdatedAt.
-- =============================================================================
CREATE TRIGGER trg_Watches_UpdateInStock
ON Watches
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(Quantity)
    BEGIN
        UPDATE w
        SET InStock = CASE WHEN i.Quantity > 0 THEN 1 ELSE 0 END,
            UpdatedAt = GETDATE()
        FROM Watches w
        INNER JOIN inserted i ON w.Id = i.Id;
    END
END
GO

-- =============================================================================
--  ТРИГГЕР 2: trg_OrderItems_UpdateStock
--  При INSERT в OrderItems автоматически:
--    - уменьшает Quantity у соответствующего товара в Watches
--    - увеличивает SoldCount
--  Атомарно через триггер — гарантия согласованности.
-- =============================================================================
CREATE TRIGGER trg_OrderItems_UpdateStock
ON OrderItems
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE w
    SET w.Quantity  = w.Quantity  - i.Quantity,
        w.SoldCount = w.SoldCount + i.Quantity
    FROM Watches w
    INNER JOIN inserted i ON w.Id = i.WatchId;
END
GO

-- =============================================================================
--  ХРАНИМАЯ ПРОЦЕДУРА 1: sp_GetWatchesFiltered
--  Получает товары по фильтрам. Все параметры опциональные.
--  Результат — вид аналогичный vw_WatchesFull.
-- =============================================================================
CREATE PROCEDURE sp_GetWatchesFiltered
    @SearchText  NVARCHAR(100) = NULL,
    @CategoryId  INT = NULL,
    @MinPrice    DECIMAL(18,2) = NULL,
    @MaxPrice    DECIMAL(18,2) = NULL,
    @InStockOnly BIT = 0,
    @SortBy      NVARCHAR(20) = 'Name'
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM vw_WatchesFull
    WHERE
        (@SearchText IS NULL OR ShortName LIKE '%' + @SearchText + '%' OR FullName LIKE '%' + @SearchText + '%')
        AND (@CategoryId IS NULL OR CategoryId = @CategoryId)
        AND (@MinPrice   IS NULL OR Price >= @MinPrice)
        AND (@MaxPrice   IS NULL OR Price <= @MaxPrice)
        AND (@InStockOnly = 0 OR InStock = 1)
    ORDER BY
        CASE WHEN @SortBy = 'PriceAsc'  THEN Price END ASC,
        CASE WHEN @SortBy = 'PriceDesc' THEN Price END DESC,
        CASE WHEN @SortBy = 'Rating'    THEN Rating END DESC,
        CASE WHEN @SortBy = 'Name'      THEN ShortName END ASC;
END
GO

-- =============================================================================
--  ХРАНИМАЯ ПРОЦЕДУРА 2: sp_CreateOrder
--  Создаёт заказ целиком (заголовок Orders + позиция OrderItems) внутри
--  транзакции. Если хоть одна часть не сохранится — откатывается всё.
--  Возвращает Id созданного заказа через OUTPUT-параметр.
-- =============================================================================
CREATE PROCEDURE sp_CreateOrder
    @UserId   INT,
    @WatchId  INT,
    @Quantity INT,
    @OrderId  INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @Price DECIMAL(18,2);
        DECLARE @Discount FLOAT;
        SELECT @Price = Price, @Discount = Discount FROM Watches WHERE Id = @WatchId;

        DECLARE @UnitPrice DECIMAL(18,2) = @Price * (1 - CAST(@Discount AS DECIMAL(18,2)) / 100.0);
        DECLARE @Total     DECIMAL(18,2) = @UnitPrice * @Quantity;

        INSERT INTO Orders (UserId, TotalAmount) VALUES (@UserId, @Total);
        SET @OrderId = SCOPE_IDENTITY();

        INSERT INTO OrderItems (OrderId, WatchId, Quantity, UnitPrice)
        VALUES (@OrderId, @WatchId, @Quantity, @UnitPrice);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================================================
--  ХРАНИМАЯ ПРОЦЕДУРА 3: sp_GetStatistics
--  Возвращает общую статистику магазина одной строкой.
-- =============================================================================
CREATE PROCEDURE sp_GetStatistics
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        (SELECT COUNT(*) FROM Watches)                               AS WatchCount,
        (SELECT COUNT(*) FROM Watches WHERE InStock = 1)             AS InStockCount,
        (SELECT COUNT(*) FROM Users   WHERE IsActive = 1)            AS UserCount,
        (SELECT COUNT(*) FROM Orders)                                AS OrderCount,
        (SELECT ISNULL(SUM(TotalAmount), 0) FROM Orders)             AS TotalRevenue,
        (SELECT ISNULL(SUM(SoldCount),   0) FROM Watches)            AS TotalSoldItems;
END
GO

-- =============================================================================
--  ХРАНИМАЯ ПРОЦЕДУРА 4: sp_GetTopSelling
--  Топ-N самых продаваемых товаров.
-- =============================================================================
CREATE PROCEDURE sp_GetTopSelling
    @TopN INT = 5
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (@TopN)
        Id, ShortName, SoldCount, Price, Rating
    FROM Watches
    ORDER BY SoldCount DESC;
END
GO
