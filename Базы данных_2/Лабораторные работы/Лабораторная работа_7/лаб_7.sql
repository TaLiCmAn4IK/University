-- Задание 1
alter session set container = FREEPDB1;
select sys_context('USERENV', 'CON_NAME') from dual;
drop sequence S1;

grant create sequence to PVRCORE;
grant create cluster to PVRCORE;
grant create synonym to PVRCORE;
grant create public synonym to PVRCORE;
grant create materialized view to PVRCORE;
grant create table to PVRCORE;

-- Задание 2
-- возрастающая, без ограничений
create sequence S1
start with 1000
increment by 10
nominvalue
nomaxvalue
nocycle
nocache
noorder;

select S1.nextval from dual;

select S1.currval from dual;

-- Задание 3
-- ограниченная сверху
create sequence S2
start with 10
increment by 10
maxvalue 10000
nocycle;

select S2.nextval from dual;

-- Задание 4
-- убывающая
create sequence S3
start with 10
increment by -10
maxvalue 10
minvalue -1000
nocycle
order;

select S3.nextval from dual;

-- Задание 5
-- циклическая
create sequence S4
start with 1
increment by 1
minvalue 1
maxvalue 10
cycle
cache 5
noorder;

select S4.nextval from dual;

-- Задание 6
select sequence_name, min_value, max_value, increment_by, cycle_flag, cache_size
from user_sequences
where sequence_name in ('S1', 'S2', 'S3', 'S4');

-- Задание 7
create table T1 (
    N1 number(20),
    N2 number(20),
    N3 number(20),
    N4 number(20)
) cache
storage (buffer_pool keep);

insert into T1 values (S1.nextval, S2.nextval, S3.nextval, S4.nextval);
commit;

select * from T1;

-- Задание 8
create cluster ABC (
    X number(10),
    V varchar2(12)
)
hashkeys 200
hash is X
size 200;

-- Задание 9
create table A (
    XA number(10),
    VA varchar2(12),
    EXTRA_COL varchar2(50)
)
cluster ABC (XA, VA);

-- Задание 10
create table B (
    XB number(10),
    VB varchar2(12),
    EXTRA_COL varchar2(50)
)
cluster ABC (XB, VB);

-- Задание 11
create table C (
    XC number(10),
    VC varchar2(12),
    EXTRA_COL varchar2(50)
)
cluster ABC (XC, VC);

-- Задание 12
-- Кластеры
select cluster_name, cluster_type, hashkeys
from user_clusters;

-- Таблицы
select table_name, cluster_name
from user_tables
where cluster_name = 'ABC';

-- Задание 13
create synonym SYN_C for C;
select * from SYN_C;

-- Задание 14
create public synonym PUB_SYN_B for B;
select * from PUB_SYN_B;

-- Задание 15
create table A2 (
    id number(5) primary key,
    name varchar2(50)
);

create table B2 (
    id number(5) primary key,
    a_id number(5),
    value varchar2(50),
    constraint fk_b2_a2 foreign key (a_id) references A2(id)
);

insert into A2 values (1, 'Запись 1');
insert into A2 values (2, 'Запись 2');
insert into A2 values (3, 'Запись 3');

insert into B2 values (10, 1, 'Данные 1');
insert into B2 values (20, 2, 'Данные 2');
insert into B2 values (30, 1, 'Данные 3');
commit;

create or replace view V1 as
select A2.id as a_id, A2.name as a_name, B2.id as b_id, B2.value as b_value
from A2
inner join B2 on A2.id = B2.a_id;

select * from V1;

-- Задание 16
create materialized view MV
refresh complete
start with sysdate
next sysdate + 2/1440
as
select A2.id as a_id, A2.name as a_name, B2.id as b_id, B2.value as b_value
from A2
inner join B2 on A2.id = B2.a_id;

select * from MV;

select mview_name, refresh_mode, refresh_method, last_refresh_date
from user_mviews
where mview_name = 'MV';

