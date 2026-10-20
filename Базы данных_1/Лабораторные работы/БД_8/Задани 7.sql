USE PVR_MyBase_1;
GO

DROP VIEW IF EXISTS Сотрудник, Количество_операций, Перечень_операций, Сложные_операции, Список_операций;
DELETE FROM ОПЕРАЦИИ WHERE Наименование_операции IN (N'Полировка', N'Закалка');
GO

CREATE VIEW Сотрудник
AS
SELECT
    ID_работника AS [Код],
    Фамилия      AS [Фамилия],
    Имя          AS [Имя],
    Отчество     AS [Отчество],
    Стаж         AS [Стаж]
FROM РАБОТНИКИ;
GO

SELECT * FROM Сотрудник;
GO

CREATE VIEW Количество_операций
AS
SELECT
    Р.Фамилия              AS [Работник],
    COUNT(В.ID_выполнения) AS [Количество операций]
FROM РАБОТНИКИ AS Р
INNER JOIN ВЫПОЛНЕНИЕ AS В
        ON Р.ID_работника = В.ID_работника
GROUP BY Р.Фамилия;
GO

SELECT * FROM Количество_операций;
GO

CREATE VIEW Перечень_операций
AS
SELECT
    Наименование_операции AS [Наименование],
    Признак_сложности     AS [Сложность]
FROM ОПЕРАЦИИ
WHERE Признак_сложности = N'Высокая';
GO

SELECT * FROM Перечень_операций;
INSERT Перечень_операций VALUES (N'Полировка', N'Низкая');
SELECT * FROM Перечень_операций;
UPDATE Перечень_операций SET [Сложность] = N'Высокая' WHERE [Наименование] = N'Полировка';
SELECT * FROM Перечень_операций;
DELETE FROM Перечень_операций WHERE [Наименование] = N'Полировка';
SELECT * FROM Перечень_операций;

DELETE FROM ОПЕРАЦИИ WHERE Наименование_операции = N'Полировка';
GO

CREATE VIEW Сложные_операции
AS
SELECT
    Наименование_операции AS [Наименование],
    Признак_сложности     AS [Сложность]
FROM ОПЕРАЦИИ
WHERE Признак_сложности = N'Высокая'
WITH CHECK OPTION;
GO

SELECT * FROM Сложные_операции;
INSERT Сложные_операции VALUES (N'Закалка', N'Низкая');
SELECT * FROM Сложные_операции;
UPDATE Сложные_операции SET [Сложность] = N'Низкая' WHERE [Наименование] = N'Сварка';
SELECT * FROM Сложные_операции;
DELETE FROM Сложные_операции WHERE [Наименование] = N'Закалка';
SELECT * FROM Сложные_операции;
GO

CREATE VIEW Список_операций
AS
SELECT TOP 20
    Наименование_операции AS [Наименование],
    Признак_сложности     AS [Сложность]
FROM ОПЕРАЦИИ
ORDER BY Наименование_операции;
GO

SELECT * FROM Список_операций ORDER BY [Наименование];
GO

ALTER VIEW Количество_операций
WITH SCHEMABINDING
AS
SELECT
    Р.Фамилия              AS [Работник],
    COUNT(В.ID_выполнения) AS [Количество операций]
FROM dbo.РАБОТНИКИ AS Р
INNER JOIN dbo.ВЫПОЛНЕНИЕ AS В
        ON Р.ID_работника = В.ID_работника
GROUP BY Р.Фамилия;
GO

SELECT * FROM Количество_операций;
GO