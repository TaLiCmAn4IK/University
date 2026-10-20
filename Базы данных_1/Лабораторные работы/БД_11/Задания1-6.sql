USE UNIVER;
GO

-- Задание 1
DECLARE @subj CHAR(10);
DECLARE @list NVARCHAR(MAX) = N'';

DECLARE SubjCur CURSOR LOCAL FOR
    SELECT SUBJECT
    FROM dbo.SUBJECT
    WHERE PULPIT = 'ИСиТ';

OPEN SubjCur;
FETCH NEXT FROM SubjCur INTO @subj;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @list = @list + RTRIM(@subj) + N', ';
    FETCH NEXT FROM SubjCur INTO @subj;
END;

CLOSE SubjCur;
DEALLOCATE SubjCur;

IF LEN(@list) > 0
    SET @list = LEFT(@list, LEN(@list) - 1);

PRINT N'Дисциплины кафедры ИСиТ: ' + @list;
GO

-- Задание 2
DECLARE TeacherCur CURSOR LOCAL FOR
    SELECT TEACHER_NAME, GENDER
    FROM dbo.TEACHER
    WHERE PULPIT = 'ИСиТ';

DECLARE @name NVARCHAR(100), @g CHAR(1);

OPEN TeacherCur;
FETCH NEXT FROM TeacherCur INTO @name, @g;
PRINT N'Локальный курсор: ' + @name + N' (' + @g + N')';
GO

PRINT N'Статус локального курсора после GO: ' + CAST(CURSOR_STATUS('local', 'TeacherCur') AS NVARCHAR(10));
GO

DECLARE TeacherCur CURSOR GLOBAL FOR
    SELECT TEACHER_NAME, GENDER
    FROM dbo.TEACHER
    WHERE PULPIT = 'ИСиТ';

DECLARE @name NVARCHAR(100), @g CHAR(1);

OPEN TeacherCur;
FETCH NEXT FROM TeacherCur INTO @name, @g;
PRINT N'Глобальный курсор, первая строка: ' + @name;
GO

DECLARE @name NVARCHAR(100), @g CHAR(1);

FETCH NEXT FROM TeacherCur INTO @name, @g;
PRINT N'Глобальный курсор после GO, следующая строка: ' + @name;

CLOSE TeacherCur;
DEALLOCATE TeacherCur;
GO

-- Задание 3
BEGIN TRANSACTION;

DECLARE @sid INT, @nm NVARCHAR(100);

DECLARE StudCur CURSOR LOCAL STATIC FOR
    SELECT IDSTUDENT, NAME
    FROM dbo.STUDENT
    WHERE IDGROUP = 22;

OPEN StudCur;
PRINT N'Количество строк в статическом курсоре: ' + CAST(@@CURSOR_ROWS AS NVARCHAR(10));

UPDATE dbo.STUDENT
SET NAME = N'### ИЗМЕНЁН ###'
WHERE IDSTUDENT = 1000;

INSERT INTO dbo.STUDENT (IDGROUP, NAME, BDAY)
VALUES (22, N'Новиков Иван Петрович', '2000-01-01');

FETCH NEXT FROM StudCur INTO @sid, @nm;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT CAST(@sid AS NVARCHAR(10)) + N' ' + @nm;
    FETCH NEXT FROM StudCur INTO @sid, @nm;
END;

CLOSE StudCur;
DEALLOCATE StudCur;

ROLLBACK;
GO

-- Задание 4
DECLARE @n INT, @nm NVARCHAR(100);

DECLARE ScrollCur CURSOR LOCAL DYNAMIC SCROLL FOR
    SELECT ROW_NUMBER() OVER (ORDER BY NAME) AS N, NAME
    FROM dbo.STUDENT
    WHERE IDGROUP IN (22, 23, 24);

OPEN ScrollCur;

FETCH FIRST FROM ScrollCur INTO @n, @nm;
PRINT N'FIRST: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @nm;

FETCH NEXT FROM ScrollCur INTO @n, @nm;
PRINT N'NEXT: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @nm;

FETCH PRIOR FROM ScrollCur INTO @n, @nm;
PRINT N'PRIOR: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @nm;

FETCH LAST FROM ScrollCur INTO @n, @nm;
PRINT N'LAST: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @nm;

FETCH ABSOLUTE 3 FROM ScrollCur INTO @n, @nm;
PRINT N'ABSOLUTE 3: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @nm;

FETCH RELATIVE 2 FROM ScrollCur INTO @n, @nm;
PRINT N'RELATIVE 2: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @nm;

