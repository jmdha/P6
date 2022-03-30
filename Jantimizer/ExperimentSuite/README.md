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
  }
}
```