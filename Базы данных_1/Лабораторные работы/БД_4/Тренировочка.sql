USE UNIVER
SELECT
FACULTY.FACULTY_NAME,
SUBJECT.SUBJECT_NAME
FROM SUBJECT
INNER JOIN PULPIT ON PULPIT.PULPIT = SUBJECT.PULPIT
INNER JOIN FACULTY ON FACULTY.FACULTY = PULPIT.FACULTY WHERE FACULTY_NAME = 'Информационных технологий'


USE UNIVER
SELECT
STUDENT.NAME,
FACULTY.FACULTY_NAME
FROM STUDENT
INNER JOIN GROUPS ON STUDENT.IDGROUP = GROUPS.IDGROUP
INNER JOIN FACULTY ON GROUPS.FACULTY = FACULTY.FACULTY


USE UNIVER
SELECT
SUBJECT.SUBJECT_NAME,
PULPIT.PULPIT_NAME
FROM SUBJECT
INNER JOIN PULPIT ON SUBJECT.PULPIT = PULPIT.PULPIT WHERE PULPIT_NAME = 'Информационных систем и технологий'

USE UNIVER
SELECT
PROGRESS.NOTE,
STUDENT.NAME,
FACULTY.FACULTY_NAME
FROM PROGRESS
INNER JOIN STUDENT ON PROGRESS.IDSTUDENT = STUDENT.IDSTUDENT
INNER JOIN GROUPS ON STUDENT.IDGROUP = GROUPS.IDGROUP
INNER JOIN FACULTY ON GROUPS.FACULTY = FACULTY.FACULTY WHERE FACULTY_NAME = 'Инженерно-экономический'



USE UNIVER
SELECT 
TEACHER.TEACHER,
TEACHER.TEACHER_NAME,
TEACHER.GENDER,
TEACHER.PULPIT
FROM TEACHER
WHERE PULPIT IN (
	SELECT PULPIT
	FROM PULPIT
	WHERE PULPIT= 'ИСиТ'
);

-- вывести все имена аудитории используя аудитории
SELECT 
AUDITORIUM.AUDITORIUM_TYPE
FROM AUDITORIUM
WHERE AUDITORIUM_TYPE IN (
	SELECT AUDITORIUM_TYPE
	FROM AUDITORIUM_TYPE
	WHERE AUDITORIUM_TYPENAME = 'Лекционная');

select TEACHER.TEACHER_NAME
from TEACHER
where TEACHER.PULPIT in (
 select PULPIT.PULPIT
 FROM PULPIT
 where PULPIT.PULPIT_NAME like '%технолог%'
)

USE UNIVER
SELECT 
PROGRESS.NOTE
FROM PROGRESS																			
WHERE IDSTUDENT IN (
	SELECT STUDENT.IDSTUDENT
	FROM STUDENT
	WHERE STUDENT.IDSTUDENT = 1008
);

SELECT 
NAME
FROM STUDENT
WHERE IDSTUDENT IN (
    SELECT IDSTUDENT
    FROM PROGRESS
    WHERE SUBJECT = 'ОАиП'
);



SELECT PULPIT.PULPIT_NAME
FROM PULPIT
WHERE PULPIT.FACULTY IN (
	SELECT FACULTY.FACULTY
	FROM FACULTY
	WHERE FACULTY.FACULTY_NAME like '%технологий%'
)



-- Вывести названия аудиторий (AUDITORIUM_NAME), тип которых имеет вместимость больше 50.
SELECT AUDITORIUM_NAME AS [Название аудитории]
FROM AUDITORIUM
WHERE AUDITORIUM_TYPE IN (
    SELECT DISTINCT AUDITORIUM_TYPE
    FROM AUDITORIUM
    WHERE AUDITORIUM_CAPACITY > 50
);


-- Вывести имена студентов (NAME), которые получили оценку 9 или 10 по любому предмету.
SELECT STUDENT.NAME
FROM STUDENT
WHERE STUDENT.IDSTUDENT IN (
    SELECT IDSTUDENT
    FROM PROGRESS
    WHERE PROGRESS.NOTE = '9'
)

