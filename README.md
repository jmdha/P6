# P6 : Jantimizer
This is a project about creating better cardinality estimates for Join Queries with inequality predicates.
Todays systems are not geared towards giving good cardinality estimates for these queries, which is something this project aims to do something about.

![maingif](https://user-images.githubusercontent.com/22596587/165292165-4e6fa380-a2a5-400d-ae0a-9e88ba139d3e.gif)

The program itself consists of a WPF interface written in C#.
The program is capable of taking a set of queries in SQL format, see what a given database system would estimate the cardinality of said query and then give a new estimate from our optimiser.

## Datasets
For this project, 3 syntetic datasets and 3 real datasets was used.
The syntetic datasets are:
* **RandomNumberBlocks**
  * With 3 tables with random values in them (See [inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_RandomNumberBlocks/inserts.sql) for details)
* **ConstantNumberBlocks**
* **ClumpedNumberBlocks**
* 

## Views

### Sentinel Viewer

### Cache Viewer