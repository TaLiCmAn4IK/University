USE PVR_MyBase_1;
GO

-- Задание 1
DECLARE @op NVARCHAR(100);
DECLARE @list NVARCHAR(MAX) = N'';

DECLARE OpCur CURSOR LOCAL FOR
    SELECT Наименование_операции
    FROM ОПЕРАЦИИ
    WHERE Признак_сложности = N'Высокая';

OPEN OpCur;
FETCH NEXT FROM OpCur INTO @op;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @list = @list + RTRIM(@op) + N', ';
    FETCH NEXT FROM OpCur INTO @op;
END;

CLOSE OpCur;
DEALLOCATE OpCur;

IF LEN(@list) > 0
    SET @list = LEFT(@list, LEN(@list) - 1);

PRINT N'Операции высокой сложности: ' + @list;
GO

-- Задание 2
DECLARE RabCur CURSOR LOCAL FOR
    SELECT Фамилия, Стаж
    FROM РАБОТНИКИ;

DECLARE @fam NVARCHAR(50), @st INT;

OPEN RabCur;
FETCH NEXT FROM RabCur INTO @fam, @st;
PRINT N'Локальный курсор: ' + @fam + N', стаж ' + CAST(@st AS NVARCHAR(10));
GO

PRINT N'Статус локального курсора после GO: ' + CAST(CURSOR_STATUS('local', 'RabCur') AS NVARCHAR(10));
GO

DECLARE RabCur CURSOR GLOBAL FOR
    SELECT Фамилия, Стаж
    FROM РАБОТНИКИ;

DECLARE @fam NVARCHAR(50), @st INT;

OPEN RabCur;
FETCH NEXT FROM RabCur INTO @fam, @st;
PRINT N'Глобальный курсор, первая строка: ' + @fam;
GO

DECLARE @fam NVARCHAR(50), @st INT;

FETCH NEXT FROM RabCur INTO @fam, @st;
PRINT N'Глобальный курсор после GO, следующая строка: ' + @fam;

CLOSE RabCur;
DEALLOCATE RabCur;
GO

-- Задание 3
BEGIN TRANSACTION;

DECLARE @id INT, @fam NVARCHAR(50);

DECLARE StRab CURSOR LOCAL STATIC FOR
    SELECT ID_работника, Фамилия
    FROM РАБОТНИКИ;

OPEN StRab;
PRINT N'Количество строк в статическом курсоре: ' + CAST(@@CURSOR_ROWS AS NVARCHAR(10));

UPDATE РАБОТНИКИ
SET Фамилия = N'### ИЗМЕНЁН ###'
WHERE ID_работника = 1;

IF NOT EXISTS (SELECT 1 FROM РАБОТНИКИ WHERE ID_работника = 99)
BEGIN
    INSERT INTO РАБОТНИКИ (ID_работника, Фамилия, Имя, Отчество, Адрес, Телефон, Стаж)
    VALUES (99, N'Новиков', N'Иван', N'Петрович', N'ул. Новая 1', N'999-99', 1);
END;

FETCH NEXT FROM StRab INTO @id, @fam;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT CAST(@id AS NVARCHAR(10)) + N' ' + @fam;
    FETCH NEXT FROM StRab INTO @id, @fam;
END;

CLOSE StRab;
DEALLOCATE StRab;

ROLLBACK;
GO

-- Задание 4
DECLARE @n INT, @fam NVARCHAR(50);

DECLARE ScrRab CURSOR LOCAL DYNAMIC SCROLL FOR
    SELECT ROW_NUMBER() OVER (ORDER BY Фамилия) AS N, Фамилия
    FROM РАБОТНИКИ;

OPEN ScrRab;

FETCH FIRST FROM ScrRab INTO @n, @fam;
PRINT N'FIRST: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @fam;

FETCH NEXT FROM ScrRab INTO @n, @fam;
PRINT N'NEXT: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @fam;

FETCH PRIOR FROM ScrRab INTO @n, @fam;
PRINT N'PRIOR: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @fam;

