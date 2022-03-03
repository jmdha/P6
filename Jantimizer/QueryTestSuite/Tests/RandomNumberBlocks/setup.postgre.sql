create table A as select s, (random() * 10)::integer from generate_Series(1,10) s;
create table B as select s, (random() * 100)::integer from generate_Series(1,50) s;
create table C as select s, (random() * 1000)::integer from generate_Series(1,100) s;
create table D as select s, (random() * 10000)::integer from generate_Series(1,500) s;
