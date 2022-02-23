-- Create tables
create table A as select s, (10)::integer from generate_Series(1,10) s;
create table B as select s, (100)::integer from generate_Series(1,50) s;
create table C as select s, (1000)::integer from generate_Series(1,100) s;
create table D as select s, (10000)::integer from generate_Series(1,500) s;

-- Manually analyze the tables
analyze;

-- Run queries
explain analyze select * from (a join b on a."int4" = b."int4");                                                                 -- 1            | 0
explain analyze select * from (a join b on a."int4" < b."int4");                                                                 -- 167          | 500
explain analyze select * from (a join b on a."int4" > b."int4");                                                                 -- 167          | 0
explain analyze select * from (a join b on a."int4" <= b."int4");                                                                -- 167          | 500
explain analyze select * from (a join b on a."int4" >= b."int4");                                                                -- 167          | 0
explain analyze select * from ((a join b on a."int4" = b."int4") join c on b."int4" = c."int4");                                 -- 1            | 0
explain analyze select * from ((a join b on a."int4" < b."int4") join c on b."int4" < c."int4");                                 -- 5567         | 50000
explain analyze select * from ((a join b on a."int4" < b."int4") join c on b."int4" > c."int4");                                 -- 5567         | 0
explain analyze select * from ((a join b on a."int4" > b."int4") join c on b."int4" > c."int4");                                 -- 5567         | 0
explain analyze select * from (((a join b on a."int4" < b."int4") join c on b."int4" < c."int4") join d on c."int4" < d."int4"); -- 966206394533 | 25000000  
explain analyze select * from (((a join b on a."int4" < b."int4") join c on b."int4" < c."int4") join d on c."int4" > d."int4"); -- 966206394533 | 0
explain analyze select * from (((a join b on a."int4" < b."int4") join c on b."int4" > c."int4") join d on c."int4" < d."int4"); -- 966206394533 | 0
explain analyze select * from (((a join b on a."int4" < b."int4") join c on b."int4" > c."int4") join d on c."int4" > d."int4"); -- 966206394533 | 0
explain analyze select * from (((a join b on a."int4" > b."int4") join c on b."int4" > c."int4") join d on c."int4" < d."int4"); -- 966206394533 | 0
explain analyze select * from (((a join b on a."int4" > b."int4") join c on b."int4" > c."int4") join d on c."int4" > d."int4"); -- 966206394533 | 0

-- Clean up
drop table A;
drop table B;
drop table C;
drop table D;