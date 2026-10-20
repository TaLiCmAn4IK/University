select user from dual;
select sys_context('USERENV', 'CON_NAME') from dual;

-- Задание 2 - Создание PVR_QDATA, квота, таблица
-- 1. Удаляем старую таблицу (без корзины)
drop table PVR_T1 purge;

-- 2. Создаём новую таблиц 
create table PVR_T1 (
    id number(5) primary key,
    name varchar2(50)
) tablespace PVR_QDATA;

-- 3. Добавляем 3 строки
insert into PVR_T1 values (1, 'Строка 1');
insert into PVR_T1 values (2, 'Строка 2');
insert into PVR_T1 values (3, 'Строка 3');
commit;

-- 4. Проверяем
select * from PVR_T1;
select count(*) from PVR_T1;

select object_name, original_name, type, droptime
from user_recyclebin;

-- Задание 4 - Удаление таблицы и корзина
drop table PVR_T1;

select object_name, original_name, type, droptime
from user_recyclebin;

-- Задание 5 - Восстановление через FLASHBACK
flashback table PVR_T1 to before drop;

select count(*) from PVR_T1;

-- Задание 6 - Заполнение 10000 строками
-- Очистить таблицу (быстро, без корзины)
truncate table PVR_T1;

begin
    for i in 1..10000 loop
        insert into PVR_T1 values (i, 'Строка ' || i);
    end loop;
    commit;
end;
/

select count(*) from PVR_T1;

-- Задание 8 - Удаление таблицы PVR_T1
drop table PVR_T1 purge;