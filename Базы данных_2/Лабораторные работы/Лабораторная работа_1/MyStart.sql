create table PVR_t (x number(3) primary key, s varchar2(50));

insert into PVR_t values (1, 'Первая строка');
insert into PVR_t values (2, 'Вторая строка');
insert into PVR_t values (3, 'Третья строка');
commit;

update PVR_t set s = 'Измененная строка 1' where x = 1;
update PVR_t set s = 'Измененная строка 2' where x = 2;
commit;

select * from PVR_t where x > 1;
select count(*) as Количество_строк, sum(x) as Сумма_X from PVR_t;

delete from PVR_t where x = 3;
commit;

create table PVR_t1 (
    id number(3) primary key,
    pvr_x number(3),
    constraint fk_pvr foreign key (pvr_x) references PVR_t(x)
);

insert into PVR_t1 values (10, 1);
insert into PVR_t1 values (20, 2);
commit;

select * from PVR_t t 
inner join PVR_t1 t1 on t.x = t1.pvr_x;

select * from PVR_t t 
left join PVR_t1 t1 on t.x = t1.pvr_x;

select * from PVR_t t 
right join PVR_t1 t1 on t.x = t1.pvr_x;

select * from PVR_t;
select * from PVR_t1;

drop table PVR_t1;
drop table PVR_t;