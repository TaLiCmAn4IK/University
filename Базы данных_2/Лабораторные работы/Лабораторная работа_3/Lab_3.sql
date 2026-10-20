alter session set container = CDB$ROOT;
select sys_context('USERENV', 'CON_NAME') from dual;
select user from dual;
-- Задание 1
select con_id, name, open_mode, open_time 
from v$pdbs;

-- Задание 2
select instance_name, host_name, version, status, startup_time 
from v$instance;

-- Задание 3
select comp_id, comp_name, version, status 
from dba_registry;

-- Задание 4
create pluggable database PVR_PDB
admin user pvr_admin identified by "PvrAdmin123"
roles = (dba)
file_name_convert = ('/opt/oracle/oradata/FREE/pdbseed/', '/opt/oracle/oradata/FREE/PVR_PDB/');

alter pluggable database PVR_PDB open;
alter pluggable database PVR_PDB save state;

-- Задание 5
select con_id, name, open_mode, open_time 
from v$pdbs;

-- Задание 6
alter session set container = PVR_PDB;

create tablespace TS_PVR_PDB
datafile 'ts_pvr_pdb.dbf'
size 7M
autoextend on next 5M
maxsize 20M;

create temporary tablespace TS_PVR_PDB_TEMP
tempfile 'ts_pvr_pdb_temp.dbf'
size 5M
autoextend on next 3M
maxsize 30M;

create role RL_PVR_PDB;
grant create session, create table, create view, create procedure to RL_PVR_PDB;

create profile PF_PVR_PDB limit
    failed_login_attempts 3
    password_lock_time 1
    password_life_time 30
    password_reuse_max 5
    password_verify_function null;
    
create user U1_PVR_PDB
identified by "Pvr123"
default tablespace TS_PVR_PDB
temporary tablespace TS_PVR_PDB_TEMP
profile PF_PVR_PDB
account unlock
password expire;

grant RL_PVR_PDB to U1_PVR_PDB;

select username, account_status, default_tablespace, temporary_tablespace, profile 
from dba_users 
where username = 'U1_PVR_PDB';

-- Задания 8
-- Просмотр словаря PVR_PDB
alter session set container = PVR_PDB;
select sys_context('USERENV', 'CON_NAME') from dual;

-- табличные пространства
select tablespace_name, status, contents 
from dba_tablespaces;

-- постоянные файлы данных
select tablespace_name, file_name, bytes/1024/1024 as size_mb 
from dba_data_files;

-- временные файлы
select tablespace_name, file_name, bytes/1024/1024 as size_mb 
from dba_temp_files;

-- роли
select role from dba_roles 
where role in ('RL_PVR_PDB', 'CONNECT', 'RESOURCE', 'DBA');

-- привилегии роли RL_PVR_PDB
select grantee, privilege 
from dba_sys_privs 
where grantee = 'RL_PVR_PDB';

-- профили безопасности
select distinct profile from dba_profiles;
-- параметры профиля
select * from dba_profiles where profile = 'PF_PVR_PDB';

-- все пользователи PDB
select username, account_status, default_tablespace, profile 
from dba_users 
where username in ('U1_PVR_PDB', 'PVR_ADMIN', 'SYSTEM', 'SYS');

-- роли, назначенные пользователю U1_PVR_PDB
select grantee, granted_role, admin_option 
from dba_role_privs 
where grantee = 'U1_PVR_PDB';

-- Задание 9
-- создание общего пользователя
create user C##PVR identified by "C##Pvr123"
container = all;

grant create session to C##PVR;

select username, common, oracle_maintained 
from dba_users 
where username = 'C##PVR';

-- Задание 10
alter pluggable database PVR_PDB close immediate;
drop pluggable database PVR_PDB including datafiles;
select con_id, name, open_mode from v$pdbs;
drop user C##PVR cascade;
select username from dba_users where username = 'C##PVR';