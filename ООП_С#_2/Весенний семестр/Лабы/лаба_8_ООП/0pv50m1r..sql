USE WatchShopDB;
SELECT name, system_type_id, max_length 
FROM sys.columns 
WHERE object_id = OBJECT_ID('Watches') AND name = 'Image';


DROP DATABASE WatchShopDB


SELECT COUNT(*) FROM Watches WHERE ShortName LIKE 'qqqqqqqqqqq%';



SELECT Id, ShortName, Quantity, SoldCount FROM Watches WHERE ShortName = 'Casio G-Shock';



USE WatchShopDB;
SELECT Id, Username, Role FROM Users;

USE WatchShopDB;
UPDATE Users SET Role = 'Admin' WHERE Username = 'admin';



USE WatchShopDB;

SELECT 
    Id,
    ShortName,
    DATALENGTH(Image) AS BlobSizeBytes,        -- размер BLOB в байтах
    LEFT(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(20), Image), 1), 20) AS FirstBytesHex,
    ImagePath
FROM Watches
ORDER BY Id DESC;


USE WatchShopDB;
SELECT Id, ShortName, Image FROM Watches WHERE Image IS NOT NULL;

USE WatchShopDB;
SELECT * FROM Watches;