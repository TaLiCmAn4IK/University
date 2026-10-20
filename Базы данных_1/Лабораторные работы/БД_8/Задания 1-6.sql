USE UNIVER;
GO

DROP VIEW IF EXISTS Преподаватель, Количество_кафедр, Аудитории, Лекционные_аудитории, Дисциплины;   
DELETE FROM AUDITORIUM WHERE AUDITORIUM IN ('999-9');  
GO

-- Задание 1
CREATE VIEW Преподаватель
AS
SELECT
    TEACHER       AS [Код],
    TEACHER_NAME  AS [Имя преподавателя],
    GENDER        AS [Пол],
    PULPIT        AS [Код кафедры]
FROM TEACHER;
GO

SELECT * FROM Преподаватель;
GO

-- Задание 2
CREATE VIEW Количество_кафедр
AS
SELECT
    F.FACULTY_NAME  AS [Факультет],
    COUNT(P.PULPIT) AS [Количество кафедр]
FROM FACULTY AS F
INNER JOIN PULPIT AS P ON F.FACULTY = P.FACULTY
GROUP BY F.FACULTY_NAME;
GO

SELECT * FROM Количество_кафедр;
GO

-- Задание 3
CREATE VIEW Аудитории
AS
SELECT
    AUDITORIUM      AS [Код],
    AUDITORIUM_NAME AS [Наименование аудитории]
FROM AUDITORIUM
WHERE AUDITORIUM_TYPE LIKE 'ЛК%';
GO


SELECT * FROM Аудитории;
INSERT Аудитории VALUES ('999-9', '999-9');
SELECT * FROM Аудитории;                                                    
UPDATE Аудитории SET [Наименование аудитории] = '999-9 (изм)' WHERE [Код] = '999-9';
SELECT * FROM Аудитории;                                                  
DELETE FROM Аудитории WHERE [Код] = '999-9';
SELECT * FROM Аудитории;     

DELETE FROM AUDITORIUM WHERE AUDITORIUM = '999-9'; 
GO 

-- Задание 4
CREATE VIEW Лекционные_аудитории
AS
SELECT
    AUDITORIUM      AS [Код],
    AUDITORIUM_NAME AS [Наименование аудитории]
FROM AUDITORIUM
WHERE AUDITORIUM_TYPE LIKE 'ЛК%'
WITH CHECK OPTION;
GO

SELECT * FROM Лекционные_аудитории;
INSERT Лекционные_аудитории VALUES ('999-9', '999-9');
SELECT * FROM Лекционные_аудитории;
UPDATE Лекционные_аудитории SET [Наименование аудитории] = '999-9 (изм)' WHERE [Код] = '999-9';
SELECT * FROM Лекционные_аудитории;
DELETE FROM Лекционные_аудитории WHERE [Код] = '999-9';
SELECT * FROM Лекционные_аудитории;
GO

-- Задание 5
CREATE VIEW Дисциплины
AS
SELECT TOP 20
    SUBJECT      AS [Код],
    SUBJECT_NAME AS [Наименование дисциплины],
    PULPIT       AS [Код кафедры]
FROM SUBJECT
ORDER BY SUBJECT_NAME;
GO

SELECT * FROM Дисциплины ORDER BY [Наименование дисциплины];
GO

-- Задание 6
ALTER VIEW Количество_кафедр
WITH SCHEMABINDING
AS
SELECT
    F.FACULTY_NAME      AS [Факультет],
    COUNT(P.PULPIT) AS [Количество кафедр]
FROM dbo.FACULTY AS F
INNER JOIN dbo.PULPIT AS P ON F.FACULTY = P.FACULTY
GROUP BY F.FACULTY_NAME;
GO

ALTER TABLE dbo.FACULTY
DROP COLUMN FACULTY_NAME;

SELECT * FROM Количество_кафедр;
GO
