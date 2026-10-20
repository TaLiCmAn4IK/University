USE UNIVER;

DROP TABLE IF EXISTS PROGRESS;
DROP TABLE IF EXISTS STUDENT;
DROP TABLE IF EXISTS GROUPS;
DROP TABLE IF EXISTS SUBJECT;
DROP TABLE IF EXISTS TEACHER;
DROP TABLE IF EXISTS PULPIT;
DROP TABLE IF EXISTS PROFESSION;
DROP TABLE IF EXISTS FACULTY;
DROP TABLE IF EXISTS AUDITORIUM;
DROP TABLE IF EXISTS AUDITORIUM_TYPE;
DROP TABLE IF EXISTS Товары;
DROP TABLE IF EXISTS Заказы;

CREATE TABLE AUDITORIUM_TYPE (
    AUDITORIUM_TYPE CHAR(10) PRIMARY KEY,
    AUDITORIUM_TYPENAME VARCHAR(30)
);

CREATE TABLE AUDITORIUM (
    AUDITORIUM CHAR(20) PRIMARY KEY,
    AUDITORIUM_TYPE CHAR(10) FOREIGN KEY REFERENCES AUDITORIUM_TYPE(AUDITORIUM_TYPE),
    AUDITORIUM_CAPACITY INT CHECK (AUDITORIUM_CAPACITY BETWEEN 1 AND 300),
    AUDITORIUM_NAME VARCHAR(50)
);

INSERT INTO AUDITORIUM_TYPE (AUDITORIUM_TYPE, AUDITORIUM_TYPENAME) VALUES
('ЛБ-Х', 'Химическая лаборатория'),
('ЛБ-К', 'Компьютерный класс'),
('ЛБ-СК', 'Спец. компьютерный класс'),
('ЛК', 'Лекционная'),
('ЛК-К', 'Лекционная с уст. проектором');

INSERT INTO AUDITORIUM (AUDITORIUM, AUDITORIUM_TYPE, AUDITORIUM_CAPACITY, AUDITORIUM_NAME) VALUES
('301-1', 'ЛБ-К', 15, '301-1'),
('304-4', 'ЛБ-К', 90, '304-4'),
('313-1', 'ЛК-К', 60, '313-1'),
('314-4', 'ЛК', 90, '314-4'),
('320-4', 'ЛК', 90, '320-4'),
('324-1', 'ЛК-К', 50, '324-1'),
('413-1', 'ЛБ-К', 15, '413-1'),
('423-1', 'ЛБ-К', 90, '423-1');

CREATE TABLE FACULTY (
    FACULTY CHAR(10) PRIMARY KEY,
    FACULTY_NAME VARCHAR(50) DEFAULT '???'
);

CREATE TABLE PROFESSION (
    PROFESSION CHAR(20) PRIMARY KEY,
    FACULTY CHAR(10) FOREIGN KEY REFERENCES FACULTY(FACULTY),
    PROFESSION_NAME VARCHAR(100),
    QUALIFICATION VARCHAR(50)
);

CREATE TABLE PULPIT (
    PULPIT CHAR(20) PRIMARY KEY,
    PULPIT_NAME VARCHAR(100),
    FACULTY CHAR(10) FOREIGN KEY REFERENCES FACULTY(FACULTY)
);

CREATE TABLE SUBJECT (
    SUBJECT CHAR(10) PRIMARY KEY,
    SUBJECT_NAME VARCHAR(100) UNIQUE,
    PULPIT CHAR(20) FOREIGN KEY REFERENCES PULPIT(PULPIT)
);

CREATE TABLE TEACHER (
    TEACHER CHAR(10) PRIMARY KEY,
    TEACHER_NAME VARCHAR(100),
    GENDER CHAR(1) CHECK (GENDER IN ('м', 'ж')),
    PULPIT CHAR(20) FOREIGN KEY REFERENCES PULPIT(PULPIT)
);

CREATE TABLE GROUPS (
    IDGROUP INT PRIMARY KEY,
    FACULTY CHAR(10) FOREIGN KEY REFERENCES FACULTY(FACULTY),
    PROFESSION CHAR(20) FOREIGN KEY REFERENCES PROFESSION(PROFESSION),
    YEAR_FIRST SMALLINT
);

CREATE TABLE STUDENT (
    IDSTUDENT INT IDENTITY(1000,1) PRIMARY KEY,
    IDGROUP INT FOREIGN KEY REFERENCES GROUPS(IDGROUP),
    NAME NVARCHAR(100),
    BDAY DATE
);

