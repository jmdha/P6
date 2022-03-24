SELECT * FROM (a JOIN b ON (a.v = b.v OR a.v < b.v)) JOIN c ON (b.v = c.v OR b.v < c.v)
