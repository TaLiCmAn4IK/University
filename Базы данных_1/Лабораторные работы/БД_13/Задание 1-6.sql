USE UNIVER;
GO

-- Задание 1
DROP PROCEDURE IF EXISTS dbo.PSUBJECT;
GO

CREATE PROCEDURE dbo.PSUBJECT
AS
BEGIN
    SELECT SUBJECT, SUBJECT_NAME, PULPIT
    FROM dbo.SUBJECT;

    RETURN @@ROWCOUNT;
END;
GO

DECLARE @rc INT;

EXEC @rc = dbo.PSUBJECT;

PRINT N'Количество строк = ' + CAST(@rc AS NVARCHAR(10));
GO

-- Задание 2
ALTER PROCEDURE dbo.PSUBJECT
    @p VARCHAR(20) = NULL,
    @c INT OUTPUT
AS
BEGIN
    SELECT SUBJECT, SUBJECT_NAME, PULPIT
    FROM dbo.SUBJECT
    WHERE @p IS NULL OR PULPIT = @p;

    SET @c = @@ROWCOUNT;

    RETURN (SELECT COUNT(*) FROM dbo.SUBJECT);
END;
GO

DECLARE @rc INT;
DECLARE @c INT;

EXEC @rc = dbo.PSUBJECT @p = 'ИСиТ', @c = @c OUTPUT;

PRINT N'Количество строк в результирующем наборе = ' + CAST(@c AS NVARCHAR(10));
PRINT N'Общее количество дисциплин = ' + CAST(@rc AS NVARCHAR(10));
GO

-- Задание 3
ALTER PROCEDURE dbo.PSUBJECT
    @p VARCHAR(20) = NULL
AS
BEGIN
    SELECT SUBJECT, SUBJECT_NAME, PULPIT
    FROM dbo.SUBJECT
    WHERE @p IS NULL OR PULPIT = @p;
END;
GO

IF OBJECT_ID('tempdb..#SUBJECT') IS NOT NULL
    DROP TABLE #SUBJECT;

CREATE TABLE #SUBJECT
(
    SUBJECT CHAR(10),
    SUBJECT_NAME VARCHAR(100),
    PULPIT CHAR(20)
);

INSERT INTO #SUBJECT
EXEC dbo.PSUBJECT @p = 'ИСиТ';

SELECT *
FROM #SUBJECT;
GO

-- Задание 4
DROP PROCEDURE IF EXISTS dbo.PAUDITORIUM_INSERT;
GO

CREATE PROCEDURE dbo.PAUDITORIUM_INSERT
    @a CHAR(20),
    @n VARCHAR(50),
    @c INT = 0,
    @t CHAR(10)
AS
BEGIN
    BEGIN TRY
        INSERT INTO dbo.AUDITORIUM
            (AUDITORIUM, AUDITORIUM_NAME, AUDITORIUM_CAPACITY, AUDITORIUM_TYPE)
        VALUES
            (@a, @n, @c, @t);

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

DELETE FROM dbo.AUDITORIUM
WHERE AUDITORIUM = '999-1';
GO

DECLARE @rc INT;

EXEC @rc = dbo.PAUDITORIUM_INSERT
    @a = '999-1',
    @n = 'Аудитория 999-1',
    @c = 50,
    @t = 'ЛК';

PRINT N'Код возврата = ' + CAST(@rc AS NVARCHAR(10));
GO

DECLARE @rc INT;

EXEC @rc = dbo.PAUDITORIUM_INSERT
    @a = '999-1',
    @n = 'Аудитория 999-1',
    @c = 50,
    @t = 'ЛК';

PRINT N'Код возврата = ' + CAST(@rc AS NVARCHAR(10));
GO

-- Задание 5
DROP PROCEDURE IF EXISTS dbo.SUBJECT_REPORT;
GO

CREATE PROCEDURE dbo.SUBJECT_REPORT
    @p CHAR(10)
AS
BEGIN
    DECLARE @rc INT = 0;
    DECLARE @subject CHAR(10);
    DECLARE @report NVARCHAR(MAX) = N'';

    BEGIN TRY
        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.PULPIT
            WHERE PULPIT = @p
        )
            RAISERROR(N'ошибка в параметрах', 11, 1);

        DECLARE SubCur CURSOR LOCAL FOR
            SELECT SUBJECT
            FROM dbo.SUBJECT
            WHERE PULPIT = @p;

        OPEN SubCur;

        FETCH NEXT FROM SubCur INTO @subject;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @report = @report + RTRIM(@subject) + N', ';
            SET @rc = @rc + 1;

            FETCH NEXT FROM SubCur INTO @subject;
        END;

        CLOSE SubCur;
        DEALLOCATE SubCur;

        IF LEN(@report) > 0
            SET @report = LEFT(@report, LEN(@report) - 1);

        PRINT N'Дисциплины кафедры ' + RTRIM(@p) + N': ' + @report;

        RETURN @rc;
    END TRY
    BEGIN CATCH
        PRINT ERROR_MESSAGE();

        RETURN @rc;
    END CATCH;
END;
GO

DECLARE @rc INT;

EXEC @rc = dbo.SUBJECT_REPORT @p = 'ИСиТ';

PRINT N'Количество дисциплин = ' + CAST(@rc AS NVARCHAR(10));
GO

DECLARE @rc INT;

EXEC @rc = dbo.SUBJECT_REPORT @p = 'XXX';

PRINT N'Количество дисциплин = ' + CAST(@rc AS NVARCHAR(10));
GO

-- Задание 6
DROP PROCEDURE IF EXISTS dbo.PAUDITORIUM_INSERTX;
GO

CREATE PROCEDURE dbo.PAUDITORIUM_INSERTX
    @a CHAR(20),
    @n VARCHAR(50),
    @c INT = 0,
    @t CHAR(10),
    @tn VARCHAR(50)
AS
BEGIN
    DECLARE @rc INT;

    BEGIN TRY
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

        BEGIN TRANSACTION;

        INSERT INTO dbo.AUDITORIUM_TYPE
            (AUDITORIUM_TYPE, AUDITORIUM_TYPENAME)
        VALUES
            (@t, @tn);

        EXEC @rc = dbo.PAUDITORIUM_INSERT
            @a = @a,
            @n = @n,
            @c = @c,
            @t = @t;

        IF @rc = -1
            RAISERROR(N'ошибка при добавлении аудитории', 11, 1);

        COMMIT TRANSACTION;

        RETURN 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        PRINT N'Номер ошибки: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
        PRINT N'Уровень серьезности: ' + CAST(ERROR_SEVERITY() AS NVARCHAR(10));
        PRINT N'Сообщение: ' + ERROR_MESSAGE();

        RETURN -1;
    END CATCH;
END;
GO

DELETE FROM dbo.AUDITORIUM
WHERE AUDITORIUM = '998-1';

DELETE FROM dbo.AUDITORIUM_TYPE
WHERE AUDITORIUM_TYPE = 'ЛБ-Т';
GO

DECLARE @rc INT;

EXEC @rc = dbo.PAUDITORIUM_INSERTX
    @a = '998-1',
    @n = 'Аудитория 998-1',
    @c = 25,
    @t = 'ЛБ-Т',
    @tn = 'Тестовая лаборатория';

PRINT N'Код возврата = ' + CAST(@rc AS NVARCHAR(10));
GO

SELECT *
FROM dbo.AUDITORIUM_TYPE
WHERE AUDITORIUM_TYPE = 'ЛБ-Т';

SELECT *
FROM dbo.AUDITORIUM
WHERE AUDITORIUM = '998-1';
GO