CREATE TABLE PROGRESS (
    SUBJECT CHAR(10) FOREIGN KEY REFERENCES SUBJECT(SUBJECT),
    IDSTUDENT INT FOREIGN KEY REFERENCES STUDENT(IDSTUDENT),
    PDATE DATE,
    NOTE INT CHECK (NOTE BETWEEN 1 AND 10),
    PRIMARY KEY (SUBJECT, IDSTUDENT, PDATE)
);

INSERT INTO FACULTY (FACULTY, FACULTY_NAME) VALUES
('ТТЛП', 'Технологии и техника лесной промышленности'),
('ТОВ', 'Технологии органических веществ'),
('ХТиТ', 'Химические технологии и техника'),
('ИЭФ', 'Инженерно-экономический'),
('ЛХ', 'Лесохозяйственный'),
('ИДиП', 'Издательское дело и полиграфия'),
('ИТ', 'Информационных технологий');

INSERT INTO PROFESSION (PROFESSION, FACULTY, PROFESSION_NAME, QUALIFICATION) VALUES
('1-75 02 01', 'ЛХ', 'Садово-парковое строительство', 'инженер садово-паркового строительства'),
('1-89 02 02', 'ЛХ', 'Туризм и природопользование', 'специалист в сфере туризма'),
('1-46 01 01', 'ТТЛП', 'Лесоинженерное дело', 'инженер-технолог'),
('1-25 01 08', 'ИЭФ', 'Экономика и управление на предприятии', 'экономист-менеджер');

INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES
('РИТ', 'Редакционно-издательских технологий', 'ИДиП'),
('СБУАиА', 'Статистики, бухгалтерского учета, анализа и аудита', 'ИЭФ'),
('ТДП', 'Технологий деревообрабатывающих производств', 'ТТЛП'),
('ТиДИД', 'Технологии и дизайна изделий из древесины', 'ТТЛП'),
('ТиП', 'Туризма и природопользования', 'ЛХ'),
('ТЛ', 'Транспорта леса', 'ТТЛП'),
('ТНВиОХТ', 'Технологии неорганических веществ и общей химической технологии', 'ХТиТ'),
('ТНХСиППМ', 'Технологии нефтехимического синтеза и переработки полимерных материалов', 'ТОВ'),
('ХПД', 'Химической переработки древесины', 'ТОВ'),
('ХТЭПиМЭЕ', 'Химии, технологии электрохимических производств и материалов электронной техники', 'ХТиТ'),
('ЭТиМ', 'Экономической теории и маркетинга', 'ИЭФ'),
('ИСиТ', 'Информационных систем и технологий', 'ИТ');

INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES
('ПЗ', 'Представление знаний в компьютерных системах', 'ИСиТ'),
('ПИС', 'Проектирование информационных систем', 'ИСиТ'),
('ПСП', 'Программирование сетевых приложений', 'ИСиТ'),
('ПЭХ', 'Прикладная электрохимия', 'ХТЭПиМЭЕ'),
('СУБД', 'Системы управления базами данных', 'ИСиТ'),
('ТиОЛ', 'Технология и оборудование лесозаготовок', 'ТЛ'),
('ТРИ', 'Технология резиновых изделий', 'ТНХСиППМ'),
('ЭП', 'Экономика природопользования', 'ТиП'),
('ЭТ', 'Экономическая теория', 'ЭТиМ'),
('ОАиП', 'Основы алгоритмизации и программирования', 'ИСиТ'),
('БД', 'Базы данных', 'ИСиТ');

INSERT INTO TEACHER (TEACHER, TEACHER_NAME, GENDER, PULPIT) VALUES
('НСКВ', 'Носков Михаил Трофимович', NULL, 'ТДП'),
('ПРКП', 'Прокопенко Николай Иванович', NULL, 'ТНХСиППМ'),
('МРЗВ', 'Морозова Елена Степановна', NULL, 'ИСиТ'),
('РВКС', 'Ровкас Андрей Петрович', NULL, 'ТиП'),
('РЖКВ', 'Рыжиков Леонид Николаевич', NULL, 'ТЛ'),
('РМНВ', 'Романов Дмитрий Михайлович', NULL, 'ИСиТ'),
('СМЛВ', 'Смелов Владимир Владиславович', NULL, 'ИСиТ'),
('КРЛВ', 'Крылов Павел Павлович', NULL, 'ИСиТ'),
('ЧРН', 'Чернова Анна Викторовна', NULL, 'ХПД'),
('МХВ', 'Мохов Михаил Сергеевич', NULL, 'ИСиТ');

