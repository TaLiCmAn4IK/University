USE PVR_MyBase_1;
GO

-- Задание 7
DROP PROCEDURE IF EXISTS dbo.P_OPERATIONS;
GO

CREATE PROCEDURE dbo.P_OPERATIONS
AS
BEGIN
    SELECT Наименование_операции, Признак_сложности
    FROM dbo.ОПЕРАЦИИ;

    RETURN @@ROWCOUNT;
END;
GO

DECLARE @rc INT;

EXEC @rc = dbo.P_OPERATIONS;

PRINT N'Количество операций = ' + CAST(@rc AS NVARCHAR(10));
GO

-- Задание 7
DROP PROCEDURE IF EXISTS dbo.P_OPERATIONS_BY_COMPLEXITY;
GO

CREATE PROCEDURE dbo.P_OPERATIONS_BY_COMPLEXITY
    @p NVARCHAR(50),
    @c INT OUTPUT
AS
BEGIN
    SELECT Наименование_операции, Признак_сложности
    FROM dbo.ОПЕРАЦИИ
    WHERE Признак_сложности = @p;

    SET @c = @@ROWCOUNT;

    RETURN (SELECT COUNT(*) FROM dbo.ОПЕРАЦИИ);
END;
GO

DECLARE @rc INT;
DECLARE @c INT;

EXEC @rc = dbo.P_OPERATIONS_BY_COMPLEXITY
    @p = N'Высокая',
    @c = @c OUTPUT;

PRINT N'Количество операций выбранной сложности = ' + CAST(@c AS NVARCHAR(10));
PRINT N'Общее количество операций = ' + CAST(@rc AS NVARCHAR(10));
GO

-- Задание 7
DROP PROCEDURE IF EXISTS dbo.P_WORKER_INSERT;
GO

CREATE PROCEDURE dbo.P_WORKER_INSERT
    @id INT,
    @f NVARCHAR(50),
    @i NVARCHAR(50),
    @o NVARCHAR(50),
    @a NVARCHAR(100),
    @tel NVARCHAR(20),
    @st INT = 0
AS
BEGIN
    BEGIN TRY
        INSERT INTO dbo.РАБОТНИКИ
            (ID_работника, Фамилия, Имя, Отчество, Адрес, Телефон, Стаж)
        VALUES
            (@id, @f, @i, @o, @a, @tel, @st);

        RETURN 1;
    END TRY
    BEGIN CATCH
        PRINT N'Номер ошибки: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT N'Уровень серьезности: ' + CAST(ERROR_SEVERITY() AS NVARCHAR(10));
        PRINT N'Сообщение: ' + ERROR_MESSAGE();

        RETURN -1;
    END CATCH;
END;
GO

DELETE FROM dbo.ВЫПОЛНЕНИЕ
WHERE ID_работника = 99;

DELETE FROM dbo.РАБОТНИКИ
WHERE ID_работника = 99;
GO

DECLARE @rc INT;

EXEC @rc = dbo.P_WORKER_INSERT
    @id = 99,
    @f = N'Новиков',
    @i = N'Иван',
    @o = N'Петрович',
    @a = N'ул. Новая 1',
    @tel = N'999-99',
    @st = 1;

PRINT N'Код возврата = ' + CAST(@rc AS NVARCHAR(10));
GO

-- Задание 7
DROP PROCEDURE IF EXISTS dbo.P_EXECUTION_INSERT;
GO

CREATE PROCEDURE dbo.P_EXECUTION_INSERT
    @idv INT,
    @op NVARCHAR(100),
    @qty INT,
    @date DATE,
    @idr INT
AS
BEGIN
    BEGIN TRY
        INSERT INTO dbo.ВЫПОЛНЕНИЕ
            (ID_выполнения, Наименование_операции, Количество_деталей, Дата, ID_работника)
        VALUES
            (@idv, @op, @qty, @date, @idr);

        RETURN 1;
    END TRY
    BEGIN CATCH
        PRINT N'Номер ошибки: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT N'Уровень серьезности: ' + CAST(ERROR_SEVERITY() AS NVARCHAR(10));
        PRINT N'Сообщение: ' + ERROR_MESSAGE();

        RETURN -1;
    END CATCH;
END;
GO

DELETE FROM dbo.ВЫПОЛНЕНИЕ
WHERE ID_выполнения = 99;
GO

DECLARE @rc INT;

EXEC @rc = dbo.P_EXECUTION_INSERT
    @idv = 99,
    @op = N'Сварка',
    @qty = 6,
    @date = '2024-04-01',
    @idr = 99;

PRINT N'Код возврата = ' + CAST(@rc AS NVARCHAR(10));
GO

-- Задание 7
DROP PROCEDURE IF EXISTS dbo.P_WORKER_REPORT;
GO

CREATE PROCEDURE dbo.P_WORKER_REPORT
    @id INT
AS
BEGIN
    DECLARE @rc INT = 0;
    DECLARE @op NVARCHAR(100);
    DECLARE @report NVARCHAR(MAX) = N'';

    BEGIN TRY
        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.РАБОТНИКИ
            WHERE ID_работника = @id
        )
            RAISERROR(N'ошибка в параметрах', 11, 1);

        DECLARE WorkCur CURSOR LOCAL FOR
            SELECT Наименование_операции
            FROM dbo.ВЫПОЛНЕНИЕ
            WHERE ID_работника = @id;

        OPEN WorkCur;

        FETCH NEXT FROM WorkCur INTO @op;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @report = @report + RTRIM(@op) + N', ';
            SET @rc = @rc + 1;

            FETCH NEXT FROM WorkCur INTO @op;
        END;

        CLOSE WorkCur;
        DEALLOCATE WorkCur;

        IF LEN(@report) > 0
            SET @report = LEFT(@report, LEN(@report) - 1);

        PRINT N'Операции работника ' + CAST(@id AS NVARCHAR(10)) + N': ' + @report;

        RETURN @rc;
    END TRY
    BEGIN CATCH
        PRINT ERROR_MESSAGE();

        RETURN @rc;
    END CATCH;
END;
GO

DECLARE @rc INT;

EXEC @rc = dbo.P_WORKER_REPORT @id = 99;

PRINT N'Количество операций = ' + CAST(@rc AS NVARCHAR(10));
GO