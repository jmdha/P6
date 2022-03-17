



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
* Right click on the project file
* Click "Manage User Secrets"
* Paste the following structure and insert your connection strings instead

```
{
  "ConnectionStrings": {
    "POSGRESQL": "connection string here",
    "MYSQL": "connection string here"
  }
}
```

# Connection Strings
All connectors in this project use standard ADO.NET style connection strings. A list of structures for the local connection strings can be seen below.
### PostgreSQL
Postgres requires a `Host`, `Port`, `Username`, `Password`, `Database` and `SearchPath` (Schema). The default for a local database are the following:

| Property Name  | Default value (Local) |
| -------------- | --------------------- |
| `Host`         | `localhost`           |
| `Port`         | `5432`                |
| `Username`     | `postgres`            |
| `Password`     | `password`            |
| `Database`     | `postgres`            |
| `SearchPath`	 | `public`	            |

**Example string**:
 `Host=localhost;Port=5432;Username=postgres;Password=password;Database=postgres`

### MySQL
MySQL requires a `Server`, `Port`, `Uid`, `Pwd`, `Database` and `Schema`. The default for a local database are the following:

| Property Name  | Default value (Local) |
| -------------- | --------------------- |
| `Host`         | `localhost`           |
| `Port`         | `3306`                |
| `Uid`          | `root`                |
| `Pwd`          | `myPassword`          |
| `Database`     | `public`              |

**Example string**:
 `Server=localhost;Port=3306;Uid=root;Pwd=password;Database=public`
	
		
		
	