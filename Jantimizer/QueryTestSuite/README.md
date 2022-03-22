



# How tests are run

Each test suite is a directory in /Tests, and is run as follows:
1. cleanup
2. setup
3. each test case
4. cleanup


## Test Naming Convention
	/Tests/{TestName}
                cleanup.sql or cleanup.[sqlType].sql
		setup.sql or setup.[sqlType].sql
		cases/
			*.sql or *.[sqlType].sql // Whereas '*' is a wildcart, but may not contain a dot '.'.

### [sqlType]
When running the sql files, each connector will first attempt to run the variant with a matching [sqltype], and if that doesn't exist, then it will use the non-generic file without [sqltype]
If there is a sqltype, the corresponding connector will use that specific variant of the file

E.g.
	The MySql connector will run setup.mysql.sql if it exists, otherwise it will run setup.sql

# User secrets
This project uses local User Secrets for connection strings.
To add a secrets file to the project:
* Right click on the project file.
* Click "Manage User Secrets".
* Paste the following structure and insert your connection strings instead.

```
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
