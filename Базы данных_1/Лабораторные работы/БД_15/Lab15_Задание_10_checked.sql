USE PVR_MyBase_1;
GO

DROP TRIGGER IF EXISTS dbo.TR_X_RAB;
DROP TRIGGER IF EXISTS dbo.TR_X_OPER_DELETE;
DROP TRIGGER IF EXISTS dbo.TR_X_VYP_LIMIT;
GO

DROP TABLE IF EXISTS dbo.TR_AUDIT_X;
GO

-- Задание 10
CREATE TABLE dbo.TR_AUDIT_X
(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    STMT VARCHAR(20),
    TRNAME VARCHAR(100),
    CC NVARCHAR(500),
    TDATE DATETIME DEFAULT GETDATE()
);
GO

CREATE TRIGGER dbo.TR_X_RAB
ON dbo.РАБОТНИКИ
AFTER INSERT, DELETE, UPDATE
AS
BEGIN
    IF EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS (SELECT 1 FROM deleted)
    BEGIN
        INSERT INTO dbo.TR_AUDIT_X (STMT, TRNAME, CC)
        SELECT 'INSERT',
               'TR_X_RAB',
               CAST(ID_работника AS NVARCHAR(20)) + N' ' + Фамилия + N' ' + CAST(Стаж AS NVARCHAR(20))
        FROM inserted;
    END;

    IF NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
    BEGIN
        INSERT INTO dbo.TR_AUDIT_X (STMT, TRNAME, CC)
        SELECT 'DELETE',
               'TR_X_RAB',
               CAST(ID_работника AS NVARCHAR(20)) + N' ' + Фамилия
        FROM deleted;
    END;

    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
    BEGIN
        INSERT INTO dbo.TR_AUDIT_X (STMT, TRNAME, CC)
        SELECT 'UPDATE',
               'TR_X_RAB',
               N'Было: ' + CAST(D.ID_работника AS NVARCHAR(20)) + N' ' + D.Фамилия + N' ' + CAST(D.Стаж AS NVARCHAR(20)) +
               N' Стало: ' + CAST(I.ID_работника AS NVARCHAR(20)) + N' ' + I.Фамилия + N' ' + CAST(I.Стаж AS NVARCHAR(20))
        FROM inserted I
        INNER JOIN deleted D ON I.ID_работника = D.ID_работника;
    END;
END;
GO

CREATE TRIGGER dbo.TR_X_OPER_DELETE
ON dbo.ОПЕРАЦИИ
INSTEAD OF DELETE
AS
BEGIN
    RAISERROR(N'Удаление операций запрещено', 16, 1);
END;
GO

CREATE TRIGGER dbo.TR_X_VYP_LIMIT
ON dbo.ВЫПОЛНЕНИЕ
AFTER INSERT, UPDATE
AS
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM inserted
        WHERE Количество_деталей > 100
    )
    BEGIN
        RAISERROR(N'Количество деталей больше 100 запрещено', 16, 1);
        ROLLBACK TRANSACTION;
    END;
END;
GO

DELETE FROM dbo.ВЫПОЛНЕНИЕ
WHERE ID_работника = 99;

DELETE FROM dbo.РАБОТНИКИ
WHERE ID_работника = 99;
GO

INSERT INTO dbo.РАБОТНИКИ
    (ID_работника, Фамилия, Имя, Отчество, Адрес, Телефон, Стаж)
VALUES
    (99, N'Новиков', N'Иван', N'Петрович', N'ул. Новая 1', N'999-99', 1);
GO

UPDATE dbo.РАБОТНИКИ
SET Стаж = 2
WHERE ID_работника = 99;
GO

DELETE FROM dbo.РАБОТНИКИ
WHERE ID_работника = 99;
GO

SELECT *
FROM dbo.TR_AUDIT_X
ORDER BY ID;
GO

BEGIN TRY
    DELETE FROM dbo.ОПЕРАЦИИ
    WHERE Наименование_операции = N'Сварка';
END TRY
BEGIN CATCH
    PRINT N'Ошибка: ' + ERROR_MESSAGE();
END CATCH;
GO

SELECT *
FROM dbo.ОПЕРАЦИИ
WHERE Наименование_операции = N'Сварка';
GO

DELETE FROM dbo.ВЫПОЛНЕНИЕ
WHERE ID_выполнения = 100;
GO

BEGIN TRY
    INSERT INTO dbo.ВЫПОЛНЕНИЕ
        (ID_выполнения, Наименование_операции, Количество_деталей, Дата, ID_работника)
    VALUES
        (100, N'Сварка', 150, '2024-04-01', 2);
END TRY
BEGIN CATCH
    PRINT N'Ошибка: ' + ERROR_MESSAGE();
END CATCH;
GO

SELECT *
FROM dbo.ВЫПОЛНЕНИЕ
WHERE ID_выполнения = 100;
GO