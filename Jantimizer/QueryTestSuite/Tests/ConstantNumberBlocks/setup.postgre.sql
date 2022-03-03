CREATE TABLE A AS SELECT s, (10)::integer FROM generate_Series(1,10) s;
CREATE TABLE B AS SELECT s, (100)::integer FROM generate_Series(1,50) s;
CREATE TABLE C AS SELECT s, (1000)::integer FROM generate_Series(1,100) s;
CREATE TABLE D AS SELECT s, (10000)::integer FROM generate_Series(1,500) s;

ANALYZE;