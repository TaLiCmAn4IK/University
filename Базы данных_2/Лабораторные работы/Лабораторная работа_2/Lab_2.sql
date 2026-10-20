-- Задание 1
create tablespace TS_PVR
datafile 'ts_pvr.dbf'
size 7M
autoextend on next 5M
maxsize 20M;

-- Задание 2
create temporary tablespace TS_PVR_TEMP
tempfile 'ts_pvr_temp.dbf'
size 5M
autoextend on next 3M
maxsize 30M;

-- Задание 3
select tablespace_name, status, contents 
from dba_tablespaces;

select tablespace_name, file_name, bytes/1024/1024 as size_mb 
from dba_data_files;

-- Задание 4
create role RL_PVRCORE;

GRANT CREATE SESSION TO RL_PVRCORE;
GRANT CREATE TABLE, DROP TABLE TO RL_PVRCORE;
GRANT CREATE VIEW, DROP ANY VIEW TO RL_PVRCORE;
GRANT CREATE PROCEDURE, DROP ANY PROCEDURE TO RL_PVRCORE;

-- Задание 5
select * from dba_roles where role = 'RL_PVRCORE';
select * from dba_sys_privs where grantee = 'RL_PVRCORE';

-- Задание 6
create profile PF_PVRCORE limit
    failed_login_attempts 3
    password_lock_time 1
    password_life_time 30
    password_reuse_max 5
    password_verify_function null;
    
-- Задание 7
select distinct profile from dba_profiles;
select * from dba_profiles where profile = 'PF_PVRCORE';
select * from dba_profiles where profile = 'DEFAULT';

-- Задание 8
create user PVRCORE
identified by "PvrCore123"
default tablespace TS_PVR
temporary tablespace TS_PVR_TEMP
profile PF_PVRCORE
account unlock
password expire;

-- Задание 10
create table TestTable (
    id number(3) primary key,
    name varchar2(50)
);

create view TestView as
select * from TestTable;

-- Задание 11
create tablespace PVR_QDATA
datafile 'pvr_qdata.dbf'
size 10M
offline;

alter tablespace PVR_QDATA online;

alter user PVRCORE quota 2M on PVR_QDATA;
