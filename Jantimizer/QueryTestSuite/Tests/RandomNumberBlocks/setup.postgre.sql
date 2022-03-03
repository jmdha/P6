create TABLE A AS SELECT s, (rANDom() * 10)::integer FROM generate_Series(1,10) s;
create TABLE B AS SELECT s, (rANDom() * 100)::integer FROM generate_Series(1,50) s;
create TABLE C AS SELECT s, (rANDom() * 1000)::integer FROM generate_Series(1,100) s;
create TABLE D AS SELECT s, (rANDom() * 10000)::integer FROM generate_Series(1,500) s;