FETCH LAST FROM ScrRab INTO @n, @fam;
PRINT N'LAST: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @fam;

FETCH ABSOLUTE 3 FROM ScrRab INTO @n, @fam;
PRINT N'ABSOLUTE 3: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @fam;

FETCH RELATIVE 2 FROM ScrRab INTO @n, @fam;
PRINT N'RELATIVE 2: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @fam;

FETCH RELATIVE -1 FROM ScrRab INTO @n, @fam;
PRINT N'RELATIVE -1: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @fam;

CLOSE ScrRab;
DEALLOCATE ScrRab;
GO

-- Задание 5
BEGIN TRANSACTION;

DECLARE @idv INT, @qty INT;

DECLARE VypCur CURSOR LOCAL FOR
    SELECT ID_выполнения, Количество_деталей
    FROM ВЫПОЛНЕНИЕ;

OPEN VypCur;
FETCH NEXT FROM VypCur INTO @idv, @qty;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF @qty < 8
        DELETE FROM ВЫПОЛНЕНИЕ
        WHERE ID_выполнения = @idv;
    ELSE
        UPDATE ВЫПОЛНЕНИЕ
        SET Количество_деталей = Количество_деталей + 1
        WHERE ID_выполнения = @idv;

    FETCH NEXT FROM VypCur INTO @idv, @qty;
END;

CLOSE VypCur;
DEALLOCATE VypCur;

SELECT ID_выполнения, Наименование_операции, Количество_деталей, Дата, ID_работника
FROM ВЫПОЛНЕНИЕ
ORDER BY ID_выполнения;

ROLLBACK;
GO

-- Задание 6
BEGIN TRANSACTION;

DECLARE @idv INT, @qty INT, @fam NVARCHAR(50), @st INT, @op NVARCHAR(100);

DECLARE DelV CURSOR LOCAL FOR
    SELECT V.ID_выполнения, V.Количество_деталей, R.Фамилия, R.Стаж, O.Наименование_операции
    FROM ВЫПОЛНЕНИЕ V
    INNER JOIN РАБОТНИКИ R ON V.ID_работника = R.ID_работника
    INNER JOIN ОПЕРАЦИИ O ON V.Наименование_операции = O.Наименование_операции
    WHERE R.Стаж < 3;

OPEN DelV;
FETCH NEXT FROM DelV INTO @idv, @qty, @fam, @st, @op;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT N'Удаляется: ' + @fam + N', стаж ' + CAST(@st AS NVARCHAR(10)) + N', ' + @op + N' = ' + CAST(@qty AS NVARCHAR(10));

    DELETE FROM ВЫПОЛНЕНИЕ
    WHERE ID_выполнения = @idv;

    FETCH NEXT FROM DelV INTO @idv, @qty, @fam, @st, @op;
END;

CLOSE DelV;
DEALLOCATE DelV;

SELECT ID_выполнения, Наименование_операции, Количество_деталей, Дата, ID_работника
FROM ВЫПОЛНЕНИЕ
ORDER BY ID_выполнения;

ROLLBACK;
GO

-- Задание 6.2
BEGIN TRANSACTION;

DECLARE @target INT = 1;
DECLARE @idv INT, @qty INT;

DECLARE UpdV CURSOR LOCAL FOR
    SELECT ID_выполнения, Количество_деталей
    FROM ВЫПОЛНЕНИЕ
    WHERE ID_работника = @target;

OPEN UpdV;
FETCH NEXT FROM UpdV INTO @idv, @qty;

WHILE @@FETCH_STATUS = 0
BEGIN
    UPDATE ВЫПОЛНЕНИЕ
    SET Количество_деталей = Количество_деталей + 1
    WHERE ID_выполнения = @idv;

    FETCH NEXT FROM UpdV INTO @idv, @qty;
END;

CLOSE UpdV;
DEALLOCATE UpdV;

SELECT ID_выполнения, Наименование_операции, Количество_деталей, Дата, ID_работника
FROM ВЫПОЛНЕНИЕ
WHERE ID_работника = @target
ORDER BY ID_выполнения;

ROLLBACK;
GO
