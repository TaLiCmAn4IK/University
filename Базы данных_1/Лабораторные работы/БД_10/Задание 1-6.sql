USE UNIVER;
GO

-- ЗАДАНИЕ 1. Кластеризованный индекс
SELECT t.name AS [Таблица], i.name AS [Индекс], i.type_desc AS [Тип],
       i.is_unique AS [Уникальный], i.is_primary_key AS [PK]
FROM   sys.indexes i JOIN sys.tables t ON t.object_id = i.object_id
WHERE  t.is_ms_shipped = 0
ORDER BY t.name, i.index_id;

EXEC SP_HELPINDEX 'STUDENT';
EXEC SP_HELPINDEX 'PROGRESS';
EXEC SP_HELPINDEX 'GROUPS';
GO

IF OBJECT_ID('tempdb..#UNI1') IS NOT NULL DROP TABLE #UNI1;
CREATE TABLE #UNI1 (TIND INT, IDSTUD INT, NAME NVARCHAR(100), BDAY DATE, INFO VARCHAR(200));

SET NOCOUNT ON;
;WITH N AS (SELECT TOP (100) ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS n FROM sys.all_objects)
INSERT INTO #UNI1 (TIND, IDSTUD, NAME, BDAY, INFO)
SELECT FLOOR(10000*RAND(CHECKSUM(NEWID()))), s.IDSTUDENT, s.NAME, s.BDAY, REPLICATE('данные ', 10)
FROM   STUDENT s CROSS JOIN N;
GO

CHECKPOINT; DBCC DROPCLEANBUFFERS;
SET STATISTICS IO ON; SET STATISTICS TIME ON;
SELECT * FROM #UNI1 WHERE TIND BETWEEN 1500 AND 2500 ORDER BY TIND;   -- ДО
GO

CREATE CLUSTERED INDEX UNI1_CL ON #UNI1(TIND ASC);

CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT * FROM #UNI1 WHERE TIND BETWEEN 1500 AND 2500 ORDER BY TIND;   -- ПОСЛЕ
GO

-- =============== ЗАДАНИЕ 2. Составной некластерный индекс ===============
IF OBJECT_ID('tempdb..#UNI2') IS NOT NULL DROP TABLE #UNI2;
CREATE TABLE #UNI2 (TKEY INT, CC INT IDENTITY(1,1), TF VARCHAR(100));

SET NOCOUNT ON;
DECLARE @i2 INT = 0;
WHILE @i2 < 20000
BEGIN
    INSERT #UNI2(TKEY, TF) VALUES (FLOOR(30000*RAND()), REPLICATE(N'строка ', 10));
    SET @i2 += 1;
END;
GO

CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT * FROM #UNI2 WHERE TKEY > 1500 AND CC < 4500;
SELECT * FROM #UNI2 ORDER BY TKEY, CC;
SELECT * FROM #UNI2 WHERE TKEY = 556 AND CC > 3;
GO

CREATE INDEX UNI2_NONCLU ON #UNI2(TKEY, CC);
GO

CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT * FROM #UNI2 WHERE TKEY > 1500 AND CC < 4500;   -- индекс НЕ применится
SELECT * FROM #UNI2 ORDER BY TKEY, CC;                 -- индекс НЕ применится (SELECT *)
SELECT * FROM #UNI2 WHERE TKEY = 556 AND CC > 3;       -- индекс ПРИМЕНИТСЯ
GO

-- =============== ЗАДАНИЕ 3. Индекс покрытия (INCLUDE) ===============
IF OBJECT_ID('tempdb..#UNI3') IS NOT NULL DROP TABLE #UNI3;
CREATE TABLE #UNI3 (TKEY INT, CC INT IDENTITY(1,1), TF VARCHAR(100));

SET NOCOUNT ON;
DECLARE @i3 INT = 0;
WHILE @i3 < 20000
BEGIN
    INSERT #UNI3(TKEY, TF) VALUES (FLOOR(30000*RAND()), REPLICATE(N'строка ', 10));
    SET @i3 += 1;
