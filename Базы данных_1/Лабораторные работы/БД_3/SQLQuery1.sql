-- 1 Задание
USE master;
CREATE database PVR_MyBase_1;

PRINT '1 Задание выполнено';
GO

-- 2 Задание
USE PVR_MyBase_1;

CREATE TABLE ОПЕРАЦИИ(
    Наименование_операции NVARCHAR(100) PRIMARY KEY,
    Признак_сложности NVARCHAR(50)
);

CREATE TABLE РАБОТНИКИ(
    ID_работника INT PRIMARY KEY,
    Фамилия NVARCHAR(50),
    Имя NVARCHAR(50),
    Отчество NVARCHAR(50),
    Адрес NVARCHAR(100),
    Телефон NVARCHAR(20),
    Стаж INT
);

CREATE TABLE ВЫПОЛНЕНИЕ(
    ID_выполнения INT PRIMARY KEY,
    Наименование_операции NVARCHAR(100) FOREIGN KEY REFERENCES ОПЕРАЦИИ(Наименование_операции),
    Количество_деталей INT,
    Дата DATE,
    ID_работника INT FOREIGN KEY REFERENCES РАБОТНИКИ(ID_работника)
);

SELECT * FROM ОПЕРАЦИИ;
SELECT * FROM РАБОТНИКИ;
SELECT * FROM ВЫПОЛНЕНИЕ;

PRINT '2 Задание выполнено';
GO

-- 3 Задание
USE PVR_MyBase_1;

ALTER TABLE ВЫПОЛНЕНИЕ ADD Новый_столбец DATE;
ALTER TABLE РАБОТНИКИ ADD DEFAULT 0 FOR Стаж;
ALTER TABLE ВЫПОЛНЕНИЕ ADD CONSTRAINT CK_Количество CHECK (Количество_деталей > 0);

SELECT * FROM ВЫПОЛНЕНИЕ;

ALTER TABLE ВЫПОЛНЕНИЕ DROP COLUMN Новый_столбец;

SELECT * FROM ВЫПОЛНЕНИЕ;

PRINT '3 Задание выполнено';
GO

-- 4 Задание
USE PVR_MyBase_1;

DELETE FROM ВЫПОЛНЕНИЕ;
DELETE FROM ОПЕРАЦИИ;
DELETE FROM РАБОТНИКИ;

INSERT INTO ОПЕРАЦИИ VALUES ('Токарная обработка', 'Высокая');
INSERT INTO ОПЕРАЦИИ VALUES ('Фрезеровка', 'Средняя');
INSERT INTO ОПЕРАЦИИ VALUES ('Сварка', 'Высокая');
INSERT INTO ОПЕРАЦИИ VALUES ('Шлифовка', 'Низкая');
INSERT INTO ОПЕРАЦИИ VALUES ('Сборка', 'Средняя');

INSERT INTO РАБОТНИКИ VALUES (1, 'Иванов', 'Петр', 'Сергеевич', 'ул. Ленина 10', '123-45', 5);
INSERT INTO РАБОТНИКИ VALUES (2, 'Петрова', 'Анна', 'Ивановна', 'ул. Гагарина 25', '234-56', 3);
INSERT INTO РАБОТНИКИ VALUES (3, 'Сидоров', 'Михаил', 'Петрович', 'ул. Пушкина 5', '345-67', 8);
INSERT INTO РАБОТНИКИ VALUES (4, 'Козлова', 'Ольга', 'Сергеевна', 'ул. Кирова 15', '456-78', 2);
INSERT INTO РАБОТНИКИ VALUES (5, 'Смирнов', 'Алексей', 'Иванович', 'ул. Советская 30', '567-89', 4);