INSERT INTO GROUPS (IDGROUP, FACULTY, PROFESSION, YEAR_FIRST) VALUES
(22, 'ЛХ', '1-75 02 01', 2011),
(23, 'ЛХ', '1-89 02 02', 2012),
(24, 'ЛХ', '1-89 02 02', 2011),
(25, 'ТТЛП', '1-46 01 01', 2013),
(26, 'ТТЛП', '1-46 01 01', 2020),
(27, 'ТТЛП', '1-46 01 01', 2012),
(28, 'ИЭФ', '1-25 01 08', 2013),
(29, 'ИЭФ', '1-25 01 08', 2012),
(30, 'ИЭФ', '1-25 01 08', 2010),
(31, 'ИЭФ', '1-25 01 08', 2013),
(32, 'ИЭФ', '1-25 01 08', 2012);

INSERT INTO STUDENT (IDGROUP, NAME, BDAY) VALUES
(22, 'Пугач Михаил Трофимович', '1996-01-12'),
(23, 'Авдеев Николай Иванович', '1996-07-19'),
(24, 'Белова Елена Степановна', '1996-05-22'),
(25, 'Вилков Андрей Петрович', '1996-12-08'),
(26, 'Грушин Леонид Николаевич', '1995-11-11'),
(27, 'Дунаев Дмитрий Михайлович', '1996-08-24'),
(28, 'Клуни Иван Владиславович', '1996-09-15'),
(29, 'Крылов Олег Павлович', '1996-10-16');

INSERT INTO PROGRESS (SUBJECT, IDSTUDENT, PDATE, NOTE) VALUES
('ОАиП', 1000, '2014-01-12', 4),
('ОАиП', 1001, '2014-01-19', 5),
('ОАиП', 1002, '2014-01-08', 9),
('БД', 1003, '2014-01-11', 8),
('БД', 1004, '2014-01-15', 4),
('СУБД', 1005, '2014-01-16', 7),
('СУБД', 1006, '2014-01-27', 6);

CREATE TABLE Товары (
    Наименование VARCHAR(50) PRIMARY KEY,
    Цена INT
);

CREATE TABLE Заказы (
    Номер_заказа INT IDENTITY(1,1) PRIMARY KEY,
    Наименование_товара VARCHAR(50),
    Цена_продажи INT,
    Дата_поставки DATE
);

INSERT INTO Товары (Наименование, Цена) VALUES
('Компьютер', 500),
('Принтер', 150),
('Сканер', 200),
('Монитор', 250),
('Клавиатура', 30);

INSERT INTO Заказы (Наименование_товара, Цена_продажи, Дата_поставки) VALUES
('Компьютер', 550, '2024-01-15'),
('Принтер', 160, '2024-01-20'),
('Монитор', 270, '2024-01-25'),
('Мышь', 25, '2024-01-30'),
('Колонки', 45, '2024-02-01');

-- ЗАДАНИЕ 1
-- INNER JOIN: Перечень кодов аудиторий и наименований типов аудиторий
SELECT 
    A.AUDITORIUM AS [Код аудитории],
    AT.AUDITORIUM_TYPENAME AS [Наименование типа аудитории]
FROM 
    AUDITORIUM AS A
    INNER JOIN AUDITORIUM_TYPE AS AT ON A.AUDITORIUM_TYPE = AT.AUDITORIUM_TYPE;


-- ЗАДАНИЕ 2
-- INNER JOIN + LIKE: Только аудитории с типом, содержащим "компьютер"
SELECT 
    A.AUDITORIUM AS [Код аудитории],
    AT.AUDITORIUM_TYPENAME AS [Наименование типа аудитории]
FROM 
    AUDITORIUM AS A
    INNER JOIN AUDITORIUM_TYPE AS AT ON A.AUDITORIUM_TYPE = AT.AUDITORIUM_TYPE
WHERE 
    AT.AUDITORIUM_TYPENAME LIKE '%компьютер%';


-- ЗАДАНИЕ 3
-- Многотабличное соединение + CASE: Студенты с оценками 6-8
SELECT 
    F.FACULTY_NAME AS [Факультет],
    P.PULPIT_NAME AS [Кафедра],
    PR.PROFESSION_NAME AS [Специальность],
    S.SUBJECT_NAME AS [Дисциплина],
    ST.NAME AS [Имя Студента],
    CASE 
        WHEN PG.NOTE = 6 THEN 'шесть'
        WHEN PG.NOTE = 7 THEN 'семь'
        WHEN PG.NOTE = 8 THEN 'восемь'
    END AS [Оценка]
