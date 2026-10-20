select user from dual;
select sys_context('USERENV', 'CON_NAME') from dual;

-- Задание 1 
-- Определить общий размер области SGA
show parameter sga_target;
show parameter sga_max_size;

-- Задание 2
-- Определить текущие размеры основных пулов SGA
SHOW SGA;

-- Задание 3 
-- Определить размеры гранулы для каждого пула
select 
    component, 
    granule_size/1024/1024 as granule_mb
from v$sga_dynamic_components;

-- Задание 4
-- Определить объем доступной свободной памяти в SGA
select * from v$sgastat where name = 'free memory';

-- Задание 5 
-- Определить максимальный и целевой размер области SGA
show parameter sga_max_size;
show parameter sga_target;

-- Задание 6
-- Определить размеры пулов KEEP, DEFAULT и RECYCLE буферного кэша
select component, 
       current_size/1024/1024 as size_mb
from v$sga_dynamic_components
where component in ('KEEP buffer cache', 'DEFAULT buffer cache', 'RECYCLE buffer cache');

-- Задание 7
-- Создайть таблицу, которая будет помещаться в пул KEEP.
-- Продемонстрировать сегмент таблицы.
alter system set db_keep_cache_size = 32M scope = both;

select component, current_size/1024/1024 as size_mb
from v$sga_dynamic_components
where component = 'KEEP buffer cache';

create table PVR_KEEP (
    id number(5) primary key,
    name varchar2(50)
) storage (buffer_pool keep);

select segment_name, segment_type, tablespace_name, bytes/1024 as size_kb
from user_segments
where segment_name = 'PVR_KEEP';

-- Задание 8
-- Создать таблицу, которая будет кэшироваться в пуле DEFAULT.
-- Продемонстрировать сегмент таблицы.
create table PVR_DEFAULT (
    id number(5) primary key,
    name varchar2(50)
) storage (buffer_pool default);

select 
    segment_name,
    segment_type,
    tablespace_name,
    bytes/1024 as size_kb
from user_segments
where segment_name = 'PVR_DEFAULT';

-- Задание 9
-- Найти размер буфера журналов повтора
select name, value/1024/1024 as size_mb
from v$sga
where name = 'Redo Buffers';

-- Задание 10
-- Найти размер свободной памяти в большом пуле
select pool, name, bytes/1024/1024 as free_mb
from v$sgastat
where pool = 'large pool' and name = 'free memory';

-- Задание 11
-- Определить режимы текущих соединений с инстансом (dedicated, shared)
select 
    username,
    server,
    program,
    status
from v$session
where type = 'USER';

-- Задание 12
-- Получить полный список работающих фоновых процессов
select 
    name,
    description 
from v$bgprocess 
where paddr != '00';

-- Задание 13
-- Получить список работающих серверных процессов
select 
    s.sid,
    s.serial#,
    p.spid,
    s.username,
    s.program,
    s.server
from v$session s
join v$process p on s.paddr = p.addr
where s.type = 'USER';

-- Задание 14
-- Определить, сколько процессов DBWn работает в настоящий момент
select 
    name,
    description 
from v$bgprocess 
where name like 'DBW%' and paddr != '00';

-- Задание 15
-- Определить сервисы (точки подключения экземпляра)
select 
    service_id,
    name,
    network_name, goal
from dba_services;

-- Задание 16
-- Получить известные параметры диспетчеров
show parameter dispatchers;

-- Задание 17
-- Указать в списке Windows-сервисов сервис, реализующий процесс LISTENER
-- docker exec -it oracle-lab lsnrctl status

-- Задание 18
-- Продемонстрировать и пояснить содержимое файла LISTENER.ORA
-- docker exec -it oracle-lab cat /opt/oracle/product/26ai/dbhomeFree/network/admin/listener.ora

-- Задание 19
-- Запустить утилиту lsnrctl и пояснить ее основные команды
-- docker exec -it oracle-lab lsnrctl help

-- Задание 20
-- Получить список служб инстанса, обслуживаемых процессом LISTENER
-- docker exec -it oracle-lab lsnrctl services