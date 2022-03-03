drop table if exists A;
drop table if exists B;
drop table if exists C;
drop table if exists D;

-- Create tables
create table A as select s, (random() * 10)::integer from generate_Series(1,10) s;
create table B as select s, (random() * 100)::integer from generate_Series(1,50) s;
create table C as select s, (random() * 1000)::integer from generate_Series(1,100) s;
create table D as select s, (random() * 10000)::integer from generate_Series(1,500) s;

-- Manually analyze the tables
analyze;

-- Run queries
explain analyze select * from (a join b on a."int4" = b."int4");
explain analyze select * from (a join b on a."int4" < b."int4");
explain analyze select * from (a join b on a."int4" > b."int4");
explain analyze select * from (a join b on a."int4" <= b."int4");
explain analyze select * from (a join b on a."int4" >= b."int4");
explain analyze select * from ((a join b on a."int4" = b."int4") join c on b."int4" = c."int4");
explain analyze select * from ((a join b on a."int4" < b."int4") join c on b."int4" < c."int4");
explain analyze select * from ((a join b on a."int4" > b."int4") join c on b."int4" > c."int4");
explain analyze select * from ((a join b on a."int4" <= b."int4") join c on b."int4" <= c."int4");
explain analyze select * from ((a join b on a."int4" >= b."int4") join c on b."int4" >= c."int4");
explain analyze select * from (((a join b on a."int4" = b."int4") join c on b."int4" = c."int4") join d on c."int4" = d."int4");
explain analyze select * from (((a join b on a."int4" < b."int4") join c on b."int4" < c."int4") join d on c."int4" < d."int4");
explain analyze select * from (((a join b on a."int4" > b."int4") join c on b."int4" > c."int4") join d on c."int4" > d."int4");
explain analyze select * from (((a join b on a."int4" <= b."int4") join c on b."int4" <= c."int4") join d on c."int4" <= d."int4");
explain analyze select * from (((a join b on a."int4" >= b."int4") join c on b."int4" >= c."int4") join d on c."int4" >= d."int4");

explain analyze select * from a, b where a."int4" = b."int4";
explain analyze select * from a, b where a."int4" < b."int4";
explain analyze select * from a, b where a."int4" > b."int4";
explain analyze select * from a, b where a."int4" <= b."int4";
explain analyze select * from a, b where a."int4" >= b."int4";
explain analyze select * from a, b, c where a."int4" = b."int4" and b."int4" = c."int4";
explain analyze select * from a, b, c where a."int4" < b."int4" and b."int4" < c."int4";
explain analyze select * from a, b, c where a."int4" > b."int4" and b."int4" > c."int4";
explain analyze select * from a, b, c where a."int4" <= b."int4" and b."int4" <= c."int4";
explain analyze select * from a, b, c where a."int4" >= b."int4" and b."int4" >= c."int4";
explain analyze select * from a, b, c, d where a."int4" = b."int4" and b."int4" = c."int4" and c."int4" = d."int4";
explain analyze select * from a, b, c, d where a."int4" < b."int4" and b."int4" < c."int4" and c."int4" < d."int4";
explain analyze select * from a, b, c, d where a."int4" > b."int4" and b."int4" > c."int4" and c."int4" > d."int4";
explain analyze select * from a, b, c, d where a."int4" <= b."int4" and b."int4" <= c."int4" and c."int4" <= d."int4";
explain analyze select * from a, b, c, d where a."int4" >= b."int4" and b."int4" >= c."int4" and c."int4" >= d."int4";

-- Clean up
drop table if exists A;
drop table if exists B;
drop table if exists C;
drop table if exists D;