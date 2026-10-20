USE UNIVER;
GO

-- ЗАДАНИЕ 1. Объявление и инициализация переменных
DECLARE @a char(4) = 'БГТУ',
        @b varchar(20) = 'лабораторная 9',
        @c datetime,
        @t time,
        @i int,
        @s smallint,
        @ti tinyint,
        @n numeric(12, 5);

SET @c = GETDATE();
SET @t = SYSDATETIME();
SET @i = 125;

SELECT @s = 20,
       @ti = 9,
       @n = 12345.67891;

SELECT @a AS 'char',
       @b AS 'varchar',
       @c AS 'datetime',
       @t AS 'time';

PRINT 'int = ' + CAST(@i AS varchar(10));
PRINT 'smallint = ' + CAST(@s AS varchar(10));
PRINT 'tinyint = ' + CAST(@ti AS varchar(10));
PRINT 'numeric(12,5) = ' + CAST(@n AS varchar(20));
GO

-- ЗАДАНИЕ 2. Анализ вместимости аудиторий
DECLARE @y1 numeric(8, 2) = (SELECT CAST(SUM(AUDITORIUM_CAPACITY) AS numeric(8, 2)) FROM dbo.AUDITORIUM),
        @y2 numeric(8, 2),
        @y3 numeric(8, 2),
        @y4 numeric(8, 2),
        @y5 numeric(8, 2);

IF @y1 > 200
BEGIN
    SELECT @y2 = (SELECT CAST(COUNT(*) AS numeric(8, 2)) FROM dbo.AUDITORIUM),
           @y3 = (SELECT CAST(AVG(AUDITORIUM_CAPACITY) AS numeric(8, 2)) FROM dbo.AUDITORIUM);

    SET @y4 = (SELECT CAST(COUNT(*) AS numeric(8, 2))
               FROM dbo.AUDITORIUM
               WHERE AUDITORIUM_CAPACITY < @y3);

    SET @y5 = @y4 / @y2 * 100;

    SELECT @y1 AS 'общая вместимость',
           @y2 AS 'количество аудиторий',
           @y3 AS 'средняя вместимость',
           @y4 AS 'количество аудиторий меньше средней',
           @y5 AS 'процент аудиторий меньше средней';
END
ELSE
    PRINT 'общая вместимость аудиторий меньше 200: ' + CAST(@y1 AS varchar(20));
GO

-- ЗАДАНИЕ 3. Глобальные переменные
SELECT TOP (5) IDSTUDENT, NAME FROM dbo.STUDENT;

PRINT '@@ROWCOUNT = ' + CAST(@@ROWCOUNT AS varchar(10));
PRINT '@@VERSION = ' + CAST(@@VERSION AS varchar(4000));
PRINT '@@SPID = ' + CAST(@@SPID AS varchar(10));
PRINT '@@ERROR = ' + CAST(@@ERROR AS varchar(10));
PRINT '@@SERVERNAME = ' + CAST(@@SERVERNAME AS varchar(100));
PRINT '@@TRANCOUNT = ' + CAST(@@TRANCOUNT AS varchar(10));
PRINT '@@FETCH_STATUS = ' + CAST(@@FETCH_STATUS AS varchar(10));
PRINT '@@NESTLEVEL = ' + CAST(@@NESTLEVEL AS varchar(10));
GO

-- ЗАДАНИЕ 4. Вычисление значений по условию + работа со строками и датами

-- Часть 1: Вычисление z по формуле
DECLARE @t_var float,
        @x_var float,
        @z_var float;

-- Случай 1: t > x
SET @t_var = 5;
SET @x_var = 3;

IF @t_var > @x_var
    SET @z_var = POWER(SIN(@t_var), 2);
ELSE IF @t_var < @x_var
    SET @z_var = 4 * (@t_var + @x_var);
ELSE
    SET @z_var = 1 - EXP(-POWER(@x_var, 2));

PRINT 'Случай 1 (t > x): t=' + CAST(@t_var AS varchar(10)) + ', x=' + CAST(@x_var AS varchar(10));
PRINT 'z = sin^2(t) = ' + CAST(@z_var AS varchar(30));

-- Случай 2: t < x
SET @t_var = 2;
SET @x_var = 6;

IF @t_var > @x_var
    SET @z_var = POWER(SIN(@t_var), 2);
ELSE IF @t_var < @x_var
    SET @z_var = 4 * (@t_var + @x_var);
ELSE
    SET @z_var = 1 - EXP(-POWER(@x_var, 2));

PRINT 'Случай 2 (t < x): t=' + CAST(@t_var AS varchar(10)) + ', x=' + CAST(@x_var AS varchar(10));
PRINT 'z = 4(t+x) = ' + CAST(@z_var AS varchar(30));

-- Случай 3: t = x
SET @t_var = 4;
SET @x_var = 4;

IF @t_var > @x_var
    SET @z_var = POWER(SIN(@t_var), 2);
ELSE IF @t_var < @x_var
    SET @z_var = 4 * (@t_var + @x_var);
ELSE
    SET @z_var = 1 - EXP(-POWER(@x_var, 2));

PRINT 'Случай 3 (t = x): t=' + CAST(@t_var AS varchar(10)) + ', x=' + CAST(@x_var AS varchar(10));
PRINT 'z = 1 - e^(-x^2) = ' + CAST(@z_var AS varchar(30));

-- Часть 2: Преобразование ФИО в сокращенное
DECLARE @fullname NVARCHAR(100) = 'Крылов Олег Павлович';

DECLARE @space1 INT = CHARINDEX(' ', @fullname);                    
DECLARE @space2 INT = CHARINDEX(' ', @fullname, @space1 + 1);      

