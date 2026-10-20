-- =============================================================
-- ЛАБОРАТОРНАЯ №10. ЗАДАНИЕ 7. БД PVR_MyBase_1
-- =============================================================
USE PVR_MyBase_1;
GO

SET STATISTICS IO ON;
SET STATISTICS TIME ON;
GO

-- 7.0. Исходные индексы (кластерные на PRIMARY KEY)
EXEC SP_HELPINDEX N'ОПЕРАЦИИ';
EXEC SP_HELPINDEX N'РАБОТНИКИ';
EXEC SP_HELPINDEX N'ВЫПОЛНЕНИЕ';

SELECT t.name [Таблица], i.name [Индекс], i.type_desc [Тип],
       i.is_unique [Уникальный], i.is_primary_key [PK]
FROM   sys.indexes i JOIN sys.tables t ON t.object_id = i.object_id
WHERE  t.is_ms_shipped = 0
ORDER BY t.name, i.index_id;
GO

-- =============================================================
-- 7.1. РАЗДУВАЕМ ТАБЛИЦЫ
-- =============================================================
SET NOCOUNT ON;

-- 7.1.1. РАБОТНИКИ до 5000 строк
DECLARE @i INT = (SELECT ISNULL(MAX(ID_работника),0) FROM РАБОТНИКИ);
WHILE @i < 5000
BEGIN
    SET @i += 1;
    INSERT РАБОТНИКИ(ID_работника, Фамилия, Имя, Отчество, Адрес, Телефон, Стаж)
    VALUES (@i,
            N'Фамилия_' + CAST(@i AS NVARCHAR(10)),
            N'Имя_'     + CAST(@i AS NVARCHAR(10)),
            N'Отч_'     + CAST(@i AS NVARCHAR(10)),
            N'ул. Тестовая, ' + CAST(@i AS NVARCHAR(10)),
            CAST(100000 + @i AS NVARCHAR(10)),
            ABS(CHECKSUM(NEWID())) % 41);
END;
SELECT COUNT(*) [РАБОТНИКИ, строк] FROM РАБОТНИКИ;
GO

-- 7.1.2. ВЫПОЛНЕНИЕ до 50000 строк
SET NOCOUNT ON;
DECLARE @j INT = (SELECT ISNULL(MAX(ID_выполнения),0) FROM ВЫПОЛНЕНИЕ);
DECLARE @ops TABLE (id INT IDENTITY(1,1), nm NVARCHAR(100));
INSERT @ops(nm) SELECT Наименование_операции FROM ОПЕРАЦИИ;
DECLARE @opsCount   INT = (SELECT COUNT(*) FROM @ops);
DECLARE @maxWorker  INT = (SELECT MAX(ID_работника) FROM РАБОТНИКИ);

WHILE @j < 50000
BEGIN
    SET @j += 1;
    DECLARE @opName NVARCHAR(100) =
        (SELECT nm FROM @ops WHERE id = ((ABS(CHECKSUM(NEWID())) % @opsCount) + 1));
    INSERT ВЫПОЛНЕНИЕ(ID_выполнения, Наименование_операции, Количество_деталей, Дата, ID_работника)
    VALUES (@j,
            @opName,
            1 + ABS(CHECKSUM(NEWID())) % 50,
            DATEADD(DAY, -1 * (ABS(CHECKSUM(NEWID())) % 720), CAST(GETDATE() AS DATE)),
            1 + ABS(CHECKSUM(NEWID())) % @maxWorker);
END;
SELECT COUNT(*) [ВЫПОЛНЕНИЕ, строк] FROM ВЫПОЛНЕНИЕ;
GO

-- =============================================================
-- 7.2. ЗАПРОСЫ ДО СОЗДАНИЯ ИНДЕКСОВ (включи Estimated Plan, Ctrl+L)
-- =============================================================

-- Q1: выборка выполнений по фамилии работника (JOIN)
CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT r.Фамилия, r.Имя, v.Дата, v.Наименование_операции, v.Количество_деталей
FROM   ВЫПОЛНЕНИЕ v JOIN РАБОТНИКИ r ON r.ID_работника = v.ID_работника
WHERE  r.Фамилия = N'Фамилия_1234';
GO