END;
GO

CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT CC FROM #UNI3 WHERE TKEY > 15000;     -- ДО
GO

CREATE INDEX UNI3_TKEY_X ON #UNI3(TKEY) INCLUDE (CC);
GO

CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT CC FROM #UNI3 WHERE TKEY > 15000;     -- ПОСЛЕ (Index Seek + покрытие)
GO

-- =============== ЗАДАНИЕ 4. Фильтруемый индекс ===============
IF OBJECT_ID('tempdb..#UNI4') IS NOT NULL DROP TABLE #UNI4;
CREATE TABLE #UNI4 (TKEY INT, CC INT IDENTITY(1,1), TF VARCHAR(100));

SET NOCOUNT ON;
DECLARE @i4 INT = 0;
WHILE @i4 < 20000
BEGIN
    INSERT #UNI4(TKEY, TF) VALUES (FLOOR(30000*RAND()), REPLICATE(N'строка ', 10));
    SET @i4 += 1;
END;
GO

CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT TKEY FROM #UNI4 WHERE TKEY BETWEEN 5000 AND 19999;
SELECT TKEY FROM #UNI4 WHERE TKEY > 15000 AND TKEY < 20000;
SELECT TKEY FROM #UNI4 WHERE TKEY = 17000;
GO

CREATE INDEX UNI4_WHERE ON #UNI4(TKEY) WHERE (TKEY >= 15000 AND TKEY < 20000);
GO

CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT TKEY FROM #UNI4 WHERE TKEY > 15000 AND TKEY < 20000;   -- использует индекс
SELECT TKEY FROM #UNI4 WHERE TKEY = 17000;                    -- использует индекс (может быть 0 строк)
SELECT TKEY FROM #UNI4 WHERE TKEY BETWEEN 5000 AND 19999;     -- Table Scan
GO

-- =============== ЗАДАНИЕ 5. Фрагментация. REORGANIZE / REBUILD ===============
IF OBJECT_ID('tempdb..#UNI5') IS NOT NULL DROP TABLE #UNI5;
CREATE TABLE #UNI5 (TKEY INT, CC INT IDENTITY(1,1), TF VARCHAR(100));

SET NOCOUNT ON;
DECLARE @i5 INT = 0;
WHILE @i5 < 20000
BEGIN
    INSERT #UNI5(TKEY, TF) VALUES (FLOOR(30000*RAND()), REPLICATE(N'строка ', 10));
    SET @i5 += 1;
END;

CREATE INDEX UNI5_TKEY ON #UNI5(TKEY);
GO

-- 5.1. Начальная фрагментация
DECLARE @oid5 INT = OBJECT_ID(N'tempdb..#UNI5');
SELECT ii.name [Индекс], ss.page_count [Страниц], ss.avg_fragmentation_in_percent [Фрагментация %]
FROM   sys.dm_db_index_physical_stats(DB_ID(N'tempdb'), @oid5, NULL, NULL, 'DETAILED') ss
       JOIN tempdb.sys.indexes ii ON ss.object_id = ii.object_id AND ss.index_id = ii.index_id
WHERE  ii.name IS NOT NULL;
GO

-- 5.2. Поднимаем фрагментацию
INSERT TOP(10000) #UNI5(TKEY, TF) SELECT TKEY, TF FROM #UNI5;

DECLARE @oid5 INT = OBJECT_ID(N'tempdb..#UNI5');
SELECT ii.name [Индекс], ss.page_count [Страниц], ss.avg_fragmentation_in_percent [После вставок %]
FROM   sys.dm_db_index_physical_stats(DB_ID(N'tempdb'), @oid5, NULL, NULL, 'DETAILED') ss
       JOIN tempdb.sys.indexes ii ON ss.object_id = ii.object_id AND ss.index_id = ii.index_id
