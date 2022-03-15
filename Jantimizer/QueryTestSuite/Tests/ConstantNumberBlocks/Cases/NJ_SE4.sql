SELECT * FROM ((a JOIN b ON a.int4 <= b.int4) JOIN c ON b.int4 <= c.int4) JOIN d ON c.int4 <= d.int4
