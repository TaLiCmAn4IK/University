select user from dual;
alter session set container = FREEPDB1;
alter session set container = CDB$ROOT;
select sys_context('USERENV', 'CON_NAME') as container_name from dual;

-- Задание 1 - Список файлов табличных пространств
-- Постоянные файлы
select tablespace_name, file_name, bytes/1024/1024 as size_mb 
from dba_data_files;

-- Временные файлы
select tablespace_name, file_name, bytes/1024/1024 as size_mb 
from dba_temp_files;

-- Задание 2 - Создание PVR_QDATA, квота, таблица
create tablespace PVR_QDATA
datafile 'pvr_qdata.dbf'
size 10M
offline;

alter tablespace PVR_QDATA online;

alter user PVRCORE quota 2M on PVR_QDATA;

-- Задание 3 - Сегменты PVR_QDATA
select segment_name, segment_type, tablespace_name, bytes/1024 as size_kb
from dba_segments
where tablespace_name = 'PVR_QDATA';

-- Задание 4 - Удаление таблицы и корзина
select segment_name, segment_type, tablespace_name
from dba_segments
where tablespace_name = 'PVR_QDATA';

-- Задание 7 - Экстенты таблицы
select segment_name, bytes, blocks, extents
from dba_segments
where segment_name = 'PVR_T1';

select extent_id, bytes, blocks
from dba_extents
where segment_name = 'PVR_T1'
order by extent_id;

-- Задание 8 - Удаление пространства PVR_QDATA
drop tablespace PVR_QDATA including contents and datafiles;

select tablespace_name from dba_tablespaces where tablespace_name = 'PVR_QDATA';
select file_name from dba_data_files where tablespace_name = 'PVR_QDATA';

-- Задание 9 - Группы журналов повтора
alter session set container = CDB$ROOT;
select sys_context('USERENV', 'CON_NAME') from dual;

select group#, sequence#, bytes/1024/1024 as size_mb, members, status, archived
from v$log;

select group#, sequence#, status
from v$log
where status = 'CURRENT';

-- Задание 10 - Файлы журналов повтора
select group#, member, status
from v$logfile
order by group#;

-- Задание 11 - Переключение журналов
select to_char(sysdate, 'YYYY-MM-DD HH24:MI:SS') as server_time from dual;
select group#, sequence#, status from v$log;

-- 3 раза переключения 
alter system switch logfile;
alter system switch logfile;
alter system switch logfile;

select group#, sequence#, status from v$log;

-- Задание 12 - Создание группы 3
alter database add logfile group 3
('/opt/oracle/oradata/FREE/redo03a.log',
 '/opt/oracle/oradata/FREE/redo03b.log',
 '/opt/oracle/oradata/FREE/redo03c.log')
size 20M;

select group#, sequence#, members, status 
from v$log 
where group# = 3;

select group#, member 
from v$logfile 
where group# = 3
order by member;

-- 3 раза переключения 
alter system switch logfile;
alter system switch logfile;
alter system switch logfile;
select group#, sequence#, status from v$log order by group#;

-- Задание 13 - Удаление группы 3
alter system checkpoint;
alter system switch logfile;
select group#, sequence#, status from v$log order by group#;

alter database drop logfile group 3;
select group#, sequence#, status from v$log order by group#;

-- docker exec -it oracle-lab rm /opt/oracle/oradata/FREE/redo03a.log
-- docker exec -it oracle-lab rm /opt/oracle/oradata/FREE/redo03b.log
-- docker exec -it oracle-lab rm /opt/oracle/oradata/FREE/redo03c.log

-- Задание 14 - Проверка архивирования
archive log list;

-- Задание 15 - Номер последнего архива
select max(sequence#) as last_archive 
from v$archived_log;

-- Задание 16 - Включение архивирования
-- docker exec -it oracle-lab sqlplus / as sysdba
-- shutdown immediate;
-- startup mount;
-- alter database archivelog;
-- alter database open;
-- archive log list;

-- Задание 17 - Создание архива
alter system archive log current;

select max(sequence#) as last_archive from v$archived_log;
select sequence#, name, first_change#, next_change# 
from v$archived_log order by sequence# desc fetch first 3 rows only;

select group#, sequence#, first_change#, next_change#, status from v$log order by group#;
select sequence#, first_change#, next_change# from v$archived_log order by sequence#;

-- docker exec -it oracle-lab ls /opt/oracle/product/26ai/dbhomeFree/dbs/ | findstr arch

-- Задание 18 - Отключение архивирования
-- shutdown immediate;
-- startup mount;
-- alter database noarchivelog;
-- alter database open;
-- archive log list;

-- Задание 19 - Управляющие файлы
select name from v$controlfile;

-- Задание 20 - Содержимое управляющего файла
select type, record_size, records_total, records_used
from v$controlfile_record_section;

-- полное содержимое
select * from v$controlfile_record_section;

-- Задание 21 - Файл параметров
select name, value  
from v$parameter 
where name = 'spfile';

-- Задание 22 - Создание PFILE
create pfile='PVR_PFILE.ORA' from spfile;

-- docker exec -it oracle-lab ls /opt/oracle/product/26ai/dbhomeFree/dbs/ | findstr PVR_PFILE
-- docker exec -it oracle-lab cat /opt/oracle/product/26ai/dbhomeFree/dbs/PVR_PFILE.ORA

-- Задание 23 - Файл паролей
show parameter remote_login_passwordfile;

-- docker exec -it oracle-lab ls /opt/oracle/product/26ai/dbhomeFree/dbs/ | findstr orapw

-- Задание 24 - Директории диагностики
select name, value 
from v$diag_info;

-- docker exec -it oracle-lab ls /opt/oracle/diag/rdbms/free/FREE/


-- Задание 25 - Поиск в LOG.XML
-- docker exec -it oracle-lab grep -a -i "LGWR" /opt/oracle/diag/rdbms/free/FREE/alert/log.xml | Select-Object -First 50

-- Задание 26
-- Под SYSTEM
drop table SYSTEM.PVR_T1 purge;
drop tablespace PVR_QDATA including contents and datafiles;

select owner, table_name from dba_tables where table_name = 'PVR_T1';
select tablespace_name from dba_tablespaces where tablespace_name = 'PVR_QDATA';
select group# from v$log where group# = 3;

-- docker exec -it oracle-lab rm /opt/oracle/product/26ai/dbhomeFree/dbs/PVR_PFILE.ORA
-- docker exec -it oracle-lab rm /opt/oracle/oradata/FREE/redo03a.log
-- docker exec -it oracle-lab rm /opt/oracle/oradata/FREE/redo03b.log
-- docker exec -it oracle-lab rm /opt/oracle/oradata/FREE/redo03c.log