FROM 
    PROGRESS AS PG
    INNER JOIN STUDENT AS ST ON PG.IDSTUDENT = ST.IDSTUDENT
    INNER JOIN GROUPS AS G ON ST.IDGROUP = G.IDGROUP
    INNER JOIN PROFESSION AS PR ON G.PROFESSION = PR.PROFESSION
    INNER JOIN FACULTY AS F ON G.FACULTY = F.FACULTY
    INNER JOIN SUBJECT AS S ON PG.SUBJECT = S.SUBJECT
    INNER JOIN PULPIT AS P ON S.PULPIT = P.PULPIT
WHERE 
    PG.NOTE IN (6, 7, 8)
ORDER BY 
    PG.NOTE DESC;


-- ЗАДАНИЕ 4
-- LEFT OUTER JOIN + ISNULL: Все кафедры и преподаватели
SELECT 
    P.PULPIT_NAME AS [Кафедра],
    ISNULL(T.TEACHER_NAME, '***') AS [Преподаватель]
FROM 
    PULPIT AS P
    LEFT OUTER JOIN TEACHER AS T ON P.PULPIT = T.PULPIT
ORDER BY 
    P.PULPIT_NAME;


-- ЗАДАНИЕ 5 
-- FULL OUTER JOIN: Демонстрация коммутативности (вариант 1)
SELECT 
    Т.Наименование AS [Товар из Товары],
    Т.Цена AS [Цена],
    З.Номер_заказа AS [Номер заказа],
    З.Наименование_товара AS [Товар из Заказы],
    З.Цена_продажи AS [Цена продажи],
    З.Дата_поставки AS [Дата поставки]
FROM Товары AS Т
FULL OUTER JOIN Заказы AS З ON Т.Наименование = З.Наименование_товара
ORDER BY Т.Наименование, З.Наименование_товара;

-- FULL OUTER JOIN: Демонстрация коммутативности
SELECT 
    Т.Наименование AS [Товар из Товары],
    Т.Цена AS [Цена],
    З.Номер_заказа AS [Номер заказа],
    З.Наименование_товара AS [Товар из Заказы],
    З.Цена_продажи AS [Цена продажи],
    З.Дата_поставки AS [Дата поставки]
FROM Заказы AS З
FULL OUTER JOIN Товары AS Т ON З.Наименование_товара = Т.Наименование
ORDER BY Т.Наименование, З.Наименование_товара;

-- FULL OUTER JOIN
SELECT 
    Т.Наименование AS [Товар],
    Т.Цена AS [Цена]
FROM Товары AS Т
LEFT JOIN Заказы AS З ON Т.Наименование = З.Наименование_товара
WHERE З.Номер_заказа IS NULL;

-- FULL OUTER JOIN
SELECT 
    З.Номер_заказа AS [Номер заказа],
    З.Наименование_товара AS [Товар],
    З.Цена_продажи AS [Цена продажи],
    З.Дата_поставки AS [Дата поставки]
FROM Товары AS Т
RIGHT JOIN Заказы AS З ON Т.Наименование = З.Наименование_товара
WHERE Т.Наименование IS NULL;


-- ЗАДАНИЕ 6
-- CROSS JOIN
SELECT 
    A.AUDITORIUM AS [Код аудитории],
    AT.AUDITORIUM_TYPENAME AS [Наименование типа аудитории]
FROM 
    AUDITORIUM AS A
    CROSS JOIN AUDITORIUM_TYPE AS AT
WHERE 
    A.AUDITORIUM_TYPE = AT.AUDITORIUM_TYPE
ORDER BY 
    A.AUDITORIUM;

-- CROSS JOIN
SELECT 
    A.AUDITORIUM AS [Код аудитории],
    A.AUDITORIUM_TYPE AS [Код типа аудитории],
    AT.AUDITORIUM_TYPE AS [Тип из AUDITORIUM_TYPE],
    AT.AUDITORIUM_TYPENAME AS [Наименование типа аудитории]
FROM 
    AUDITORIUM AS A
    CROSS JOIN AUDITORIUM_TYPE AS AT
ORDER BY 
    A.AUDITORIUM, AT.AUDITORIUM_TYPE; 