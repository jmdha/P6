CREATE TABLE A AS SELECT generate_Series(1,10) AS s, (RANDOM() * 10)::int AS v;
CREATE TABLE B AS SELECT generate_Series(1,50) AS s, (RANDOM() * 100)::int AS v;
CREATE TABLE C AS SELECT generate_Series(1,100) AS s, (RANDOM() * 1000)::int AS v;
CREATE TABLE D AS SELECT generate_Series(1,500) AS s, (RANDOM() * 10000)::int AS v;

ANALYZE;