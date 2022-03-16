CREATE TABLE A AS SELECT generate_Series(1,10) AS s, (10) AS v;
CREATE TABLE B AS SELECT generate_Series(1,50) AS s, (100) AS v;
CREATE TABLE C AS SELECT generate_Series(1,100) AS s, (1000) AS v;
CREATE TABLE D AS SELECT generate_Series(1,500) AS s, (10000) AS v;

ANALYZE;

explain select * from ((a as c join b on ((c.v > b.v and c.v < b.v) or (c.v = b.v))) join a on a.v > c.v );