INSERT INTO ВЫПОЛНЕНИЕ VALUES (1, 'Токарная обработка', 10, '2024-03-15', 1);
INSERT INTO ВЫПОЛНЕНИЕ VALUES (2, 'Сварка', 5, '2024-03-15', 2);
INSERT INTO ВЫПОЛНЕНИЕ VALUES (3, 'Фрезеровка', 15, '2024-03-16', 3);
INSERT INTO ВЫПОЛНЕНИЕ VALUES (4, 'Шлифовка', 20, '2024-03-16', 1);
INSERT INTO ВЫПОЛНЕНИЕ VALUES (5, 'Сборка', 7, '2024-03-17', 4);
INSERT INTO ВЫПОЛНЕНИЕ VALUES (6, 'Токарная обработка', 12, '2024-03-17', 5);
INSERT INTO ВЫПОЛНЕНИЕ VALUES (7, 'Сварка', 8, '2024-03-18', 2);
INSERT INTO ВЫПОЛНЕНИЕ VALUES (8, 'Сборка', 10, '2024-03-18', 3);

SELECT * FROM ОПЕРАЦИИ;
SELECT * FROM РАБОТНИКИ;
SELECT * FROM ВЫПОЛНЕНИЕ;

PRINT '4 Задание выполнено';
GO

-- 5 Задание
USE PVR_MyBase_1;

SELECT * FROM РАБОТНИКИ;
SELECT Фамилия, Имя FROM РАБОТНИКИ;
SELECT COUNT(*) AS 'Количество работников' FROM РАБОТНИКИ;
UPDATE РАБОТНИКИ SET Стаж = Стаж + 1;
SELECT * FROM РАБОТНИКИ;

PRINT '5 Задание выполнено';
GO

-- 6 Задание 
USE master;

ALTER DATABASE PVR_MyBase_1 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE IF EXISTS PVR_MyBase_1;
GO

CREATE DATABASE PVR_MyBase_1
ON PRIMARY
(
    NAME = N'PVR_MyBase_1_data',
    FILENAME = N'D:\Документы\Базы данных\Лабораторные работы\БД_3\PVR_MyBase_1_data.mdf',
    SIZE = 10240KB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 1024KB
),
FILEGROUP FG1
(
    NAME = N'PVR_MyBase_1_fg1',
    FILENAME = N'D:\Документы\Базы данных\Лабораторные работы\БД_3\PVR_MyBase_1_fg1.ndf',
    SIZE = 10240KB,
    MAXSIZE = 1GB,
    FILEGROWTH = 1024KB
)
LOG ON
(
    NAME = N'PVR_MyBase_1_log',
    FILENAME = N'D:\Документы\Базы данных\Лабораторные работы\БД_3\PVR_MyBase_1_log.ldf',
    SIZE = 5120KB,
    MAXSIZE = 2048GB,
    FILEGROWTH = 10%
);

PRINT '6 Задание выполнено';
GO

-- 7 Задание 
USE PVR_MyBase_1;

CREATE TABLE ОПЕРАЦИИ(
    Наименование_операции NVARCHAR(100) PRIMARY KEY,
    Признак_сложности NVARCHAR(50)
) ON FG1;

CREATE TABLE РАБОТНИКИ(
    ID_работника INT PRIMARY KEY,
    Фамилия NVARCHAR(50),
    Имя NVARCHAR(50),
    Отчество NVARCHAR(50),
    Адрес NVARCHAR(100),
    Телефон NVARCHAR(20),
    Стаж INT
) ON FG1;

CREATE TABLE ВЫПОЛНЕНИЕ(
    ID_выполнения INT PRIMARY KEY,
    Наименование_операции NVARCHAR(100) FOREIGN KEY REFERENCES ОПЕРАЦИИ(Наименование_операции),
    Количество_деталей INT,
    Дата DATE,
    ID_работника INT FOREIGN KEY REFERENCES РАБОТНИКИ(ID_работника)
) ON FG1;

PRINT '7 Задание выполнено';
GO

-- 8 Задание
PRINT 'Для выполнения 8 задания открыть командную строку и ввести:';
PRINT 'sqlcmd -S DESKTOP-IMNOJB5 -E -C';
PRINT 'USE PVR_MyBase_1;';
PRINT 'GO';
PRINT 'SELECT * FROM ОПЕРАЦИИ;';
PRINT 'GO';
PRINT 'EXIT;';
GO