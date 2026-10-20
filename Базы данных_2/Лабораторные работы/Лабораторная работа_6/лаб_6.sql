-- Задание 1
-- Найти файлы SQLNET.ORA и TNSNAMES.ORA
docker exec -it oracle-lab ls /opt/oracle/product/26ai/dbhomeFree/network/admin/
docker exec -it oracle-lab cat /opt/oracle/product/26ai/dbhomeFree/network/admin/sqlnet.ora
docker exec -it oracle-lab cat /opt/oracle/product/26ai/dbhomeFree/network/admin/tnsnames.ora

-- Задание 2
-- Соединится через SQL*Plus как SYSTEM и получить параметры экземпляра
docker exec -it oracle-lab sqlplus system/Oracle@//localhost/FREEPDB1
show parameters;

-- Задание 3
-- Табличные пространства
select tablespace_name, status, contents from dba_tablespaces;

-- Файлы табличных пространств
select tablespace_name, file_name, bytes/1024/1024 as size_mb from dba_data_files;
select tablespace_name, file_name, bytes/1024/1024 as size_mb from dba_temp_files;

-- Роли
select role from dba_roles where role in ('RL_PVR_PDB', 'CONNECT', 'RESOURCE', 'DBA');

-- Пользователи
select username, account_status from dba_users where username in ('PVRCORE', 'U1_PVR_PDB', 'SYSTEM', 'SYS');

-- Задание 4
reg query "HKEY_LOCAL_MACHINE\SOFTWARE\ORACLE" /s

-- Задание 5
select 'PVR_FREEPDB1' as connection_name from dual;
-- Добавляем строку в tnsnames.ora
docker exec -it oracle-lab bash -c "echo 'PVR_FREEPDB1 = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521)) (CONNECT_DATA = (SERVER = DEDICATED) (SERVICE_NAME = FREEPDB1)))' >> /opt/oracle/product/26ai/dbhomeFree/network/admin/tnsnames.ora"
docker exec -it oracle-lab cat /opt/oracle/product/26ai/dbhomeFree/network/admin/tnsnames.ora

-- Задание 6
docker exec -it oracle-lab sqlplus PVRCORE/PvrCore2026!@PVR_FREEPDB1
select user from dual;

-- Задание 7
-- селект к таблице любого пользователя
select table_name from user_tables;
select * from TESTTABLE;

-- Задание 8
-- справка по SQL*Plus.
help
-- справка по TIMING.
help timing
timing start my_select
select * from TESTTABLE;
timing stop

-- Задание 9
-- Получить описание столбцов любой таблицы
help describe
describe TESTTABLE;
desc TESTTABLE;

-- Задание 10
-- перечень сегментов моего пользователя
select segment_name, segment_type, tablespace_name, bytes/1024 as size_kb
from user_segments;

-- Задание 11
-- создать представление
create or replace view PVR_SEGMENT_STATS as
select 
    count(*) as total_segments,
    sum(extents) as total_extents,
    sum(blocks) as total_blocks,
    sum(bytes)/1024 as total_kb
from user_segments;

select * from PVR_SEGMENT_STATS;