# JANTIMATOR: Join Analysis of iNequality esTIMATOR
This is a project about creating better cardinality estimates for Join Queries with inequality predicates.
Todays systems are not geared towards giving good cardinality estimates for these queries, which is something this project aims to do something about.

![ezgif-2-c037fe5652](https://user-images.githubusercontent.com/22596587/168040218-78df703b-a2b3-4f91-bece-fd86420f6ae4.gif)

The program itself consists of a WPF interface written in C#.
The program is capable of taking a set of queries in a json format, see what a given database system would estimate the cardinality of said query and then give a new estimate from our estimator.
This program itself uses [MySQL](https://www.mysql.com/) and [PostgeSQL](https://www.postgresql.org/) to run queries on.

## Datasets
For this project, 10 syntetic datasets and 3 real datasets was used.
The syntetic datasets have the same table setup (See [mysql table definitions](/Jantimizer/ExperimentSuite/Tests/BasicTests_SetupTables/setup.mysql.sql) and [postgres table definitions](/Jantimizer/ExperimentSuite/Tests/BasicTests_SetupTables/setup.posgresql.sql) for details) and the data is:
* **RandomNumberBlocks**
  * Consisting of 4 tables with random values in them (See [inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_RandomNumberBlocks/inserts.sql) for details)
* **ConstantNumberBlocks**
  * Consisting of 4 tables with constant values in them (See [mysql inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_ConstantNumberBlocks/inserts.mysql.sql) and [postgres inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_ConstantNumberBlocks/inserts.posgresql.sql) for details)
* **ClumpedNumberBlocks_Difficult**
  * Consisting of 4 tables with sin-curve without offset data distribution (See [mysql inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_ClumpedNumberBlocks_Difficult/inserts.mysql.sql) and [postgres inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_ClumpedNumberBlocks_Difficult/inserts.posgresql.sql) for details)
* **ClumpedNumberBlocks_Possible**
  * Consisting of 4 tables with sin-curve with offset data distribution (See [mysql inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_ClumpedNumberBlocks_Possible/inserts.mysql.sql) and [postgres inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_ClumpedNumberBlocks_Possible/inserts.posgresql.sql) for details)
* **LinearNumberBlocks**
  * Consisting of 4 tables with a linear data distribution (See [mysql inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_LinearNumberBlocks/inserts.mysql.sql) and [postgres inserts](/Jantimizer/ExperimentSuite/Tests/BasicTests_LinearNumberBlocks/inserts.posgresql.sql) for details)

In addition to this, there are `Large` verisons of them, with 5 times more rows in each table.

The real datasets are:
* [**Census 2013**](https://github.com/sfu-db/AreCELearnedYet#dataset) (See [mysql setup](/Jantimizer/ExperimentSuite/Tests/Census_2013_Setup/setup.mysql.sql) and [postgres setup](/Jantimizer/ExperimentSuite/Tests/Census_2013_Setup/setup.posgresql.sql) for table definitions)
  * Consisting of one large table with a lot of data from a census.
* [**COVID-19 Funding**](https://data.world/hdx/e24de323-ed64-4c33-8eda-dace55d107b9) (See [mysql setup](/Jantimizer/ExperimentSuite/Tests/COVID_Funding_Setup/setup.mysql.sql) and [postgres setup](/Jantimizer/ExperimentSuite/Tests/COVID_Funding_Setup/setup.posgresql.sql) for table definitions)
  * Two tables with data regarding organisational funding for COVID-19
* [**World happiness 2022**](https://www.kaggle.com/datasets/mathurinache/world-happiness-report-2022) with [GDP data](https://data.worldbank.org/indicator/Ny.Gdp.Mktp.Cd) (See [mysql setup](/Jantimizer/ExperimentSuite/Tests/WorldHappiness_With_GDP_Setup/setup.mysql.sql) and [postgres setup](/Jantimizer/ExperimentSuite/Tests/WorldHappiness_With_GDP_Setup/setup.posgresql.sql) for table definitions)
  * Consisting 3 Tables one with large one with survey data, one with countries and one yearly GDP

## Setup
You need to setup the two database systems (or alternatively disable one them in the [`experiments.json` file](/Jantimizer/ExperimentSuite/experiments.json)).
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
The Actual Cardinalities are saved to a cache file during runtime (and is loaded automatically on next launch).
What is saved in these caches can be seen in the Cache Viewer, consisting of a MD5 hash of the data as well as the data itself.

## Subviews
There are also two subviews, that can be used to debug or get a better understanding of what happens during the bounding and estimation process.
One subview is for seeing how Milestones got generated, while the other is for each of the query file runs you can see how the bounding and estimation was set.
These views are buttons under each testrunner that is displayed when the tests starts running.

# Flow
A general flowchart of how the tests are run can be seen below. This is for each connector.
![flow](https://user-images.githubusercontent.com/22596587/168037861-e6e492cf-fe36-4b06-9043-8451c3881487.png)
