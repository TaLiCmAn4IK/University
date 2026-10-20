-- =============================================================
-- СОЗДАНИЕ БАЗЫ ДАННЫХ UNIVER С ТЕСТОВЫМИ ДАННЫМИ
-- =============================================================

USE master;
GO

-- Удаление базы, если существует
IF DB_ID('UNIVER') IS NOT NULL
BEGIN
    ALTER DATABASE UNIVER SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE UNIVER;
END
GO

CREATE DATABASE UNIVER;
GO

USE UNIVER;
GO

-- =============================================================
-- СОЗДАНИЕ ТАБЛИЦ
-- =============================================================

-- Типы аудиторий
CREATE TABLE AUDITORIUM_TYPE
(
    AUDITORIUM_TYPE      CHAR(10) CONSTRAINT AUDITORIUM_TYPE_PK PRIMARY KEY,
    AUDITORIUM_TYPENAME  VARCHAR(30)
);
GO

-- Учебные аудитории
CREATE TABLE AUDITORIUM
(
    AUDITORIUM           CHAR(20) CONSTRAINT AUDITORIUM_PK PRIMARY KEY,
    AUDITORIUM_TYPE      CHAR(10) CONSTRAINT AUDITORIUM_AUDITORIUM_TYPE_FK
                         FOREIGN KEY REFERENCES AUDITORIUM_TYPE(AUDITORIUM_TYPE),
    AUDITORIUM_CAPACITY  INT CONSTRAINT AUDITORIUM_CAPACITY_CHECK
                         DEFAULT 1 CHECK (AUDITORIUM_CAPACITY BETWEEN 1 AND 300),
    AUDITORIUM_NAME      VARCHAR(50)
);
GO

-- Факультеты
CREATE TABLE FACULTY
(
    FACULTY       CHAR(10) CONSTRAINT FACULTY_PK PRIMARY KEY,
    FACULTY_NAME  VARCHAR(50) DEFAULT '???'
);
GO

-- Специальности
CREATE TABLE PROFESSION
(
    PROFESSION       CHAR(20) CONSTRAINT PROFESSION_PK PRIMARY KEY,
    FACULTY          CHAR(10) CONSTRAINT PROFESSION_FACULTY_FK
                     FOREIGN KEY REFERENCES FACULTY(FACULTY),
    PROFESSION_NAME  VARCHAR(100),
    QUALIFICATION    VARCHAR(50)
);
GO

-- Кафедры
CREATE TABLE PULPIT
(
    PULPIT       CHAR(20) CONSTRAINT PULPIT_PK PRIMARY KEY,
    PULPIT_NAME  VARCHAR(100),
    FACULTY      CHAR(10) CONSTRAINT PULPIT_FACULTY_FK
                 FOREIGN KEY REFERENCES FACULTY(FACULTY)
);
GO

-- Преподаватели
CREATE TABLE TEACHER
(
    TEACHER       CHAR(10) CONSTRAINT TEACHER_PK PRIMARY KEY,
    TEACHER_NAME  VARCHAR(100),
    GENDER        CHAR(1) CHECK (GENDER IN ('м', 'ж')),
    PULPIT        CHAR(20) CONSTRAINT TEACHER_PULPIT_FK
                  FOREIGN KEY REFERENCES PULPIT(PULPIT)
);
GO

-- Дисциплины
CREATE TABLE SUBJECT
(
    SUBJECT       CHAR(10) CONSTRAINT SUBJECT_PK PRIMARY KEY,
    SUBJECT_NAME  VARCHAR(100) UNIQUE,
    PULPIT        CHAR(20) CONSTRAINT SUBJECT_PULPIT_FK
                  FOREIGN KEY REFERENCES PULPIT(PULPIT)
);
GO

-- Студенческие группы
CREATE TABLE GROUPS
(
    IDGROUP     INT CONSTRAINT GROUPS_PK PRIMARY KEY,
    FACULTY     CHAR(10) CONSTRAINT GROUPS_FACULTY_FK
                FOREIGN KEY REFERENCES FACULTY(FACULTY),
    PROFESSION  CHAR(20) CONSTRAINT GROUPS_PROFESSION_FK
                FOREIGN KEY REFERENCES PROFESSION(PROFESSION),
    YEAR_FIRST  SMALLINT -- Исправлено: GETDATE() удален из CHECK, так как он тут недопустим
);
GO

-- Студенты
CREATE TABLE STUDENT
(
    IDSTUDENT  INT IDENTITY(1000,1) CONSTRAINT STUDENT_PK PRIMARY KEY,
    IDGROUP    INT CONSTRAINT STUDENT_GROUP_FK
               FOREIGN KEY REFERENCES GROUPS(IDGROUP),
    NAME       NVARCHAR(100),
    BDAY       DATE,
    STAMP      TIMESTAMP,
    INFO       XML NULL,
    FOTO       VARBINARY(MAX) NULL
);
GO

