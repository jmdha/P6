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
Postgres requires a `Host`, `Username`, `Password` and `Database`. The default for a local database are the following:

| Property Name  | Default value (Local) |
| -------------- | --------------------- |
| `Host`         | `localhost`           |
| `Username`     | `postgres`            |
| `Password`     | `password`            |
| `Database`     | `postgres`            |

**Example string**:
 `Host=localhost;Username=postgres;Password=password;Database=postgres`

### MySQL
MySQL requires a `Host`, `Username`, `Password`. The default for a local database are the following:

| Property Name  | Default value (Local) |
| -------------- | --------------------- |
| `Host`         | `localhost`           |
| `Username`     | `myUsername`          |
| `Password`     | `myPassword`          |

**Example string**:
 `Host=localhost;Username=myUsername;Password=myPassword`
