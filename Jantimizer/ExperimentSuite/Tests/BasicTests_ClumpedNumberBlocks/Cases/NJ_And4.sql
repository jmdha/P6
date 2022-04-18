SELECT * FROM ((a JOIN b ON (a.v >= b.v AND a.v <= b.v)) join c ON (a.v <= c.v AND b.v <= c.v)) JOIN d ON (b.v <= d.v AND c.v <= d.v)