-- Q2: операция + диапазон дат
CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT ID_выполнения, Дата, Количество_деталей, ID_работника
FROM   ВЫПОЛНЕНИЕ
WHERE  Наименование_операции = N'Токарная обработка'
  AND  Дата BETWEEN '2025-01-01' AND '2025-06-30';
GO

-- Q3: агрегат — суммарные детали по дате для одной операции (кандидат на покрытие)
CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT Дата, SUM(Количество_деталей) [Сумма]
FROM   ВЫПОЛНЕНИЕ
WHERE  Наименование_операции = N'Сварка'
GROUP BY Дата
ORDER BY Дата;
GO

-- Q4: крупные партии >40 деталей (кандидат на фильтруемый индекс)
CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT ID_выполнения, Дата, Количество_деталей
FROM   ВЫПОЛНЕНИЕ
WHERE  Количество_деталей > 40;
GO

-- =============================================================
-- 7.3. СОЗДАНИЕ ИНДЕКСОВ
-- =============================================================

-- IX1: некластерный по фамилии (для Q1)
CREATE INDEX IX_РАБОТНИКИ_Фамилия
    ON РАБОТНИКИ(Фамилия);

-- IX2: некластерный по внешнему ключу ID_работника (ускорит JOIN в Q1)
CREATE INDEX IX_ВЫПОЛНЕНИЕ_IDраб
    ON ВЫПОЛНЕНИЕ(ID_работника);

-- IX3: составной (Операция, Дата) — для Q2
CREATE INDEX IX_ВЫП_Опер_Дата
    ON ВЫПОЛНЕНИЕ(Наименование_операции, Дата);

-- IX4: индекс покрытия для Q3 (включает Количество_деталей)
CREATE INDEX IX_ВЫП_Опер_Дата_INC
    ON ВЫПОЛНЕНИЕ(Наименование_операции, Дата)
    INCLUDE (Количество_деталей);

-- IX5: фильтруемый индекс — только крупные партии (для Q4)
CREATE INDEX IX_ВЫП_Колво_большие
    ON ВЫПОЛНЕНИЕ(Количество_деталей)
    WHERE Количество_деталей > 40;
GO

EXEC SP_HELPINDEX N'РАБОТНИКИ';
EXEC SP_HELPINDEX N'ВЫПОЛНЕНИЕ';
GO

-- =============================================================
-- 7.4. ТЕ ЖЕ ЗАПРОСЫ ПОСЛЕ СОЗДАНИЯ ИНДЕКСОВ
-- =============================================================

-- Q1 -> Index Seek по IX_РАБОТНИКИ_Фамилия + Nested Loops + Seek по IX_ВЫПОЛНЕНИЕ_IDраб
CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT r.Фамилия, r.Имя, v.Дата, v.Наименование_операции, v.Количество_деталей
FROM   ВЫПОЛНЕНИЕ v JOIN РАБОТНИКИ r ON r.ID_работника = v.ID_работника
WHERE  r.Фамилия = N'Фамилия_1234';
GO

-- Q2 -> Index Seek по IX_ВЫП_Опер_Дата (+ Key Lookup за остальными колонками)
CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT ID_выполнения, Дата, Количество_деталей, ID_работника
FROM   ВЫПОЛНЕНИЕ
WHERE  Наименование_операции = N'Токарная обработка'
  AND  Дата BETWEEN '2025-01-01' AND '2025-06-30';
GO

-- Q3 -> Index Seek по IX_ВЫП_Опер_Дата_INC БЕЗ Key Lookup (полное покрытие)
CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT Дата, SUM(Количество_деталей) [Сумма]
FROM   ВЫПОЛНЕНИЕ
WHERE  Наименование_операции = N'Сварка'
GROUP BY Дата
ORDER BY Дата;
GO

-- Q4 -> Index Seek по фильтруемому IX_ВЫП_Колво_большие
CHECKPOINT; DBCC DROPCLEANBUFFERS;
SELECT ID_выполнения, Дата, Количество_деталей
FROM   ВЫПОЛНЕНИЕ
WHERE  Количество_деталей > 40;
GO

PRINT 'Задание 7 выполнено. Сравните планы запросов 7.2 и 7.4.';
GO