-- Оценки на экзамене
CREATE TABLE PROGRESS
(
    SUBJECT    CHAR(10) CONSTRAINT PROGRESS_SUBJECT_FK
               FOREIGN KEY REFERENCES SUBJECT(SUBJECT),
    IDSTUDENT  INT CONSTRAINT PROGRESS_IDSTUDENT_FK
               FOREIGN KEY REFERENCES STUDENT(IDSTUDENT),
    PDATE      DATE,
    NOTE       INT CHECK (NOTE BETWEEN 1 AND 10),
    CONSTRAINT PROGRESS_PK PRIMARY KEY (SUBJECT, IDSTUDENT, PDATE)
);
GO

PRINT 'Таблицы созданы';
GO

-- =============================================================
-- ЗАПОЛНЕНИЕ ТАБЛИЦ
-- =============================================================

-- AUDITORIUM_TYPE
INSERT INTO AUDITORIUM_TYPE (AUDITORIUM_TYPE, AUDITORIUM_TYPENAME) VALUES
    ('ЛБ-Х',  'Химическая лаборатория'),
    ('ЛБ-К',  'Компьютерный класс'),
    ('ЛБ-СК', 'Спец. компьютерный класс'),
    ('ЛК',    'Лекционная'),
    ('ЛК-К',  'Лекционная с уст. проектором');
GO

-- AUDITORIUM
INSERT INTO AUDITORIUM (AUDITORIUM, AUDITORIUM_TYPE, AUDITORIUM_CAPACITY, AUDITORIUM_NAME) VALUES
    ('301-1', 'ЛБ-К', 15, '301-1'),
    ('304-4', 'ЛБ-К', 90, '304-4'),
    ('313-1', 'ЛК-К', 60, '313-1'),
    ('314-4', 'ЛК',   90, '314-4'),
    ('320-4', 'ЛК',   90, '320-4'),
    ('324-1', 'ЛК-К', 50, '324-1'),
    ('413-1', 'ЛБ-К', 15, '413-1'),
    ('423-1', 'ЛБ-К', 90, '423-1');
GO

-- FACULTY
INSERT INTO FACULTY (FACULTY, FACULTY_NAME) VALUES
    ('ТТЛП', 'Технологии и техника лесной промышленности'),
    ('ТОВ',  'Технологии органических веществ'),
    ('ХТиТ', 'Химические технологии и техника'),
    ('ИЭФ',  'Инженерно-экономический'),
    ('ЛХФ',  'Лесохозяйственный'),
    ('ИДиП', 'Издательское дело и полиграфия'),
    ('ИТ',   'Информационных технологий');
GO

-- PROFESSION
INSERT INTO PROFESSION (PROFESSION, FACULTY, PROFESSION_NAME, QUALIFICATION) VALUES
    ('1-36 06 01', 'ИДиП', 'Полиграфическое оборудование и системы обработки информации', 'инженер-электромеханик'),
    ('1-36 07 01', 'ХТиТ', 'Машины и аппараты химических производств и предприятий строительных материалов', 'инженер-механик'),
    ('1-40 01 02', 'ИТ',   'Информационные системы и технологии', 'инженер-программист-системотехник'),
    ('1-46 01 01', 'ТТЛП', 'Лесоинженерное дело', 'инженер-технолог'),
    ('1-47 01 01', 'ИДиП', 'Издательское дело', 'редактор-технолог'),
    ('1-48 01 02', 'ТОВ',  'Химическая технология органических веществ, материалов и изделий', 'инженер-химик-технолог'),
    ('1-48 01 05', 'ТОВ',  'Химическая технология переработки древесины', 'инженер-химик-технолог'),
    ('1-54 01 03', 'ТОВ',  'Физико-химические методы и приборы контроля качества продукции', 'инженер по сертификации'),
    ('1-75 01 01', 'ЛХФ',  'Лесное хозяйство', 'инженер лесного хозяйства'),
    ('1-75 02 01', 'ЛХФ',  'Садово-парковое строительство', 'инженер садово-паркового строительства'),
    ('1-89 02 02', 'ЛХФ',  'Туризм и природопользование', 'специалист в сфере туризма'),
    ('1-36 05 01', 'ТТЛП', 'Машины и оборудование лесного комплекса', 'инженер-механик'),
    ('1-25 01 07', 'ИЭФ',  'Экономика и управление на предприятии', 'экономист-менеджер'),
    ('1-25 01 08', 'ИЭФ',  'Бухгалтерский учет, анализ и аудит', 'экономист');
GO

