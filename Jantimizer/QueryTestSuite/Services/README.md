# User secrets
This project uses local User Secrets for connection strings.
To add a secrets file to the project:
* Right click on the project file.
* Click "Manage User Secrets".
* Paste the following structure and insert your connection strings instead.
* Also optional to disable some of the database connectors if needed.

```
{
  "ConnectionStrings": {
    "POSGRESQL": "connection string here",
    "MYSQL": "connection string here"
  },
  "LaunchSystem": {
    "POSGRESQL": "True",
    "MYSQL": "True"
  }
}
```

# Connection Strings
All connectors in this project use standard ADO.NET style connection strings. A list of structures for the local connection strings can be seen below.
### PostgreSQL
Postgres requires a `Host`, `Port`, `Username`, `Password` and `Database`. The default for a local database are the following:

| Property Name  | Default value (Local) |
| -------------- | --------------------- |
| `Host`         | `localhost`           |
| `Port`         | `5432`                |
| `Username`     | `postgres`            |
| `Password`     | `password`            |
| `Database`     | `postgres`            |

**Example string**:
 `Host=localhost;Port=5432;Username=postgres;Password=password;Database=postgres`

### MySQL
MySQL requires a `Server`, `Port`, `Uid`, `Pwd`, `Database`. The default for a local database are the following:

| Property Name  | Default value (Local) |
| -------------- | --------------------- |
| `Host`         | `localhost`           |
| `Port`         | `3306`                |
| `Uid`          | `root`                |
| `Pwd`          | `myPassword`          |
| `Database`     | `public`              |

**Example string**:
 `Server=localhost;Port=3306;Uid=root;Pwd=password;Database=public`