-- Вывести названия всех факультетов (FACULTY_NAME), у которых код начинается на букву 'И'.
SELECT FACULTY.FACULTY_NAME
FROM FACULTY
WHERE FACULTY.FACULTY IN (
	SELECT FACULTY.FACULTY
	FROM FACULTY
	WHERE FACULTY like '%И%')

-- Вывести имена преподавателей (TEACHER_NAME), которые работают на кафедре "Информационных систем и технологий" (ИСиТ). Использовать подзапрос с IN.
SELECT TEACHER.TEACHER_NAME
FROM TEACHER
WHERE TEACHER.PULPIT IN (
	SELECT PULPIT.PULPIT
	FROM PULPIT
	WHERE PULPIT.PULPIT_NAME like '%Информационных%')

select AUDITORIUM.AUDITORIUM_TYPE as [тип],
max(AUDITORIUM.AUDITORIUM_CAPACITY) as [макс вместимость]
from AUDITORIUM
group by AUDITORIUM.AUDITORIUM_TYPE









-- Курсоры
-- взять конкретного тсудента и повысить его отметку на 1
USE UNIVER;
GO 

DECLARE @NOTE INT, @IDST INT = 1000;
DECLARE cur CURSOR FOR 
	SELECT NOTE
	FROM PROGRESS
	WHERE IDSTUDENT = @IDST
	FOR UPDATE OF NOTE;

OPEN cur;
FETCH NEXT FROM cur INTO @NOTE;

WHILE @@FETCH_STATUS = 0
BEGIN
	UPDATE PROGRESS
	SET NOTE = NOTE + 1
	WHERE CURRENT OF cur;
	FETCH NEXT FROM cur INTO @NOTE;
END;

CLOSE cur;
DEALLOCATE cur;
GO



-- перевести на другую кафедру
DECLARE @TEACH CHAR(10) = 'РМНВ' , @PULP CHAR(10);
DECLARE cur1 CURSOR FOR
	SELECT PULPIT
	FROM TEACHER
	WHERE TEACHER = @TEACH
	FOR UPDATE OF PULPIT;

OPEN cur1;
FETCH NEXT FROM cur1 INTO @PULP;

WHILE @@FETCH_STATUS = 0
BEGIN
	UPDATE TEACHER
	SET PULPIT = 'ТЛ'
	WHERE CURRENT OF cur1;
	FETCH NEXT FROM cur1 INTO @PULP;
END;

CLOSE cur1;
DEALLOCATE cur1;
GO


---курсор который выведет всех студентов с опр диапазоном оценок на экзамене
DECLARE @NOTE INT, @IDST INT;
DECLARE cur2 CURSOR FOR
	SELECT IDSTUDENT
	FROM PROGRESS
    WHERE PROGRESS.NOTE BETWEEN 4 AND 8;

OPEN cur2;
FETCH NEXT FROM cur2 INTO @IDST;

WHILE @@FETCH_STATUS = 0
BEGIN
	PRINT @IDST;
	FETCH NEXT FROM cur2 INTO @IDST;
END;

CLOSE cur2;
DEALLOCATE cur2;
GO


--Написать курсор, который выводит все дисциплины кафедры ИСиТ
DECLARE @SUBJ CHAR(10), @PULP CHAR(10) = 'ИСиТ';
DECLARE cur3 CURSOR FOR 
    SELECT SUBJECT
    FROM SUBJECT
    WHERE PULPIT = @PULP;

OPEN cur3;
FETCH NEXT FROM cur3 INTO @SUBJ;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT @SUBJ
    FETCH NEXT FROM cur3 INTO @SUBJ;
END;

CLOSE cur3;
DEALLOCATE cur3;
GO







INSERT INTO PROGRESS (SUBJECT, IDSTUDENT, PDATE, NOTE)
VALUES ('СУБД', 1002, '2026-01-15', 9);

-- Процедуры
-- вывести все экзамены в период какого-то месяца
USE UNIVER;
GO

