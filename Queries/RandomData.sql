-- Create tables
create table A as select s, (random() * 10)::integer from generate_Series(1,10) s;
create table B as select s, (random() * 100)::integer from generate_Series(1,50) s;
create table C as select s, (random() * 1000)::integer from generate_Series(1,100) s;
create table D as select s, (random() * 10000)::integer from generate_Series(1,500) s;

-- Manually analyze the tables
analyze;

-- Run queries
explain analyze select * from (a join b on a."int4" = b."int4");                                                                 -- 12     | 6
explain analyze select * from (a join b on a."int4" < b."int4");                                                                 -- 167    | 470
explain analyze select * from (a join b on a."int4" > b."int4");                                                                 -- 167    | 24
explain analyze select * from (a join b on a."int4" <= b."int4");                                                                -- 167    | 476
explain analyze select * from (a join b on a."int4" >= b."int4");                                                                -- 167    | 30
explain analyze select * from ((a join b on a."int4" = b."int4") join c on b."int4" = c."int4");                                 -- 12     | 0
explain analyze select * from ((a join b on a."int4" < b."int4") join c on b."int4" < c."int4");                                 -- 5567   | 44500
explain analyze select * from ((a join b on a."int4" < b."int4") join c on b."int4" > c."int4");                                 -- 5567   | 3330
explain analyze select * from ((a join b on a."int4" > b."int4") join c on b."int4" > c."int4");                                 -- 5567   | 0
explain analyze select * from (((a join b on a."int4" < b."int4") join c on b."int4" < c."int4") join d on c."int4" < d."int4"); -- 927833 | 20956870  
explain analyze select * from (((a join b on a."int4" < b."int4") join c on b."int4" < c."int4") join d on c."int4" > d."int4"); -- 927833 | 1235810
explain analyze select * from (((a join b on a."int4" < b."int4") join c on b."int4" > c."int4") join d on c."int4" < d."int4"); -- 927833 | 1780120
explain analyze select * from (((a join b on a."int4" < b."int4") join c on b."int4" > c."int4") join d on c."int4" > d."int4"); -- 927833 | 6800
explain analyze select * from (((a join b on a."int4" > b."int4") join c on b."int4" > c."int4") join d on c."int4" < d."int4"); -- 927833 | 16000
explain analyze select * from (((a join b on a."int4" > b."int4") join c on b."int4" > c."int4") join d on c."int4" > d."int4"); -- 927833 | 0

-- Clean up
drop table A;
drop table B;
drop table C;
drop table D;