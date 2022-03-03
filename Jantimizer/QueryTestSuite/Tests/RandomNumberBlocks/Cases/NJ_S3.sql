explain analyze select * from ((a join b on a."int4" < b."int4") join c on b."int4" < c."int4");
