USE PVR_MyBase_1;
GO

-- Задание 5
DROP FUNCTION IF EXISTS dbo.COUNT_WORKERS;
GO

CREATE FUNCTION dbo.COUNT_WORKERS
(
    @st INT = NULL
)
RETURNS INT
AS
BEGIN
    DECLARE @count INT;

    SELECT @count = COUNT(*)
    FROM РАБОТНИКИ
    WHERE @st IS NULL OR Стаж >= @st;

    RETURN @count;
END;
GO

SELECT dbo.COUNT_WORKERS(DEFAULT) AS Всего_работников;
SELECT dbo.COUNT_WORKERS(3) AS Работников_со_стажем_от_3;
GO

-- Задание 5
DROP FUNCTION IF EXISTS dbo.FWORKER_OPERATIONS;
GO

CREATE FUNCTION dbo.FWORKER_OPERATIONS
(
    @id INT
)
RETURNS NVARCHAR(300)
AS
BEGIN
    DECLARE @op NVARCHAR(100);
    DECLARE @result NVARCHAR(300) = N'';

    DECLARE OpCur CURSOR LOCAL STATIC FOR
        SELECT Наименование_операции
        FROM ВЫПОЛНЕНИЕ
        WHERE ID_работника = @id;

    OPEN OpCur;

    FETCH NEXT FROM OpCur INTO @op;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @result = @result + RTRIM(@op) + N', ';
        FETCH NEXT FROM OpCur INTO @op;
    END;

    CLOSE OpCur;
    DEALLOCATE OpCur;

    IF LEN(@result) > 0
        SET @result = LEFT(@result, LEN(@result) - 1);

    RETURN @result;
END;
GO

SELECT ID_работника,
       Фамилия,
       dbo.FWORKER_OPERATIONS(ID_работника) AS Операции
FROM РАБОТНИКИ;
GO

-- Задание 5
DROP FUNCTION IF EXISTS dbo.FOPERATIONS_BY_COMPLEXITY;
GO

CREATE FUNCTION dbo.FOPERATIONS_BY_COMPLEXITY
(
    @complexity NVARCHAR(50) = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT Наименование_операции,
           Признак_сложности
    FROM ОПЕРАЦИИ
    WHERE @complexity IS NULL OR Признак_сложности = @complexity
);
GO

SELECT *
FROM dbo.FOPERATIONS_BY_COMPLEXITY(DEFAULT);
GO

SELECT *
FROM dbo.FOPERATIONS_BY_COMPLEXITY(N'Высокая');
GO

-- Задание 5
DROP FUNCTION IF EXISTS dbo.FWORKER_REPORT;
GO

CREATE FUNCTION dbo.FWORKER_REPORT
(
    @id INT = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT R.ID_работника,
           R.Фамилия,
           R.Стаж,
           V.Наименование_операции,
           V.Количество_деталей,
           V.Дата
    FROM РАБОТНИКИ R
    INNER JOIN ВЫПОЛНЕНИЕ V ON R.ID_работника = V.ID_работника
    WHERE @id IS NULL OR R.ID_работника = @id
);
GO

SELECT *
FROM dbo.FWORKER_REPORT(DEFAULT);
GO

SELECT *
FROM dbo.FWORKER_REPORT(1);
GO