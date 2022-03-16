# Query Handler
The goal of this library is for a given query and respective histograms give back a new query with optimized order of joins (In terms of smallest intermiediate result)

Unary comparisons:
e.g. A JOIN B ON A.ID > 10

Binary comparisons:
e.g. A JOIN B ON A.ID > B.ID;

