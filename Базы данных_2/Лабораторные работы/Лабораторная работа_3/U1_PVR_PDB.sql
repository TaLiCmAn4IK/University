-- Задание 7
select user from dual;

create table PVR_table (
    id number(3) primary key,
    name varchar2(50)
);

insert into PVR_table values (1, 'Строка 1');
insert into PVR_table values (2, 'Строка 2');
insert into PVR_table values (3, 'Строка 3');
commit;

select * from PVR_table;