DECLARE @lastname NVARCHAR(50) = SUBSTRING(@fullname, 1, @space1 - 1);    
DECLARE @firstletter NVARCHAR(1) = SUBSTRING(@fullname, @space1 + 1, 1);  
DECLARE @middleletter NVARCHAR(1) = SUBSTRING(@fullname, @space2 + 1, 1);  

SELECT 
    @fullname AS 'полное ФИО',
    @lastname + ' ' + @firstletter + '. ' + @middleletter + '.' AS 'сокращенное ФИО';
-- Часть 3: Поиск студентов с днём рождения в следующем месяце
SELECT 
    IDSTUDENT AS 'код студента',
    NAME AS 'ФИО',
    BDAY AS 'дата рождения',
    DATEDIFF(year, BDAY, GETDATE()) -
        CASE
            WHEN DATEADD(year, DATEDIFF(year, BDAY, GETDATE()), BDAY) > GETDATE()
                THEN 1
            ELSE 0
        END AS 'возраст'
FROM dbo.STUDENT
WHERE MONTH(BDAY) = MONTH(DATEADD(month, 1, GETDATE()));

-- Часть 4: Поиск дня недели экзамена по БД
SET DATEFIRST 1;

SELECT 
    G.IDGROUP AS 'группа',
    S.NAME AS 'студент',
    P.PDATE AS 'дата экзамена',
    DATENAME(weekday, P.PDATE) AS 'день недели',
    P.NOTE AS 'оценка'
FROM dbo.PROGRESS P
     INNER JOIN dbo.STUDENT S ON P.IDSTUDENT = S.IDSTUDENT
     INNER JOIN dbo.GROUPS G ON S.IDGROUP = G.IDGROUP
     INNER JOIN dbo.SUBJECT SUB ON P.SUBJECT = SUB.SUBJECT
WHERE SUB.SUBJECT_NAME = 'Базы данных' OR SUB.SUBJECT_NAME = 'БД';
GO

-- ЗАДАНИЕ 5. Конструкция IF/ELSE
DECLARE @high_marks int = (SELECT COUNT(*) FROM dbo.PROGRESS WHERE NOTE >= 8);

PRINT 'Количество оценок >= 8: ' + CAST(@high_marks AS varchar(10));

IF @high_marks > 3
BEGIN
    PRINT 'количество высоких оценок больше 3';
    PRINT 'количество = ' + CAST(@high_marks AS varchar(10));
END
ELSE
BEGIN
    PRINT 'количество высоких оценок меньше или равно 3';
    PRINT 'количество = ' + CAST(@high_marks AS varchar(10));
END;
GO

-- ЗАДАНИЕ 6. Анализ оценок студентов с помощью CASE
SELECT 
    F.FACULTY_NAME AS 'факультет',
    CASE
        WHEN P.NOTE BETWEEN 9 AND 10 THEN 'отлично'
        WHEN P.NOTE BETWEEN 7 AND 8 THEN 'хорошо'
        WHEN P.NOTE BETWEEN 4 AND 6 THEN 'удовлетворительно'
        ELSE 'неудовлетворительно'
    END AS 'оценка словами',
    COUNT(*) AS 'Количество'
FROM dbo.PROGRESS P
     INNER JOIN dbo.STUDENT S ON P.IDSTUDENT = S.IDSTUDENT
     INNER JOIN dbo.GROUPS G ON S.IDGROUP = G.IDGROUP
     INNER JOIN dbo.FACULTY F ON G.FACULTY = F.FACULTY
GROUP BY F.FACULTY_NAME,
         CASE
             WHEN P.NOTE BETWEEN 9 AND 10 THEN 'отлично'
             WHEN P.NOTE BETWEEN 7 AND 8 THEN 'хорошо'
             WHEN P.NOTE BETWEEN 4 AND 6 THEN 'удовлетворительно'
             ELSE 'неудовлетворительно'
         END;
GO

-- ЗАДАНИЕ 7. Временная таблица и цикл WHILE
CREATE TABLE #LAB9
(
    TIND int,
    TFIELD nvarchar(200),
    TDATE datetime
);

DECLARE @j int = 1;

WHILE @j <= 10
BEGIN
    INSERT #LAB9(TIND, TFIELD, TDATE)
    VALUES (@j, REPLICATE(N'строка ', @j), DATEADD(day, @j, GETDATE()));

    PRINT 'добавлена строка: ' + CAST(@j AS varchar(10));
    SET @j = @j + 1;
END;

SELECT * FROM #LAB9;
DROP TABLE #LAB9;
GO

-- ЗАДАНИЕ 8. Оператор RETURN
DECLARE @r int = 1;

PRINT 'Начало выполнения...';
PRINT @r + 1;
PRINT @r + 2;
RETURN;
PRINT @r + 3;
GO

-- ЗАДАНИЕ 9. Обработка ошибок TRY/CATCH
BEGIN TRY
    DECLARE @bad int;
    SET @bad = CAST('ошибка' AS int);
    PRINT 'не выполнится ';
END TRY
BEGIN CATCH
    PRINT 'ERROR_NUMBER = ' + CAST(ERROR_NUMBER() AS varchar(10));
    PRINT 'ERROR_MESSAGE = ' + ERROR_MESSAGE();
    PRINT 'ERROR_LINE = ' + CAST(ERROR_LINE() AS varchar(10));
    PRINT 'ERROR_PROCEDURE = ' + ISNULL(ERROR_PROCEDURE(), 'NULL');
    PRINT 'ERROR_SEVERITY = ' + CAST(ERROR_SEVERITY() AS varchar(10));
    PRINT 'ERROR_STATE = ' + CAST(ERROR_STATE() AS varchar(10));
END CATCH;
GO

PRINT 'Лабораторная работа №9 выполнена!';
GO