# Tests
This folder contains the possible test cases to run. They usually consist of a setup, and then an actual query run.
Each folder contains the following files:
* `setup.sql` and/or `setup.[DB Name].sql`
  * For setting up tables and database schemas
* `cleanup.sql` and/or `cleanup.[DB Name].sql`
  * For removing inserted data
* `inserts.sql` and/or `inserts.[DB Name].sql`
  * For inserting data into the database
* `analyse.sql` and/or `analyse.[DB Name].sql`
  * For analysing the tables in the database
* `testSettings.json` and/or `testSettings.[DB Name].json`
  * Settings for what sql files to run.

## `testSettings.json`
The `testSettings.json` file have the following structure:
```json
{
  "DoPreCleanup": true,
  "DoSetup": true,
  "DoInserts": true,
  "DoAnalyse": true,
  "DoPostCleanup": true,
  "DoMakeHistograms": true,
  "DoRunTests": true,
  "DoMakeReport": true,
  "DoMakeTimeReport": true,
  "Properties": {
    "Database": "postgres-database-name",
    "Schema": "schema-in-postgres-database"
  }
}
```
Where each bool indicate what should be run in that order.
* `DoPreCleanup`: Run the `cleanup.sql` file before inserting
* `DoSetup`: Run the `setup.sql` file
* `DoInserts`: Run the `inserts.sql` file
* `DoAnalyse`: Run the `analyse.sql` file
* `DoPostCleanup`: Run the `cleanuo.sql` file after test runs
* `DoMakeHistograms`: If the system is to make histograms of what is in the database
* `DoRunTests`: Run the all the `.json` files in the `Cases/` subfolder
* `DoMakeReport`: Make a report of the results in the end, and save it to a `.csv` file
* `DoMakeTimeReport`: Make a report of the execution time of each case in the end, and save it to a `.csv` file

As well as some additional connection properties, that should match up with what database and schema you are trying to do your setup in:
* `Database`: name of the database
* `Schema` name of the schema

## `Cases/`
The folder called `Cases/` contains all the test cases to run in this test.
This is in a `.json` format with the following syntax:
```json
{
  "EquivalentSQLQuery": "SELECT * ...",
  "DoRun": true,
  "JoinNodes": [
    {
      "Predicates": [
        {
          "LeftAttribute": {
            ...
          },
          "RightAttribute": {
            ...
          },
          "ComType": ">"
        },
        ...
      ]
    }
  ]
}

```
Where the predicates can either be a Filter predicate or a TableAttribute predicate.
Table attribute predicates would look like:
```json
{
    "LeftAttribute": {
        "Attribute": {
            "Table": {
                "TableName": "a"
            },
            "Attribute": "v"
        }
    },
    "RightAttribute": {
        "Attribute": {
            "Table": {
                "TableName": "b"
            },
            "Attribute": "v"
        }
    },
    "ComType": ">"
},
```
While a Filter predicate could look like:
```json
{
    "LeftAttribute": {
        "Attribute": {
            "Table": {
                "TableName": "a"
            },
            "Attribute": "v"
        }
    },
    "RightAttribute": {
        "ConstantValue": "50"
    },
    "ComType": ">"
},
```

An example could be.
```json
{
  "EquivalentSQLQuery": "SELECT * FROM a JOIN b ON a.v > b.v",
  "DoRun": true,
  "JoinNodes": [
    {
      "Predicates": [
        {
          "LeftAttribute": {
            "Attribute": {
              "Table": {
                "TableName": "a"
              },
              "Attribute": "v"
            }
          },
          "RightAttribute": {
            "Attribute": {
              "Table": {
                "TableName": "b"
              },
              "Attribute": "v"
            }
          },
          "ComType": ">"
        }
      ]
    }
  ]
}


```