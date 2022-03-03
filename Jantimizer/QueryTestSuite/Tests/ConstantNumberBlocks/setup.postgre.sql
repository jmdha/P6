create TABLE A AS SELECT s, (10)::integer FROM generate_Series(1,10) s;
create TABLE B AS SELECT s, (100)::integer FROM generate_Series(1,50) s;
create TABLE C AS SELECT s, (1000)::integer FROM generate_Series(1,100) s;
create TABLE D AS SELECT s, (10000)::integer FROM generate_Series(1,500) s;