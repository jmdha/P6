SELECT * FROM (a JOIN b ON (a.v >= b.v AND a.v <= b.v)) join c ON (a.v <= c.v AND b.v <= c.v)