WHERE  ii.name IS NOT NULL;
GO

-- 5.3. REORGANIZE
ALTER INDEX UNI5_TKEY ON #UNI5 REORGANIZE;

DECLARE @oid5 INT = OBJECT_ID(N'tempdb..#UNI5');
SELECT ii.name [Индекс], ss.page_count [Страниц], ss.avg_fragmentation_in_percent [После REORGANIZE %]
FROM   sys.dm_db_index_physical_stats(DB_ID(N'tempdb'), @oid5, NULL, NULL, 'DETAILED') ss
       JOIN tempdb.sys.indexes ii ON ss.object_id = ii.object_id AND ss.index_id = ii.index_id
WHERE  ii.name IS NOT NULL;
GO

-- 5.4. REBUILD
ALTER INDEX UNI5_TKEY ON #UNI5 REBUILD WITH (ONLINE = OFF);

DECLARE @oid5 INT = OBJECT_ID(N'tempdb..#UNI5');
SELECT ii.name [Индекс], ss.page_count [Страниц], ss.avg_fragmentation_in_percent [После REBUILD %]
FROM   sys.dm_db_index_physical_stats(DB_ID(N'tempdb'), @oid5, NULL, NULL, 'DETAILED') ss
       JOIN tempdb.sys.indexes ii ON ss.object_id = ii.object_id AND ss.index_id = ii.index_id
WHERE  ii.name IS NOT NULL;
GO

-- =============== ЗАДАНИЕ 6. FILLFACTOR ===============
IF OBJECT_ID('tempdb..#UNI6') IS NOT NULL DROP TABLE #UNI6;
CREATE TABLE #UNI6 (TKEY INT, CC INT IDENTITY(1,1), TF VARCHAR(100));

SET NOCOUNT ON;
DECLARE @i6 INT = 0;
WHILE @i6 < 20000
BEGIN
    INSERT #UNI6(TKEY, TF) VALUES (FLOOR(30000*RAND()), REPLICATE(N'строка ', 10));
    SET @i6 += 1;
END;
GO

-- 6.1. FILLFACTOR = 100 (по умолчанию)
CREATE INDEX UNI6_TKEY_100 ON #UNI6(TKEY);
INSERT TOP(50) PERCENT INTO #UNI6(TKEY, TF) SELECT TKEY, TF FROM #UNI6;

DECLARE @oid6 INT = OBJECT_ID(N'tempdb..#UNI6');
SELECT ii.name [Индекс], ss.page_count [Страниц], ss.avg_fragmentation_in_percent [FF=100 %]
FROM   sys.dm_db_index_physical_stats(DB_ID(N'tempdb'), @oid6, NULL, NULL, 'DETAILED') ss
       JOIN tempdb.sys.indexes ii ON ss.object_id = ii.object_id AND ss.index_id = ii.index_id
WHERE  ii.name IS NOT NULL;
GO

-- 6.2. FILLFACTOR = 65
DROP INDEX UNI6_TKEY_100 ON #UNI6;

CREATE INDEX UNI6_TKEY_65 ON #UNI6(TKEY) WITH (FILLFACTOR = 65);
INSERT TOP(50) PERCENT INTO #UNI6(TKEY, TF) SELECT TKEY, TF FROM #UNI6;

DECLARE @oid6 INT = OBJECT_ID(N'tempdb..#UNI6');
SELECT ii.name [Индекс], ss.page_count [Страниц], ss.avg_fragmentation_in_percent [FF=65 %]
FROM   sys.dm_db_index_physical_stats(DB_ID(N'tempdb'), @oid6, NULL, NULL, 'DETAILED') ss
       JOIN tempdb.sys.indexes ii ON ss.object_id = ii.object_id AND ss.index_id = ii.index_id
WHERE  ii.name IS NOT NULL;
GO

PRINT 'Лабораторная работа №10 (задания 1-6) выполнена.';
GO