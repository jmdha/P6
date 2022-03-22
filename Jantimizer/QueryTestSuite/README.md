# How tests are run

Each test suite is a directory in /Tests, and is run as follows:
1. (optional) cleanup
2. (optional) setup
3. each test case
4. (optional) cleanup

## Test Naming Convention

* `/Tests/testorder.json`
* `/Tests/{TestName}`
  * (Optional) `cleanup.sql` or `cleanup.[sqlType].sql`
  * (Optional) `setup.sql` or `setup.[sqlType].sql`
  * (Required) `testSettings.json` or `testSettings.[sqlType].json`
  * `/cases/`
    * `*.sql` or `*.[sqlType].sql`

### Test Order
The file `testorder.json` contains the order that the tests should run it. 
The file should be structured as following:
```json
{
  "Order": ["Dir1", "Dir2", "Dir3", "*"]
}
```
Where the `Dir*` values can be changed into the names of the folders for the tests.
The `*` is a special wild card where it will just run all files, that have not been specified in the order.
E.g. with the folders "Dir1", "Dir2" and "Dir3", and the order file:
```json
{
  "Order": ["Dir1", "*", "Dir2"]
}
```
The program would first run "Dir1" tests, then the "Dir3" and lastly "Dir2".

### Test Settings
Each test can have settings to run, named `testSettings.json` or `testSettings.[sqlType].json`, with the following structure:
```json
{
  "DoCleanup": <bool>,
  "DoSetup": <bool>,
  "DoMakeHistograms": <bool>,
  "Properties": {
    "Database": <string>,
    "Schema": <string>
  }
}
```
This file have to be made for each of the tests in the `Tests` folder.
The optional files does not have to be there, e.g. `cleanup.sql`, if the setting is not enabled in the test settings file.

### [sqlType]
When running the sql files, each connector will first attempt to run the variant with a matching [sqltype], and if that doesn't exist, then it will use the non-generic file without [sqltype]
If there is a sqltype, the corresponding connector will use that specific variant of the file

E.g.
	The MySql connector will run `setup.mysql.sql` if it exists, otherwise it will run `setup.sql`

# User secrets
This project uses local User Secrets for connection strings.
To add a secrets file to the project:
* Right click on the project file.
* Click "Manage User Secrets".
* Paste the following structure and insert your connection strings instead.

```json
{
  "ConnectionProperties": {
    "POSGRESQL": {
      "Server": "localhost",
      "Port": 5432,
      "Username": "postgres",
      "Password": "password"
    },
    "MYSQL": {
      "Server": "localhost",
      "Port": 3306,
      "Username": "root",
      "Password": "password"
    }
  },
  "LaunchSystem": {
    "POSGRESQL": "True",
    "MYSQL": "True"
  }
}
```
