# P6 : Jantimizer
This is a project about creating better cardinality estimates for Join Queries with inequality predicates.
Todays systems are not geared towards giving good cardinality estimates for these queries, which is something this project aims to do something about.

![maingif](https://user-images.githubusercontent.com/22596587/165292165-4e6fa380-a2a5-400d-ae0a-9e88ba139d3e.gif)

The program itself consists of a WPF interface written in C#.
The program is capable of taking a set of queries in SQL format, see what a given database system would estimate the cardinality of said query and then give a new estimate from our optimiser.
This program itself uses [MySQL](https://www.mysql.com/) and [PostgeSQL](https://www.postgresql.org/) to run queries on.

## Datasets
For this project, 3 syntetic datasets and 3 real datasets was used.
The syntetic datasets have the same table setup (See [mysql table definitions](/Jantimizer/ExperimentSuite/Tests/BasicTests_SetupTables/setup.mysql.sql) and [postgres table definitions](/Jantimizer/ExperimentSuite/Tests/BasicTests_SetupTables/setup.posgresql.sql) for details) and the data is:
* **RandomNumberBlocks**
  * Consisting of 3 tables with random values in them (See [inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_RandomNumberBlocks/inserts.sql) for details)
* **ConstantNumberBlocks**
  * Consisting of 3 tables with constant values in them (See [mysql inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_ConstantNumberBlocks/inserts.mysql.sql) and [postgres inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_ConstantNumberBlocks/inserts.posgresql.sql) for details)
* **ClumpedNumberBlocks**
  * Consisting of 3 tables with sin-curve data distribution (See [mysql inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_ClumpedNumberBlocks/inserts.mysql.sql) and [postgres inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_ClumpedNumberBlocks/inserts.posgresql.sql) for details)

The real datasets are:
* [**Census 2013**](https://github.com/sfu-db/AreCELearnedYet#dataset) (See [mysql setup](/Jantimizer/ExperimentSuite/Tests/Census_2013_Setup/setup.mysql.sql) and [postgres setup](/Jantimizer/ExperimentSuite/Tests/Census_2013_Setup/setup.posgresql.sql) for table definitions)
  * Consisting of one large table with a lot of data from a census.
* [**COVID-19 Funding**](https://data.world/hdx/e24de323-ed64-4c33-8eda-dace55d107b9) (See [mysql setup](/Jantimizer/ExperimentSuite/Tests/COVID_Funding_Setup/setup.mysql.sql) and [postgres setup](/Jantimizer/ExperimentSuite/Tests/COVID_Funding_Setup/setup.posgresql.sql) for table definitions)
  * Two tables with data regarding organisational funding for COVID-19
* [**World happiness 2022**](https://www.kaggle.com/datasets/mathurinache/world-happiness-report-2022) with [GDP data](https://data.worldbank.org/indicator/Ny.Gdp.Mktp.Cd) (See [mysql setup](/Jantimizer/ExperimentSuite/Tests/WorldHappiness_With_GDP_Setup/setup.mysql.sql) and [postgres setup](/Jantimizer/ExperimentSuite/Tests/WorldHappiness_With_GDP_Setup/setup.posgresql.sql) for table definitions)
  * Consisting 3 Tables one with large one with survey data, one with countries and one yearly GDP

## Setup
You need to setup the two database systems (or alternatively disable them in the [`experiments.json` file](/Jantimizer/ExperimentSuite/experiments.json)).
The current tests are setup to use default users for PostgreSQL and MySQL.

MySQL: Username: `root` password `password`

PostgrSQL: Username: `postgre` password `password`

These can be changed in the secrets file (See [the readme](/Jantimizer/ExperimentSuite/README.md) for more info).

## Views
The program consist of 3 main views, the main view, the Sentinel Viewer and the Cache Viewer.
The main view is the one shown when the program starts.
This has options to turn on/off sentinels and a run button.
If you press said button the program will start running through all the experiments defined in the [`experiments.json` file](/Jantimizer/ExperimentSuite/experiments.json) (See [the readme](/Jantimizer/ExperimentSuite/README.md) for more info)

### Sentinel Viewer
Result sentinels are there to make sure that the database systems get the same result between runs.
This is an additional safety check, to make sure the results are actually correct.
See [ResultsSentinel](/Jantimizer/ResultsSentinel/README.md) for more info.

### Cache Viewer
This program makes use of caches to reduce execution times for tests.
There are two caches one for Histograms and one for Actual Cardinalities from the database systems.

The Actual Cardinalities are saved to a cache file during runtime (and is loaded automatically on next launch).
However the histograms are regenerated each time to program launches.

What is saved in these caches can be seen in the Cache Viewer, consisting of a MD5 hash of the data as well as the data itself.

# Flow
A general flowchart of how the tests are run can be seen below. This is for each connector.
![flow drawio](https://user-images.githubusercontent.com/22596587/165300166-b8815196-00d8-490d-8f18-0a1b3c549ec8.png)