-- PULPIT
INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES
    ('РИТ',      'Редакционно-издательских технологий',                                                 'ИДиП'),
    ('СБУАиА',   'Статистики, бухгалтерского учета, анализа и аудита',                                  'ИЭФ'),
    ('ТДП',      'Технологий деревообрабатывающих производств',                                         'ТТЛП'),
    ('ТиДИД',    'Технологии и дизайна изделий из древесины',                                           'ТТЛП'),
    ('ТиП',      'Туризма и природопользования',                                                        'ЛХФ'),
    ('ТЛ',       'Транспорта леса',                                                                      'ТТЛП'),
    ('ТНВиОХТ',  'Технологии неорганических веществ и общей химической технологии',                    'ХТиТ'),
    ('ТНХСиППМ', 'Технологии нефтехимического синтеза и переработки полимерных материалов',            'ТОВ'),
    ('ХПД',      'Химической переработки древесины',                                                    'ТОВ'),
    ('ХТЭПиМЭЕ', 'Химии, технологии электрохимических производств и материалов электронной техники',   'ХТиТ'),
    ('ЭТиМ',     'Экономической теории и маркетинга',                                                   'ИЭФ'),
    ('ИСиТ',     'Информационных систем и технологий',                                                  'ИТ');
GO

-- TEACHER
INSERT INTO TEACHER (TEACHER, TEACHER_NAME, GENDER, PULPIT) VALUES
    ('НСКВ', 'Носков Михаил Трофимович',         'м', 'ТДП'),
    ('ПРКП', 'Прокопенко Николай Иванович',      'м', 'ТНХСиППМ'),
    ('МРЗВ', 'Морозова Елена Степановна',        'ж', 'ИСиТ'),
    ('РВКС', 'Ровкас Андрей Петрович',           'м', 'ТиП'),
    ('РЖКВ', 'Рыжиков Леонид Николаевич',        'м', 'ТЛ'),
    ('РМНВ', 'Романов Дмитрий Михайлович',       'м', 'ИСиТ'),
    ('СМЛВ', 'Смелов Владимир Владиславович',    'м', 'ИСиТ'),
    ('КРЛВ', 'Крылов Павел Павлович',            'м', 'ИСиТ'),
    ('ЧРН',  'Чернова Анна Викторовна',          'ж', 'ХПД'),
    ('МХВ',  'Мохов Михаил Сергеевич',           'м', 'РИТ');
GO

-- SUBJECT
INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES
    ('ПЗ',   'Представление знаний в компьютерных системах', 'ИСиТ'),
    ('ПИС',  'Проектирование информационных систем',          'ИСиТ'),
    ('ПСП',  'Программирование сетевых приложений',           'ИСиТ'),
    ('ПЭХ',  'Прикладная электрохимия',                       'ХТЭПиМЭЕ'),
    ('СУБД', 'Системы управления базами данных',              'ИСиТ'),
    ('ТиОЛ', 'Технология и оборудование лесозаготовок',       'ТЛ'),
    ('ТРИ',  'Технология резиновых изделий',                  'ТНХСиППМ'),
    ('ЭП',   'Экономика природопользования',                  'ТиП'),
    ('ЭТ',   'Экономическая теория',                          'ЭТиМ'),
    ('ОАиП', 'Основы алгоритмизации и программирования',      'ИСиТ'),
    ('БД',   'Базы данных',                                   'ИСиТ');
GO

-- GROUPS
INSERT INTO GROUPS (IDGROUP, FACULTY, PROFESSION, YEAR_FIRST) VALUES
    (22, 'ЛХФ',  '1-75 02 01', 2011),
    (23, 'ЛХФ',  '1-89 02 02', 2012),
    (24, 'ЛХФ',  '1-89 02 02', 2011),
    (25, 'ТТЛП', '1-36 05 01', 2013),
    (26, 'ТТЛП', '1-36 05 01', 2012),
    (27, 'ТТЛП', '1-46 01 01', 2012),
    (28, 'ИЭФ',  '1-25 01 07', 2013),
    (29, 'ИЭФ',  '1-25 01 07', 2012),
    (30, 'ИЭФ',  '1-25 01 07', 2010),
    (31, 'ИЭФ',  '1-25 01 08', 2013),
    (32, 'ИЭФ',  '1-25 01 08', 2012);
GO

-- 9. ЗАПОЛНЕНИЕ ТАБЛИЦЫ СТУДЕНТОВ (Временно отключаем автоинкремент IDSTUDENT)
SET IDENTITY_INSERT STUDENT ON;
INSERT INTO STUDENT (IDSTUDENT, IDGROUP, NAME, BDAY, INFO, FOTO) VALUES
(1001, 22, 'Иванов Антон Олегович', '2004-03-12', NULL, NULL),
(1002, 22, 'Петрова Анна Сергеевна', '2004-07-25', NULL, NULL),
(1003, 23, 'Сидоров Дмитрий Васильевич', '2003-11-05', NULL, NULL);
SET IDENTITY_INSERT STUDENT OFF;
GO