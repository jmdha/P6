CREATE TABLE A AS SELECT s, (RANDOM() * 10)::integer FROM generate_Series(1,10) s;
CREATE TABLE B AS SELECT s, (RANDOM() * 100)::integer FROM generate_Series(1,50) s;
CREATE TABLE C AS SELECT s, (RANDOM() * 1000)::integer FROM generate_Series(1,100) s;
CREATE TABLE D AS SELECT s, (RANDOM() * 10000)::integer FROM generate_Series(1,500) s;

ANALYZE;