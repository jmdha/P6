# Query Parser
This is a project to parse SQL queries. Currently, it is specialized in parsing Join queries (in a fairly specific format).
It requires a Join chain to be in the style:
```
SELECT * FROM (A JOIN B ON ...) JOIN C ON ...
SELECT * FROM ((A JOIN B ON ...) JOIN C ON ...) JOIN D ON ...
```
With each inner join in brackets. 

This project will then output the Joins and as a list, with their tables and conditions.