FETCH RELATIVE -1 FROM ScrollCur INTO @n, @nm;
PRINT N'RELATIVE -1: ' + CAST(@n AS NVARCHAR(10)) + N' ' + @nm;

CLOSE ScrollCur;
DEALLOCATE ScrollCur;
GO

-- Задание 5
BEGIN TRANSACTION;

INSERT INTO dbo.PROGRESS (SUBJECT, IDSTUDENT, PDATE, NOTE)
VALUES ('ОАиП', 1002, '2014-02-01', 3);

DECLARE @subj CHAR(10), @sid INT, @pdate DATE, @note INT;

DECLARE ProgCur CURSOR LOCAL FOR
    SELECT SUBJECT, IDSTUDENT, PDATE, NOTE
    FROM dbo.PROGRESS
    WHERE SUBJECT = 'ОАиП'
    FOR UPDATE OF NOTE;

OPEN ProgCur;
FETCH NEXT FROM ProgCur INTO @subj, @sid, @pdate, @note;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF @note < 4
        DELETE FROM dbo.PROGRESS WHERE CURRENT OF ProgCur;
    ELSE IF @note < 10
        UPDATE dbo.PROGRESS SET NOTE = NOTE + 1 WHERE CURRENT OF ProgCur;

    FETCH NEXT FROM ProgCur INTO @subj, @sid, @pdate, @note;
END;

CLOSE ProgCur;
DEALLOCATE ProgCur;

SELECT SUBJECT, IDSTUDENT, PDATE, NOTE
FROM dbo.PROGRESS
WHERE SUBJECT = 'ОАиП'
ORDER BY IDSTUDENT, PDATE;

ROLLBACK;
GO

-- Задание 6
BEGIN TRANSACTION;

INSERT INTO dbo.PROGRESS (SUBJECT, IDSTUDENT, PDATE, NOTE)
VALUES ('ОАиП', 1002, '2014-02-01', 3);

DECLARE @subj CHAR(10), @sid INT, @pdate DATE, @note INT, @sname NVARCHAR(100), @grp INT;

DECLARE DelCur CURSOR LOCAL FOR
    SELECT P.SUBJECT, P.IDSTUDENT, P.PDATE, P.NOTE, S.NAME, S.IDGROUP
    FROM dbo.PROGRESS P
    INNER JOIN dbo.STUDENT S ON P.IDSTUDENT = S.IDSTUDENT
    WHERE P.NOTE < 4;

OPEN DelCur;
FETCH NEXT FROM DelCur INTO @subj, @sid, @pdate, @note, @sname, @grp;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT N'Удаляется: ' + @sname + N' группа ' + CAST(@grp AS NVARCHAR(10)) + N', ' + RTRIM(@subj) + N' = ' + CAST(@note AS NVARCHAR(10));

    DELETE FROM dbo.PROGRESS
    WHERE SUBJECT = @subj AND IDSTUDENT = @sid AND PDATE = @pdate;

    FETCH NEXT FROM DelCur INTO @subj, @sid, @pdate, @note, @sname, @grp;
END;

CLOSE DelCur;
DEALLOCATE DelCur;

SELECT SUBJECT, IDSTUDENT, PDATE, NOTE
FROM dbo.PROGRESS
ORDER BY IDSTUDENT, SUBJECT, PDATE;

ROLLBACK;
GO

-- Задание 6.2
BEGIN TRANSACTION;

DECLARE @target INT = 1007;
DECLARE @subj CHAR(10), @sid INT, @pdate DATE, @note INT;

DECLARE UpdCur CURSOR LOCAL FOR
    SELECT SUBJECT, IDSTUDENT, PDATE, NOTE
    FROM dbo.PROGRESS
    WHERE IDSTUDENT = @target;

OPEN UpdCur;
FETCH NEXT FROM UpdCur INTO @subj, @sid, @pdate, @note;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF @note < 10
        UPDATE dbo.PROGRESS
        SET NOTE = NOTE + 1
        WHERE SUBJECT = @subj AND IDSTUDENT = @sid AND PDATE = @pdate;

    FETCH NEXT FROM UpdCur INTO @subj, @sid, @pdate, @note;
END;

CLOSE UpdCur;
DEALLOCATE UpdCur;

SELECT SUBJECT, IDSTUDENT, PDATE, NOTE
FROM dbo.PROGRESS
WHERE IDSTUDENT = @target;

ROLLBACK;
GO