CREATE or ALTER PROCEDURE ExamsByMonth12
    @month INT
AS
BEGIN
    SELECT SUBJECT, IDSTUDENT, PDATE, NOTE
    FROM PROGRESS
    WHERE MONTH(PDATE) = @month;
END;
GO

EXEC ExamsByMonth12 1;
GO
USE UNIVER;
GO



--Создать/изменить процедуру, которая принимает предмет,
--ID студента, дату экзамена и оценку, а потом вставляет
--эти данные в таблицу PROGRESS
USE UNIVER;
GO

CREATE or ALTER PROCEDURE pro1
    @sub CHAR(10),
    @id INT,
    @pdate DATE,
    @note INT
AS
BEGIN
    INSERT INTO PROGRESS (SUBJECT, IDSTUDENT, PDATE, NOTE)
    VALUES (@sub, @id, @pdate, @note);
END;
GO

EXEC pro1 'ОАиП', 1005, '2024-01-12', 4;
GO

SELECT SUBJECT, IDSTUDENT, PDATE, NOTE
FROM PROGRESS
WHERE IDSTUDENT = 1005;
GO


-- вывести максимальную вместимость всех типов аудиторий
USE UNIVER;
GO

CREATE or ALTER PROCEDURE MaxCapacity1
    @type char(10)
AS
BEGIN
	SELECT AUDITORIUM_TYPE, MAX(AUDITORIUM_CAPACITY)
	FROM AUDITORIUM
    where AUDITORIUM_TYPE = @type
	GROUP BY AUDITORIUM_TYPE;
END;
GO

EXEC MaxCapacity1 'ЛБ-К';
GO

-- меняет всех преподов с одной кафедры на другую
USE UNIVER;
GO

CREATE or ALTER PROCEDURE ChangeOneTeacherPulpit1
    @teacher CHAR(10),
    @newPulpit CHAR(20)
AS
BEGIN
    UPDATE TEACHER
    SET PULPIT = @newPulpit
    WHERE TEACHER = @teacher;
END;
GO

EXEC ChangeOneTeacherPulpit
    @teacher = 'РМНВ',
    @newPulpit = 'ИСиТ';
GO



--Задание П1. Создать процедуру, которая принимает код
--группы и выводит всех студентов этой группы (IDSTUDENT, NAME, BDAY).


CREATE OR ALTER PROCEDURE FFF
    @code_group INT
AS
BEGIN
    SELECT IDSTUDENT, NAME, BDAY
    FROM STUDENT
    WHERE IDGROUP = @code_group
END;
GO

EXEC FFF
    @code_group = 22;


-- Создайте процедуру GetTeachersByGender,
--которая принимает на вход пол преподавателя 
--('м' или 'ж') и выводит список всех преподавателей этого пола.
CREATE OR ALTER PROCEDURE GetTeachersByGender
    @pol CHAR(1)
as
BEGIN
    SELECT TEACHER_NAME
    FROM TEACHER
    WHERE GENDER = @pol
END;
GO

EXEC GetTeachersByGender
    @pol =  'ж';


-- Функции
-- функция выводит максимальную вместимость аудитории определенного типа
USE UNIVER;
GO

CREATE OR ALTER FUNCTION MaxCapacityByType
(
    @type CHAR(10)
)
RETURNS INT
AS
BEGIN
    DECLARE @maxCapacity INT;

    SELECT @maxCapacity = MAX(AUDITORIUM_CAPACITY)
    FROM AUDITORIUM
    WHERE AUDITORIUM_TYPE = @type;

    RETURN @maxCapacity;
END;
GO

SELECT dbo.MaxCapacityByType('ЛК') AS Максимальная_вместимость;
GO

-- вывести студентов, которые закончили учёбу в 2024 году
USE UNIVER;
GO

CREATE OR ALTER FUNCTION StudentsFinishedYear
(
    @year INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT S.IDSTUDENT,
           S.NAME,
           S.IDGROUP,
           G.YEAR_FIRST,
           G.YEAR_FIRST + 4 AS YEAR_FINISH
    FROM STUDENT S
    INNER JOIN GROUPS G ON S.IDGROUP = G.IDGROUP
    WHERE G.YEAR_FIRST + 4 = @year
);
GO

