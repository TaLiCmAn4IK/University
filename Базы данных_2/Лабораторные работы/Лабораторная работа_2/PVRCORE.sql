-- Задание 10
create table TestTable (
    id number(3) primary key,
    name varchar2(50)
);

create view TestView as
select * from TestTable;


-- Задание 11
create table PVR_T1 (
    id number(3),
    val varchar2(50)
) tablespace PVR_QDATA;

insert into PVR_T1 values (1, 'Строка 1');
insert into PVR_T1 values (2, 'Строка 2');
insert into PVR_T1 values (3, 'Строка 3');
commit;

select * from PVR_T1;