-- Задание 1.
USE UNIVER
GO

SELECT a.AUDITORIUM AS [Код аудитории],
       at.AUDITORIUM_TYPENAME AS [Наименование типа аудитории]
FROM AUDITORIUM a
INNER JOIN AUDITORIUM_TYPE at ON a.AUDITORIUM_TYPE = at.AUDITORIUM_TYPE;
GO

-- Задание 2.
SELECT a.AUDITORIUM AS [Код аудитории],
       at.AUDITORIUM_TYPENAME AS [Наименование типа аудитории]
FROM AUDITORIUM a
INNER JOIN AUDITORIUM_TYPE at ON a.AUDITORIUM_TYPE = at.AUDITORIUM_TYPE
WHERE at.AUDITORIUM_TYPENAME LIKE N'%компьютер%';
GO

-- Задание 3.
SELECT f.FACULTY_NAME AS Факультет,
       p.PULPIT_NAME AS Кафедра,
       pr.PROFESSION_NAME AS Специальность,
       s.SUBJECT_NAME AS Дисциплина,
       st.NAME AS [Имя студента],
       CASE pg.NOTE
           WHEN 6 THEN 'шесть'
           WHEN 7 THEN 'семь'
           WHEN 8 THEN 'восемь'
       END AS Оценка
FROM PROGRESS pg
INNER JOIN STUDENT st ON pg.IDSTUDENT = st.IDSTUDENT
INNER JOIN GROUPS g ON st.IDGROUP = g.IDGROUP
INNER JOIN PROFESSION pr ON g.PROFESSION = pr.PROFESSION
INNER JOIN FACULTY f ON pr.FACULTY = f.FACULTY
INNER JOIN SUBJECT s ON pg.SUBJECT = s.SUBJECT
INNER JOIN PULPIT p ON s.PULPIT = p.PULPIT
WHERE pg.NOTE BETWEEN 6 AND 8
ORDER BY pg.NOTE DESC;
GO

-- Задание 4.
SELECT p.PULPIT_NAME AS Кафедра,
       ISNULL(t.TEACHER_NAME, '***') AS Преподаватель
FROM PULPIT p
LEFT OUTER JOIN TEACHER t ON p.PULPIT = t.PULPIT
ORDER BY p.PULPIT_NAME;
GO


-- Задание 5.
-- 1. Данные левой таблицы (PULPIT), которых нет в правой (TEACHER)
--    Т.е. кафедры, на которых нет преподавателей
SELECT p.PULPIT_NAME AS Кафедра, 
       'нет преподавателей' AS Информация
FROM PULPIT p
LEFT OUTER JOIN TEACHER t ON p.PULPIT = t.PULPIT
WHERE t.TEACHER IS NULL;
GO

-- 2. Данные правой таблицы (TEACHER), которых нет в левой (PULPIT)
--    Т.е. преподаватели, которые не привязаны ни к одной кафедре
--    (такого не должно быть из-за внешнего ключа, но для демонстрации)
SELECT t.TEACHER_NAME AS Преподаватель,
       'нет на кафедре' AS Информация
FROM PULPIT p
RIGHT OUTER JOIN TEACHER t ON p.PULPIT = t.PULPIT
WHERE p.PULPIT IS NULL;
GO

-- 3. Данные, которые есть и там, и там (пересечение)
--    Т.е. преподаватели, привязанные к кафедрам, и сами кафедры
SELECT p.PULPIT_NAME AS Кафедра,
       t.TEACHER_NAME AS Преподаватель
FROM PULPIT p
INNER JOIN TEACHER t ON p.PULPIT = t.PULPIT
ORDER BY p.PULPIT_NAME, t.TEACHER_NAME;
GO

-- Задание 6.
SELECT a.AUDITORIUM AS [Код аудитории],
       at.AUDITORIUM_TYPENAME AS [Наименование типа аудитории]
FROM AUDITORIUM a
CROSS JOIN AUDITORIUM_TYPE at
WHERE a.AUDITORIUM_TYPE = at.AUDITORIUM_TYPE
ORDER BY a.AUDITORIUM;
GO