SELECT * FROM dbo.StudentsFinishedYear(2015);
GO

--
CREATE OR ALTER FUNCTION GetTeacherCountByPulpit
    (
    @code_pulpit CHAR(20)
    )
RETURNS INT
AS
BEGIN
    DECLARE @count INT; -- Создаем коробку для результата

    SELECT @count = COUNT(TEACHER_NAME)
    FROM TEACHER
    WHERE PULPIT = @code_pulpit; -- 3. Добавили фильтр по кафедре

    RETURN @count; -- Возвращаем число наружу
END;
GO

SELECT * FROM GetTeacherCountByPulpit('ИСиТ');
GO

-- Создать функцию CountStudentsInGroup, которая 
-- принимает код группы (IDGROUP) и возвращает 
-- количество студентов в этой группе.

CREATE OR ALTER FUNCTION dbo.CountStudentsInGroup
(
    @groupId INT
)
RETURNS INT
AS
BEGIN
    DECLARE @cnt INT;

    SELECT @cnt = COUNT(*)
    FROM dbo.STUDENT
    WHERE IDGROUP = @groupId;

    RETURN @cnt;
END;
GO




CREATE OR ALTER PROCEDURE ExamsBySubject
    @subject CHAR(10)
AS
BEGIN
    SELECT IDSTUDENT, PDATE, NOTE
    FROM PROGRESS
    WHERE SUBJECT = @subject
END;
GO

EXEC ExamsBySubject 'ОАиП';
GO


CREATE OR ALTER FUNCTION GetExamCountByMonth
(
    @month INT
)
RETURNS INT
AS
BEGIN
    DECLARE @count INT;

    SELECT @count = COUNT(*)
    FROM PROGRESS
    WHERE MONTH(PDATE) = @month;

    RETURN @count;
END;
GO

-- вызов
SELECT dbo.GetExamCountByMonth(1) AS 'Экзаменов в январе';
GO



-- Процедура
USE UNIVER;
GO

CREATE or ALTER PROCEDURE ExamsByMonth12
    @month INT
AS
BEGIN
    SELECT SUBJECT, IDSTUDENT, PDATE, NOTE
    FROM PROGRESS
    WHERE MONTH(PDATE) = @month;
END;
GO

EXEC ExamsByMonth12 1;
GO

-- вывести максимальную вместимость всех типов аудиторий
USE UNIVER;
GO

CREATE or ALTER PROCEDURE MaxCapacity1
    @type char(10)
AS
BEGIN
	SELECT AUDITORIUM_TYPE, MAX(AUDITORIUM_CAPACITY)
	FROM AUDITORIUM
    where AUDITORIUM_TYPE = @type
	GROUP BY AUDITORIUM_TYPE;
END;
GO

EXEC MaxCapacity1 'ЛБ-К';
GO

-- Функция
CREATE OR ALTER FUNCTION GetExamCountByMonth
(
    @month INT
)
RETURNS INT
AS
BEGIN
    DECLARE @count INT;

    SELECT @count = COUNT(*)
    FROM PROGRESS
    WHERE MONTH(PDATE) = @month;

    RETURN @count;
END;
GO

-- вызов
SELECT dbo.GetExamCountByMonth(1) AS 'Экзаменов в январе';
GO

-- функция выводит максимальную вместимость аудитории определенного типа
USE UNIVER;
GO

CREATE OR ALTER FUNCTION MaxCapacityByType
(
    @type CHAR(10)
)
RETURNS INT
AS
BEGIN
    DECLARE @maxCapacity INT;

    SELECT @maxCapacity = MAX(AUDITORIUM_CAPACITY)
    FROM AUDITORIUM
    WHERE AUDITORIUM_TYPE = @type;

    RETURN @maxCapacity;
END;
GO

SELECT dbo.MaxCapacityByType('ЛК') AS Максимальная_вместимость;
GO